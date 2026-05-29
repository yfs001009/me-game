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
            SelectedBuildingCardIds.Clear();
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
        [ProtoMember(8)]
        public List<int> SelectedBuildingCardIds { get; set; } = new List<int>();
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
    /// 拉取自定义房间详情。
    /// </summary>
    [ProtoContract]
    public partial class C2G_RoomDetailRequest : AMessage, IRequest
    {
        public static C2G_RoomDetailRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_RoomDetailRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            RoomId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_RoomDetailRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_RoomDetailRequest; } 
        [ProtoIgnore]
        public G2C_RoomDetailResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public int RoomId { get; set; }
    }
    [ProtoContract]
    public partial class G2C_RoomDetailResponse : AMessage, IResponse
    {
        public static G2C_RoomDetailResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_RoomDetailResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Room = default;
            Battle = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_RoomDetailResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_RoomDetailResponse; } 
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
    /// <summary>
    /// 切换自定义房间准备状态。
    /// </summary>
    [ProtoContract]
    public partial class C2G_SetRoomReadyRequest : AMessage, IRequest
    {
        public static C2G_SetRoomReadyRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_SetRoomReadyRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            RoomId = default;
            IsReady = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_SetRoomReadyRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_SetRoomReadyRequest; } 
        [ProtoIgnore]
        public G2C_SetRoomReadyResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public int RoomId { get; set; }
        [ProtoMember(3)]
        public bool IsReady { get; set; }
    }
    [ProtoContract]
    public partial class G2C_SetRoomReadyResponse : AMessage, IResponse
    {
        public static G2C_SetRoomReadyResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_SetRoomReadyResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Room = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_SetRoomReadyResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_SetRoomReadyResponse; } 
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
    /// 房主开始自定义房间，进入战斗。
    /// </summary>
    [ProtoContract]
    public partial class C2G_StartRoomRequest : AMessage, IRequest
    {
        public static C2G_StartRoomRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_StartRoomRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            RoomId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_StartRoomRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_StartRoomRequest; } 
        [ProtoIgnore]
        public G2C_StartRoomResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public int RoomId { get; set; }
    }
    [ProtoContract]
    public partial class G2C_StartRoomResponse : AMessage, IResponse
    {
        public static G2C_StartRoomResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_StartRoomResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Room = default;
            Battle = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_StartRoomResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_StartRoomResponse; } 
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
    /// <summary>
    /// 战斗启动信息。当前先用于客户端加载地图，后续扩展同步服地址/随机种子/阵营。
    /// </summary>
    [ProtoContract]
    public partial class BattleStartInfo : AMessage
    {
        public static BattleStartInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<BattleStartInfo>();
        }

        public override void Dispose()
        {
            BattleId = default;
            RoomId = default;
            MapId = default;
            MapAsset = default;
            Mode = default;
            BattleHost = default;
            BattlePort = default;
            BattleProtocol = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<BattleStartInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public int BattleId { get; set; }
        [ProtoMember(2)]
        public int RoomId { get; set; }
        [ProtoMember(3)]
        public int MapId { get; set; }
        [ProtoMember(4)]
        public string MapAsset { get; set; }
        [ProtoMember(5)]
        public string Mode { get; set; }
        [ProtoMember(6)]
        public string BattleHost { get; set; }
        [ProtoMember(7)]
        public int BattlePort { get; set; }
        [ProtoMember(8)]
        public string BattleProtocol { get; set; }
    }
    /// <summary>
    /// 战斗玩家同步状态。
    /// </summary>
    [ProtoContract]
    public partial class BattlePlayerStateInfo : AMessage
    {
        public static BattlePlayerStateInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<BattlePlayerStateInfo>();
        }

        public override void Dispose()
        {
            PlayerId = default;
            Nickname = default;
            Camp = default;
            SceneLoaded = default;
            Gold = default;
            Wood = default;
            PosX = default;
            PosY = default;
            MoveSpeed = default;
            Hp = default;
            MaxHp = default;
            SelectedBuildingCardIds.Clear();
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<BattlePlayerStateInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public long PlayerId { get; set; }
        [ProtoMember(2)]
        public string Nickname { get; set; }
        [ProtoMember(3)]
        public string Camp { get; set; }
        [ProtoMember(4)]
        public bool SceneLoaded { get; set; }
        [ProtoMember(5)]
        public int Gold { get; set; }
        [ProtoMember(6)]
        public int Wood { get; set; }
        [ProtoMember(7)]
        public float PosX { get; set; }
        [ProtoMember(8)]
        public float PosY { get; set; }
        [ProtoMember(9)]
        public float MoveSpeed { get; set; }
        [ProtoMember(10)]
        public int Hp { get; set; }
        [ProtoMember(11)]
        public int MaxHp { get; set; }
        [ProtoMember(12)]
        public List<int> SelectedBuildingCardIds { get; set; } = new List<int>();
    }
    /// <summary>
    /// 战斗建筑同步状态。
    /// </summary>
    [ProtoContract]
    public partial class BattleBuildingStateInfo : AMessage
    {
        public static BattleBuildingStateInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<BattleBuildingStateInfo>();
        }

        public override void Dispose()
        {
            InstanceId = default;
            OwnerPlayerId = default;
            BuildingId = default;
            Level = default;
            GridX = default;
            GridY = default;
            Hp = default;
            MaxHp = default;
            State = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<BattleBuildingStateInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public long InstanceId { get; set; }
        [ProtoMember(2)]
        public long OwnerPlayerId { get; set; }
        [ProtoMember(3)]
        public int BuildingId { get; set; }
        [ProtoMember(4)]
        public int Level { get; set; }
        [ProtoMember(5)]
        public int GridX { get; set; }
        [ProtoMember(6)]
        public int GridY { get; set; }
        [ProtoMember(7)]
        public int Hp { get; set; }
        [ProtoMember(8)]
        public int MaxHp { get; set; }
        [ProtoMember(9)]
        public string State { get; set; }
    }
    /// <summary>
    /// 战斗攻击表现事件。服务端发起，客户端只播放表现。
    /// </summary>
    [ProtoContract]
    public partial class BattleAttackEventInfo : AMessage
    {
        public static BattleAttackEventInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<BattleAttackEventInfo>();
        }

        public override void Dispose()
        {
            EventId = default;
            SourceBuildingInstanceId = default;
            TargetPlayerId = default;
            FromX = default;
            FromY = default;
            ToX = default;
            ToY = default;
            Damage = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<BattleAttackEventInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public long EventId { get; set; }
        [ProtoMember(2)]
        public long SourceBuildingInstanceId { get; set; }
        [ProtoMember(3)]
        public long TargetPlayerId { get; set; }
        [ProtoMember(4)]
        public float FromX { get; set; }
        [ProtoMember(5)]
        public float FromY { get; set; }
        [ProtoMember(6)]
        public float ToX { get; set; }
        [ProtoMember(7)]
        public float ToY { get; set; }
        [ProtoMember(8)]
        public int Damage { get; set; }
    }
    /// <summary>
    /// 战斗快照。服务端权威，客户端只显示。
    /// </summary>
    [ProtoContract]
    public partial class BattleSnapshotInfo : AMessage
    {
        public static BattleSnapshotInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<BattleSnapshotInfo>();
        }

        public override void Dispose()
        {
            BattleId = default;
            Tick = default;
            State = default;
            Players.Clear();
            Buildings.Clear();
            AttackEvents.Clear();
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<BattleSnapshotInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public int BattleId { get; set; }
        [ProtoMember(2)]
        public long Tick { get; set; }
        [ProtoMember(3)]
        public string State { get; set; }
        [ProtoMember(4)]
        public List<BattlePlayerStateInfo> Players { get; set; } = new List<BattlePlayerStateInfo>();
        [ProtoMember(5)]
        public List<BattleBuildingStateInfo> Buildings { get; set; } = new List<BattleBuildingStateInfo>();
        [ProtoMember(6)]
        public List<BattleAttackEventInfo> AttackEvents { get; set; } = new List<BattleAttackEventInfo>();
    }
    /// <summary>
    /// 客户端场景加载完成，等待所有玩家准备后服务端进入 Running。
    /// </summary>
    [ProtoContract]
    public partial class C2G_BattleSceneLoadedRequest : AMessage, IRequest
    {
        public static C2G_BattleSceneLoadedRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_BattleSceneLoadedRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            BattleId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_BattleSceneLoadedRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_BattleSceneLoadedRequest; } 
        [ProtoIgnore]
        public G2C_BattleSceneLoadedResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public int BattleId { get; set; }
    }
    [ProtoContract]
    public partial class G2C_BattleSceneLoadedResponse : AMessage, IResponse
    {
        public static G2C_BattleSceneLoadedResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_BattleSceneLoadedResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Snapshot = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_BattleSceneLoadedResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_BattleSceneLoadedResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public BattleSnapshotInfo Snapshot { get; set; }
    }
    /// <summary>
    /// 拉取战斗快照。当前先用固定间隔 RPC 模拟持续同步，后续替换为 S2C 推送。
    /// </summary>
    [ProtoContract]
    public partial class C2G_BattleSnapshotRequest : AMessage, IRequest
    {
        public static C2G_BattleSnapshotRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_BattleSnapshotRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            BattleId = default;
            LastKnownTick = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_BattleSnapshotRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_BattleSnapshotRequest; } 
        [ProtoIgnore]
        public G2C_BattleSnapshotResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public int BattleId { get; set; }
        [ProtoMember(3)]
        public long LastKnownTick { get; set; }
    }
    [ProtoContract]
    public partial class G2C_BattleSnapshotResponse : AMessage, IResponse
    {
        public static G2C_BattleSnapshotResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_BattleSnapshotResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Snapshot = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_BattleSnapshotResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_BattleSnapshotResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public BattleSnapshotInfo Snapshot { get; set; }
    }
    /// <summary>
    /// 玩家移动输入。客户端提交 WASD 方向，服务端权威更新位置并通过快照同步。
    /// </summary>
    [ProtoContract]
    public partial class C2G_BattleMoveCommand : AMessage, IRequest
    {
        public static C2G_BattleMoveCommand Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_BattleMoveCommand>();
        }

        public override void Dispose()
        {
            Token = default;
            BattleId = default;
            AxisX = default;
            AxisY = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_BattleMoveCommand>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_BattleMoveCommand; } 
        [ProtoIgnore]
        public G2C_BattleMoveCommandResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public int BattleId { get; set; }
        [ProtoMember(3)]
        public float AxisX { get; set; }
        [ProtoMember(4)]
        public float AxisY { get; set; }
    }
    [ProtoContract]
    public partial class G2C_BattleMoveCommandResponse : AMessage, IResponse
    {
        public static G2C_BattleMoveCommandResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_BattleMoveCommandResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Snapshot = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_BattleMoveCommandResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_BattleMoveCommandResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public BattleSnapshotInfo Snapshot { get; set; }
    }
    /// <summary>
    /// 建造命令。客户端只提交意图，服务端校验资源、占格、战斗状态后返回快照。
    /// </summary>
    [ProtoContract]
    public partial class C2G_BuildCommand : AMessage, IRequest
    {
        public static C2G_BuildCommand Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_BuildCommand>();
        }

        public override void Dispose()
        {
            Token = default;
            BattleId = default;
            BuildingId = default;
            GridX = default;
            GridY = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_BuildCommand>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_BuildCommand; } 
        [ProtoIgnore]
        public G2C_BuildCommandResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
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
    public partial class G2C_BuildCommandResponse : AMessage, IResponse
    {
        public static G2C_BuildCommandResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_BuildCommandResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Snapshot = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_BuildCommandResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_BuildCommandResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public BattleSnapshotInfo Snapshot { get; set; }
    }
    /// <summary>
    /// 升级建筑。
    /// </summary>
    [ProtoContract]
    public partial class C2G_UpgradeBuildingCommand : AMessage, IRequest
    {
        public static C2G_UpgradeBuildingCommand Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_UpgradeBuildingCommand>();
        }

        public override void Dispose()
        {
            Token = default;
            BattleId = default;
            BuildingInstanceId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_UpgradeBuildingCommand>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_UpgradeBuildingCommand; } 
        [ProtoIgnore]
        public G2C_UpgradeBuildingCommandResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public int BattleId { get; set; }
        [ProtoMember(3)]
        public long BuildingInstanceId { get; set; }
    }
    [ProtoContract]
    public partial class G2C_UpgradeBuildingCommandResponse : AMessage, IResponse
    {
        public static G2C_UpgradeBuildingCommandResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_UpgradeBuildingCommandResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Snapshot = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_UpgradeBuildingCommandResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_UpgradeBuildingCommandResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public BattleSnapshotInfo Snapshot { get; set; }
    }
    /// <summary>
    /// 回收建筑。
    /// </summary>
    [ProtoContract]
    public partial class C2G_RecycleBuildingCommand : AMessage, IRequest
    {
        public static C2G_RecycleBuildingCommand Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_RecycleBuildingCommand>();
        }

        public override void Dispose()
        {
            Token = default;
            BattleId = default;
            BuildingInstanceId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_RecycleBuildingCommand>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_RecycleBuildingCommand; } 
        [ProtoIgnore]
        public G2C_RecycleBuildingCommandResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public int BattleId { get; set; }
        [ProtoMember(3)]
        public long BuildingInstanceId { get; set; }
    }
    [ProtoContract]
    public partial class G2C_RecycleBuildingCommandResponse : AMessage, IResponse
    {
        public static G2C_RecycleBuildingCommandResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_RecycleBuildingCommandResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Snapshot = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_RecycleBuildingCommandResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_RecycleBuildingCommandResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public BattleSnapshotInfo Snapshot { get; set; }
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
            SelectedBuildingCardIds.Clear();
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
        [ProtoMember(6)]
        public List<int> SelectedBuildingCardIds { get; set; } = new List<int>();
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