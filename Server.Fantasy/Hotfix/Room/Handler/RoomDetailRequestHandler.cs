using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Room;
using Hotfix.Shared;

namespace Hotfix.Room.Handler;

/// <summary>
/// 拉取自定义房间详情 RPC。
/// </summary>
public sealed class RoomDetailRequestHandler : MessageRPC<C2G_RoomDetailRequest, G2C_RoomDetailResponse>
{
    protected override async FTask Run(Session session, C2G_RoomDetailRequest request, G2C_RoomDetailResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.Success = false;
            response.Message = message;
            await FTask.CompletedTask;
            return;
        }

        var result = await RoomSceneGateway.GetDetail(session.Scene, profile, request);
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
