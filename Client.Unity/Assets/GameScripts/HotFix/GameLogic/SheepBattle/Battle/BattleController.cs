using Fantasy;
using Fantasy.Async;
using Cysharp.Threading.Tasks;
using GameLogic.SheepBattle.Network;
using GameConfig.battle;
using GameLogic.SheepBattle.Common;
using GameLogic.SheepBattle.Config;
using System.Linq;
using System.Collections.Generic;
using TEngine;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Log = TEngine.Log;

namespace GameLogic.SheepBattle.Battle
{
    public sealed class BattleController : IBattleCommand
    {
        public static BattleController Instance { get; } = new BattleController();

        public BattleStartInfo CurrentBattle { get; private set; }
        public TiledMapData CurrentMap { get; private set; }
        public BattleSnapshotInfo CurrentSnapshot { get; private set; }
        public int SelectedBuildingId { get; private set; }
        public long SelectedBuildingInstanceId { get; private set; }
        public bool IsBuildMode => _buildSkillActive;
        public int BattleElapsedSeconds => _battleStartedRealtime <= 0f ? 0 : Mathf.Max(0, Mathf.FloorToInt(Time.realtimeSinceStartup - _battleStartedRealtime));

        private GameObject _mapRoot;
        private GameObject _buildingRoot;
        private GameObject _playerRoot;
        private GameObject _objectRoot;
        private bool _syncing;
        private bool _moving;
        private float _lastMoveCommandAt;
        private int _moveCommandSerial;
        private bool _cameraDragging;
        private Vector3 _lastDragWorldPosition;
        private Vector3 _localPlayerPosition;
        private bool _hasLocalPlayerPosition;
        private string _localPlayerCamp = "Elf";
        private GameObject _buildPreviewRoot;
        private GameObject _buildPreviewBody;
        private GameObject _buildPreviewArea;
        private GameObject _buildRangeRoot;
        private GameObject _selectedBuildingInfoRoot;
        private GameObject _selectedTowerRangeRoot;
        private GameObject _effectRoot;
        private bool _battleSceneVisible;
        private bool _buildSkillActive;
        private bool _buildPreviewValid;
        private int _buildPreviewGridX;
        private int _buildPreviewGridY;
        private float _battleStartedRealtime;
        private bool _buildRequesting;
        private float _tileWorldSize = DefaultTileWorldSize;
        private Sprite _attackDistanceSprite;
        private readonly Dictionary<Collider, long> _buildingColliders = new();
        private readonly Dictionary<long, int> _playerHpCache = new();
        private readonly Dictionary<long, int> _buildingHpCache = new();
        private readonly Dictionary<long, float> _healthBarVisibleUntil = new();
        private readonly HashSet<long> _playedAttackEvents = new();
        private const float DefaultTileWorldSize = 1f;
        private const float TiledImporterPixelsPerUnit = 100f;
        private const float BattleCameraOrthographicSize = 4f;
        private const float PlayerMarkerSize = 0.5f;
        private const float MoveCommandIntervalSeconds = 0.1f;
        private const int BattleRunningPollIntervalMs = 200;
        private const float HealthBarVisibleSeconds = 5f;
        private const float HealthBarWidth = 1.35f;
        private const float HealthBarHeight = 0.18f;
        private const float ProjectileDurationSeconds = 0.22f;
        private const float SelectedInfoLineHeight = 0.22f;
        private const int GroundSortingOrder = 0;
        private const int ObjectSortingOrder = 160;
        private const int BuildingSortingOrder = 120;
        private const int PlayerSortingOrder = 140;
        private const int EffectSortingOrder = 180;
        private const int UiWorldSortingOrder = 220;
        private const string AttackDistanceSpriteLocation = "attack_distance";

        private BattleController()
        {
        }

        public void EnterBattle(BattleStartInfo battle)
        {
            CurrentBattle = battle;
            CurrentSnapshot = null;
            SelectedBuildingId = 0;
            SelectedBuildingInstanceId = 0;
            _battleSceneVisible = false;
            _battleStartedRealtime = 0f;
            GameModule.UI.ShowUIAsync<BattleMainUI>();
            LoadBattleMapAsync(battle).Coroutine();
        }

        public void LeaveBattle()
        {
            if (_mapRoot != null)
            {
                Object.Destroy(_mapRoot);
                _mapRoot = null;
            }

            _tileWorldSize = DefaultTileWorldSize;
            CurrentBattle = null;
            CurrentMap = null;
            CurrentSnapshot = null;
            SelectedBuildingId = 0;
            SelectedBuildingInstanceId = 0;
            _battleSceneVisible = false;
            _battleStartedRealtime = 0f;
            _hasLocalPlayerPosition = false;
            _cameraDragging = false;
            _syncing = false;
            _moving = false;
            _moveCommandSerial++;
            _playerHpCache.Clear();
            _buildingHpCache.Clear();
            _healthBarVisibleUntil.Clear();
            _playedAttackEvents.Clear();
            DestroySelectedBuildingHelpers();
            DestroyBuildHelpers();
            GameModule.UI.CloseUI<BattleMainUI>();
            GameModule.UI.ShowUIAsync<LobbyUI>();
        }

        public void SelectBuilding(int buildingId)
        {
            if (_localPlayerCamp == "Troll")
            {
                CommonNoticeService.Show("巨魔不能建造", "无法建造");
                return;
            }

            _buildSkillActive = true;
            SelectedBuildingId = buildingId;
            SelectedBuildingInstanceId = 0;
            _cameraDragging = false;
            EnsureBuildPreview();
            EnsureBuildRangeIndicator();
            Log.Info($"选择建造建筑：BuildingId={buildingId}");
        }

        public void ExitBuildMode()
        {
            _buildSkillActive = false;
            SelectedBuildingId = 0;
            SelectedBuildingInstanceId = 0;
            _cameraDragging = false;
            SetBuildPreviewVisible(false);
            SetBuildRangeVisible(false);
            FollowLocalPlayer();
            Log.Info("退出建造模式，恢复角色跟随。");
        }

        public void RequestBuildAt(int gridX, int gridY)
        {
            if (CurrentBattle == null || !_buildSkillActive || SelectedBuildingId <= 0 || !_battleSceneVisible)
            {
                return;
            }

            BuildAtAsync(SelectedBuildingId, gridX, gridY).Coroutine();
        }

        public void RequestUpgrade(long instanceId)
        {
            if (CurrentBattle == null || instanceId <= 0)
            {
                return;
            }

            UpgradeAsync(instanceId).Coroutine();
        }

        public void RequestRecycle(long instanceId)
        {
            if (CurrentBattle == null || instanceId <= 0)
            {
                return;
            }

            RecycleAsync(instanceId).Coroutine();
        }

        public void ClearSelectedBuilding()
        {
            if (SelectedBuildingInstanceId <= 0)
            {
                return;
            }

            SelectedBuildingInstanceId = 0;
            RefreshSelectedBuildingHelpers();
        }

        public void StartMoveInput()
        {
            if (_moving || CurrentBattle == null)
            {
                return;
            }

            _moving = true;
            MoveInputLoopAsync(CurrentBattle.BattleId).Coroutine();
        }

        public void OnSelectBuilding(int buildingId)
        {
            if (SelectedBuildingId == buildingId)
            {
                ExitBuildMode();
                return;
            }

            SelectBuilding(buildingId);
        }

        public void OnOpenBuildPanel()
        {
            if (_localPlayerCamp == "Troll")
            {
                CommonNoticeService.Show("巨魔不能建造", "无法建造");
                return;
            }

            _buildSkillActive = true;
            SelectedBuildingId = 0;
            SelectedBuildingInstanceId = 0;
            _cameraDragging = false;
            SetBuildPreviewVisible(false);
            EnsureBuildRangeIndicator();
            SetBuildRangeVisible(true);
            FollowLocalPlayer();
            Log.Info("开启建造技能，等待选择建筑卡片。");
        }

        public void OnBuildAt(int gridX, int gridY)
        {
            RequestBuildAt(gridX, gridY);
        }

        public void OnUpgradeBuilding(long instanceId)
        {
            RequestUpgrade(instanceId);
        }

        public void OnRecycleBuilding(long instanceId)
        {
            RequestRecycle(instanceId);
        }

        public void OnExitBuildMode()
        {
            ExitBuildMode();
        }

        public void ApplyExternalSnapshot(BattleSnapshotInfo snapshot)
        {
            ApplySnapshot(snapshot);
        }

        public void FocusPlayer(long playerId)
        {
            var player = CurrentSnapshot?.Players.FirstOrDefault(item => item.PlayerId == playerId);
            var camera = Camera.main;
            if (player == null || camera == null)
            {
                return;
            }

            var target = BattlePositionToWorld(player.PosX, player.PosY, -10f);
            camera.transform.position = new Vector3(target.x, target.y, -10f);
            ClampCameraToMap(camera);
            _cameraDragging = false;
        }

        public BattleBuildingStateInfo GetSelectedBuilding()
        {
            if (SelectedBuildingInstanceId <= 0 || CurrentSnapshot == null)
            {
                return null;
            }

            return CurrentSnapshot.Buildings.FirstOrDefault(item => item.InstanceId == SelectedBuildingInstanceId);
        }

        private async Fantasy.Async.FTask LoadBattleMapAsync(BattleStartInfo battle)
        {
            try
            {
                var mapAsset = NormalizeMapAssetName(battle?.MapAsset);
                var map = await TiledMapLoader.LoadAsync(mapAsset);
                if (await TryLoadUnityMapPrefabAsync(mapAsset, map))
                {
                    if (await ReportSceneLoadedAsync() && await WaitForBattleRunningAsync(battle.BattleId))
                    {
                        SetBattleSceneVisible(true);
                        StartSnapshotLoop();
                        StartMoveInput();
                    }

                    return;
                }

                CurrentMap = map;
                try
                {
                    BuildMapPreview(map);
                }
                catch (System.Exception exception)
                {
                    Log.Error($"Build battle map preview failed, continue battle sync: {exception}");
                }

                if (await ReportSceneLoadedAsync() && await WaitForBattleRunningAsync(battle.BattleId))
                {
                    SetBattleSceneVisible(true);
                    StartSnapshotLoop();
                    StartMoveInput();
                }
            }
            catch (System.Exception exception)
            {
                Log.Error($"Load battle map flow failed: {exception}");
            }
        }

        private async FTask<bool> TryLoadUnityMapPrefabAsync(string mapAsset, TiledMapData map)
        {
            foreach (var location in GetMapPrefabCandidateLocations(mapAsset))
            {
                if (!GameModule.Resource.CheckLocationValid(location))
                {
                    continue;
                }

                var prefab = await GameModule.Resource.LoadGameObjectAsync(location);
                if (prefab == null)
                {
                    continue;
                }

                BuildUnityMap(prefab, mapAsset, map, true);
                Log.Info($"Unity battle map prefab loaded: {location}");
                return true;
            }

#if UNITY_EDITOR
            var editorMap = LoadEditorTiledMap(mapAsset);
            if (editorMap != null)
            {
                BuildUnityMap(editorMap, mapAsset, map, false);
                Log.Info($"Unity editor Tiled map loaded: Assets/AssetRaw/TiledMaps/Maps/{mapAsset}.tmx");
                return true;
            }
#endif

            Log.Warning($"Unity battle map prefab not found for mapAsset={mapAsset}, fallback to Tiled JSON preview.");
            return false;
        }

        private static string[] GetMapPrefabCandidateLocations(string mapAsset)
        {
            var bareName = NormalizeMapAssetName(mapAsset);
            var prefabName = $"{bareName}.prefab";
            return new[]
            {
                $"MapPrefabs_{bareName}",
                $"MapPrefabs/{prefabName}",
                $"Assets/AssetRaw/MapPrefabs/{prefabName}"
            };
        }

#if UNITY_EDITOR
        private static GameObject LoadEditorTiledMap(string mapAsset)
        {
            var bareName = NormalizeMapAssetName(mapAsset);
            return AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/AssetRaw/TiledMaps/Maps/{bareName}.tmx");
        }
#endif

        private static string NormalizeMapAssetName(string mapAsset)
        {
            if (string.IsNullOrWhiteSpace(mapAsset))
            {
                return "battle_map_1";
            }

            var normalized = mapAsset.Replace("\\", "/").Trim();
            var slashIndex = normalized.LastIndexOf('/');
            if (slashIndex >= 0)
            {
                normalized = normalized.Substring(slashIndex + 1);
            }

            if (normalized.EndsWith(".prefab", System.StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring(0, normalized.Length - 7);
            }
            else if (normalized.EndsWith(".json", System.StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring(0, normalized.Length - 5);
            }

            return string.IsNullOrWhiteSpace(normalized) ? "battle_map_1" : normalized;
        }

        private void BuildUnityMap(GameObject mapObject, string mapAsset, TiledMapData map, bool mapObjectAlreadyInstantiated)
        {
            if (_mapRoot != null)
            {
                Object.Destroy(_mapRoot);
            }

            CurrentMap = map;
            _tileWorldSize = GetPrefabTileWorldSize(map);
            _mapRoot = new GameObject("BattleMap_UnityPrefab");
            SetDontDestroyOnLoad(_mapRoot);
            var mapInstance = mapObjectAlreadyInstantiated
                ? mapObject
                : Object.Instantiate(mapObject, _mapRoot.transform, false);
            if (mapInstance.transform.parent != _mapRoot.transform)
            {
                mapInstance.transform.SetParent(_mapRoot.transform, false);
            }

            mapInstance.name = mapAsset;
            mapInstance.transform.localPosition = Vector3.zero;
            mapInstance.transform.localRotation = Quaternion.identity;
            HideRuleLayerObjects(mapInstance.transform);
            _buildingRoot = new GameObject("BattleBuildings_ServerSnapshot");
            _buildingRoot.transform.SetParent(_mapRoot.transform, false);
            _playerRoot = new GameObject("BattlePlayers_ServerSnapshot");
            _playerRoot.transform.SetParent(_mapRoot.transform, false);
            _objectRoot = new GameObject("BattleObjects_UnityMap");
            _objectRoot.transform.SetParent(_mapRoot.transform, false);
            _effectRoot = new GameObject("BattleEffects");
            _effectRoot.transform.SetParent(_mapRoot.transform, false);
            SetBattleSceneVisible(false);

            var camera = Camera.main;
            if (camera != null)
            {
                FrameCameraToMap(camera, mapInstance, map, _tileWorldSize);
            }

            Log.Info($"Unity battle map scale: map={mapAsset}, grid={map?.Width ?? 0}x{map?.Height ?? 0}, tilePixels={map?.TileWidth ?? 0}, tileWorldSize={_tileWorldSize:0.###}");
        }

        private static void FrameCameraToMap(Camera camera, GameObject mapInstance, TiledMapData map, float tileWorldSize)
        {
            var width = Mathf.Max(map?.Width ?? 16, 1);
            var height = Mathf.Max(map?.Height ?? 9, 1);
            camera.orthographic = true;
            camera.orthographicSize = BattleCameraOrthographicSize;
            camera.transform.position = new Vector3(width * tileWorldSize * 0.5f, -height * tileWorldSize * 0.5f, -10f);
            Log.Info($"Battle map camera initialized: pos=({camera.transform.position.x:0.0},{camera.transform.position.y:0.0}), size={camera.orthographicSize:0.0}, map={mapInstance?.name}");
        }

        private async FTask<bool> ReportSceneLoadedAsync()
        {
            if (CurrentBattle == null)
            {
                return false;
            }

            try
            {
                var response = await SheepNetworkService.Instance.BattleSceneLoadedAsync(CurrentBattle.BattleId);
                ApplySnapshot(response.Snapshot);
                return response.Success;
            }
            catch (System.Exception exception)
            {
                Log.Error($"Report battle scene loaded failed: {exception}");
                return false;
            }
        }

        private async FTask<bool> WaitForBattleRunningAsync(int battleId)
        {
            while (CurrentBattle != null && CurrentBattle.BattleId == battleId)
            {
                if (CurrentSnapshot != null && CurrentSnapshot.State == "Running")
                {
                    Log.Info($"战斗全员加载完成，开始同步与输入：BattleId={battleId}");
                    return true;
                }

                try
                {
                    var response = await SheepNetworkService.Instance.RequestBattleSnapshotAsync(battleId, CurrentSnapshot?.Tick ?? 0);
                    if (response.Success)
                    {
                        ApplySnapshot(response.Snapshot);
                    }
                    else
                    {
                        Log.Warning($"Battle running wait rejected: {response.Message}");
                    }
                }
                catch (System.Exception exception)
                {
                    Log.Warning($"Battle running wait failed, retry later: {exception.Message}");
                }

                await UniTask.Delay(BattleRunningPollIntervalMs);
            }

            return false;
        }

        private void StartSnapshotLoop()
        {
            if (_syncing || CurrentBattle == null)
            {
                return;
            }

            _syncing = true;
            SnapshotLoopAsync(CurrentBattle.BattleId).Coroutine();
        }

        private async FTask SnapshotLoopAsync(int battleId)
        {
            while (_syncing && CurrentBattle != null && CurrentBattle.BattleId == battleId)
            {
                if (!SheepNetworkService.Instance.IsSessionAvailable)
                {
                    Log.Warning("Battle snapshot loop stopped: network session is unavailable.");
                    _syncing = false;
                    break;
                }

                try
                {
                    var response = await SheepNetworkService.Instance.RequestBattleSnapshotAsync(battleId, CurrentSnapshot?.Tick ?? 0);
                    if (response.Success)
                    {
                        ApplySnapshot(response.Snapshot);
                    }
                    else
                    {
                        Log.Warning($"Battle snapshot rejected: {response.Message}");
                    }
                }
                catch (System.Exception exception)
                {
                    if (IsSessionDisposedException(exception))
                    {
                        Log.Warning("Battle snapshot loop stopped: network session has been disposed.");
                        _syncing = false;
                        break;
                    }

                    Log.Error($"Battle snapshot loop error, retry later: {exception}");
                }

                await UniTask.Delay(200);
            }
        }

        private static bool IsSessionDisposedException(System.Exception exception)
        {
            return exception?.Message?.IndexOf("session is dispose", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                   exception?.ToString().IndexOf("session is dispose", System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private async FTask BuildAtAsync(int buildingId, int gridX, int gridY)
        {
            if (_buildRequesting)
            {
                Log.Info("Build request ignored: previous request is still pending.");
                return;
            }

            await RefreshBattleSnapshotOnceAsync();
            if (!CanBuildAt(buildingId, gridX, gridY, out var reason))
            {
                CommonNoticeService.Show(GetBuildRejectMessage(reason), "无法建造");
                Log.Warning($"Invalid build position: BuildingId={buildingId}, Grid={gridX},{gridY}, Reason={reason}");
                return;
            }

            _buildRequesting = true;
            try
            {
                var response = await SheepNetworkService.Instance.BuildAsync(CurrentBattle.BattleId, buildingId, gridX, gridY);
                if (!response.Success)
                {
                    CommonNoticeService.Show(response.Message, "建造失败");
                    Log.Warning($"Build rejected: BuildingId={buildingId}, Grid={gridX},{gridY}, Message={response.Message}");
                    ApplySnapshot(response.Snapshot);
                    return;
                }

                ApplySnapshot(response.Snapshot);
            }
            finally
            {
                _buildRequesting = false;
            }
        }
        private async FTask MoveInputLoopAsync(int battleId)
        {
            while (_moving && CurrentBattle != null && CurrentBattle.BattleId == battleId)
            {
                var moveAxis = GetMoveAxis();
                if (IsBuildMode)
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        ExitBuildMode();
                        await UniTask.Yield(PlayerLoopTiming.Update);
                        continue;
                    }

                    if (Mathf.Abs(moveAxis.x) > 0.01f || Mathf.Abs(moveAxis.y) > 0.01f)
                    {
                        ExitBuildMode();
                        TrySendMoveCommand(battleId, moveAxis.x, moveAxis.y);
                        await UniTask.Yield(PlayerLoopTiming.Update);
                        continue;
                    }

                    if (TrySelectBuildingFromClick())
                    {
                        await UniTask.Yield(PlayerLoopTiming.Update);
                        continue;
                    }

                    UpdateBuildPreview();
                    TryConfirmBuildPlacement();
                    UpdateCameraControl();
                    await UniTask.Yield(PlayerLoopTiming.Update);
                    continue;
                }

                TrySelectBuildingFromClick();
                TrySendMoveCommand(battleId, moveAxis.x, moveAxis.y);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }

        private bool TrySelectBuildingFromClick()
        {
            if (!Input.GetMouseButtonDown(0) || IsPointerOverUI())
            {
                return false;
            }

            var camera = Camera.main;
            if (camera == null)
            {
                return false;
            }

            var ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100f) && hit.collider != null && _buildingColliders.TryGetValue(hit.collider, out var instanceId))
            {
                SelectedBuildingInstanceId = instanceId;
                RefreshSelectedBuildingHelpers();
                Log.Info($"选中建筑：InstanceId={instanceId}");
                return true;
            }

            ClearSelectedBuilding();
            return false;
        }

        private static Vector2 GetMoveAxis()
        {
            var axisX = 0f;
            var axisY = 0f;
            if (Input.GetKey(KeyCode.A))
            {
                axisX -= 1f;
            }

            if (Input.GetKey(KeyCode.D))
            {
                axisX += 1f;
            }

            if (Input.GetKey(KeyCode.W))
            {
                axisY += 1f;
            }

            if (Input.GetKey(KeyCode.S))
            {
                axisY -= 1f;
            }

            return new Vector2(axisX, axisY);
        }

        private void TrySendMoveCommand(int battleId, float axisX, float axisY)
        {
            if (Mathf.Abs(axisX) <= 0.01f && Mathf.Abs(axisY) <= 0.01f)
            {
                return;
            }

            if (Time.realtimeSinceStartup - _lastMoveCommandAt < MoveCommandIntervalSeconds)
            {
                return;
            }

            _lastMoveCommandAt = Time.realtimeSinceStartup;
            var serial = ++_moveCommandSerial;
            SendMoveCommandAsync(battleId, axisX, axisY, serial).Coroutine();
        }

        private async FTask SendMoveCommandAsync(int battleId, float axisX, float axisY, int serial)
        {
            if (!SheepNetworkService.Instance.IsSessionAvailable)
            {
                _moving = false;
                return;
            }

            try
            {
                var response = await SheepNetworkService.Instance.MoveBattlePlayerAsync(battleId, axisX, axisY);
                if (response.Success)
                {
                    ApplySnapshot(response.Snapshot);
                }
                else
                {
                    Log.Warning($"Move input rejected: {response.Message}");
                }
            }
            catch (System.Exception exception)
            {
                if (IsSessionDisposedException(exception))
                {
                    Log.Warning("Move input stopped: network session has been disposed.");
                    _moving = false;
                    return;
                }

                Log.Error($"Move input failed, input loop will continue: {exception}");
            }
        }

        private async FTask RefreshBattleSnapshotOnceAsync()
        {
            if (CurrentBattle == null)
            {
                return;
            }

            try
            {
                var response = await SheepNetworkService.Instance.RequestBattleSnapshotAsync(CurrentBattle.BattleId, CurrentSnapshot?.Tick ?? 0);
                if (response.Success)
                {
                    ApplySnapshot(response.Snapshot);
                }
            }
            catch (System.Exception exception)
            {
                Log.Warning($"Refresh battle snapshot before build failed: {exception.Message}");
            }
        }

        private async FTask UpgradeAsync(long instanceId)
        {
            var response = await SheepNetworkService.Instance.UpgradeBuildingAsync(CurrentBattle.BattleId, instanceId);
            ApplySnapshot(response.Snapshot);
        }

        private async FTask RecycleAsync(long instanceId)
        {
            var response = await SheepNetworkService.Instance.RecycleBuildingAsync(CurrentBattle.BattleId, instanceId);
            ApplySnapshot(response.Snapshot);
        }

        private void ApplySnapshot(BattleSnapshotInfo snapshot)
        {
            if (snapshot == null)
            {
                return;
            }

            if (CurrentSnapshot != null && snapshot.Tick < CurrentSnapshot.Tick)
            {
                return;
            }

            CurrentSnapshot = snapshot;
            if (SelectedBuildingInstanceId > 0 && snapshot.Buildings.All(item => item.InstanceId != SelectedBuildingInstanceId))
            {
                SelectedBuildingInstanceId = 0;
            }

            try
            {
                TrackHealthChanges(snapshot);
                RefreshPlayerObjects(snapshot);
            }
            catch (System.Exception exception)
            {
                Log.Error($"Refresh battle players failed: {exception}");
            }

            try
            {
                RefreshBuildingObjects(snapshot);
                RefreshSelectedBuildingHelpers();
            }
            catch (System.Exception exception)
            {
                Log.Error($"Refresh battle buildings failed: {exception}");
            }

            try
            {
                PlayAttackEvents(snapshot);
            }
            catch (System.Exception exception)
            {
                Log.Error($"Play battle attack events failed: {exception}");
            }

            try
            {
                if (!IsBuildMode)
                {
                    FollowLocalPlayer();
                }
            }
            catch (System.Exception exception)
            {
                Log.Error($"Follow local player failed: {exception}");
            }

            Log.Info($"战斗快照：BattleId={snapshot.BattleId}, Tick={snapshot.Tick}, State={snapshot.State}, Players={snapshot.Players.Count}, Buildings={snapshot.Buildings.Count}");
        }

        private void BuildMapPreview(TiledMapData map)
        {
            if (_mapRoot != null)
            {
                Object.Destroy(_mapRoot);
            }

            _mapRoot = new GameObject("BattleMap_TiledPreview");
            SetDontDestroyOnLoad(_mapRoot);
            _buildingRoot = new GameObject("BattleBuildings_ServerSnapshot");
            _buildingRoot.transform.SetParent(_mapRoot.transform, false);
            _playerRoot = new GameObject("BattlePlayers_ServerSnapshot");
            _playerRoot.transform.SetParent(_mapRoot.transform, false);
            _objectRoot = new GameObject("BattleObjects_Tiled");
            _objectRoot.transform.SetParent(_mapRoot.transform, false);
            _effectRoot = new GameObject("BattleEffects");
            _effectRoot.transform.SetParent(_mapRoot.transform, false);
            SetBattleSceneVisible(false);

            var width = Mathf.Max(map?.Width ?? 16, 1);
            var height = Mathf.Max(map?.Height ?? 9, 1);
            _tileWorldSize = DefaultTileWorldSize;
            var camera = Camera.main;
            if (camera != null)
            {
                camera.orthographic = true;
                camera.orthographicSize = BattleCameraOrthographicSize;
                camera.transform.position = new Vector3(width * _tileWorldSize * 0.5f, -height * _tileWorldSize * 0.5f, -10f);
            }

            var ground = GameObject.CreatePrimitive(PrimitiveType.Quad);
            ground.name = "Ground_Preview";
            ground.transform.SetParent(_mapRoot.transform, false);
            ground.transform.position = new Vector3(width * _tileWorldSize * 0.5f, -height * _tileWorldSize * 0.5f, 0.05f);
            ground.transform.localScale = new Vector3(width * _tileWorldSize, height * _tileWorldSize, 1f);
            SetRenderOrder(ground, GroundSortingOrder);

            BuildObjectPreview(map, _tileWorldSize);
            Log.Info($"Fast battle map preview loaded: {map?.AssetName}, size={width}x{height}, tileWorldSize={_tileWorldSize:0.###}");
        }

        private static float GetPrefabTileWorldSize(TiledMapData map)
        {
            var tilePixels = Mathf.Max(map?.TileWidth ?? 0, map?.TileHeight ?? 0);
            return tilePixels > 0 ? tilePixels / TiledImporterPixelsPerUnit : DefaultTileWorldSize;
        }

        private void SetBattleSceneVisible(bool visible)
        {
            _battleSceneVisible = visible;
            if (visible && _battleStartedRealtime <= 0f)
            {
                _battleStartedRealtime = Time.realtimeSinceStartup;
            }

            if (_mapRoot != null)
            {
                _mapRoot.SetActive(visible);
            }

            SetBuildPreviewVisible(visible && IsBuildMode);
            SetBuildRangeVisible(visible && IsBuildMode);

            if (_selectedBuildingInfoRoot != null)
            {
                _selectedBuildingInfoRoot.SetActive(visible);
            }

            if (_selectedTowerRangeRoot != null)
            {
                _selectedTowerRangeRoot.SetActive(visible);
            }
        }

        private void RefreshPlayerObjects(BattleSnapshotInfo snapshot)
        {
            if (_playerRoot == null)
            {
                return;
            }

            for (var i = _playerRoot.transform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(_playerRoot.transform.GetChild(i).gameObject);
            }

            var myPlayerId = SheepNetworkService.Instance.Profile?.PlayerId ?? 0;
            for (var i = 0; i < snapshot.Players.Count; i++)
            {
                var player = snapshot.Players[i];
                var isLocalPlayer = player.PlayerId == myPlayerId;
                var marker = GameObject.CreatePrimitive(PrimitiveType.Quad);
                marker.name = $"player_{player.PlayerId}_{player.Nickname}";
                marker.transform.SetParent(_playerRoot.transform, false);
                marker.transform.position = BattlePositionToWorld(player.PosX, player.PosY, -0.75f);
                marker.transform.localScale = Vector3.one * PlayerMarkerSize * _tileWorldSize;
                SetRendererColor(marker, player.Camp == "Troll" ? new Color(0.8f, 0.16f, 0.1f, 1f) : new Color(0.22f, 0.75f, 1f, 1f));
                SetRenderOrder(marker, PlayerSortingOrder);
                AddHealthBar(marker.transform, player.PlayerId, player.Hp, player.MaxHp, 0.65f * _tileWorldSize);

                if (isLocalPlayer)
                {
                    _localPlayerPosition = marker.transform.position;
                    _hasLocalPlayerPosition = true;
                    _localPlayerCamp = string.IsNullOrWhiteSpace(player.Camp) ? "Elf" : player.Camp;
                    Log.Info($"Local player marker: playerId={player.PlayerId}, grid=({player.PosX:0.0},{player.PosY:0.0}), world=({_localPlayerPosition.x:0.0},{_localPlayerPosition.y:0.0})");
                }
            }
        }

        private void TrackHealthChanges(BattleSnapshotInfo snapshot)
        {
            for (var i = 0; i < snapshot.Players.Count; i++)
            {
                var player = snapshot.Players[i];
                if (_playerHpCache.TryGetValue(player.PlayerId, out var previousHp) && previousHp != player.Hp)
                {
                    _healthBarVisibleUntil[player.PlayerId] = Time.realtimeSinceStartup + HealthBarVisibleSeconds;
                }

                _playerHpCache[player.PlayerId] = player.Hp;
            }

            for (var i = 0; i < snapshot.Buildings.Count; i++)
            {
                var building = snapshot.Buildings[i];
                if (_buildingHpCache.TryGetValue(building.InstanceId, out var previousHp) && previousHp != building.Hp)
                {
                    _healthBarVisibleUntil[building.InstanceId] = Time.realtimeSinceStartup + HealthBarVisibleSeconds;
                }

                _buildingHpCache[building.InstanceId] = building.Hp;
            }
        }

        private void AddHealthBar(Transform parent, long id, int hp, int maxHp, float yOffset)
        {
            if (maxHp <= 0 || !_healthBarVisibleUntil.TryGetValue(id, out var visibleUntil) || Time.realtimeSinceStartup > visibleUntil)
            {
                return;
            }

            var root = new GameObject("HealthBar");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = new Vector3(0f, yOffset, -0.08f);
            root.transform.localScale = Vector3.one;

            var bg = GameObject.CreatePrimitive(PrimitiveType.Quad);
            bg.name = "HealthBarBg";
            bg.transform.SetParent(root.transform, false);
            bg.transform.localPosition = Vector3.zero;
            bg.transform.localScale = new Vector3(HealthBarWidth * _tileWorldSize, HealthBarHeight * _tileWorldSize, 1f);
            SetRendererColor(bg, new Color(0.08f, 0.08f, 0.08f, 0.85f));
            SetRenderOrder(bg, UiWorldSortingOrder);

            var fill = GameObject.CreatePrimitive(PrimitiveType.Quad);
            fill.name = "HealthBarFill";
            fill.transform.SetParent(root.transform, false);
            var ratio = Mathf.Clamp01(hp / (float)maxHp);
            fill.transform.localPosition = new Vector3((-HealthBarWidth * 0.5f + HealthBarWidth * 0.5f * ratio) * _tileWorldSize, 0f, -0.02f);
            fill.transform.localScale = new Vector3(HealthBarWidth * ratio * _tileWorldSize, HealthBarHeight * 0.72f * _tileWorldSize, 1f);
            SetRendererColor(fill, ratio > 0.35f ? new Color(0.1f, 0.86f, 0.25f, 0.95f) : new Color(0.95f, 0.16f, 0.1f, 0.95f));
            SetRenderOrder(fill, UiWorldSortingOrder + 1);
        }

        private void PlayAttackEvents(BattleSnapshotInfo snapshot)
        {
            if (snapshot.AttackEvents == null)
            {
                return;
            }

            for (var i = 0; i < snapshot.AttackEvents.Count; i++)
            {
                var attackEvent = snapshot.AttackEvents[i];
                if (!_playedAttackEvents.Add(attackEvent.EventId))
                {
                    continue;
                }

                SpawnProjectile(attackEvent);
            }

            if (_playedAttackEvents.Count > 128)
            {
                var recentMin = snapshot.AttackEvents.Count > 0 ? snapshot.AttackEvents.Min(item => item.EventId) : 0;
                _playedAttackEvents.RemoveWhere(id => id < recentMin);
            }
        }

        private void SpawnProjectile(BattleAttackEventInfo attackEvent)
        {
            var projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectile.name = $"Projectile_{attackEvent.EventId}";
            projectile.transform.SetParent(_effectRoot != null ? _effectRoot.transform : _mapRoot.transform, false);
            projectile.transform.position = BattlePositionToWorld(attackEvent.FromX, attackEvent.FromY, -0.9f);
            projectile.transform.localScale = Vector3.one * 0.18f * _tileWorldSize;
            SetRendererColor(projectile, new Color(1f, 0.82f, 0.18f, 1f));
            SetRenderOrder(projectile, EffectSortingOrder);
            MoveProjectileAsync(projectile, attackEvent).Coroutine();
        }

        private async FTask MoveProjectileAsync(GameObject projectile, BattleAttackEventInfo attackEvent)
        {
            var start = BattlePositionToWorld(attackEvent.FromX, attackEvent.FromY, -0.9f);
            var startedAt = Time.realtimeSinceStartup;
            while (projectile != null)
            {
                var progress = Mathf.Clamp01((Time.realtimeSinceStartup - startedAt) / ProjectileDurationSeconds);
                var end = GetProjectileTargetWorld(attackEvent);
                projectile.transform.position = Vector3.Lerp(start, end, progress);
                if (progress >= 1f)
                {
                    Object.Destroy(projectile);
                    return;
                }

                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }

        private Vector3 GetProjectileTargetWorld(BattleAttackEventInfo attackEvent)
        {
            var target = CurrentSnapshot?.Players.FirstOrDefault(item => item.PlayerId == attackEvent.TargetPlayerId);
            return target == null
                ? BattlePositionToWorld(attackEvent.ToX, attackEvent.ToY, -0.9f)
                : BattlePositionToWorld(target.PosX, target.PosY, -0.9f);
        }

        private void UpdateCameraControl()
        {
            if (!IsBuildMode)
            {
                return;
            }

            var camera = Camera.main;
            if (camera == null)
            {
                return;
            }

            if (IsPointerOverUI())
            {
                _cameraDragging = false;
                return;
            }

            if (Input.GetMouseButtonDown(0) && _buildPreviewValid)
            {
                _cameraDragging = false;
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                _cameraDragging = true;
                _lastDragWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _cameraDragging = false;
            }

            if (!_cameraDragging || !Input.GetMouseButton(0))
            {
                return;
            }

            var currentWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            var delta = _lastDragWorldPosition - currentWorldPosition;
            camera.transform.position += new Vector3(delta.x, delta.y, 0f);
            ClampCameraToMap(camera);
            _lastDragWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        }

        private void UpdateBuildPreview()
        {
            EnsureBuildRangeIndicator();
            if (SelectedBuildingId <= 0)
            {
                SetBuildPreviewVisible(false);
                UpdateBuildRangePosition();
                return;
            }

            EnsureBuildPreview();
            if (!TryGetMouseGrid(out var gridX, out var gridY))
            {
                SetBuildPreviewVisible(false);
                return;
            }

            _buildPreviewGridX = gridX;
            _buildPreviewGridY = gridY;
            _buildPreviewValid = CanBuildAt(SelectedBuildingId, gridX, gridY);
            _buildPreviewRoot.transform.position = GetBuildingCenterWorld(SelectedBuildingId, gridX, gridY, -0.28f);
            SetBuildPreviewVisible(true);
            UpdateBuildPreviewSize(SelectedBuildingId);
            SetBuildPreviewColor(_buildPreviewValid ? new Color(0.18f, 0.58f, 1f, 0.55f) : new Color(1f, 0.16f, 0.12f, 0.55f));
            UpdateBuildRangePosition();
        }

        private void TryConfirmBuildPlacement()
        {
            if (SelectedBuildingId <= 0)
            {
                return;
            }

            if (IsPointerOverUI())
            {
                return;
            }

            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            if (_buildRequesting)
            {
                Log.Info("Build click ignored: previous request is still pending.");
                return;
            }

            if (!TryGetMouseGrid(out var gridX, out var gridY))
            {
                Log.Warning("Build click ignored: mouse is outside valid map grid.");
                return;
            }

            _buildPreviewGridX = gridX;
            _buildPreviewGridY = gridY;
            _buildPreviewValid = CanBuildAt(SelectedBuildingId, gridX, gridY, out var reason);
            if (!_buildPreviewValid)
            {
                if (reason == "target area occupied")
                {
                    Log.Info($"Build click needs fresh snapshot before occupied reject: BuildingId={SelectedBuildingId}, Grid={gridX},{gridY}");
                    RequestBuildAt(gridX, gridY);
                    return;
                }

                CommonNoticeService.Show(GetBuildRejectMessage(reason), "无法建造");
                Log.Warning($"Build click ignored: BuildingId={SelectedBuildingId}, Grid={gridX},{gridY}, Reason={reason}");
                return;
            }

            _cameraDragging = false;
            LogBuildRangeCheck("Build request", SelectedBuildingId, gridX, gridY);
            RequestBuildAt(gridX, gridY);
        }

        private bool TryGetMouseGrid(out int gridX, out int gridY)
        {
            gridX = 0;
            gridY = 0;
            var camera = Camera.main;
            if (camera == null)
            {
                return false;
            }

            var world = camera.ScreenToWorldPoint(Input.mousePosition);
            gridX = Mathf.FloorToInt(world.x / _tileWorldSize);
            gridY = Mathf.FloorToInt(-world.y / _tileWorldSize);
            return gridX >= 0 && gridY >= 0;
        }

        private bool CanBuildAt(int buildingId, int gridX, int gridY)
        {
            return CanBuildAt(buildingId, gridX, gridY, out _);
        }

        private bool CanBuildAt(int buildingId, int gridX, int gridY, out string reason)
        {
            var config = ConfigSystem.Instance.Tables.TbBuilding.GetOrDefault(buildingId);
            if (config == null)
            {
                reason = "building config not found";
                return false;
            }

            if (CurrentSnapshot == null)
            {
                reason = "battle snapshot not ready";
                return false;
            }

            if (!_hasLocalPlayerPosition)
            {
                reason = "local player position not ready";
                return false;
            }

            if (_localPlayerCamp == "Troll")
            {
                reason = "troll cannot build";
                return false;
            }

            var width = Mathf.Max(config.FootprintWidth, 1);
            var height = Mathf.Max(config.FootprintHeight, 1);
            if (!IsBuildingAreaInMap(gridX, gridY, width, height))
            {
                reason = "outside map";
                return false;
            }

            if (IsBuildingAreaForbiddenByMap(gridX, gridY, width, height))
            {
                reason = "map blocked";
                return false;
            }

            if (!IsAnyBuildCellInRange(gridX, gridY, width, height))
            {
                reason = "out of build range";
                return false;
            }

            if (IsBuildingAreaOccupied(gridX, gridY, width, height))
            {
                reason = "target area occupied";
                return false;
            }

            reason = string.Empty;
            return true;
        }

        private static string GetBuildRejectMessage(string reason)
        {
            return reason switch
            {
                "building config not found" => "建筑配置不存在",
                "battle snapshot not ready" => "战斗数据同步中",
                "local player position not ready" => "玩家位置同步中",
                "troll cannot build" => "巨魔不能建造",
                "outside map" => "超出地图范围",
                "map blocked" => "当前位置不能建造",
                "out of build range" => "超出建造范围",
                "target area occupied" => "目标格子已被占用",
                _ => "当前位置不能建造"
            };
        }

        private bool IsBuildingAreaInMap(int gridX, int gridY, int width, int height)
        {
            if (CurrentMap == null)
            {
                return true;
            }

            return gridX >= 0 &&
                   gridY >= 0 &&
                   gridX + width <= CurrentMap.Width &&
                   gridY + height <= CurrentMap.Height;
        }

        private bool IsBuildingAreaForbiddenByMap(int gridX, int gridY, int width, int height)
        {
            if (CurrentMap == null)
            {
                return false;
            }

            for (var y = gridY; y < gridY + height; y++)
            {
                for (var x = gridX; x < gridX + width; x++)
                {
                    if (CurrentMap.IsBuildForbiddenTile(x, y))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsAnyBuildCellInRange(int gridX, int gridY, int width, int height)
        {
            var player = WorldToBattlePosition(_localPlayerPosition);
            var closestX = Mathf.Clamp(player.x, gridX, gridX + width);
            var closestY = Mathf.Clamp(player.y, gridY, gridY + height);
            return Vector2.Distance(player, new Vector2(closestX, closestY)) <= BuildRange;
        }

        private void LogBuildRangeCheck(string prefix, int buildingId, int gridX, int gridY)
        {
            var config = ConfigSystem.Instance.Tables.TbBuilding.GetOrDefault(buildingId);
            var width = Mathf.Max(config?.FootprintWidth ?? 1, 1);
            var height = Mathf.Max(config?.FootprintHeight ?? 1, 1);
            var player = WorldToBattlePosition(_localPlayerPosition);
            var closestX = Mathf.Clamp(player.x, gridX, gridX + width);
            var closestY = Mathf.Clamp(player.y, gridY, gridY + height);
            var distance = Vector2.Distance(player, new Vector2(closestX, closestY));
            Log.Info($"{prefix}: BuildingId={buildingId}, Grid=({gridX},{gridY}), Size=({width},{height}), Player=({player.x:0.00},{player.y:0.00}), Closest=({closestX:0.00},{closestY:0.00}), Distance={distance:0.00}, Range={BuildRange:0.00}");
        }

        private bool IsBuildingAreaOccupied(int gridX, int gridY, int width, int height)
        {
            if (CurrentSnapshot?.Buildings == null)
            {
                return false;
            }

            for (var i = 0; i < CurrentSnapshot.Buildings.Count; i++)
            {
                var building = CurrentSnapshot.Buildings[i];
                var config = ConfigSystem.Instance.Tables.TbBuilding.GetOrDefault(building.BuildingId);
                var otherWidth = Mathf.Max(config?.FootprintWidth ?? 1, 1);
                var otherHeight = Mathf.Max(config?.FootprintHeight ?? 1, 1);
                if (gridX < building.GridX + otherWidth &&
                    gridX + width > building.GridX &&
                    gridY < building.GridY + otherHeight &&
                    gridY + height > building.GridY)
                {
                    return true;
                }
            }

            for (var i = 0; i < CurrentSnapshot.Players.Count; i++)
            {
                var player = CurrentSnapshot.Players[i];
                if (player.Camp != "Troll")
                {
                    continue;
                }

                var trollGridX = Mathf.FloorToInt(player.PosX);
                var trollGridY = Mathf.FloorToInt(player.PosY);
                if (gridX < trollGridX + 1 &&
                    gridX + width > trollGridX &&
                    gridY < trollGridY + 1 &&
                    gridY + height > trollGridY)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsPointerOverUI()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }

        private void EnsureBuildPreview()
        {
            if (_buildPreviewRoot != null)
            {
                return;
            }

            _buildPreviewRoot = new GameObject("BuildPreview");
            SetDontDestroyOnLoad(_buildPreviewRoot);
            _buildPreviewBody = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _buildPreviewBody.name = "BuildPreviewBody";
            _buildPreviewBody.transform.SetParent(_buildPreviewRoot.transform, false);
            _buildPreviewBody.transform.localPosition = Vector3.zero;
            _buildPreviewBody.transform.localScale = new Vector3(0.82f, 0.82f, 0.82f);
            SetRenderOrder(_buildPreviewBody, EffectSortingOrder);
            _buildPreviewArea = GameObject.CreatePrimitive(PrimitiveType.Quad);
            _buildPreviewArea.name = "BuildPreviewArea";
            _buildPreviewArea.transform.SetParent(_buildPreviewRoot.transform, false);
            _buildPreviewArea.transform.localPosition = new Vector3(0f, 0f, 0.08f);
            _buildPreviewArea.transform.localScale = Vector3.one;
            SetRenderOrder(_buildPreviewArea, EffectSortingOrder - 1);
            SetBuildPreviewVisible(false);
        }

        private void UpdateBuildPreviewSize(int buildingId)
        {
            var config = ConfigSystem.Instance.Tables.TbBuilding.GetOrDefault(buildingId);
            var width = Mathf.Max(config?.FootprintWidth ?? 1, 1);
            var height = Mathf.Max(config?.FootprintHeight ?? 1, 1);
            if (_buildPreviewBody != null)
            {
                _buildPreviewBody.transform.localScale = new Vector3(width * _tileWorldSize * 0.82f, height * _tileWorldSize * 0.82f, _tileWorldSize * 0.82f);
            }

            if (_buildPreviewArea != null)
            {
                _buildPreviewArea.transform.localScale = new Vector3(width * _tileWorldSize, height * _tileWorldSize, 1f);
            }
        }

        private void SetBuildPreviewVisible(bool visible)
        {
            if (_buildPreviewRoot != null)
            {
                _buildPreviewRoot.SetActive(visible && _battleSceneVisible);
            }
        }

        private void SetBuildPreviewColor(Color color)
        {
            SetRendererColor(_buildPreviewBody, color);
            SetRendererColor(_buildPreviewArea, new Color(color.r, color.g, color.b, 0.32f));
        }

        private static void SetRendererColor(GameObject target, Color color)
        {
            if (target == null)
            {
                return;
            }

            var renderers = target.GetComponentsInChildren<Renderer>(true);
            for (var i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].material != null)
                {
                    renderers[i].material.color = color;
                }
            }
        }

        private static void SetRenderOrder(GameObject target, int sortingOrder)
        {
            if (target == null)
            {
                return;
            }

            var renderers = target.GetComponentsInChildren<Renderer>(true);
            for (var i = 0; i < renderers.Length; i++)
            {
                renderers[i].sortingOrder = sortingOrder;
            }
        }

        private static void HideRuleLayerObjects(Transform root)
        {
            if (root == null)
            {
                return;
            }

            for (var i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                if (IsRuleLayerObject(child.name))
                {
                    child.gameObject.SetActive(false);
                    continue;
                }

                HideRuleLayerObjects(child);
            }
        }

        private static bool IsRuleLayerObject(string objectName)
        {
            return string.Equals(objectName, "no_move", System.StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(objectName, "no_build", System.StringComparison.OrdinalIgnoreCase);
        }

        private static BuildingLevelConfig GetBuildingLevel(int buildingId, int level)
        {
            return ConfigSystem.Instance.Tables.TbBuildingLevel.DataList.FirstOrDefault(item =>
                item.BuildingId == buildingId && item.Level == level);
        }

        private void EnsureBuildRangeIndicator()
        {
            if (_buildRangeRoot != null)
            {
                SetBuildRangeVisible(_battleSceneVisible);
                return;
            }

            _buildRangeRoot = new GameObject("BuildRangeIndicator");
            SetDontDestroyOnLoad(_buildRangeRoot);
            var radius = BuildRange * _tileWorldSize;
            CreateDistanceSpriteOrCircle(_buildRangeRoot.transform, "build_distance", radius, EffectSortingOrder - 2);

            UpdateBuildRangePosition();
            SetBuildRangeVisible(_battleSceneVisible);
        }

        private void UpdateBuildRangePosition()
        {
            if (_buildRangeRoot != null && _hasLocalPlayerPosition)
            {
                _buildRangeRoot.transform.position = new Vector3(_localPlayerPosition.x, _localPlayerPosition.y, 0f);
            }
        }

        private void SetBuildRangeVisible(bool visible)
        {
            if (_buildRangeRoot != null)
            {
                _buildRangeRoot.SetActive(visible && _battleSceneVisible);
            }
        }

        private void RefreshSelectedBuildingHelpers()
        {
            DestroySelectedBuildingHelpers();
            var selected = GetSelectedBuilding();
            if (selected == null)
            {
                return;
            }

            var config = ConfigSystem.Instance.Tables.TbBuilding.GetOrDefault(selected.BuildingId);
            var level = GetBuildingLevel(selected.BuildingId, selected.Level);
            var width = Mathf.Max(config?.FootprintWidth ?? 1, 1);
            var height = Mathf.Max(config?.FootprintHeight ?? 1, 1);
            var center = GetBuildingCenterWorld(selected.BuildingId, selected.GridX, selected.GridY, -0.92f);
            var topOffset = (height * 0.5f + 0.6f) * _tileWorldSize;

            _selectedBuildingInfoRoot = new GameObject("SelectedBuildingInfo");
            _selectedBuildingInfoRoot.transform.SetParent(_effectRoot != null ? _effectRoot.transform : _mapRoot.transform, false);
            _selectedBuildingInfoRoot.transform.position = new Vector3(center.x, center.y + topOffset, center.z);
            AddSelectedInfoText(_selectedBuildingInfoRoot.transform, config?.BuildingName ?? selected.BuildingId.ToString(), 0f, 0.13f);
            AddSelectedInfoText(_selectedBuildingInfoRoot.transform, $"{selected.Level}级", -SelectedInfoLineHeight * _tileWorldSize, 0.11f);

            if (string.Equals(config?.BuildingType, "Tower", System.StringComparison.OrdinalIgnoreCase))
            {
                var range = Mathf.Max(level?.AttackRange ?? 0, 0);
                if (range > 0)
                {
                    _selectedTowerRangeRoot = CreateAttackRangeIndicator(center, range * _tileWorldSize);
                }
            }
        }

        private void AddSelectedInfoText(Transform parent, string content, float yOffset, float characterSize)
        {
            AddWorldLabel(parent, content, yOffset, Color.white, UiWorldSortingOrder + 2, characterSize);
        }

        private void AddWorldLabel(Transform parent, string content, float yOffset, Color color, int sortingOrder, float characterSize = 0.13f)
        {
            var textObject = new GameObject("SelectedBuildingText");
            textObject.transform.SetParent(parent, false);
            textObject.transform.localPosition = new Vector3(0f, yOffset, 0f);
            var text = textObject.AddComponent<TextMesh>();
            text.text = content ?? string.Empty;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.characterSize = characterSize * _tileWorldSize;
            text.fontSize = 32;
            text.color = color;
            var renderer = textObject.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = sortingOrder;
            }
        }

        private GameObject CreateAttackRangeIndicator(Vector3 center, float radius)
        {
            var root = new GameObject("SelectedTowerRange");
            root.transform.SetParent(_effectRoot != null ? _effectRoot.transform : _mapRoot.transform, false);
            root.transform.position = new Vector3(center.x, center.y, -0.94f);
            CreateDistanceSpriteOrCircle(root.transform, "attack_distance", radius, EffectSortingOrder - 1);

            return root;
        }

        private void CreateDistanceSpriteOrCircle(Transform parent, string objectName, float radius, int sortingOrder)
        {
            var sprite = GetAttackDistanceSprite();
            if (sprite == null)
            {
                CreateRangeCircle(parent, objectName, radius, 96, 0.07f * _tileWorldSize, sortingOrder);
                return;
            }

            var spriteObject = new GameObject(objectName);
            spriteObject.transform.SetParent(parent, false);
            spriteObject.transform.localPosition = Vector3.zero;
            var renderer = spriteObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;

            var bounds = sprite.bounds.size;
            var diameter = Mathf.Max(radius * 2f, 0.01f);
            var scaleX = diameter / Mathf.Max(bounds.x, 0.01f);
            var scaleY = diameter / Mathf.Max(bounds.y, 0.01f);
            spriteObject.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }

        private Sprite GetAttackDistanceSprite()
        {
            if (_attackDistanceSprite != null)
            {
                return _attackDistanceSprite;
            }

            if (!GameModule.Resource.CheckLocationValid(AttackDistanceSpriteLocation))
            {
                return null;
            }

            _attackDistanceSprite = GameModule.Resource.LoadAsset<Sprite>(AttackDistanceSpriteLocation);
            if (_attackDistanceSprite != null)
            {
                return _attackDistanceSprite;
            }

            var texture = GameModule.Resource.LoadAsset<Texture2D>(AttackDistanceSpriteLocation);
            if (texture == null)
            {
                return null;
            }

            _attackDistanceSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f);
            return _attackDistanceSprite;
        }

        private GameObject CreateRangeCircle(string objectName, Vector3 center, float radius, int segmentCount, float segmentWidth)
        {
            var root = new GameObject(objectName);
            root.transform.SetParent(_effectRoot != null ? _effectRoot.transform : _mapRoot.transform, false);
            root.transform.position = new Vector3(center.x, center.y, -0.94f);
            CreateRangeCircle(root.transform, "RangeSegment", radius, segmentCount, segmentWidth, EffectSortingOrder - 1);

            return root;
        }

        private GameObject CreateRangeCircle(string objectName, Vector3 center, float radius, int segmentCount, float segmentWidth, int sortingOrder)
        {
            var root = new GameObject(objectName);
            root.transform.SetParent(_effectRoot != null ? _effectRoot.transform : _mapRoot.transform, false);
            root.transform.position = center;
            CreateRangeCircle(root.transform, "RangeSegment", radius, segmentCount, segmentWidth, sortingOrder);

            return root;
        }

        private void CreateRangeCircle(Transform parent, string segmentNamePrefix, float radius, int segmentCount, float segmentWidth, int sortingOrder)
        {
            var segmentLength = Mathf.Max(0.04f * _tileWorldSize, 2f * Mathf.PI * radius / segmentCount * 0.72f);

            for (var i = 0; i < segmentCount; i++)
            {
                var angle = i / (float)segmentCount * Mathf.PI * 2f;
                var segment = GameObject.CreatePrimitive(PrimitiveType.Quad);
                segment.name = $"{segmentNamePrefix}_{i:00}";
                segment.transform.SetParent(parent, false);
                segment.transform.localPosition = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
                segment.transform.localRotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg);
                segment.transform.localScale = new Vector3(segmentLength, segmentWidth, 1f);
                SetRenderOrder(segment, sortingOrder);
            }
        }

        private void DestroySelectedBuildingHelpers()
        {
            if (_selectedBuildingInfoRoot != null)
            {
                Object.Destroy(_selectedBuildingInfoRoot);
                _selectedBuildingInfoRoot = null;
            }

            if (_selectedTowerRangeRoot != null)
            {
                Object.Destroy(_selectedTowerRangeRoot);
                _selectedTowerRangeRoot = null;
            }
        }

        private static void SetDontDestroyOnLoad(GameObject target)
        {
            if (target != null && Application.isPlaying)
            {
                Object.DontDestroyOnLoad(target);
            }
        }

        private void DestroyBuildHelpers()
        {
            if (_buildPreviewRoot != null)
            {
                Object.Destroy(_buildPreviewRoot);
                _buildPreviewRoot = null;
                _buildPreviewBody = null;
                _buildPreviewArea = null;
            }

            if (_buildRangeRoot != null)
            {
                Object.Destroy(_buildRangeRoot);
                _buildRangeRoot = null;
            }
        }

        private void FollowLocalPlayer()
        {
            if (!_hasLocalPlayerPosition)
            {
                return;
            }

            var camera = Camera.main;
            if (camera == null)
            {
                return;
            }

            camera.transform.position = new Vector3(_localPlayerPosition.x, _localPlayerPosition.y, -10f);
            ClampCameraToMap(camera);
        }

        private void ClampCameraToMap(Camera camera)
        {
            if (CurrentMap == null)
            {
                return;
            }

            var maxX = Mathf.Max(CurrentMap.Width * _tileWorldSize, 1f);
            var minY = -Mathf.Max(CurrentMap.Height * _tileWorldSize, 1f);
            var halfHeight = camera.orthographicSize;
            var halfWidth = halfHeight * camera.aspect;
            var minX = halfWidth;
            var clampedMaxX = Mathf.Max(minX, maxX - halfWidth);
            var clampedMinY = Mathf.Min(-halfHeight, minY + halfHeight);
            var clampedMaxY = -halfHeight;
            var position = camera.transform.position;
            position.x = Mathf.Clamp(position.x, minX, clampedMaxX);
            position.y = Mathf.Clamp(position.y, clampedMinY, clampedMaxY);
            position.z = -10f;
            camera.transform.position = position;
        }

        private void RefreshBuildingObjects(BattleSnapshotInfo snapshot)
        {
            if (_buildingRoot == null)
            {
                return;
            }

            for (var i = _buildingRoot.transform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(_buildingRoot.transform.GetChild(i).gameObject);
            }

            _buildingColliders.Clear();
            for (var i = 0; i < snapshot.Buildings.Count; i++)
            {
                var building = snapshot.Buildings[i];
                var config = ConfigSystem.Instance.Tables.TbBuilding.GetOrDefault(building.BuildingId);
                var buildingName = config?.BuildingName ?? building.BuildingId.ToString();
                var width = Mathf.Max(config?.FootprintWidth ?? 1, 1);
                var height = Mathf.Max(config?.FootprintHeight ?? 1, 1);
                var marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
                marker.name = $"building_{building.InstanceId}_{buildingName}";
                marker.transform.SetParent(_buildingRoot.transform, false);
                marker.transform.position = GetBuildingCenterWorld(building.BuildingId, building.GridX, building.GridY, -0.35f);
                marker.transform.localScale = new Vector3(width * _tileWorldSize * 0.82f, height * _tileWorldSize * 0.82f, _tileWorldSize * 0.82f);
                SetRenderOrder(marker, BuildingSortingOrder);
                AddHealthBar(marker.transform, building.InstanceId, building.Hp, building.MaxHp, (height * 0.55f + 0.2f) * _tileWorldSize);
                var collider = marker.GetComponent<Collider>();
                if (collider != null)
                {
                    _buildingColliders[collider] = building.InstanceId;
                }
            }
        }

        private void BuildObjectPreview(TiledMapData map, float tileSize)
        {
            if (map?.layers == null)
            {
                return;
            }

            for (var i = 0; i < map.layers.Length; i++)
            {
                var layer = map.layers[i];
                if (layer?.objects == null)
                {
                    continue;
                }

                if (layer.name == "shop")
                {
                    continue;
                }

                if (layer.name == "monster")
                {
                    BuildMonsterObjects(layer, tileSize);
                }
                else if (layer.name == "birth_area")
                {
                    BuildBirthAreas(layer, tileSize);
                }
            }
        }

        private void BuildShopObjects(TiledLayerData layer, float tileSize)
        {
            for (var i = 0; i < layer.objects.Length; i++)
            {
                var mapObject = layer.objects[i];
                var shopId = mapObject.GetIntProperty("shop_id", mapObject.GetIntProperty("shopid"));
                var config = ConfigSystem.Instance.Tables.TbBattleShop.GetOrDefault(shopId);
                var marker = CreateObjectMarker(mapObject, tileSize, $"shop_{shopId}");
                marker.name = $"shop_{shopId}_{config?.ShopName ?? mapObject.name}";
                SetRendererColor(marker, new Color(1f, 0.72f, 0.16f, 1f));
                AddWorldLabel(marker.transform, "商店", 0.62f * tileSize, new Color(1f, 0.92f, 0.46f, 1f), UiWorldSortingOrder + 2);
                var range = Mathf.Max(mapObject.GetFloatProperty("shoprange", 0f), 0f);
                if (range > 0f)
                {
                    var center = marker.transform.position;
                    CreateRangeCircle($"shop_range_{shopId}", new Vector3(center.x, center.y, -0.89f), range * tileSize, 96, 0.055f * tileSize, ObjectSortingOrder - 1);
                }

                Log.Info($"Tiled shop point: shopId={shopId}, name={config?.ShopName}, goodsGroup={config?.GoodsGroupId}, range={range:0.##}");
            }
        }

        private void BuildMonsterObjects(TiledLayerData layer, float tileSize)
        {
            for (var i = 0; i < layer.objects.Length; i++)
            {
                var mapObject = layer.objects[i];
                var monsterId = mapObject.GetIntProperty("monster_id", mapObject.GetIntProperty("monsterid"));
                var config = ConfigSystem.Instance.Tables.TbMonster.GetOrDefault(monsterId);
                var marker = CreateObjectMarker(mapObject, tileSize, $"monster_{monsterId}");
                marker.name = $"monster_{monsterId}_{config?.MonsterName ?? mapObject.name}";
                Log.Info($"Tiled monster point: monsterId={monsterId}, name={config?.MonsterName}, prefab={config?.PrefabAsset}");
            }
        }

        private void BuildBirthAreas(TiledLayerData layer, float tileSize)
        {
            for (var i = 0; i < layer.objects.Length; i++)
            {
                var mapObject = layer.objects[i];
                var marker = CreateAreaMarker(mapObject, tileSize, $"birth_area_{i + 1}");
                marker.name = string.IsNullOrWhiteSpace(mapObject?.name) ? $"birth_area_{i + 1}" : mapObject.name;
                Log.Info($"Tiled birth area: name={marker.name}, x={mapObject?.x}, y={mapObject?.y}, width={mapObject?.width}, height={mapObject?.height}");
            }
        }

        private GameObject CreateObjectMarker(TiledObjectData mapObject, float tileSize, string fallbackName)
        {
            var marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            marker.name = string.IsNullOrWhiteSpace(mapObject?.name) ? fallbackName : mapObject.name;
            marker.transform.SetParent(_objectRoot != null ? _objectRoot.transform : _mapRoot.transform, false);
            var grid = ObjectPixelToGrid(mapObject);
            marker.transform.position = GridToWorld(grid.x, grid.y, -0.88f);
            marker.transform.localScale = new Vector3(tileSize * 0.46f, tileSize * 0.18f, tileSize * 0.46f);
            SetRenderOrder(marker, ObjectSortingOrder);

            return marker;
        }

        private GameObject CreateAreaMarker(TiledObjectData mapObject, float tileSize, string fallbackName)
        {
            var marker = GameObject.CreatePrimitive(PrimitiveType.Quad);
            marker.name = string.IsNullOrWhiteSpace(mapObject?.name) ? fallbackName : mapObject.name;
            marker.transform.SetParent(_objectRoot != null ? _objectRoot.transform : _mapRoot.transform, false);

            var tilePixelWidth = Mathf.Max(CurrentMap?.TileWidth ?? 32, 1);
            var tilePixelHeight = Mathf.Max(CurrentMap?.TileHeight ?? tilePixelWidth, 1);
            var width = Mathf.Max((mapObject?.width ?? tilePixelWidth) / tilePixelWidth, 1f);
            var height = Mathf.Max((mapObject?.height ?? tilePixelHeight) / tilePixelHeight, 1f);
            var gridX = (mapObject?.x ?? 0f) / tilePixelWidth + width * 0.5f;
            var gridY = (mapObject?.y ?? 0f) / tilePixelHeight + height * 0.5f;

            marker.transform.position = GridToWorld(gridX - 0.5f, gridY - 0.5f, -0.15f);
            marker.transform.localScale = new Vector3(width * tileSize, height * tileSize, 1f);
            SetRenderOrder(marker, ObjectSortingOrder);

            return marker;
        }

        private Vector3 GridToWorld(float gridX, float gridY, float z)
        {
            return new Vector3((gridX + 0.5f) * _tileWorldSize, -(gridY + 0.5f) * _tileWorldSize, z);
        }

        private Vector3 BattlePositionToWorld(float posX, float posY, float z)
        {
            return new Vector3(posX * _tileWorldSize, -posY * _tileWorldSize, z);
        }

        private Vector2 WorldToBattlePosition(Vector3 world)
        {
            return new Vector2(world.x / _tileWorldSize, -world.y / _tileWorldSize);
        }

        private Vector3 GetBuildingCenterWorld(int buildingId, int gridX, int gridY, float z)
        {
            var config = ConfigSystem.Instance.Tables.TbBuilding.GetOrDefault(buildingId);
            var width = Mathf.Max(config?.FootprintWidth ?? 1, 1);
            var height = Mathf.Max(config?.FootprintHeight ?? 1, 1);
            return GridToWorld(gridX + (width - 1) * 0.5f, gridY + (height - 1) * 0.5f, z);
        }

        private Vector2 ObjectPixelToGrid(TiledObjectData mapObject)
        {
            var tilePixelWidth = Mathf.Max(CurrentMap?.TileWidth ?? 64, 1);
            var tilePixelHeight = Mathf.Max(CurrentMap?.TileHeight ?? tilePixelWidth, 1);
            return new Vector2((mapObject?.x ?? 0f) / tilePixelWidth, (mapObject?.y ?? 0f) / tilePixelHeight);
        }

        private static float BuildRange => Mathf.Max(GameRuleService.Instance.BuildRange, 0f);

    }
}

