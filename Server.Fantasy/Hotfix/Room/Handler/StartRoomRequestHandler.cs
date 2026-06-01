using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Room;
using Hotfix.Shared;

namespace Hotfix.Room.Handler;

/// <summary>
/// 房主开始自定义房间，进入战斗。
/// </summary>
public sealed class StartRoomRequestHandler : MessageRPC<C2G_StartRoomRequest, G2C_StartRoomResponse>
{
    protected override async FTask Run(Session session, C2G_StartRoomRequest request, G2C_StartRoomResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.ErrorCode = 401;
            response.Success = false;
            response.Message = message;
            return;
        }

        var result = await RoomSceneGateway.Start(session.Scene, profile, request);
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

    }
}
