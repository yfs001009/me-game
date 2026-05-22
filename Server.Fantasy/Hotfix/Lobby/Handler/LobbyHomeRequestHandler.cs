using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Lobby.Handler;

/// <summary>
/// 大厅首页 RPC。聚合玩家资料、房间列表和匹配状态。
/// </summary>
public sealed class LobbyHomeRequestHandler : MessageRPC<C2G_LobbyHomeRequest, G2C_LobbyHomeResponse>
{
    protected override async FTask Run(Session session, C2G_LobbyHomeRequest request, G2C_LobbyHomeResponse response, Action reply)
    {
        var profile = SheepServices.Auth.RequireProfile(request.Token);
        response.Profile = profile;
        response.Rooms.AddRange(SheepServices.Rooms.ListWaitingRooms());
        response.MatchStatus = SheepServices.Match.GetStatus(profile.PlayerId);
        Log.Info($"玩家请求大厅首页：玩家ID={profile.PlayerId}，昵称={profile.Nickname}，房间数量={response.Rooms.Count}");
        await FTask.CompletedTask;
    }
}
