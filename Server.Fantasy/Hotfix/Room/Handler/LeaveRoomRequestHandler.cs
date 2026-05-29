using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Room;
using Hotfix.Shared;

namespace Hotfix.Room.Handler;

/// <summary>
/// 离开自定义房间 RPC。
/// </summary>
public sealed class LeaveRoomRequestHandler : MessageRPC<C2G_LeaveRoomRequest, G2C_LeaveRoomResponse>
{
    protected override async FTask Run(Session session, C2G_LeaveRoomRequest request, G2C_LeaveRoomResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.Success = false;
            response.Message = message;
            response.Room = CreateClosedRoomDetail(request.RoomId);
            await FTask.CompletedTask;
            return;
        }

        var result = await RoomSceneGateway.Leave(session.Scene, profile, request);
        response.Success = result.Success;
        response.Message = result.Message;
        response.Room = result.Room ?? CreateClosedRoomDetail(request.RoomId);
    }

    private static RoomDetailInfo CreateClosedRoomDetail(int roomId)
    {
        return new RoomDetailInfo
        {
            Summary = new RoomSummaryInfo
            {
                RoomId = roomId,
                RoomName = string.Empty,
                Mode = string.Empty,
                MapId = 0,
                CurrentPlayers = 0,
                MaxPlayers = 0,
                IsPrivate = false,
                State = "Closed"
            }
        };
    }
}
