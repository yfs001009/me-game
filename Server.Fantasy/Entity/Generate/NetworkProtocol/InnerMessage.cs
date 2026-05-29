using ProtoBuf;
using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Fantasy;
using Fantasy.Network.Interface;
using Fantasy.Serialize;
// ReSharper disable InconsistentNaming
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable RedundantTypeArgumentsOfMethod
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PreferConcreteValueOverDefault
// ReSharper disable RedundantNameQualifier
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CheckNamespace
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable RedundantUsingDirective
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8618
namespace Fantasy
{
    [ProtoContract]
    public partial class G2Room_ListWaitingRoomsRequest : AMessage, IAddressRequest
    {
        public static G2Room_ListWaitingRoomsRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2Room_ListWaitingRoomsRequest>();
        }

        public override void Dispose()
        {
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2Room_ListWaitingRoomsRequest>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.G2Room_ListWaitingRoomsRequest; } 
        [ProtoIgnore]
        public Room2G_ListWaitingRoomsResponse ResponseType { get; set; }
    }
    [ProtoContract]
    public partial class Room2G_ListWaitingRoomsResponse : AMessage, IAddressResponse
    {
        public static Room2G_ListWaitingRoomsResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<Room2G_ListWaitingRoomsResponse>();
        }

        public override void Dispose()
        {
            Rooms.Clear();
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<Room2G_ListWaitingRoomsResponse>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.Room2G_ListWaitingRoomsResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public List<RoomSummaryInfo> Rooms { get; set; } = new List<RoomSummaryInfo>();
    }
    [ProtoContract]
    public partial class G2Room_CreateRoomRequest : AMessage, IAddressRequest
    {
        public static G2Room_CreateRoomRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2Room_CreateRoomRequest>();
        }

        public override void Dispose()
        {
            Owner = default;
            RoomName = default;
            Mode = default;
            MapId = default;
            MaxPlayers = default;
            IsPrivate = default;
            Password = default;
            SelectedBuildingCardIds.Clear();
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2Room_CreateRoomRequest>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.G2Room_CreateRoomRequest; } 
        [ProtoIgnore]
        public Room2G_CreateRoomResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public PlayerProfileInfo Owner { get; set; }
        [ProtoMember(2)]
        public string RoomName { get; set; }
        [ProtoMember(3)]
        public string Mode { get; set; }
        [ProtoMember(4)]
        public int MapId { get; set; }
        [ProtoMember(5)]
        public int MaxPlayers { get; set; }
        [ProtoMember(6)]
        public bool IsPrivate { get; set; }
        [ProtoMember(7)]
        public string Password { get; set; }
        [ProtoMember(8)]
        public List<int> SelectedBuildingCardIds { get; set; } = new List<int>();
    }
    [ProtoContract]
    public partial class Room2G_CreateRoomResponse : AMessage, IAddressResponse
    {
        public static Room2G_CreateRoomResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<Room2G_CreateRoomResponse>();
        }

        public override void Dispose()
        {
            Room = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<Room2G_CreateRoomResponse>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.Room2G_CreateRoomResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public RoomDetailInfo Room { get; set; }
    }
    [ProtoContract]
    public partial class G2Room_JoinRoomRequest : AMessage, IAddressRequest
    {
        public static G2Room_JoinRoomRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2Room_JoinRoomRequest>();
        }

        public override void Dispose()
        {
            Profile = default;
            RoomId = default;
            Password = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2Room_JoinRoomRequest>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.G2Room_JoinRoomRequest; } 
        [ProtoIgnore]
        public Room2G_JoinRoomResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public PlayerProfileInfo Profile { get; set; }
        [ProtoMember(2)]
        public int RoomId { get; set; }
        [ProtoMember(3)]
        public string Password { get; set; }
    }
    [ProtoContract]
    public partial class Room2G_JoinRoomResponse : AMessage, IAddressResponse
    {
        public static Room2G_JoinRoomResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<Room2G_JoinRoomResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Room = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<Room2G_JoinRoomResponse>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.Room2G_JoinRoomResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public RoomDetailInfo Room { get; set; }
    }
    [ProtoContract]
    public partial class G2Room_LeaveRoomRequest : AMessage, IAddressRequest
    {
        public static G2Room_LeaveRoomRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2Room_LeaveRoomRequest>();
        }

        public override void Dispose()
        {
            Profile = default;
            RoomId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2Room_LeaveRoomRequest>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.G2Room_LeaveRoomRequest; } 
        [ProtoIgnore]
        public Room2G_LeaveRoomResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public PlayerProfileInfo Profile { get; set; }
        [ProtoMember(2)]
        public int RoomId { get; set; }
    }
    [ProtoContract]
    public partial class Room2G_LeaveRoomResponse : AMessage, IAddressResponse
    {
        public static Room2G_LeaveRoomResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<Room2G_LeaveRoomResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Room = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<Room2G_LeaveRoomResponse>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.Room2G_LeaveRoomResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public RoomDetailInfo Room { get; set; }
    }
    [ProtoContract]
    public partial class G2Room_RoomDetailRequest : AMessage, IAddressRequest
    {
        public static G2Room_RoomDetailRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2Room_RoomDetailRequest>();
        }

        public override void Dispose()
        {
            Profile = default;
            RoomId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2Room_RoomDetailRequest>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.G2Room_RoomDetailRequest; } 
        [ProtoIgnore]
        public Room2G_RoomDetailResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public PlayerProfileInfo Profile { get; set; }
        [ProtoMember(2)]
        public int RoomId { get; set; }
    }
    [ProtoContract]
    public partial class Room2G_RoomDetailResponse : AMessage, IAddressResponse
    {
        public static Room2G_RoomDetailResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<Room2G_RoomDetailResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Room = default;
            Battle = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<Room2G_RoomDetailResponse>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.Room2G_RoomDetailResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public RoomDetailInfo Room { get; set; }
        [ProtoMember(5)]
        public BattleStartInfo Battle { get; set; }
    }
    [ProtoContract]
    public partial class G2Room_SetRoomReadyRequest : AMessage, IAddressRequest
    {
        public static G2Room_SetRoomReadyRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2Room_SetRoomReadyRequest>();
        }

        public override void Dispose()
        {
            Profile = default;
            RoomId = default;
            IsReady = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2Room_SetRoomReadyRequest>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.G2Room_SetRoomReadyRequest; } 
        [ProtoIgnore]
        public Room2G_SetRoomReadyResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public PlayerProfileInfo Profile { get; set; }
        [ProtoMember(2)]
        public int RoomId { get; set; }
        [ProtoMember(3)]
        public bool IsReady { get; set; }
    }
    [ProtoContract]
    public partial class Room2G_SetRoomReadyResponse : AMessage, IAddressResponse
    {
        public static Room2G_SetRoomReadyResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<Room2G_SetRoomReadyResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Room = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<Room2G_SetRoomReadyResponse>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.Room2G_SetRoomReadyResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public RoomDetailInfo Room { get; set; }
    }
    [ProtoContract]
    public partial class G2Room_StartRoomRequest : AMessage, IAddressRequest
    {
        public static G2Room_StartRoomRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2Room_StartRoomRequest>();
        }

        public override void Dispose()
        {
            Profile = default;
            RoomId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2Room_StartRoomRequest>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.G2Room_StartRoomRequest; } 
        [ProtoIgnore]
        public Room2G_StartRoomResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public PlayerProfileInfo Profile { get; set; }
        [ProtoMember(2)]
        public int RoomId { get; set; }
    }
    [ProtoContract]
    public partial class Room2G_StartRoomResponse : AMessage, IAddressResponse
    {
        public static Room2G_StartRoomResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<Room2G_StartRoomResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Room = default;
            Battle = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<Room2G_StartRoomResponse>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.Room2G_StartRoomResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public RoomDetailInfo Room { get; set; }
        [ProtoMember(5)]
        public BattleStartInfo Battle { get; set; }
    }
    [ProtoContract]
    public partial class Room2Battle_CreateBattleRequest : AMessage, IAddressRequest
    {
        public static Room2Battle_CreateBattleRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<Room2Battle_CreateBattleRequest>();
        }

        public override void Dispose()
        {
            Room = default;
            Battle = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<Room2Battle_CreateBattleRequest>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.Room2Battle_CreateBattleRequest; } 
        [ProtoIgnore]
        public Battle2Room_CreateBattleResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public RoomDetailInfo Room { get; set; }
        [ProtoMember(2)]
        public BattleStartInfo Battle { get; set; }
    }
    [ProtoContract]
    public partial class Battle2Room_CreateBattleResponse : AMessage, IAddressResponse
    {
        public static Battle2Room_CreateBattleResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<Battle2Room_CreateBattleResponse>();
        }

        public override void Dispose()
        {
            Battle = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<Battle2Room_CreateBattleResponse>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.Battle2Room_CreateBattleResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public BattleStartInfo Battle { get; set; }
    }
    [ProtoContract]
    public partial class G2Battle_SceneLoadedRequest : AMessage, IAddressRequest
    {
        public static G2Battle_SceneLoadedRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2Battle_SceneLoadedRequest>();
        }

        public override void Dispose()
        {
            Profile = default;
            BattleId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2Battle_SceneLoadedRequest>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.G2Battle_SceneLoadedRequest; } 
        [ProtoIgnore]
        public Battle2G_SceneLoadedResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public PlayerProfileInfo Profile { get; set; }
        [ProtoMember(2)]
        public int BattleId { get; set; }
    }
    [ProtoContract]
    public partial class Battle2G_SceneLoadedResponse : AMessage, IAddressResponse
    {
        public static Battle2G_SceneLoadedResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<Battle2G_SceneLoadedResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Snapshot = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<Battle2G_SceneLoadedResponse>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.Battle2G_SceneLoadedResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public BattleSnapshotInfo Snapshot { get; set; }
    }
    [ProtoContract]
    public partial class G2Battle_SnapshotRequest : AMessage, IAddressRequest
    {
        public static G2Battle_SnapshotRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2Battle_SnapshotRequest>();
        }

        public override void Dispose()
        {
            Profile = default;
            BattleId = default;
            LastKnownTick = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2Battle_SnapshotRequest>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.G2Battle_SnapshotRequest; } 
        [ProtoIgnore]
        public Battle2G_SnapshotResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public PlayerProfileInfo Profile { get; set; }
        [ProtoMember(2)]
        public int BattleId { get; set; }
        [ProtoMember(3)]
        public long LastKnownTick { get; set; }
    }
    [ProtoContract]
    public partial class Battle2G_SnapshotResponse : AMessage, IAddressResponse
    {
        public static Battle2G_SnapshotResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<Battle2G_SnapshotResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Snapshot = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<Battle2G_SnapshotResponse>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.Battle2G_SnapshotResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public BattleSnapshotInfo Snapshot { get; set; }
    }
    [ProtoContract]
    public partial class G2Battle_MoveRequest : AMessage, IAddressRequest
    {
        public static G2Battle_MoveRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2Battle_MoveRequest>();
        }

        public override void Dispose()
        {
            Profile = default;
            BattleId = default;
            AxisX = default;
            AxisY = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2Battle_MoveRequest>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.G2Battle_MoveRequest; } 
        [ProtoIgnore]
        public Battle2G_MoveResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public PlayerProfileInfo Profile { get; set; }
        [ProtoMember(2)]
        public int BattleId { get; set; }
        [ProtoMember(3)]
        public float AxisX { get; set; }
        [ProtoMember(4)]
        public float AxisY { get; set; }
    }
    [ProtoContract]
    public partial class Battle2G_MoveResponse : AMessage, IAddressResponse
    {
        public static Battle2G_MoveResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<Battle2G_MoveResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Snapshot = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<Battle2G_MoveResponse>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.Battle2G_MoveResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public BattleSnapshotInfo Snapshot { get; set; }
    }
    [ProtoContract]
    public partial class G2Battle_BuildRequest : AMessage, IAddressRequest
    {
        public static G2Battle_BuildRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2Battle_BuildRequest>();
        }

        public override void Dispose()
        {
            Profile = default;
            BattleId = default;
            BuildingId = default;
            GridX = default;
            GridY = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2Battle_BuildRequest>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.G2Battle_BuildRequest; } 
        [ProtoIgnore]
        public Battle2G_BuildResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public PlayerProfileInfo Profile { get; set; }
        [ProtoMember(2)]
        public int BattleId { get; set; }
        [ProtoMember(3)]
        public int BuildingId { get; set; }
        [ProtoMember(4)]
        public int GridX { get; set; }
        [ProtoMember(5)]
        public int GridY { get; set; }
    }
    [ProtoContract]
    public partial class Battle2G_BuildResponse : AMessage, IAddressResponse
    {
        public static Battle2G_BuildResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<Battle2G_BuildResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Snapshot = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<Battle2G_BuildResponse>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.Battle2G_BuildResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public BattleSnapshotInfo Snapshot { get; set; }
    }
    [ProtoContract]
    public partial class G2Battle_UpgradeBuildingRequest : AMessage, IAddressRequest
    {
        public static G2Battle_UpgradeBuildingRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2Battle_UpgradeBuildingRequest>();
        }

        public override void Dispose()
        {
            Profile = default;
            BattleId = default;
            BuildingInstanceId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2Battle_UpgradeBuildingRequest>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.G2Battle_UpgradeBuildingRequest; } 
        [ProtoIgnore]
        public Battle2G_UpgradeBuildingResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public PlayerProfileInfo Profile { get; set; }
        [ProtoMember(2)]
        public int BattleId { get; set; }
        [ProtoMember(3)]
        public long BuildingInstanceId { get; set; }
    }
    [ProtoContract]
    public partial class Battle2G_UpgradeBuildingResponse : AMessage, IAddressResponse
    {
        public static Battle2G_UpgradeBuildingResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<Battle2G_UpgradeBuildingResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Snapshot = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<Battle2G_UpgradeBuildingResponse>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.Battle2G_UpgradeBuildingResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public BattleSnapshotInfo Snapshot { get; set; }
    }
    [ProtoContract]
    public partial class G2Battle_RecycleBuildingRequest : AMessage, IAddressRequest
    {
        public static G2Battle_RecycleBuildingRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2Battle_RecycleBuildingRequest>();
        }

        public override void Dispose()
        {
            Profile = default;
            BattleId = default;
            BuildingInstanceId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2Battle_RecycleBuildingRequest>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.G2Battle_RecycleBuildingRequest; } 
        [ProtoIgnore]
        public Battle2G_RecycleBuildingResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public PlayerProfileInfo Profile { get; set; }
        [ProtoMember(2)]
        public int BattleId { get; set; }
        [ProtoMember(3)]
        public long BuildingInstanceId { get; set; }
    }
    [ProtoContract]
    public partial class Battle2G_RecycleBuildingResponse : AMessage, IAddressResponse
    {
        public static Battle2G_RecycleBuildingResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<Battle2G_RecycleBuildingResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Snapshot = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<Battle2G_RecycleBuildingResponse>(this);
#endif
        }
public uint OpCode() { return InnerOpcode.Battle2G_RecycleBuildingResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public BattleSnapshotInfo Snapshot { get; set; }
    }
}