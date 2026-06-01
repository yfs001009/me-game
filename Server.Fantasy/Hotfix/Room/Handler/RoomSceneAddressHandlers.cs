using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Battle;
using Hotfix.Shared;

namespace Hotfix.Room.Handler;

/// <summary>
/// Room Scene 内的房间列表查询。
/// </summary>
public sealed class ListWaitingRoomsAddressHandler : AddressRPC<Scene, G2Room_ListWaitingRoomsRequest, Room2G_ListWaitingRoomsResponse>
{
    protected override async FTask Run(Scene scene, G2Room_ListWaitingRoomsRequest request, Room2G_ListWaitingRoomsResponse response, Action reply)
    {
        response.Rooms.AddRange(SheepServices.Rooms.ListWaitingRooms());
        await FTask.CompletedTask;
    }
}

/// <summary>
/// Room Scene 内创建房间，保持房间状态归属单一 Scene。
/// </summary>
public sealed class CreateRoomAddressHandler : AddressRPC<Scene, G2Room_CreateRoomRequest, Room2G_CreateRoomResponse>
{
    protected override async FTask Run(Scene scene, G2Room_CreateRoomRequest request, Room2G_CreateRoomResponse response, Action reply)
    {
        response.Room = SheepServices.Rooms.Create(scene, request.Owner, new C2G_CreateRoomRequest
        {
            RoomName = request.RoomName,
            Mode = request.Mode,
            MapId = request.MapId,
            MaxPlayers = request.MaxPlayers,
            IsPrivate = request.IsPrivate,
            Password = request.Password,
            SelectedBuildingCardIds = request.SelectedBuildingCardIds
        });
        await FTask.CompletedTask;
    }
}

/// <summary>
/// Room Scene 内处理加入房间。
/// </summary>
public sealed class JoinRoomAddressHandler : AddressRPC<Scene, G2Room_JoinRoomRequest, Room2G_JoinRoomResponse>
{
    protected override async FTask Run(Scene scene, G2Room_JoinRoomRequest request, Room2G_JoinRoomResponse response, Action reply)
    {
        var result = SheepServices.Rooms.Join(request.Profile, new C2G_JoinRoomRequest
        {
            RoomId = request.RoomId,
            Password = request.Password
        });
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Room != null)
        {
            response.Room = result.Room;
        }
        await FTask.CompletedTask;
    }
}

/// <summary>
/// Room Scene 内处理离开房间。
/// </summary>
public sealed class LeaveRoomAddressHandler : AddressRPC<Scene, G2Room_LeaveRoomRequest, Room2G_LeaveRoomResponse>
{
    protected override async FTask Run(Scene scene, G2Room_LeaveRoomRequest request, Room2G_LeaveRoomResponse response, Action reply)
    {
        var result = SheepServices.Rooms.Leave(request.Profile, new C2G_LeaveRoomRequest { RoomId = request.RoomId });
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Room != null)
        {
            response.Room = result.Room;
        }
        await FTask.CompletedTask;
    }
}

/// <summary>
/// Room Scene 内处理房间详情查询，并携带已开始战斗的入口信息。
/// </summary>
public sealed class RoomDetailAddressHandler : AddressRPC<Scene, G2Room_RoomDetailRequest, Room2G_RoomDetailResponse>
{
    protected override async FTask Run(Scene scene, G2Room_RoomDetailRequest request, Room2G_RoomDetailResponse response, Action reply)
    {
        var result = SheepServices.Rooms.GetDetail(request.Profile, new C2G_RoomDetailRequest { RoomId = request.RoomId });
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Room != null)
        {
            response.Room = result.Room;
        }

        if (result.Room?.Summary?.State == "Playing")
        {
            var battle = SheepServices.Rooms.GetBattleByRoom(result.Room.Summary.RoomId);
            if (battle != null)
            {
                response.Battle = battle;
            }
        }

        await FTask.CompletedTask;
    }
}

/// <summary>
/// Room Scene 内切换准备状态。
/// </summary>
public sealed class SetRoomReadyAddressHandler : AddressRPC<Scene, G2Room_SetRoomReadyRequest, Room2G_SetRoomReadyResponse>
{
    protected override async FTask Run(Scene scene, G2Room_SetRoomReadyRequest request, Room2G_SetRoomReadyResponse response, Action reply)
    {
        var result = SheepServices.Rooms.SetReady(request.Profile, new C2G_SetRoomReadyRequest
        {
            RoomId = request.RoomId,
            IsReady = request.IsReady
        });
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Room != null)
        {
            response.Room = result.Room;
        }
        await FTask.CompletedTask;
    }
}

/// <summary>
/// Room Scene 内开始房间，并通过 Address 调用创建 Battle Scene 状态。
/// </summary>
public sealed class StartRoomAddressHandler : AddressRPC<Scene, G2Room_StartRoomRequest, Room2G_StartRoomResponse>
{
    protected override async FTask Run(Scene scene, G2Room_StartRoomRequest request, Room2G_StartRoomResponse response, Action reply)
    {
        var result = SheepServices.Rooms.Start(request.Profile, new C2G_StartRoomRequest { RoomId = request.RoomId });
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Room != null)
        {
            response.Room = result.Room;
        }
        if (!result.Success || result.Room == null || result.Battle == null)
        {
            if (result.Battle != null)
            {
                response.Battle = result.Battle;
            }
            return;
        }

        var battle = await BattleSceneGateway.Create(scene, result.Room, result.Battle);
        response.Battle = battle.Battle;
        await FTask.CompletedTask;
    }
}
