using Fantasy;
using Hotfix.Asset.Service;
using Hotfix.Auth.Service;
using Hotfix.Battle.Service;
using Hotfix.Character.Service;
using Hotfix.Chat.Service;
using Hotfix.Feature.Service;
using Hotfix.Lobby.Service;
using Hotfix.Lottery.Service;
using Hotfix.Mail.Service;
using Hotfix.Room.Service;
using Hotfix.Shop.Service;
using Hotfix.Social.Service;
using Hotfix.Task.Service;

namespace Hotfix.Shared;

/// <summary>
/// Hotfix 层的进程内服务定位器。
/// 当前阶段用于单进程开发，后续拆分 Fantasy Scene 后替换为 Scene 组件或 Actor 调用。
/// </summary>
public static class SheepServices
{
    public static readonly AuthService Auth = new();
    public static readonly AssetService Assets = new();
    public static readonly MatchService Match = new();
    public static readonly CustomRoomService Rooms = new();
    public static readonly BattleService Battles = new();
    public static readonly GameRuleService Rules = new();
    public static readonly CharacterService Characters = new();
    public static readonly MailService Mails = new();
    public static readonly LotteryService Lottery = new();
    public static readonly SocialService Social = new();
    public static readonly ChatService Chat = new();
    public static readonly FeatureGateService Features = new();
    public static readonly OutgameShopService Shops = new();
    public static readonly OutgameTaskService Tasks = new();
}

public sealed class MatchTicket
{
    public long PlayerId { get; init; }
    public string Mode { get; init; } = string.Empty;
    public DateTimeOffset StartTimeUtc { get; init; }
}
