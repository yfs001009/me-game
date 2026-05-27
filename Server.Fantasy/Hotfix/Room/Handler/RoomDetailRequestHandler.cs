using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Room.Handler;

/// <summary>
/// 拉取自定义房间详情 RPC。
/// </summary>
public sealed class RoomDetailRequestHandler : MessageRPC<C2G_RoomDetailRequest, G2C_RoomDetailResponse>
{
    protected override async FTask Run(Session session, C2G_RoomDetailRequest request, G2C_RoomDetailResponse response, Action reply)
    {
        var profile = SheepServices.Auth.RequireProfile(request.Token);
        var result = SheepServices.Rooms.GetDetail(profile, request);
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Room != null)
        {
            response.Room = result.Room;
        }

        if (result.Room?.Summary?.State == "Playing")
        {
            var battle = SheepServices.Battles.GetStartInfoByRoom(result.Room.Summary.RoomId);
            if (battle != null)
            {
                response.Battle = battle;
            }
        }

        await FTask.CompletedTask;
    }
}
