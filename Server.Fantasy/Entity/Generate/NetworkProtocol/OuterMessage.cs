using ProtoBuf;
using System;
using System.Collections.Generic;
using Fantasy;
using Fantasy.Network.Interface;
using Fantasy.Serialize;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8618
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
namespace Fantasy
{
    /// <summary>
    /// 玩家基础资料，登录和大厅通用。
    /// </summary>
    [ProtoContract]
    public partial class PlayerProfileInfo : AMessage
    {
        public static PlayerProfileInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<PlayerProfileInfo>();
        }

        public override void Dispose()
        {
            PlayerId = default;
            Account = default;
            Nickname = default;
            Level = default;
            Exp = default;
            AvatarId = default;
            RankScore = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<PlayerProfileInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public long PlayerId { get; set; }
        [ProtoMember(2)]
        public string Account { get; set; }
        [ProtoMember(3)]
        public string Nickname { get; set; }
        [ProtoMember(4)]
        public int Level { get; set; }
        [ProtoMember(5)]
        public int Exp { get; set; }
        [ProtoMember(6)]
        public int AvatarId { get; set; }
        [ProtoMember(7)]
        public int RankScore { get; set; }
    }
    /// <summary>
    /// 注册账号。
    /// </summary>
    [ProtoContract]
    public partial class C2G_RegisterRequest : AMessage, IRequest
    {
        public static C2G_RegisterRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_RegisterRequest>();
        }

        public override void Dispose()
        {
            Account = default;
            Password = default;
            Nickname = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_RegisterRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_RegisterRequest; } 
        [ProtoIgnore]
        public G2C_RegisterResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Account { get; set; }
        [ProtoMember(2)]
        public string Password { get; set; }
        [ProtoMember(3)]
        public string Nickname { get; set; }
    }
    [ProtoContract]
    public partial class G2C_RegisterResponse : AMessage, IResponse
    {
        public static G2C_RegisterResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_RegisterResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_RegisterResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_RegisterResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
    }
    /// <summary>
    /// 登录账号并获取会话 Token。
    /// </summary>
    [ProtoContract]
    public partial class C2G_LoginRequest : AMessage, IRequest
    {
        public static C2G_LoginRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_LoginRequest>();
        }

        public override void Dispose()
        {
            Account = default;
            Password = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_LoginRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_LoginRequest; } 
        [ProtoIgnore]
        public G2C_LoginResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Account { get; set; }
        [ProtoMember(2)]
        public string Password { get; set; }
    }
    [ProtoContract]
    public partial class G2C_LoginResponse : AMessage, IResponse
    {
        public static G2C_LoginResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_LoginResponse>();
        }

        public override void Dispose()
        {
            Token = default;
            Profile = default;
            Success = default;
            Message = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_LoginResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_LoginResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public string Token { get; set; }
        [ProtoMember(3)]
        public PlayerProfileInfo Profile { get; set; }
        [ProtoMember(4)]
        public bool Success { get; set; }
        [ProtoMember(5)]
        public string Message { get; set; }
    }
    /// <summary>
    /// 首次登录后设置昵称。
    /// </summary>
    [ProtoContract]
    public partial class C2G_SetNicknameRequest : AMessage, IRequest
    {
        public static C2G_SetNicknameRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_SetNicknameRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            Nickname = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_SetNicknameRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_SetNicknameRequest; } 
        [ProtoIgnore]
        public G2C_SetNicknameResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public string Nickname { get; set; }
    }
    [ProtoContract]
    public partial class G2C_SetNicknameResponse : AMessage, IResponse
    {
        public static G2C_SetNicknameResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_SetNicknameResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Profile = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_SetNicknameResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_SetNicknameResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public PlayerProfileInfo Profile { get; set; }
    }
    /// <summary>
    /// 拉取大厅首页。
    /// </summary>
    [ProtoContract]
    public partial class C2G_LobbyHomeRequest : AMessage, IRequest
    {
        public static C2G_LobbyHomeRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_LobbyHomeRequest>();
        }

        public override void Dispose()
        {
            Token = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_LobbyHomeRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_LobbyHomeRequest; } 
        [ProtoIgnore]
        public G2C_LobbyHomeResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
    }
    [ProtoContract]
    public partial class G2C_LobbyHomeResponse : AMessage, IResponse
    {
        public static G2C_LobbyHomeResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_LobbyHomeResponse>();
        }

        public override void Dispose()
        {
            Profile = default;
            Rooms.Clear();
            MatchStatus = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_LobbyHomeResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_LobbyHomeResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public PlayerProfileInfo Profile { get; set; }
        [ProtoMember(3)]
        public List<RoomSummaryInfo> Rooms { get; set; } = new List<RoomSummaryInfo>();
        [ProtoMember(4)]
        public MatchStatusInfo MatchStatus { get; set; }
    }
    /// <summary>
    /// 匹配状态。
    /// </summary>
    [ProtoContract]
    public partial class MatchStatusInfo : AMessage
    {
        public static MatchStatusInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<MatchStatusInfo>();
        }

        public override void Dispose()
        {
            IsMatching = default;
            Mode = default;
            EstimatedSeconds = default;
            AllocatedRoomId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<MatchStatusInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public bool IsMatching { get; set; }
        [ProtoMember(2)]
        public string Mode { get; set; }
        [ProtoMember(3)]
        public int EstimatedSeconds { get; set; }
        [ProtoMember(4)]
        public int AllocatedRoomId { get; set; }
    }
    /// <summary>
    /// 开始匹配。
    /// </summary>
    [ProtoContract]
    public partial class C2G_StartMatchRequest : AMessage, IRequest
    {
        public static C2G_StartMatchRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_StartMatchRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            Mode = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_StartMatchRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_StartMatchRequest; } 
        [ProtoIgnore]
        public G2C_StartMatchResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public string Mode { get; set; }
    }
    [ProtoContract]
    public partial class G2C_StartMatchResponse : AMessage, IResponse
    {
        public static G2C_StartMatchResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_StartMatchResponse>();
        }

        public override void Dispose()
        {
            Status = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_StartMatchResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_StartMatchResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public MatchStatusInfo Status { get; set; }
    }
    /// <summary>
    /// 创建自定义房间。
    /// </summary>
    [ProtoContract]
    public partial class C2G_CreateRoomRequest : AMessage, IRequest
    {
        public static C2G_CreateRoomRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_CreateRoomRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            RoomName = default;
            Mode = default;
            MapId = default;
            MaxPlayers = default;
            IsPrivate = default;
            Password = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_CreateRoomRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_CreateRoomRequest; } 
        [ProtoIgnore]
        public G2C_CreateRoomResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
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
    }
    [ProtoContract]
    public partial class G2C_CreateRoomResponse : AMessage, IResponse
    {
        public static G2C_CreateRoomResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_CreateRoomResponse>();
        }

        public override void Dispose()
        {
            Room = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_CreateRoomResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_CreateRoomResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public RoomDetailInfo Room { get; set; }
    }
    /// <summary>
    /// 加入自定义房间。
    /// </summary>
    [ProtoContract]
    public partial class C2G_JoinRoomRequest : AMessage, IRequest
    {
        public static C2G_JoinRoomRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_JoinRoomRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            RoomId = default;
            Password = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_JoinRoomRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_JoinRoomRequest; } 
        [ProtoIgnore]
        public G2C_JoinRoomResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public int RoomId { get; set; }
        [ProtoMember(3)]
        public string Password { get; set; }
    }
    [ProtoContract]
    public partial class G2C_JoinRoomResponse : AMessage, IResponse
    {
        public static G2C_JoinRoomResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_JoinRoomResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Room = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_JoinRoomResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_JoinRoomResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public RoomDetailInfo Room { get; set; }
    }
    /// <summary>
    /// 离开自定义房间。
    /// </summary>
    [ProtoContract]
    public partial class C2G_LeaveRoomRequest : AMessage, IRequest
    {
        public static C2G_LeaveRoomRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_LeaveRoomRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            RoomId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_LeaveRoomRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_LeaveRoomRequest; } 
        [ProtoIgnore]
        public G2C_LeaveRoomResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public int RoomId { get; set; }
    }
    [ProtoContract]
    public partial class G2C_LeaveRoomResponse : AMessage, IResponse
    {
        public static G2C_LeaveRoomResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_LeaveRoomResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Room = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_LeaveRoomResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_LeaveRoomResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public RoomDetailInfo Room { get; set; }
    }
    /// <summary>
    /// 房间摘要，用于大厅列表。
    /// </summary>
    [ProtoContract]
    public partial class RoomSummaryInfo : AMessage
    {
        public static RoomSummaryInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<RoomSummaryInfo>();
        }

        public override void Dispose()
        {
            RoomId = default;
            RoomName = default;
            Mode = default;
            MapId = default;
            CurrentPlayers = default;
            MaxPlayers = default;
            IsPrivate = default;
            State = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<RoomSummaryInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public int RoomId { get; set; }
        [ProtoMember(2)]
        public string RoomName { get; set; }
        [ProtoMember(3)]
        public string Mode { get; set; }
        [ProtoMember(4)]
        public int MapId { get; set; }
        [ProtoMember(5)]
        public int CurrentPlayers { get; set; }
        [ProtoMember(6)]
        public int MaxPlayers { get; set; }
        [ProtoMember(7)]
        public bool IsPrivate { get; set; }
        [ProtoMember(8)]
        public string State { get; set; }
    }
    /// <summary>
    /// 房间玩家槽位。
    /// </summary>
    [ProtoContract]
    public partial class RoomPlayerInfo : AMessage
    {
        public static RoomPlayerInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<RoomPlayerInfo>();
        }

        public override void Dispose()
        {
            PlayerId = default;
            Nickname = default;
            Level = default;
            IsOwner = default;
            IsReady = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<RoomPlayerInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public long PlayerId { get; set; }
        [ProtoMember(2)]
        public string Nickname { get; set; }
        [ProtoMember(3)]
        public int Level { get; set; }
        [ProtoMember(4)]
        public bool IsOwner { get; set; }
        [ProtoMember(5)]
        public bool IsReady { get; set; }
    }
    /// <summary>
    /// 房间详情。
    /// </summary>
    [ProtoContract]
    public partial class RoomDetailInfo : AMessage
    {
        public static RoomDetailInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<RoomDetailInfo>();
        }

        public override void Dispose()
        {
            Summary = default;
            Players.Clear();
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<RoomDetailInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public RoomSummaryInfo Summary { get; set; }
        [ProtoMember(2)]
        public List<RoomPlayerInfo> Players { get; set; } = new List<RoomPlayerInfo>();
    }
}