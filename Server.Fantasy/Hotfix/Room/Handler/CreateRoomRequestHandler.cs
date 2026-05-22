using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Room.Handler;

/// <summary>
/// 创建自定义房间 RPC。当前创建大厅房间，后续由 Room Scene 接管生命周期。
/// </summary>
public sealed class CreateRoomRequestHandler : MessageRPC<C2G_CreateRoomRequest, G2C_CreateRoomResponse>
{
    protected override async FTask Run(Session session, C2G_CreateRoomRequest request, G2C_CreateRoomResponse response, Action reply)
    {
        var profile = SheepServices.Auth.RequireProfile(request.Token);
        response.Room = SheepServices.Rooms.Create(profile, request);
        Log.Info($"玩家创建房间请求完成：玩家ID={profile.PlayerId}，房间ID={response.Room?.Summary?.RoomId}");
        await FTask.CompletedTask;
    }
}
