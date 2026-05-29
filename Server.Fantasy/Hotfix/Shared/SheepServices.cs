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

public sealed class MatchTicket
{
    public long PlayerId { get; init; }
    public string Mode { get; init; } = string.Empty;
    public DateTimeOffset StartTimeUtc { get; init; }
}
