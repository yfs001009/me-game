using Fantasy;
using GameConfig.battle;
using Fantasy.Entitas;
using Hotfix.Config;
using Hotfix.Shared;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Hotfix.Battle.Service;

/// <summary>
/// 单进程权威战斗服务。当前用 RPC 快照拉取支撑同步测试，后续可迁移到 Battle Scene 广播。
/// </summary>
public sealed class BattleService
{
    private const int TickMs = 100;
    private const int TrollSelectDelaySeconds = 30;
    private const int InitialGold = 300;
    private const int InitialWood = 180;
    private const int ElfMaxHp = 100;
    private const int TrollMaxHp = 500;
    private const int TowerProjectileFlightMs = 220;
    private const string ElfCamp = "Elf";
    private const string TrollCamp = "Troll";

    private readonly object gate = new();
    private readonly Dictionary<int, BattleEntity> battles = new();
    private readonly Dictionary<string, List<SpawnArea>> spawnAreaCache = new();
    private readonly Dictionary<string, MapRuleData> mapRuleCache = new();
    private readonly Random random = new();
    private long nextBuildingInstanceId = 100000;
    private long nextAttackEventId = 1;

    public BattleStartInfo CreateFromRoom(Scene scene, RoomDetailInfo room, BattleStartInfo startInfo)
    {
        lock (gate)
        {
            var battle = Entity.Create<BattleEntity>(scene, id: startInfo.BattleId, isPool: false, isRunEvent: true);
            battle.BattleId = startInfo.BattleId;
            battle.RoomId = startInfo.RoomId;
            battle.MapId = startInfo.MapId;
            battle.MapAsset = startInfo.MapAsset;
            battle.Mode = startInfo.Mode;
            battle.State = "Loading";
            battle.LastTickAtUtc = DateTimeOffset.UtcNow;

            var mapRules = GetMapRules(battle.MapAsset);
            for (var i = 0; i < room.Players.Count; i++)
            {
                var player = room.Players[i];
                var spawn = GetRandomSpawn(mapRules.SpawnAreas, i);
                var battlePlayer = Entity.Create<BattlePlayerEntity>(scene, id: player.PlayerId, isPool: false, isRunEvent: true);
                battlePlayer.PlayerId = player.PlayerId;
                battlePlayer.Nickname = player.Nickname;
                battlePlayer.Camp = ElfCamp;
                battlePlayer.PosX = spawn.X;
                battlePlayer.PosY = spawn.Y;
                battlePlayer.Hp = ElfMaxHp;
                battlePlayer.MaxHp = ElfMaxHp;
                battlePlayer.SelectedBuildingCardIds.AddRange(player.SelectedBuildingCardIds);
                battle.Players.Add(battlePlayer);
            }

            battles[battle.BattleId] = battle;
            Log.Info($"创建战斗实例：BattleId={battle.BattleId}，玩家数={battle.Players.Count}，地图={battle.MapAsset}");
            return startInfo;
        }
    }

    public BattleStartInfo? GetStartInfoByRoom(int roomId)
    {
        lock (gate)
        {
            var battle = battles.Values.FirstOrDefault(item => item.RoomId == roomId);
            if (battle == null)
            {
                return null;
            }

            return new BattleStartInfo
            {
                BattleId = battle.BattleId,
                RoomId = battle.RoomId,
                MapId = battle.MapId,
                MapAsset = battle.MapAsset,
                Mode = battle.Mode
            };
        }
    }

    public (bool Success, string Message, BattleSnapshotInfo? Snapshot) MarkSceneLoaded(PlayerProfileInfo profile, int battleId)
    {
        lock (gate)
        {
            if (!TryGetBattleAndPlayer(profile, battleId, out var battle, out var player, out var error))
            {
                return (false, error, null);
            }

            player.SceneLoaded = true;
            if (battle.State == "Loading" && battle.Players.Count > 0 && battle.Players.All(item => item.SceneLoaded))
            {
                battle.State = "Running";
                battle.RunningStartedAtUtc = DateTimeOffset.UtcNow;
                battle.LastTickAtUtc = DateTimeOffset.UtcNow;
                Log.Info($"战斗全员加载完成，进入 Running：BattleId={battle.BattleId}");
            }

            AdvanceTick(battle);
            return (true, battle.State == "Running" ? "战斗已运行。" : "等待其他玩家加载。", ToSnapshot(battle));
        }
    }

    public (bool Success, string Message, BattleSnapshotInfo? Snapshot) GetSnapshot(PlayerProfileInfo profile, int battleId)
    {
        lock (gate)
        {
            if (!TryGetBattleAndPlayer(profile, battleId, out var battle, out _, out var error))
            {
                return (false, error, null);
            }

            AdvanceTick(battle);
            return (true, "快照获取成功。", ToSnapshot(battle));
        }
    }

    public (bool Success, string Message, BattleSnapshotInfo? Snapshot) Move(PlayerProfileInfo profile, C2G_BattleMoveCommand request)
    {
        lock (gate)
        {
            if (!TryGetRunningBattleAndPlayer(profile, request.BattleId, out var battle, out var player, out var error))
            {
                return (false, error, null);
            }

            var axisX = Math.Clamp(request.AxisX, -1f, 1f);
            var axisY = Math.Clamp(request.AxisY, -1f, 1f);
            var length = MathF.Sqrt(axisX * axisX + axisY * axisY);
            if (length > 1f)
            {
                axisX /= length;
                axisY /= length;
            }

            AdvanceTick(battle, true);
            var mapRules = GetMapRules(battle.MapAsset);
            var stepSeconds = TickMs / 1000f;
            var nextX = Math.Clamp(player.PosX + axisX * player.MoveSpeed * stepSeconds, 0f, mapRules.MaxPosX);
            var nextY = Math.Clamp(player.PosY - axisY * player.MoveSpeed * stepSeconds, 0f, mapRules.MaxPosY);
            if (!IsPositionBlocked(battle, mapRules, nextX, nextY))
            {
                player.PosX = nextX;
                player.PosY = nextY;
            }

            return (true, "移动成功。", ToSnapshot(battle));
        }
    }

    public (bool Success, string Message, BattleSnapshotInfo? Snapshot) Build(PlayerProfileInfo profile, C2G_BuildCommand request)
    {
        lock (gate)
        {
            if (!TryGetRunningBattleAndPlayer(profile, request.BattleId, out var battle, out var player, out var error))
            {
                return (false, error, null);
            }

            var building = GetBuilding(request.BuildingId);
            var level = GetBuildingLevel(request.BuildingId, 1);
            var card = GetBuildingCardByBuilding(request.BuildingId);
            if (building == null || level == null || card == null)
            {
                return (false, "建筑配置不存在。", ToSnapshot(battle));
            }

            if (player.Camp != ElfCamp)
            {
                return (false, "只有精灵可以建造。", ToSnapshot(battle));
            }

            if (!CanBuildFromLoadout(player, card.CardId))
            {
                return (false, "未携带该建筑卡。", ToSnapshot(battle));
            }

            var width = Math.Max(building.FootprintWidth, 1);
            var height = Math.Max(building.FootprintHeight, 1);
            var mapRules = GetMapRules(battle.MapAsset);
            LogBuildRangeCheck("Build range check", player, request.GridX, request.GridY, width, height);
            if (!mapRules.IsAreaInMap(request.GridX, request.GridY, width, height))
            {
                return (false, "超出地图范围。", ToSnapshot(battle));
            }

            if (mapRules.IsBuildForbiddenArea(request.GridX, request.GridY, width, height))
            {
                return (false, "当前位置不能建造。", ToSnapshot(battle));
            }

            if (!IsAnyBuildCellInRange(player, request.GridX, request.GridY, width, height))
            {
                Log.Warning($"Build rejected by range: Player=({player.PosX:0.00},{player.PosY:0.00}), Grid=({request.GridX},{request.GridY}), Size=({width},{height}), Range={SheepServices.Rules.BuildRange}");
                return (false, "建造距离不足。", ToSnapshot(battle));
            }

            if (IsOccupied(battle, request.GridX, request.GridY, width, height, 0))
            {
                return (false, "目标格子已被占用。", ToSnapshot(battle));
            }

            if (IsTrollOccupyingArea(battle, request.GridX, request.GridY, width, height))
            {
                return (false, "巨魔所在格子不能建造。", ToSnapshot(battle));
            }

            if (player.Gold < card.CostGold || player.Wood < card.CostWood)
            {
                return (false, "资源不足。", ToSnapshot(battle));
            }

            player.Gold -= card.CostGold;
            player.Wood -= card.CostWood;
            var buildingEntity = Entity.Create<BattleBuildingEntity>(battle.Scene, id: ++nextBuildingInstanceId, isPool: false, isRunEvent: true);
            buildingEntity.InstanceId = buildingEntity.Id;
            buildingEntity.OwnerPlayerId = profile.PlayerId;
            buildingEntity.BuildingId = request.BuildingId;
            buildingEntity.Level = 1;
            buildingEntity.GridX = request.GridX;
            buildingEntity.GridY = request.GridY;
            buildingEntity.Width = width;
            buildingEntity.Height = height;
            buildingEntity.Hp = level.Hp > 0 ? level.Hp : building.BaseHp;
            buildingEntity.MaxHp = level.Hp > 0 ? level.Hp : building.BaseHp;
            buildingEntity.LastEffectTick = battle.Tick;
            battle.Buildings.Add(buildingEntity);

            AdvanceTick(battle, true);
            return (true, "建造成功。", ToSnapshot(battle));
        }
    }

    public (bool Success, string Message, BattleSnapshotInfo? Snapshot) Upgrade(PlayerProfileInfo profile, C2G_UpgradeBuildingCommand request)
    {
        lock (gate)
        {
            if (!TryGetRunningBattleAndPlayer(profile, request.BattleId, out var battle, out var player, out var error))
            {
                return (false, error, null);
            }

            var instance = battle.Buildings.FirstOrDefault(item => item.InstanceId == request.BuildingInstanceId);
            if (instance == null)
            {
                return (false, "建筑不存在。", ToSnapshot(battle));
            }

            if (instance.OwnerPlayerId != profile.PlayerId)
            {
                return (false, "只能升级自己的建筑。", ToSnapshot(battle));
            }

            var building = GetBuilding(instance.BuildingId);
            var currentLevel = GetBuildingLevel(instance.BuildingId, instance.Level);
            if (building == null || currentLevel == null || !building.CanUpgrade)
            {
                return (false, "建筑不可升级。", ToSnapshot(battle));
            }

            if (instance.Level >= building.MaxLevel || currentLevel.NextLevelId <= 0)
            {
                return (false, "建筑已满级。", ToSnapshot(battle));
            }

            var nextLevel = GetBuildingLevelById(currentLevel.NextLevelId);
            if (nextLevel == null)
            {
                return (false, "下一级配置不存在。", ToSnapshot(battle));
            }

            if (player.Gold < currentLevel.UpgradeCostGold || player.Wood < currentLevel.UpgradeCostWood)
            {
                return (false, "升级资源不足。", ToSnapshot(battle));
            }

            player.Gold -= currentLevel.UpgradeCostGold;
            player.Wood -= currentLevel.UpgradeCostWood;
            instance.Level = nextLevel.Level;
            instance.MaxHp = nextLevel.Hp > 0 ? nextLevel.Hp : instance.MaxHp;
            instance.Hp = instance.MaxHp;

            AdvanceTick(battle, true);
            return (true, "升级成功。", ToSnapshot(battle));
        }
    }

    public (bool Success, string Message, BattleSnapshotInfo? Snapshot) Recycle(PlayerProfileInfo profile, C2G_RecycleBuildingCommand request)
    {
        lock (gate)
        {
            if (!TryGetRunningBattleAndPlayer(profile, request.BattleId, out var battle, out var player, out var error))
            {
                return (false, error, null);
            }

            var instance = battle.Buildings.FirstOrDefault(item => item.InstanceId == request.BuildingInstanceId);
            if (instance == null)
            {
                return (false, "建筑不存在。", ToSnapshot(battle));
            }

            if (instance.OwnerPlayerId != profile.PlayerId)
            {
                return (false, "只能回收自己的建筑。", ToSnapshot(battle));
            }

            var card = GetBuildingCardByBuilding(instance.BuildingId);
            var building = GetBuilding(instance.BuildingId);
            var percent = Math.Clamp(building?.RecyclePercent ?? 25, 0, 100);
            if (card != null)
            {
                player.Gold += card.CostGold * percent / 100;
                player.Wood += card.CostWood * percent / 100;
            }

            battle.Buildings.Remove(instance);
            AdvanceTick(battle, true);
            return (true, "回收成功。", ToSnapshot(battle));
        }
    }

    private bool TryGetBattleAndPlayer(PlayerProfileInfo profile, int battleId, out BattleEntity battle, out BattlePlayerEntity player, out string error)
    {
        battle = null!;
        player = null!;
        error = string.Empty;

        if (!battles.TryGetValue(battleId, out battle!))
        {
            error = "战斗不存在。";
            return false;
        }

        player = battle.Players.FirstOrDefault(item => item.PlayerId == profile.PlayerId)!;
        if (player == null)
        {
            error = "玩家不在战斗中。";
            return false;
        }

        return true;
    }

    private bool TryGetRunningBattleAndPlayer(PlayerProfileInfo profile, int battleId, out BattleEntity battle, out BattlePlayerEntity player, out string error)
    {
        if (!TryGetBattleAndPlayer(profile, battleId, out battle, out player, out error))
        {
            return false;
        }

        if (battle.State != "Running")
        {
            error = "战斗尚未开始同步运行。";
            return false;
        }

        return true;
    }

    private void AdvanceTick(BattleEntity battle, bool forceOneTick = false)
    {
        var now = DateTimeOffset.UtcNow;
        TrySelectTroll(battle, now);
        var elapsedMs = (now - battle.LastTickAtUtc).TotalMilliseconds;
        var ticks = forceOneTick ? Math.Max(1, (long)(elapsedMs / TickMs)) : (long)(elapsedMs / TickMs);
        if (battle.State == "Running" && ticks > 0)
        {
            battle.Tick += ticks;
            battle.LastTickAtUtc = battle.LastTickAtUtc.AddMilliseconds(ticks * TickMs);
            ResolvePendingTowerHits(battle);
            ApplyBuildingEffects(battle);
        }
    }

    private static void ResolvePendingTowerHits(BattleEntity battle)
    {
        for (var i = battle.PendingTowerHits.Count - 1; i >= 0; i--)
        {
            var hit = battle.PendingTowerHits[i];
            if (hit.ResolveTick > battle.Tick)
            {
                continue;
            }

            battle.PendingTowerHits.RemoveAt(i);
            var source = battle.Buildings.FirstOrDefault(item => item.InstanceId == hit.SourceBuildingInstanceId);
            var target = battle.Players.FirstOrDefault(item => item.PlayerId == hit.TargetPlayerId);
            if (source == null || target == null || target.Camp != TrollCamp || target.Hp <= 0)
            {
                continue;
            }

            target.Hp = Math.Max(0, target.Hp - hit.Damage);
        }
    }

    private void ApplyBuildingEffects(BattleEntity battle)
    {
        foreach (var building in battle.Buildings)
        {
            var config = GetBuilding(building.BuildingId);
            var level = GetBuildingLevel(building.BuildingId, building.Level);
            if (config?.BuildingType == "Tower")
            {
                ApplyTowerAttack(battle, building, level);
                continue;
            }

            if (config?.BuildingType != "Support" && config?.BuildingType != "Lumber")
            {
                continue;
            }

            if (level == null || level.RepairValue <= 0 || level.AttackIntervalMs <= 0)
            {
                continue;
            }

            var intervalTicks = Math.Max(1, level.AttackIntervalMs / TickMs);
            var elapsedTicks = battle.Tick - building.LastEffectTick;
            if (elapsedTicks < intervalTicks)
            {
                continue;
            }

            var times = Math.Max(1, elapsedTicks / intervalTicks);
            var owner = battle.Players.FirstOrDefault(item => item.PlayerId == building.OwnerPlayerId);
            if (owner != null)
            {
                var amount = (int)(level.RepairValue * times);
                if (config.BuildingType == "Lumber")
                {
                    owner.Wood += amount;
                }
                else
                {
                    owner.Gold += amount;
                }
            }

            building.LastEffectTick += times * intervalTicks;
        }
    }

    private void ApplyTowerAttack(BattleEntity battle, BattleBuildingEntity building, BuildingLevelConfig? level)
    {
        if (level == null || level.Attack <= 0 || level.AttackRange <= 0 || level.AttackIntervalMs <= 0)
        {
            return;
        }

        var intervalTicks = Math.Max(1, level.AttackIntervalMs / TickMs);
        var elapsedTicks = battle.Tick - building.LastEffectTick;
        if (elapsedTicks < intervalTicks)
        {
            return;
        }

        var centerX = building.GridX + building.Width * 0.5f;
        var centerY = building.GridY + building.Height * 0.5f;
        var range = level.AttackRange;
        var target = battle.Players
            .Where(item => item.Camp == TrollCamp && item.Hp > 0)
            .Select(item => new
            {
                Player = item,
                Distance = MathF.Sqrt((item.PosX - centerX) * (item.PosX - centerX) + (item.PosY - centerY) * (item.PosY - centerY))
            })
            .Where(item => item.Distance <= range)
            .OrderBy(item => item.Distance)
            .FirstOrDefault()?.Player;
        if (target == null)
        {
            building.LastEffectTick += Math.Max(1, elapsedTicks / intervalTicks) * intervalTicks;
            return;
        }

        var eventId = nextAttackEventId++;
        var resolveDelayTicks = Math.Max(1, TowerProjectileFlightMs / TickMs);
        battle.AttackEvents.Add(new BattleAttackEventRecord
        {
            EventId = eventId,
            SourceBuildingInstanceId = building.InstanceId,
            TargetPlayerId = target.PlayerId,
            FromX = centerX,
            FromY = centerY,
            ToX = target.PosX,
            ToY = target.PosY,
            Damage = level.Attack
        });
        battle.PendingTowerHits.Add(new PendingTowerHitRecord
        {
            EventId = eventId,
            SourceBuildingInstanceId = building.InstanceId,
            TargetPlayerId = target.PlayerId,
            ResolveTick = battle.Tick + resolveDelayTicks,
            Damage = level.Attack
        });
        if (battle.AttackEvents.Count > 32)
        {
            battle.AttackEvents.RemoveRange(0, battle.AttackEvents.Count - 32);
        }

        building.LastEffectTick += Math.Max(1, elapsedTicks / intervalTicks) * intervalTicks;
    }

    private static bool IsOccupied(BattleEntity battle, int gridX, int gridY, int width, int height, long ignoreInstanceId)
    {
        foreach (var building in battle.Buildings)
        {
            if (building.InstanceId == ignoreInstanceId)
            {
                continue;
            }

            if (gridX < building.GridX + building.Width &&
                gridX + width > building.GridX &&
                gridY < building.GridY + building.Height &&
                gridY + height > building.GridY)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsTrollOccupyingArea(BattleEntity battle, int gridX, int gridY, int width, int height)
    {
        foreach (var player in battle.Players)
        {
            if (player.Camp != TrollCamp)
            {
                continue;
            }

            var trollGridX = (int)MathF.Floor(player.PosX);
            var trollGridY = (int)MathF.Floor(player.PosY);
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

    private static bool IsAnyBuildCellInRange(BattlePlayerEntity player, int gridX, int gridY, int width, int height)
    {
        var closestX = Math.Clamp(player.PosX, gridX, gridX + width);
        var closestY = Math.Clamp(player.PosY, gridY, gridY + height);
        var dx = closestX - player.PosX;
        var dy = closestY - player.PosY;
        return MathF.Sqrt(dx * dx + dy * dy) <= SheepServices.Rules.BuildRange;
    }

    private static void LogBuildRangeCheck(string prefix, BattlePlayerEntity player, int gridX, int gridY, int width, int height)
    {
        var closestX = Math.Clamp(player.PosX, gridX, gridX + width);
        var closestY = Math.Clamp(player.PosY, gridY, gridY + height);
        var dx = closestX - player.PosX;
        var dy = closestY - player.PosY;
        var distance = MathF.Sqrt(dx * dx + dy * dy);
        Log.Info($"{prefix}: Player=({player.PosX:0.00},{player.PosY:0.00}), Grid=({gridX},{gridY}), Size=({width},{height}), Closest=({closestX:0.00},{closestY:0.00}), Distance={distance:0.00}, Range={SheepServices.Rules.BuildRange}");
    }

    private static bool IsPositionBlocked(BattleEntity battle, MapRuleData mapRules, float posX, float posY)
    {
        var gridX = (int)MathF.Floor(posX);
        var gridY = (int)MathF.Floor(posY);
        if (mapRules.IsNoMove(gridX, gridY))
        {
            return true;
        }

        foreach (var building in battle.Buildings)
        {
            var config = GetBuilding(building.BuildingId);
            if (config?.CanBlockPath != true)
            {
                continue;
            }

            if (gridX >= building.GridX &&
                gridX < building.GridX + building.Width &&
                gridY >= building.GridY &&
                gridY < building.GridY + building.Height)
            {
                return true;
            }
        }

        return false;
    }

    private static BuildingConfig? GetBuilding(int buildingId)
    {
        return ConfigSystem.Instance.Tables.TbBuilding.GetOrDefault(buildingId);
    }

    private static BuildingLevelConfig? GetBuildingLevel(int buildingId, int level)
    {
        return ConfigSystem.Instance.Tables.TbBuildingLevel.DataList.FirstOrDefault(item => item.BuildingId == buildingId && item.Level == level);
    }

    private static BuildingLevelConfig? GetBuildingLevelById(int levelId)
    {
        return ConfigSystem.Instance.Tables.TbBuildingLevel.GetOrDefault(levelId);
    }

    private static BuildingCardConfig? GetBuildingCardByBuilding(int buildingId)
    {
        return ConfigSystem.Instance.Tables.TbBuildingCard.DataList.FirstOrDefault(item => item.BuildingId == buildingId);
    }

    private static bool CanBuildFromLoadout(BattlePlayerEntity player, int cardId)
    {
        return player.SelectedBuildingCardIds.Count <= 0 || player.SelectedBuildingCardIds.Contains(cardId);
    }

    private void TrySelectTroll(BattleEntity battle, DateTimeOffset now)
    {
        if (battle.State != "Running" || battle.TrollSelected)
        {
            return;
        }

        if (battle.RunningStartedAtUtc == default || (now - battle.RunningStartedAtUtc).TotalSeconds < TrollSelectDelaySeconds)
        {
            return;
        }

        var candidates = battle.Players.Where(item => item.Camp == ElfCamp).ToList();
        if (candidates.Count <= 0)
        {
            battle.TrollSelected = true;
            return;
        }

        var troll = candidates[random.Next(candidates.Count)];
        var spawn = GetRandomSpawn(GetMapRules(battle.MapAsset).SpawnAreas, 0);
        battle.Buildings.RemoveAll(item => item.OwnerPlayerId == troll.PlayerId);
        troll.Camp = TrollCamp;
        troll.Gold = InitialGold;
        troll.Wood = InitialWood;
        troll.PosX = spawn.X;
        troll.PosY = spawn.Y;
        troll.MoveSpeed = 4f;
        troll.MaxHp = TrollMaxHp;
        troll.Hp = TrollMaxHp;
        battle.TrollSelected = true;
        battle.Tick++;
        Log.Info($"Battle troll selected: BattleId={battle.BattleId}, PlayerId={troll.PlayerId}, Spawn=({troll.PosX:0.00},{troll.PosY:0.00})");
    }

    private (float X, float Y) GetRandomSpawn(IReadOnlyList<SpawnArea> spawnAreas, int fallbackIndex)
    {
        if (spawnAreas.Count <= 0)
        {
            return (7f + fallbackIndex * 1.5f, 4.5f);
        }

        var area = spawnAreas[random.Next(spawnAreas.Count)];
        return (
            area.X + (float)random.NextDouble() * area.Width,
            area.Y + (float)random.NextDouble() * area.Height);
    }

    private List<SpawnArea> GetSpawnAreas(string mapAsset)
    {
        var normalized = NormalizeMapAssetName(mapAsset);
        if (spawnAreaCache.TryGetValue(normalized, out var cached))
        {
            return cached;
        }

        var areas = GetMapRules(normalized).SpawnAreas;
        spawnAreaCache[normalized] = areas;
        return areas;
    }

    private MapRuleData GetMapRules(string mapAsset)
    {
        var normalized = NormalizeMapAssetName(mapAsset);
        if (mapRuleCache.TryGetValue(normalized, out var cached))
        {
            return cached;
        }

        var rules = LoadMapRules(normalized);
        mapRuleCache[normalized] = rules;
        return rules;
    }

    private static MapRuleData LoadMapRules(string mapAsset)
    {
        foreach (var path in GetTmxCandidatePaths(mapAsset))
        {
            if (!File.Exists(path))
            {
                continue;
            }

            var text = File.ReadAllText(path);
            var mapWidth = Math.Max(1, (int)ReadFloatAttribute(text, "width", 100f));
            var mapHeight = Math.Max(1, (int)ReadFloatAttribute(text, "height", 100f));
            var tileWidth = ReadFloatAttribute(text, "tilewidth", 64f);
            var tileHeight = ReadFloatAttribute(text, "tileheight", tileWidth);
            var noMove = new bool[mapWidth * mapHeight];
            var noBuild = new bool[mapWidth * mapHeight];
            LoadTileRuleLayer(text, mapWidth, mapHeight, "no_move", noMove);
            LoadTileRuleLayer(text, mapWidth, mapHeight, "no_build", noBuild);

            var spawnAreas = new List<SpawnArea>();
            var groupMatch = Regex.Match(
                text,
                "<objectgroup[^>]*name=\"birth_area\"[^>]*>(?<content>.*?)</objectgroup>",
                RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (groupMatch.Success)
            {
                foreach (Match objectMatch in Regex.Matches(groupMatch.Groups["content"].Value, "<object\\b(?<attrs>[^>]*)/?>", RegexOptions.IgnoreCase))
                {
                    var attrs = objectMatch.Groups["attrs"].Value;
                    var width = ReadFloatAttribute(attrs, "width", tileWidth) / tileWidth;
                    var height = ReadFloatAttribute(attrs, "height", tileHeight) / tileHeight;
                    spawnAreas.Add(new SpawnArea(
                        ReadFloatAttribute(attrs, "x", 0f) / tileWidth,
                        ReadFloatAttribute(attrs, "y", 0f) / tileHeight,
                        Math.Max(width, 1f),
                        Math.Max(height, 1f)));
                }
            }

            Log.Info($"Loaded TMX map rules: map={mapAsset}, path={path}, size={mapWidth}x{mapHeight}, birth_area={spawnAreas.Count}");
            return new MapRuleData(mapWidth, mapHeight, noMove, noBuild, spawnAreas);
        }

        Log.Warning($"TMX map rules not found, using fallback rules. map={mapAsset}");
        return MapRuleData.Fallback;
    }

    private static void LoadTileRuleLayer(string text, int mapWidth, int mapHeight, string layerName, bool[] target)
    {
        foreach (Match layerMatch in Regex.Matches(text, "<layer\\b(?<attrs>[^>]*)>\\s*<data\\b(?<dataAttrs>[^>]*)>(?<data>.*?)</data>\\s*</layer>", RegexOptions.Singleline | RegexOptions.IgnoreCase))
        {
            var attrs = layerMatch.Groups["attrs"].Value;
            if (!string.Equals(ReadStringAttribute(attrs, "name"), layerName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!layerMatch.Groups["dataAttrs"].Value.Contains("encoding=\"csv\"", StringComparison.OrdinalIgnoreCase))
            {
                Log.Warning($"TMX rule layer ignored because it is not CSV encoded: {layerName}");
                return;
            }

            var layerWidth = Math.Max(1, (int)ReadFloatAttribute(attrs, "width", mapWidth));
            var values = layerMatch.Groups["data"].Value.Split(new[] { ',', '\r', '\n', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var count = Math.Min(values.Length, target.Length);
            for (var i = 0; i < count; i++)
            {
                if (!int.TryParse(values[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out var tileId) || tileId <= 0)
                {
                    continue;
                }

                var x = i % layerWidth;
                var y = i / layerWidth;
                var targetIndex = y * mapWidth + x;
                if (targetIndex >= 0 && targetIndex < target.Length)
                {
                    target[targetIndex] = true;
                }
            }

            return;
        }
    }

    private static IEnumerable<string> GetTmxCandidatePaths(string mapAsset)
    {
        yield return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../Client.Unity/Assets/AssetRaw/TiledMaps/Maps", $"{mapAsset}.tmx"));
        yield return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../Tools.Map/TiledProject/Maps", $"{mapAsset}.tmx"));
        yield return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../Client.Unity/Assets/AssetRaw/TiledMaps/Maps", $"{mapAsset}.tmx"));
        yield return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../Tools.Map/TiledProject/Maps", $"{mapAsset}.tmx"));
        yield return Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../Client.Unity/Assets/AssetRaw/TiledMaps/Maps", $"{mapAsset}.tmx"));
        yield return Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../Tools.Map/TiledProject/Maps", $"{mapAsset}.tmx"));
        yield return Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../Client.Unity/Assets/AssetRaw/TiledMaps/Maps", $"{mapAsset}.tmx"));
        yield return Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../Tools.Map/TiledProject/Maps", $"{mapAsset}.tmx"));
    }

    private static float ReadFloatAttribute(string text, string name, float defaultValue)
    {
        var match = Regex.Match(text, $"{name}=\"(?<value>[^\"]+)\"", RegexOptions.IgnoreCase);
        return match.Success && float.TryParse(match.Groups["value"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
            ? value
            : defaultValue;
    }

    private static string ReadStringAttribute(string text, string name)
    {
        var match = Regex.Match(text, $"{name}=\"(?<value>[^\"]*)\"", RegexOptions.IgnoreCase);
        return match.Success ? match.Groups["value"].Value : string.Empty;
    }

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
            normalized = normalized[(slashIndex + 1)..];
        }

        return normalized.EndsWith(".tmx", StringComparison.OrdinalIgnoreCase)
            ? normalized[..^4]
            : normalized;
    }

    private static BattleSnapshotInfo ToSnapshot(BattleEntity battle)
    {
        var snapshot = new BattleSnapshotInfo
        {
            BattleId = battle.BattleId,
            Tick = battle.Tick,
            State = battle.State
        };
        snapshot.Players.AddRange(battle.Players.Select(ToPlayerState));
        snapshot.Buildings.AddRange(battle.Buildings.Select(building => new BattleBuildingStateInfo
        {
            InstanceId = building.InstanceId,
            OwnerPlayerId = building.OwnerPlayerId,
            BuildingId = building.BuildingId,
            Level = building.Level,
            GridX = building.GridX,
            GridY = building.GridY,
            Hp = building.Hp,
            MaxHp = building.MaxHp,
            State = building.State
        }));
        snapshot.AttackEvents.AddRange(battle.AttackEvents.Select(item => new BattleAttackEventInfo
        {
            EventId = item.EventId,
            SourceBuildingInstanceId = item.SourceBuildingInstanceId,
            TargetPlayerId = item.TargetPlayerId,
            FromX = item.FromX,
            FromY = item.FromY,
            ToX = item.ToX,
            ToY = item.ToY,
            Damage = item.Damage
        }));
        return snapshot;
    }

    private static BattlePlayerStateInfo ToPlayerState(BattlePlayerEntity player)
    {
        var state = new BattlePlayerStateInfo
        {
            PlayerId = player.PlayerId,
            Nickname = player.Nickname,
            Camp = player.Camp,
            SceneLoaded = player.SceneLoaded,
            Gold = player.Gold,
            Wood = player.Wood,
            PosX = player.PosX,
            PosY = player.PosY,
            MoveSpeed = player.MoveSpeed,
            Hp = player.Hp,
            MaxHp = player.MaxHp
        };
        state.SelectedBuildingCardIds.AddRange(player.SelectedBuildingCardIds);
        return state;
    }

    private readonly record struct SpawnArea(float X, float Y, float Width, float Height);

    private sealed class MapRuleData
    {
        public static MapRuleData Fallback { get; } = new(100, 100, new bool[100 * 100], new bool[100 * 100], new List<SpawnArea>());

        public MapRuleData(int width, int height, bool[] noMove, bool[] noBuild, List<SpawnArea> spawnAreas)
        {
            Width = Math.Max(width, 1);
            Height = Math.Max(height, 1);
            NoMove = noMove ?? new bool[Width * Height];
            NoBuild = noBuild ?? new bool[Width * Height];
            SpawnAreas = spawnAreas ?? new List<SpawnArea>();
        }

        public int Width { get; }
        public int Height { get; }
        public bool[] NoMove { get; }
        public bool[] NoBuild { get; }
        public List<SpawnArea> SpawnAreas { get; }
        public float MaxPosX => Math.Max(Width - 0.001f, 0f);
        public float MaxPosY => Math.Max(Height - 0.001f, 0f);

        public bool IsAreaInMap(int gridX, int gridY, int width, int height)
        {
            return gridX >= 0 &&
                   gridY >= 0 &&
                   gridX + width <= Width &&
                   gridY + height <= Height;
        }

        public bool IsNoMove(int gridX, int gridY)
        {
            return GetRule(NoMove, gridX, gridY);
        }

        public bool IsBuildForbiddenArea(int gridX, int gridY, int width, int height)
        {
            for (var y = gridY; y < gridY + height; y++)
            {
                for (var x = gridX; x < gridX + width; x++)
                {
                    if (GetRule(NoMove, x, y) || GetRule(NoBuild, x, y))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool GetRule(bool[] rules, int gridX, int gridY)
        {
            if (gridX < 0 || gridY < 0 || gridX >= Width || gridY >= Height)
            {
                return true;
            }

            var index = gridY * Width + gridX;
            return index >= 0 && index < rules.Length && rules[index];
        }
    }
}
