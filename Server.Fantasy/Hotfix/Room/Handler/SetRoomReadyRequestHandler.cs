using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Room.Handler;

/// <summary>
/// 切换自定义房间准备状态 RPC。
/// </summary>
public sealed class SetRoomReadyRequestHandler : MessageRPC<C2G_SetRoomReadyRequest, G2C_SetRoomReadyResponse>
{
    protected override async FTask Run(Session session, C2G_SetRoomReadyRequest request, G2C_SetRoomReadyResponse response, Action reply)
    {
        var profile = SheepServices.Auth.RequireProfile(request.Token);
        var result = SheepServices.Rooms.SetReady(profile, request);
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Room != null)
        {
            response.Room = result.Room;
        }
        await FTask.CompletedTask;
    }
}
