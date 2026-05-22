using Fantasy;
using Hotfix.Auth.Service;
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

