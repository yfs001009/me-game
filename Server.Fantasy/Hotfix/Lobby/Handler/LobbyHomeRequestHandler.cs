using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Room;
using Hotfix.Shared;

namespace Hotfix.Lobby.Handler;

/// <summary>
/// 大厅首页 RPC。聚合玩家资料、房间列表和匹配状态。
/// </summary>
public sealed class LobbyHomeRequestHandler : MessageRPC<C2G_LobbyHomeRequest, G2C_LobbyHomeResponse>
{
    protected override async FTask Run(Session session, C2G_LobbyHomeRequest request, G2C_LobbyHomeResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out _))
        {
            response.ErrorCode = 401;
            return;
        }

        response.Profile = profile;
        var rooms = await RoomSceneGateway.ListWaitingRooms(session.Scene);
        response.Rooms.AddRange(rooms.Rooms);
        response.MatchStatus = SheepServices.Match.GetStatus(profile.PlayerId);
        Log.Info($"玩家请求大厅首页：玩家ID={profile.PlayerId}，昵称={profile.Nickname}，房间数量={response.Rooms.Count}");
        await FTask.CompletedTask;
    }
}
