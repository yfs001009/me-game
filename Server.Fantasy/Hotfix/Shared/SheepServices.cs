using Fantasy;
using Hotfix.Auth.Service;
using Hotfix.Battle.Service;
using Hotfix.Lobby.Service;
using Hotfix.Room.Service;

namespace Hotfix.Shared;

/// <summary>
/// Hotfix 层的进程内服务定位器。
/// 当前阶段用于单进程开发，后续拆分 Fantasy Scene 后替换为 Scene 组件或 Actor 调用。
/// </summary>
public static class SheepServices
{
    public static readonly AuthService Auth = new();
    public static readonly MatchService Match = new();
    public static readonly CustomRoomService Rooms = new();
    public static readonly BattleService Battles = new();
    public static readonly GameRuleService Rules = new();
}

public sealed class AccountRecord
{
    public long PlayerId { get; init; }
    public string Account { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public int Exp { get; set; }
    public int AvatarId { get; set; } = 1;
    public int RankScore { get; set; } = 1000;
}

public sealed class MatchTicket
{
    public long PlayerId { get; init; }
    public string Mode { get; init; } = string.Empty;
    public DateTimeOffset StartTimeUtc { get; init; }
}

public sealed class CustomRoomRecord
{
    public int RoomId { get; init; }
    public string RoomName { get; init; } = string.Empty;
    public string Mode { get; init; } = string.Empty;
    public int MapId { get; init; }
    public int MaxPlayers { get; init; }
    public bool IsPrivate { get; init; }
    public string Password { get; init; } = string.Empty;
    public string State { get; set; } = "Waiting";
    public long OwnerPlayerId { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public List<RoomPlayerInfo> Players { get; } = new();
}

public sealed class BattleRecord
{
    public int BattleId { get; init; }
    public int RoomId { get; init; }
    public int MapId { get; init; }
    public string MapAsset { get; init; } = string.Empty;
    public string Mode { get; init; } = string.Empty;
    public string State { get; set; } = "Loading";
    public long Tick { get; set; }
    public DateTimeOffset StartedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastTickAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset RunningStartedAtUtc { get; set; }
    public bool TrollSelected { get; set; }
    public List<BattlePlayerRecord> Players { get; } = new();
    public List<BattleBuildingRecord> Buildings { get; } = new();
    public List<BattleAttackEventRecord> AttackEvents { get; } = new();
    public List<PendingTowerHitRecord> PendingTowerHits { get; } = new();
}

public sealed class BattlePlayerRecord
{
    public long PlayerId { get; init; }
    public string Nickname { get; init; } = string.Empty;
    public string Camp { get; set; } = "Elf";
    public bool SceneLoaded { get; set; }
    public int Gold { get; set; } = 300;
    public int Wood { get; set; } = 180;
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float MoveSpeed { get; set; } = 4f;
    public int Hp { get; set; } = 100;
    public int MaxHp { get; set; } = 100;
}

public sealed class BattleBuildingRecord
{
    public long InstanceId { get; init; }
    public long OwnerPlayerId { get; init; }
    public int BuildingId { get; init; }
    public int Level { get; set; } = 1;
    public int GridX { get; init; }
    public int GridY { get; init; }
    public int Width { get; init; } = 1;
    public int Height { get; init; } = 1;
    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public long LastEffectTick { get; set; }
    public string State { get; set; } = "Built";
}

public sealed class BattleAttackEventRecord
{
    public long EventId { get; init; }
    public long SourceBuildingInstanceId { get; init; }
    public long TargetPlayerId { get; init; }
    public float FromX { get; init; }
    public float FromY { get; init; }
    public float ToX { get; init; }
    public float ToY { get; init; }
    public int Damage { get; init; }
}

public sealed class PendingTowerHitRecord
{
    public long EventId { get; init; }
    public long SourceBuildingInstanceId { get; init; }
    public long TargetPlayerId { get; init; }
    public long ResolveTick { get; init; }
    public int Damage { get; init; }
}
