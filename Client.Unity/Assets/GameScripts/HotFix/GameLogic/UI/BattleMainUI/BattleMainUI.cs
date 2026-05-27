using System.Collections.Generic;
using System.Linq;
using GameConfig.battle;
using GameLogic.SheepBattle.Battle;
using GameLogic.SheepBattle.Config;
using GameLogic.SheepBattle.Network;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI, location: "BattleMainUI")]
    internal sealed class BattleMainUI : UIWindow
    {
        private GameObject _goTopInfo;
        private GameObject _itemRoleInfo;
        private GameObject _itemMonsterInfo;
        private GameObject _itemTouch;
        private GameObject _legacyRootBackground;

        private Text _txtElfCount;
        private Text _txtBattleTime;
        private Text _txtTrollCount;
        private RectTransform _teammateAvatarRoot;
        private Image _avatarTemplate;
        private Text _txtGold;
        private Text _txtWood;
        private RectTransform _buildCardRoot;
        private Button _buildCardTemplate;
        private Text _txtBuildingInfo;
        private RectTransform _buildingOperationButtonRoot;
        private Button _buildingOperationButtonTemplate;
        private Text _txtSelectedHint;
        private Text _txtSyncState;
        private Text _txtPosition;

        private readonly List<GameObject> _spawnedAvatars = new();
        private readonly List<GameObject> _spawnedBuildCards = new();
        private readonly List<GameObject> _spawnedOperationButtons = new();
        private float _nextHudRefreshTime;
        private long _lastOperationBuildingId;
        private string _lastOperationSignature = string.Empty;
        private bool _buildCardsReady;

        protected override void ScriptGenerator()
        {
            _goTopInfo = FindChild("m_goTopInfo")?.gameObject;
            _itemRoleInfo = FindChild("m_goTopInfo/m_itemRoleInfo")?.gameObject;
            _itemMonsterInfo = FindChild("m_goTopInfo/m_itemMonsterInfo")?.gameObject;
            _itemTouch = FindChild("m_rectContainer/m_itemTouch")?.gameObject;
            _legacyRootBackground = FindChild("m_imgBg")?.gameObject
                                    ?? FindChild("m_imgBackground")?.gameObject
                                    ?? FindChild("Background")?.gameObject
                                    ?? FindChild("Bg")?.gameObject;

            _txtElfCount = FindHudComponent<Text>("m_txtElfCount");
            _txtBattleTime = FindHudComponent<Text>("m_txtBattleTime");
            _txtTrollCount = FindHudComponent<Text>("m_txtTrollCount");
            _teammateAvatarRoot = FindHudChild("m_avatarList") as RectTransform;
            _avatarTemplate = FindHudComponent<Image>("m_avatarTemplate");
            _txtGold = FindResourceValueText("m_goldRow");
            _txtWood = FindResourceValueText("m_woodRow");
            _buildCardRoot = FindHudChild("m_cardList") as RectTransform;
            _buildCardTemplate = FindHudComponent<Button>("m_buildCardTemplate");
            _txtBuildingInfo = FindHudComponent<Text>("m_txtBuildingInfo");
            _buildingOperationButtonRoot = FindHudChild("m_buttonList") as RectTransform;
            _buildingOperationButtonTemplate = FindHudComponent<Button>("m_operationButtonTemplate");
            _txtSelectedHint = FindHudComponent<Text>("m_txtSelectedHint");
            _txtSyncState = FindHudComponent<Text>("m_txtSyncState");
            _txtPosition = FindHudComponent<Text>("m_txtPosition");
        }

        protected override void OnCreate()
        {
            SetLegacyHudVisible(false);
            SetTemplateVisible(_avatarTemplate, false);
            SetTemplateVisible(_buildCardTemplate, false);
            SetTemplateVisible(_buildingOperationButtonTemplate, false);
            BuildBuildingCardsFromPrefab();
            RefreshBattleHud();
        }

        protected override void OnRefresh()
        {
            RefreshBattleHud();
        }

        protected override void OnUpdate()
        {
            if (Time.unscaledTime < _nextHudRefreshTime)
            {
                return;
            }

            _nextHudRefreshTime = Time.unscaledTime + 0.25f;
            RefreshBattleHud();
        }

        private void SetLegacyHudVisible(bool visible)
        {
            _legacyRootBackground?.SetActive(false);
            _goTopInfo?.SetActive(visible);
            _itemRoleInfo?.SetActive(visible);
            _itemMonsterInfo?.SetActive(visible);
            _itemTouch?.SetActive(visible);
        }

        private static void SetTemplateVisible(Component template, bool visible)
        {
            if (template != null)
            {
                template.gameObject.SetActive(visible);
            }
        }

        private void BuildBuildingCardsFromPrefab()
        {
            if (_buildCardsReady || _buildCardRoot == null || _buildCardTemplate == null)
            {
                return;
            }

            ClearSpawned(_spawnedBuildCards);
            var cards = ConfigSystem.Instance.Tables.TbBuildingCard.DataList
                .OrderBy(card => card.SortOrder)
                .ThenBy(card => card.CardId)
                .ToList();

            for (var i = 0; i < cards.Count; i++)
            {
                CreateBuildingCard(cards[i]);
            }

            _buildCardsReady = true;
        }

        private void CreateBuildingCard(BuildingCardConfig card)
        {
            var instance = Object.Instantiate(_buildCardTemplate.gameObject, _buildCardRoot, false);
            instance.name = $"m_buildCard_{card.CardId}";
            instance.SetActive(true);
            _spawnedBuildCards.Add(instance);

            SetChildText(instance.transform, "m_txtName", card.CardName);
            SetChildText(instance.transform, "m_txtCost", $"金 {card.CostGold}  木 {card.CostWood}");
            SetChildText(instance.transform, "m_txtDesc", string.Empty);

            var button = instance.GetComponent<Button>();
            button.onClick.AddListener(() => OnClickBuildCard(card));
        }

        private void RefreshBattleHud()
        {
            var snapshot = BattleController.Instance.CurrentSnapshot;
            var profile = SheepNetworkService.Instance.Profile;
            var me = snapshot?.Players.FirstOrDefault(item => item.PlayerId == profile?.PlayerId);

            RefreshCampScore(snapshot);
            RefreshTime();
            RefreshResources(me);
            RefreshTeammateAvatars(snapshot, me);
            RefreshStateTexts(snapshot, me);
            RefreshBuildingOperationButtons(me);
        }

        private void RefreshCampScore(Fantasy.BattleSnapshotInfo snapshot)
        {
            SetText(_txtElfCount, $"精灵  {snapshot?.Players.Count(IsElf) ?? 0}");
            SetText(_txtTrollCount, $"巨魔  {snapshot?.Players.Count(IsTroll) ?? 0}");
        }

        private void RefreshTime()
        {
            var duration = Mathf.Max(GameRuleService.Instance.BattleDurationSeconds, 0);
            var remain = Mathf.Max(duration - BattleController.Instance.BattleElapsedSeconds, 0);
            SetText(_txtBattleTime, FormatTime(remain));
        }

        private void RefreshResources(Fantasy.BattlePlayerStateInfo me)
        {
            SetText(_txtGold, me == null ? "--" : me.Gold.ToString());
            SetText(_txtWood, me == null ? "--" : me.Wood.ToString());
        }

        private void RefreshTeammateAvatars(Fantasy.BattleSnapshotInfo snapshot, Fantasy.BattlePlayerStateInfo me)
        {
            if (_teammateAvatarRoot == null || _avatarTemplate == null)
            {
                return;
            }

            ClearSpawned(_spawnedAvatars);
            if (snapshot == null)
            {
                return;
            }

            var myCamp = me?.Camp ?? "Elf";
            var teammates = snapshot.Players.Where(player => SameCamp(player.Camp, myCamp)).ToList();
            for (var i = 0; i < teammates.Count; i++)
            {
                CreateAvatar(teammates[i], me != null && teammates[i].PlayerId == me.PlayerId);
            }
        }

        private void CreateAvatar(Fantasy.BattlePlayerStateInfo player, bool isMe)
        {
            var instance = Object.Instantiate(_avatarTemplate.gameObject, _teammateAvatarRoot, false);
            instance.name = $"m_avatar_{player.PlayerId}";
            instance.SetActive(true);
            _spawnedAvatars.Add(instance);

            var image = instance.GetComponent<Image>();
            if (image != null)
            {
                image.color = isMe ? new Color(0.34f, 0.96f, 0.62f, 0.96f) : new Color(0.82f, 0.92f, 1f, 0.92f);
            }

            SetChildText(instance.transform, "m_txtInitial", GetAvatarInitial(player));
            SetChildText(instance.transform, "m_txtSelf", isMe ? "我" : string.Empty);

            var button = instance.GetComponent<Button>() ?? instance.AddComponent<Button>();
            button.targetGraphic = image;
            var playerId = player.PlayerId;
            button.onClick.AddListener(() => BattleController.Instance.FocusPlayer(playerId));
        }

        private void RefreshStateTexts(Fantasy.BattleSnapshotInfo snapshot, Fantasy.BattlePlayerStateInfo me)
        {
            SetText(_txtSyncState, snapshot == null
                ? "同步：等待服务器快照"
                : $"同步：{snapshot.State} Tick {snapshot.Tick} 玩家 {snapshot.Players.Count} 建筑 {snapshot.Buildings.Count}");
            SetText(_txtPosition, me == null ? "位置 --" : $"位置 {me.PosX:0.0}, {me.PosY:0.0}");

            var selectedId = BattleController.Instance.SelectedBuildingId;
            var selected = selectedId <= 0 ? null : ConfigSystem.Instance.Tables.TbBuilding.GetOrDefault(selectedId);
            SetText(_txtSelectedHint, selected == null ? "移动模式：WASD 控制角色" : $"建造模式：{selected.BuildingName}");
        }

        private void OnClickBuildCard(BuildingCardConfig card)
        {
            GameEvent.Get<IBattleCommand>()?.OnSelectBuilding(card.BuildingId);
            RefreshBattleHud();
        }

        private void RefreshBuildingOperationButtons(Fantasy.BattlePlayerStateInfo me)
        {
            if (_buildingOperationButtonRoot == null || _buildingOperationButtonTemplate == null)
            {
                return;
            }

            var selected = BattleController.Instance.GetSelectedBuilding();
            if (selected == null || BattleController.Instance.IsBuildMode)
            {
                ClearSpawned(_spawnedOperationButtons);
                _lastOperationBuildingId = 0;
                _lastOperationSignature = string.Empty;
                SetText(_txtBuildingInfo, string.Empty);
                return;
            }

            var config = ConfigSystem.Instance.Tables.TbBuilding.GetOrDefault(selected.BuildingId);
            var isOwner = me != null && selected.OwnerPlayerId == me.PlayerId;
            SetText(_txtBuildingInfo, FormatBuildingInfo(selected, config, false));

            var canUpgrade = CanUpgrade(selected, config, isOwner);
            var signature = $"{selected.InstanceId}:{isOwner}:{canUpgrade}";
            if (_lastOperationBuildingId == selected.InstanceId && _lastOperationSignature == signature)
            {
                return;
            }

            ClearSpawned(_spawnedOperationButtons);
            _lastOperationBuildingId = selected.InstanceId;
            _lastOperationSignature = signature;

            CreateOperationButton("信息", () => SetText(_txtBuildingInfo, FormatBuildingInfo(selected, config, true)));
            if (canUpgrade)
            {
                CreateOperationButton("升级", () => GameEvent.Get<IBattleCommand>()?.OnUpgradeBuilding(selected.InstanceId));
            }

            if (isOwner)
            {
                CreateOperationButton("回收", () => GameEvent.Get<IBattleCommand>()?.OnRecycleBuilding(selected.InstanceId));
            }
        }

        private void CreateOperationButton(string label, UnityEngine.Events.UnityAction onClick)
        {
            var instance = Object.Instantiate(_buildingOperationButtonTemplate.gameObject, _buildingOperationButtonRoot, false);
            instance.name = $"m_btnOperation_{label}";
            instance.SetActive(true);
            _spawnedOperationButtons.Add(instance);
            SetChildText(instance.transform, "m_txtLabel", label);
            instance.GetComponent<Button>().onClick.AddListener(onClick);
        }

        private static bool CanUpgrade(Fantasy.BattleBuildingStateInfo selected, BuildingConfig config, bool isOwner)
        {
            if (!isOwner || config == null || !config.CanUpgrade || selected.Level >= config.MaxLevel)
            {
                return false;
            }

            return ConfigSystem.Instance.Tables.TbBuildingLevel.DataList.Any(level =>
                level.BuildingId == selected.BuildingId &&
                level.Level == selected.Level &&
                level.NextLevelId > 0);
        }

        private static string FormatBuildingInfo(Fantasy.BattleBuildingStateInfo selected, BuildingConfig config, bool includeEffect)
        {
            var name = config?.BuildingName ?? $"建筑 {selected.BuildingId}";
            var baseText = $"{name}  Lv.{selected.Level}  HP {selected.Hp}/{selected.MaxHp}";
            if (!includeEffect)
            {
                return baseText;
            }

            var effect = config == null ? string.Empty : (string.IsNullOrWhiteSpace(config.EffectDesc) ? config.BuildingType : config.EffectDesc);
            return string.IsNullOrWhiteSpace(effect) ? baseText : $"{baseText}  {effect}";
        }

        protected override void OnDestroy()
        {
            ClearSpawned(_spawnedAvatars);
            ClearSpawned(_spawnedBuildCards);
            ClearSpawned(_spawnedOperationButtons);
            Log.Info("战斗界面关闭");
        }

        private static void ClearSpawned(List<GameObject> spawned)
        {
            for (var i = spawned.Count - 1; i >= 0; i--)
            {
                if (spawned[i] != null)
                {
                    Object.Destroy(spawned[i]);
                }
            }

            spawned.Clear();
        }

        private static bool IsElf(Fantasy.BattlePlayerStateInfo player)
        {
            return SameCamp(player?.Camp, "Elf");
        }

        private static bool IsTroll(Fantasy.BattlePlayerStateInfo player)
        {
            return SameCamp(player?.Camp, "Troll");
        }

        private static bool SameCamp(string left, string right)
        {
            return string.Equals(left, right, System.StringComparison.OrdinalIgnoreCase);
        }

        private static string FormatTime(int seconds)
        {
            return $"{seconds / 60:00}:{seconds % 60:00}";
        }

        private static string GetAvatarInitial(Fantasy.BattlePlayerStateInfo player)
        {
            if (!string.IsNullOrWhiteSpace(player?.Nickname))
            {
                return player.Nickname.Substring(0, 1);
            }

            return player == null ? "?" : (player.PlayerId % 10).ToString();
        }

        private Transform FindHudChild(string name)
        {
            var hud = FindChild("m_battleHud");
            return FindDescendant(hud ?? transform, name);
        }

        private T FindHudComponent<T>(string name) where T : Component
        {
            var child = FindHudChild(name);
            return child == null ? null : child.GetComponent<T>();
        }

        private Text FindResourceValueText(string rowName)
        {
            var row = FindHudChild(rowName);
            var value = FindDescendant(row, "m_txtValue");
            return value == null ? null : value.GetComponent<Text>();
        }

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }

        private static void SetChildText(Transform root, string childName, string value)
        {
            var child = FindDescendant(root, childName);
            var text = child == null ? null : child.GetComponent<Text>();
            if (text != null)
            {
                text.text = value ?? string.Empty;
            }
        }

        private static Transform FindDescendant(Transform root, string name)
        {
            if (root == null)
            {
                return null;
            }

            for (var i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                if (child.name == name)
                {
                    return child;
                }

                var match = FindDescendant(child, name);
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }
    }
}
