using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Room.Handler;

/// <summary>
/// 房主开始自定义房间，进入战斗。
/// </summary>
public sealed class StartRoomRequestHandler : MessageRPC<C2G_StartRoomRequest, G2C_StartRoomResponse>
{
    protected override async FTask Run(Session session, C2G_StartRoomRequest request, G2C_StartRoomResponse response, Action reply)
    {
        var profile = SheepServices.Auth.RequireProfile(request.Token);
        var result = SheepServices.Rooms.Start(profile, request);
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Room != null)
        {
            response.Room = result.Room;
        }

        if (result.Battle != null)
        {
            response.Battle = result.Battle;
        }

        await FTask.CompletedTask;
    }
}
