using Fantasy;
using Fantasy.Async;
using Fantasy.Platform.Net;

namespace Hotfix.Room;

/// <summary>
/// Gate Scene 到 Room Scene 的 Address 调用门面。
/// 客户端 RPC 入口只做认证和协议适配，房间生命周期统一收敛到 Room Scene。
/// </summary>
internal static class RoomSceneGateway
{
    public static async FTask<Room2G_ListWaitingRoomsResponse> ListWaitingRooms(Scene gateScene)
    {
        return (Room2G_ListWaitingRoomsResponse)await gateScene.Call(RoomSceneAddress(), new G2Room_ListWaitingRoomsRequest());
    }

    public static async FTask<Room2G_CreateRoomResponse> Create(Scene gateScene, PlayerProfileInfo owner, C2G_CreateRoomRequest request)
    {
        return (Room2G_CreateRoomResponse)await gateScene.Call(RoomSceneAddress(), new G2Room_CreateRoomRequest
        {
            Owner = owner,
            RoomName = request.RoomName,
            Mode = request.Mode,
            MapId = request.MapId,
            MaxPlayers = request.MaxPlayers,
            IsPrivate = request.IsPrivate,
            Password = request.Password,
            SelectedBuildingCardIds = request.SelectedBuildingCardIds
        });
    }

    public static async FTask<Room2G_JoinRoomResponse> Join(Scene gateScene, PlayerProfileInfo profile, C2G_JoinRoomRequest request)
    {
        return (Room2G_JoinRoomResponse)await gateScene.Call(RoomSceneAddress(), new G2Room_JoinRoomRequest
        {
            Profile = profile,
            RoomId = request.RoomId,
            Password = request.Password
        });
    }

    public static async FTask<Room2G_LeaveRoomResponse> Leave(Scene gateScene, PlayerProfileInfo profile, C2G_LeaveRoomRequest request)
    {
        return (Room2G_LeaveRoomResponse)await gateScene.Call(RoomSceneAddress(), new G2Room_LeaveRoomRequest
        {
            Profile = profile,
            RoomId = request.RoomId
        });
    }

    public static async FTask<Room2G_RoomDetailResponse> GetDetail(Scene gateScene, PlayerProfileInfo profile, C2G_RoomDetailRequest request)
    {
        return (Room2G_RoomDetailResponse)await gateScene.Call(RoomSceneAddress(), new G2Room_RoomDetailRequest
        {
            Profile = profile,
            RoomId = request.RoomId
        });
    }

    public static async FTask<Room2G_SetRoomReadyResponse> SetReady(Scene gateScene, PlayerProfileInfo profile, C2G_SetRoomReadyRequest request)
    {
        return (Room2G_SetRoomReadyResponse)await gateScene.Call(RoomSceneAddress(), new G2Room_SetRoomReadyRequest
        {
            Profile = profile,
            RoomId = request.RoomId,
            IsReady = request.IsReady
        });
    }

    public static async FTask<Room2G_StartRoomResponse> Start(Scene gateScene, PlayerProfileInfo profile, C2G_StartRoomRequest request)
    {
        return (Room2G_StartRoomResponse)await gateScene.Call(RoomSceneAddress(), new G2Room_StartRoomRequest
        {
            Profile = profile,
            RoomId = request.RoomId
        });
    }

    private static long RoomSceneAddress()
    {
        var roomScenes = SceneConfigData.Instance.GetSceneBySceneType(SceneType.Room);
        if (roomScenes.Count <= 0)
        {
            Log.Error("Room Scene is not configured.");
            return 0;
        }

        // MVP currently runs a single Room Scene. Keep this choice centralized
        // before adding room-id based sharding.
        return roomScenes[0].Address;
    }
}
