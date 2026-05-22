using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Room.Handler;

/// <summary>
/// 加入自定义房间 RPC。
/// </summary>
public sealed class JoinRoomRequestHandler : MessageRPC<C2G_JoinRoomRequest, G2C_JoinRoomResponse>
{
    protected override async FTask Run(Session session, C2G_JoinRoomRequest request, G2C_JoinRoomResponse response, Action reply)
    {
        var profile = SheepServices.Auth.RequireProfile(request.Token);
        var result = SheepServices.Rooms.Join(profile, request);
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Room != null)
        {
            response.Room = result.Room;
        }
        await FTask.CompletedTask;
    }
}
