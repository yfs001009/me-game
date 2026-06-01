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
    /// 聊天消息树节点。
    /// </summary>
    [ProtoContract]
    public partial class ChatInfoNode : AMessage
    {
        public static ChatInfoNode Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<ChatInfoNode>();
        }

        public override void Dispose()
        {
            NodeType = default;
            NodeEvent = default;
            Content = default;
            Color = default;
            Data = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<ChatInfoNode>(this);
#endif
        }
        [ProtoMember(1)]
        public int NodeType { get; set; }
        [ProtoMember(2)]
        public int NodeEvent { get; set; }
        [ProtoMember(3)]
        public string Content { get; set; }
        [ProtoMember(4)]
        public string Color { get; set; }
        [ProtoMember(5)]
        public string Data { get; set; }
    }
    /// <summary>
    /// 通用聊天消息树。
    /// </summary>
    [ProtoContract]
    public partial class ChatMessageTreeInfo : AMessage
    {
        public static ChatMessageTreeInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<ChatMessageTreeInfo>();
        }

        public override void Dispose()
        {
            ChannelType = default;
            ChannelId = default;
            UnitId = default;
            UserName = default;
            IsHaveLinkItem = default;
            SystemBroadcastId = default;
            Targets.Clear();
            Nodes.Clear();
            MessageId = default;
            SendTimeUnixSeconds = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<ChatMessageTreeInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public int ChannelType { get; set; }
        [ProtoMember(2)]
        public long ChannelId { get; set; }
        [ProtoMember(3)]
        public long UnitId { get; set; }
        [ProtoMember(4)]
        public string UserName { get; set; }
        [ProtoMember(5)]
        public bool IsHaveLinkItem { get; set; }
        [ProtoMember(6)]
        public int SystemBroadcastId { get; set; }
        [ProtoMember(7)]
        public List<long> Targets { get; set; } = new List<long>();
        [ProtoMember(8)]
        public List<ChatInfoNode> Nodes { get; set; } = new List<ChatInfoNode>();
        [ProtoMember(9)]
        public long MessageId { get; set; }
        [ProtoMember(10)]
        public long SendTimeUnixSeconds { get; set; }
    }
    /// <summary>
    /// 发送聊天消息。
    /// </summary>
    [ProtoContract]
    public partial class C2G_SendChatMessageRequest : AMessage, IRequest
    {
        public static C2G_SendChatMessageRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_SendChatMessageRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            MessageTree = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_SendChatMessageRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_SendChatMessageRequest; } 
        [ProtoIgnore]
        public G2C_SendChatMessageResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public ChatMessageTreeInfo MessageTree { get; set; }
    }
    [ProtoContract]
    public partial class G2C_SendChatMessageResponse : AMessage, IResponse
    {
        public static G2C_SendChatMessageResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_SendChatMessageResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            MessageTree = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_SendChatMessageResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_SendChatMessageResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public ChatMessageTreeInfo MessageTree { get; set; }
    }
    /// <summary>
    /// 拉取最近聊天消息。用于登录后补世界频道最近记录。
    /// </summary>
    [ProtoContract]
    public partial class C2G_ChatHistoryRequest : AMessage, IRequest
    {
        public static C2G_ChatHistoryRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_ChatHistoryRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            ChannelType = default;
            ChannelId = default;
            Limit = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_ChatHistoryRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_ChatHistoryRequest; } 
        [ProtoIgnore]
        public G2C_ChatHistoryResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public int ChannelType { get; set; }
        [ProtoMember(3)]
        public long ChannelId { get; set; }
        [ProtoMember(4)]
        public int Limit { get; set; }
    }
    [ProtoContract]
    public partial class G2C_ChatHistoryResponse : AMessage, IResponse
    {
        public static G2C_ChatHistoryResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_ChatHistoryResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Messages.Clear();
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_ChatHistoryResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_ChatHistoryResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public List<ChatMessageTreeInfo> Messages { get; set; } = new List<ChatMessageTreeInfo>();
    }
    /// <summary>
    /// 服务端主动推送聊天消息。
    /// </summary>
    [ProtoContract]
    public partial class G2C_ChatMessageNotify : AMessage, IMessage
    {
        public static G2C_ChatMessageNotify Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_ChatMessageNotify>();
        }

        public override void Dispose()
        {
            MessageTree = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_ChatMessageNotify>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_ChatMessageNotify; } 
        [ProtoMember(1)]
        public ChatMessageTreeInfo MessageTree { get; set; }
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
    /// 角色基础信息。配置驱动，服务端补充拥有与选中状态。
    /// </summary>
    [ProtoContract]
    public partial class CharacterInfo : AMessage
    {
        public static CharacterInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<CharacterInfo>();
        }

        public override void Dispose()
        {
            CharacterId = default;
            Category = default;
            Race = default;
            Name = default;
            AbilityId = default;
            AbilityName = default;
            AbilityDesc = default;
            IconAsset = default;
            PrefabAsset = default;
            IsUnlocked = default;
            IsSelected = default;
            Description = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<CharacterInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public int CharacterId { get; set; }
        [ProtoMember(2)]
        public string Category { get; set; }
        [ProtoMember(3)]
        public string Race { get; set; }
        [ProtoMember(4)]
        public string Name { get; set; }
        [ProtoMember(5)]
        public int AbilityId { get; set; }
        [ProtoMember(6)]
        public string AbilityName { get; set; }
        [ProtoMember(7)]
        public string AbilityDesc { get; set; }
        [ProtoMember(8)]
        public string IconAsset { get; set; }
        [ProtoMember(9)]
        public string PrefabAsset { get; set; }
        [ProtoMember(10)]
        public bool IsUnlocked { get; set; }
        [ProtoMember(11)]
        public bool IsSelected { get; set; }
        [ProtoMember(12)]
        public string Description { get; set; }
    }
    /// <summary>
    /// 拉取玩家角色列表。
    /// </summary>
    [ProtoContract]
    public partial class C2G_CharacterListRequest : AMessage, IRequest
    {
        public static C2G_CharacterListRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_CharacterListRequest>();
        }

        public override void Dispose()
        {
            Token = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_CharacterListRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_CharacterListRequest; } 
        [ProtoIgnore]
        public G2C_CharacterListResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
    }
    [ProtoContract]
    public partial class G2C_CharacterListResponse : AMessage, IResponse
    {
        public static G2C_CharacterListResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_CharacterListResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Characters.Clear();
            SelectedHeroId = default;
            SelectedGhostId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_CharacterListResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_CharacterListResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public List<CharacterInfo> Characters { get; set; } = new List<CharacterInfo>();
        [ProtoMember(5)]
        public int SelectedHeroId { get; set; }
        [ProtoMember(6)]
        public int SelectedGhostId { get; set; }
    }
    /// <summary>
    /// 选择出战角色。
    /// </summary>
    [ProtoContract]
    public partial class C2G_SelectCharacterRequest : AMessage, IRequest
    {
        public static C2G_SelectCharacterRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_SelectCharacterRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            CharacterId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_SelectCharacterRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_SelectCharacterRequest; } 
        [ProtoIgnore]
        public G2C_SelectCharacterResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public int CharacterId { get; set; }
    }
    [ProtoContract]
    public partial class G2C_SelectCharacterResponse : AMessage, IResponse
    {
        public static G2C_SelectCharacterResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_SelectCharacterResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Characters.Clear();
            SelectedHeroId = default;
            SelectedGhostId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_SelectCharacterResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_SelectCharacterResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public List<CharacterInfo> Characters { get; set; } = new List<CharacterInfo>();
        [ProtoMember(5)]
        public int SelectedHeroId { get; set; }
        [ProtoMember(6)]
        public int SelectedGhostId { get; set; }
    }
    /// <summary>
    /// 货币余额。
    /// </summary>
    [ProtoContract]
    public partial class CurrencyBalanceInfo : AMessage
    {
        public static CurrencyBalanceInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<CurrencyBalanceInfo>();
        }

        public override void Dispose()
        {
            CurrencyId = default;
            Code = default;
            Name = default;
            Amount = default;
            IconAsset = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<CurrencyBalanceInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public int CurrencyId { get; set; }
        [ProtoMember(2)]
        public string Code { get; set; }
        [ProtoMember(3)]
        public string Name { get; set; }
        [ProtoMember(4)]
        public long Amount { get; set; }
        [ProtoMember(5)]
        public string IconAsset { get; set; }
    }
    /// <summary>
    /// 背包道具堆叠。
    /// </summary>
    [ProtoContract]
    public partial class BagItemInfo : AMessage
    {
        public static BagItemInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<BagItemInfo>();
        }

        public override void Dispose()
        {
            ItemId = default;
            ItemType = default;
            Name = default;
            Count = default;
            MaxStack = default;
            UseType = default;
            IconAsset = default;
            Description = default;
            Quality = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<BagItemInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public int ItemId { get; set; }
        [ProtoMember(2)]
        public string ItemType { get; set; }
        [ProtoMember(3)]
        public string Name { get; set; }
        [ProtoMember(4)]
        public int Count { get; set; }
        [ProtoMember(5)]
        public int MaxStack { get; set; }
        [ProtoMember(6)]
        public string UseType { get; set; }
        [ProtoMember(7)]
        public string IconAsset { get; set; }
        [ProtoMember(8)]
        public string Description { get; set; }
        [ProtoMember(9)]
        public int Quality { get; set; }
    }
    /// <summary>
    /// 临时增益状态。
    /// </summary>
    [ProtoContract]
    public partial class BuffStateInfo : AMessage
    {
        public static BuffStateInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<BuffStateInfo>();
        }

        public override void Dispose()
        {
            BuffKey = default;
            ExpiresAtUnixSeconds = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<BuffStateInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public string BuffKey { get; set; }
        [ProtoMember(2)]
        public long ExpiresAtUnixSeconds { get; set; }
    }
    /// <summary>
    /// 局外进度值，如保底、任务或赛季经验。
    /// </summary>
    [ProtoContract]
    public partial class ProgressValueInfo : AMessage
    {
        public static ProgressValueInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<ProgressValueInfo>();
        }

        public override void Dispose()
        {
            Key = default;
            Value = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<ProgressValueInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public string Key { get; set; }
        [ProtoMember(2)]
        public long Value { get; set; }
    }
    /// <summary>
    /// 局外资产快照。
    /// </summary>
    [ProtoContract]
    public partial class AssetSnapshotInfo : AMessage
    {
        public static AssetSnapshotInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<AssetSnapshotInfo>();
        }

        public override void Dispose()
        {
            Currencies.Clear();
            BagItems.Clear();
            UnlockedCharacterIds.Clear();
            UnlockedBuildingCardIds.Clear();
            Buffs.Clear();
            ProgressValues.Clear();
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<AssetSnapshotInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public List<CurrencyBalanceInfo> Currencies { get; set; } = new List<CurrencyBalanceInfo>();
        [ProtoMember(2)]
        public List<BagItemInfo> BagItems { get; set; } = new List<BagItemInfo>();
        [ProtoMember(3)]
        public List<int> UnlockedCharacterIds { get; set; } = new List<int>();
        [ProtoMember(4)]
        public List<int> UnlockedBuildingCardIds { get; set; } = new List<int>();
        [ProtoMember(5)]
        public List<BuffStateInfo> Buffs { get; set; } = new List<BuffStateInfo>();
        [ProtoMember(6)]
        public List<ProgressValueInfo> ProgressValues { get; set; } = new List<ProgressValueInfo>();
    }
    /// <summary>
    /// 拉取局外资产。
    /// </summary>
    [ProtoContract]
    public partial class C2G_AssetSnapshotRequest : AMessage, IRequest
    {
        public static C2G_AssetSnapshotRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_AssetSnapshotRequest>();
        }

        public override void Dispose()
        {
            Token = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_AssetSnapshotRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_AssetSnapshotRequest; } 
        [ProtoIgnore]
        public G2C_AssetSnapshotResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
    }
    [ProtoContract]
    public partial class G2C_AssetSnapshotResponse : AMessage, IResponse
    {
        public static G2C_AssetSnapshotResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_AssetSnapshotResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Snapshot = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_AssetSnapshotResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_AssetSnapshotResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public AssetSnapshotInfo Snapshot { get; set; }
    }
    /// <summary>
    /// 使用背包道具。
    /// </summary>
    [ProtoContract]
    public partial class C2G_UseItemRequest : AMessage, IRequest
    {
        public static C2G_UseItemRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_UseItemRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            ItemId = default;
            Count = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_UseItemRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_UseItemRequest; } 
        [ProtoIgnore]
        public G2C_UseItemResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public int ItemId { get; set; }
        [ProtoMember(3)]
        public int Count { get; set; }
    }
    [ProtoContract]
    public partial class G2C_UseItemResponse : AMessage, IResponse
    {
        public static G2C_UseItemResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_UseItemResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Snapshot = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_UseItemResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_UseItemResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public AssetSnapshotInfo Snapshot { get; set; }
    }
    /// <summary>
    /// 局外商店商品。FeatureId 引用统一开放定义表，控制活动/功能同步开放。
    /// </summary>
    [ProtoContract]
    public partial class OutgameShopGoodsInfo : AMessage
    {
        public static OutgameShopGoodsInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<OutgameShopGoodsInfo>();
        }

        public override void Dispose()
        {
            GoodsId = default;
            ShopId = default;
            GoodsGroupId = default;
            Name = default;
            PriceCurrencyId = default;
            PriceItemId = default;
            PriceAmount = default;
            BuyLimit = default;
            BoughtCount = default;
            IsAvailable = default;
            UnlockRule = default;
            Description = default;
            Reward = default;
            FeatureId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<OutgameShopGoodsInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public int GoodsId { get; set; }
        [ProtoMember(2)]
        public int ShopId { get; set; }
        [ProtoMember(3)]
        public int GoodsGroupId { get; set; }
        [ProtoMember(4)]
        public string Name { get; set; }
        [ProtoMember(5)]
        public int PriceCurrencyId { get; set; }
        [ProtoMember(6)]
        public int PriceItemId { get; set; }
        [ProtoMember(7)]
        public long PriceAmount { get; set; }
        [ProtoMember(8)]
        public int BuyLimit { get; set; }
        [ProtoMember(9)]
        public int BoughtCount { get; set; }
        [ProtoMember(10)]
        public bool IsAvailable { get; set; }
        [ProtoMember(11)]
        public string UnlockRule { get; set; }
        [ProtoMember(12)]
        public string Description { get; set; }
        [ProtoMember(13)]
        public RewardInfo Reward { get; set; }
        [ProtoMember(14)]
        public string FeatureId { get; set; }
    }
    /// <summary>
    /// 局外商店。FeatureId 引用统一开放定义表，ActivityId 仅用于活动归类/筛选。
    /// </summary>
    [ProtoContract]
    public partial class OutgameShopInfo : AMessage
    {
        public static OutgameShopInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<OutgameShopInfo>();
        }

        public override void Dispose()
        {
            ShopId = default;
            ShopName = default;
            ShopType = default;
            ActivityId = default;
            RefreshGroup = default;
            OpensAtUnixSeconds = default;
            ClosesAtUnixSeconds = default;
            Goods.Clear();
            FeatureId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<OutgameShopInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public int ShopId { get; set; }
        [ProtoMember(2)]
        public string ShopName { get; set; }
        [ProtoMember(3)]
        public string ShopType { get; set; }
        [ProtoMember(4)]
        public string ActivityId { get; set; }
        [ProtoMember(5)]
        public string RefreshGroup { get; set; }
        [ProtoMember(6)]
        public long OpensAtUnixSeconds { get; set; }
        [ProtoMember(7)]
        public long ClosesAtUnixSeconds { get; set; }
        [ProtoMember(8)]
        public List<OutgameShopGoodsInfo> Goods { get; set; } = new List<OutgameShopGoodsInfo>();
        [ProtoMember(9)]
        public string FeatureId { get; set; }
    }
    /// <summary>
    /// 拉取局外商店列表。ShopType/ActivityId 为空表示全部。
    /// </summary>
    [ProtoContract]
    public partial class C2G_OutgameShopListRequest : AMessage, IRequest
    {
        public static C2G_OutgameShopListRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_OutgameShopListRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            ShopType = default;
            ActivityId = default;
            FeatureId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_OutgameShopListRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_OutgameShopListRequest; } 
        [ProtoIgnore]
        public G2C_OutgameShopListResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public string ShopType { get; set; }
        [ProtoMember(3)]
        public string ActivityId { get; set; }
        [ProtoMember(4)]
        public string FeatureId { get; set; }
    }
    [ProtoContract]
    public partial class G2C_OutgameShopListResponse : AMessage, IResponse
    {
        public static G2C_OutgameShopListResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_OutgameShopListResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Shops.Clear();
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_OutgameShopListResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_OutgameShopListResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public List<OutgameShopInfo> Shops { get; set; } = new List<OutgameShopInfo>();
    }
    /// <summary>
    /// 购买局外商品。
    /// </summary>
    [ProtoContract]
    public partial class C2G_BuyOutgameShopGoodsRequest : AMessage, IRequest
    {
        public static C2G_BuyOutgameShopGoodsRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_BuyOutgameShopGoodsRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            GoodsId = default;
            Count = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_BuyOutgameShopGoodsRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_BuyOutgameShopGoodsRequest; } 
        [ProtoIgnore]
        public G2C_BuyOutgameShopGoodsResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public int GoodsId { get; set; }
        [ProtoMember(3)]
        public int Count { get; set; }
    }
    [ProtoContract]
    public partial class G2C_BuyOutgameShopGoodsResponse : AMessage, IResponse
    {
        public static G2C_BuyOutgameShopGoodsResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_BuyOutgameShopGoodsResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Goods = default;
            Snapshot = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_BuyOutgameShopGoodsResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_BuyOutgameShopGoodsResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public OutgameShopGoodsInfo Goods { get; set; }
        [ProtoMember(5)]
        public AssetSnapshotInfo Snapshot { get; set; }
    }
    /// <summary>
    /// 奖励内容，用于邮件附件和奖励预览。
    /// </summary>
    [ProtoContract]
    public partial class RewardInfo : AMessage
    {
        public static RewardInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<RewardInfo>();
        }

        public override void Dispose()
        {
            Currencies.Clear();
            Items.Clear();
            CharacterIds.Clear();
            BuildingCardIds.Clear();
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<RewardInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public List<CurrencyBalanceInfo> Currencies { get; set; } = new List<CurrencyBalanceInfo>();
        [ProtoMember(2)]
        public List<BagItemInfo> Items { get; set; } = new List<BagItemInfo>();
        [ProtoMember(3)]
        public List<int> CharacterIds { get; set; } = new List<int>();
        [ProtoMember(4)]
        public List<int> BuildingCardIds { get; set; } = new List<int>();
    }
    /// <summary>
    /// 邮件摘要。
    /// </summary>
    [ProtoContract]
    public partial class MailInfo : AMessage
    {
        public static MailInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<MailInfo>();
        }

        public override void Dispose()
        {
            MailId = default;
            Title = default;
            Content = default;
            Sender = default;
            SentAtUnixSeconds = default;
            IsRead = default;
            IsAttachmentClaimed = default;
            Attachment = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<MailInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public long MailId { get; set; }
        [ProtoMember(2)]
        public string Title { get; set; }
        [ProtoMember(3)]
        public string Content { get; set; }
        [ProtoMember(4)]
        public string Sender { get; set; }
        [ProtoMember(5)]
        public long SentAtUnixSeconds { get; set; }
        [ProtoMember(6)]
        public bool IsRead { get; set; }
        [ProtoMember(7)]
        public bool IsAttachmentClaimed { get; set; }
        [ProtoMember(8)]
        public RewardInfo Attachment { get; set; }
    }
    /// <summary>
    /// 拉取邮件列表。
    /// </summary>
    [ProtoContract]
    public partial class C2G_MailListRequest : AMessage, IRequest
    {
        public static C2G_MailListRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_MailListRequest>();
        }

        public override void Dispose()
        {
            Token = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_MailListRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_MailListRequest; } 
        [ProtoIgnore]
        public G2C_MailListResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
    }
    [ProtoContract]
    public partial class G2C_MailListResponse : AMessage, IResponse
    {
        public static G2C_MailListResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_MailListResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Mails.Clear();
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_MailListResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_MailListResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public List<MailInfo> Mails { get; set; } = new List<MailInfo>();
    }
    /// <summary>
    /// 标记邮件已读。
    /// </summary>
    [ProtoContract]
    public partial class C2G_ReadMailRequest : AMessage, IRequest
    {
        public static C2G_ReadMailRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_ReadMailRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            MailId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_ReadMailRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_ReadMailRequest; } 
        [ProtoIgnore]
        public G2C_ReadMailResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public long MailId { get; set; }
    }
    [ProtoContract]
    public partial class G2C_ReadMailResponse : AMessage, IResponse
    {
        public static G2C_ReadMailResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_ReadMailResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Mails.Clear();
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_ReadMailResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_ReadMailResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public List<MailInfo> Mails { get; set; } = new List<MailInfo>();
    }
    /// <summary>
    /// 领取邮件附件。
    /// </summary>
    [ProtoContract]
    public partial class C2G_ClaimMailAttachmentRequest : AMessage, IRequest
    {
        public static C2G_ClaimMailAttachmentRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_ClaimMailAttachmentRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            MailId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_ClaimMailAttachmentRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_ClaimMailAttachmentRequest; } 
        [ProtoIgnore]
        public G2C_ClaimMailAttachmentResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public long MailId { get; set; }
    }
    [ProtoContract]
    public partial class G2C_ClaimMailAttachmentResponse : AMessage, IResponse
    {
        public static G2C_ClaimMailAttachmentResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_ClaimMailAttachmentResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Mails.Clear();
            Snapshot = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_ClaimMailAttachmentResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_ClaimMailAttachmentResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public List<MailInfo> Mails { get; set; } = new List<MailInfo>();
        [ProtoMember(5)]
        public AssetSnapshotInfo Snapshot { get; set; }
    }
    /// <summary>
    /// 抽奖结果。
    /// </summary>
    [ProtoContract]
    public partial class LotteryDrawResultInfo : AMessage
    {
        public static LotteryDrawResultInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<LotteryDrawResultInfo>();
        }

        public override void Dispose()
        {
            Pool = default;
            Reward = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<LotteryDrawResultInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public string Pool { get; set; }
        [ProtoMember(2)]
        public RewardInfo Reward { get; set; }
    }
    /// <summary>
    /// 抽奖。
    /// </summary>
    [ProtoContract]
    public partial class C2G_LotteryDrawRequest : AMessage, IRequest
    {
        public static C2G_LotteryDrawRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_LotteryDrawRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            Pool = default;
            Count = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_LotteryDrawRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_LotteryDrawRequest; } 
        [ProtoIgnore]
        public G2C_LotteryDrawResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public string Pool { get; set; }
        [ProtoMember(3)]
        public int Count { get; set; }
    }
    [ProtoContract]
    public partial class G2C_LotteryDrawResponse : AMessage, IResponse
    {
        public static G2C_LotteryDrawResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_LotteryDrawResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Results.Clear();
            Snapshot = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_LotteryDrawResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_LotteryDrawResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public List<LotteryDrawResultInfo> Results { get; set; } = new List<LotteryDrawResultInfo>();
        [ProtoMember(5)]
        public AssetSnapshotInfo Snapshot { get; set; }
    }
    /// <summary>
    /// 局外任务状态。FeatureId 引用统一开放定义表，TaskType 仅用于任务归类/筛选。
    /// </summary>
    [ProtoContract]
    public partial class OutgameTaskInfo : AMessage
    {
        public static OutgameTaskInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<OutgameTaskInfo>();
        }

        public override void Dispose()
        {
            TaskId = default;
            TaskType = default;
            ActivityId = default;
            Title = default;
            Description = default;
            ProgressKey = default;
            Current = default;
            Target = default;
            State = default;
            RefreshGroup = default;
            EndsAtUnixSeconds = default;
            Reward = default;
            FeatureId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<OutgameTaskInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public int TaskId { get; set; }
        [ProtoMember(2)]
        public string TaskType { get; set; }
        [ProtoMember(3)]
        public string ActivityId { get; set; }
        [ProtoMember(4)]
        public string Title { get; set; }
        [ProtoMember(5)]
        public string Description { get; set; }
        [ProtoMember(6)]
        public string ProgressKey { get; set; }
        [ProtoMember(7)]
        public long Current { get; set; }
        [ProtoMember(8)]
        public long Target { get; set; }
        [ProtoMember(9)]
        public string State { get; set; }
        [ProtoMember(10)]
        public string RefreshGroup { get; set; }
        [ProtoMember(11)]
        public long EndsAtUnixSeconds { get; set; }
        [ProtoMember(12)]
        public RewardInfo Reward { get; set; }
        [ProtoMember(13)]
        public string FeatureId { get; set; }
    }
    /// <summary>
    /// 拉取局外任务列表。TaskType/ActivityId 为空表示全部。
    /// </summary>
    [ProtoContract]
    public partial class C2G_OutgameTaskListRequest : AMessage, IRequest
    {
        public static C2G_OutgameTaskListRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_OutgameTaskListRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            TaskType = default;
            ActivityId = default;
            FeatureId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_OutgameTaskListRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_OutgameTaskListRequest; } 
        [ProtoIgnore]
        public G2C_OutgameTaskListResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public string TaskType { get; set; }
        [ProtoMember(3)]
        public string ActivityId { get; set; }
        [ProtoMember(4)]
        public string FeatureId { get; set; }
    }
    [ProtoContract]
    public partial class G2C_OutgameTaskListResponse : AMessage, IResponse
    {
        public static G2C_OutgameTaskListResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_OutgameTaskListResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Tasks.Clear();
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_OutgameTaskListResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_OutgameTaskListResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public List<OutgameTaskInfo> Tasks { get; set; } = new List<OutgameTaskInfo>();
    }
    /// <summary>
    /// 领取局外任务奖励。
    /// </summary>
    [ProtoContract]
    public partial class C2G_ClaimOutgameTaskRewardRequest : AMessage, IRequest
    {
        public static C2G_ClaimOutgameTaskRewardRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_ClaimOutgameTaskRewardRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            TaskId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_ClaimOutgameTaskRewardRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_ClaimOutgameTaskRewardRequest; } 
        [ProtoIgnore]
        public G2C_ClaimOutgameTaskRewardResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public int TaskId { get; set; }
    }
    [ProtoContract]
    public partial class G2C_ClaimOutgameTaskRewardResponse : AMessage, IResponse
    {
        public static G2C_ClaimOutgameTaskRewardResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_ClaimOutgameTaskRewardResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Tasks.Clear();
            Snapshot = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_ClaimOutgameTaskRewardResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_ClaimOutgameTaskRewardResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public List<OutgameTaskInfo> Tasks { get; set; } = new List<OutgameTaskInfo>();
        [ProtoMember(5)]
        public AssetSnapshotInfo Snapshot { get; set; }
    }
    /// <summary>
    /// 社交列表中的玩家。
    /// </summary>
    [ProtoContract]
    public partial class SocialPlayerInfo : AMessage
    {
        public static SocialPlayerInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<SocialPlayerInfo>();
        }

        public override void Dispose()
        {
            Profile = default;
            IsFollowing = default;
            IsFollower = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<SocialPlayerInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public PlayerProfileInfo Profile { get; set; }
        [ProtoMember(2)]
        public bool IsFollowing { get; set; }
        [ProtoMember(3)]
        public bool IsFollower { get; set; }
    }
    /// <summary>
    /// 拉取关注/粉丝列表。ViewMode 使用 Following/Fans/Search。
    /// </summary>
    [ProtoContract]
    public partial class C2G_SocialListRequest : AMessage, IRequest
    {
        public static C2G_SocialListRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_SocialListRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            ViewMode = default;
            Keyword = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_SocialListRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_SocialListRequest; } 
        [ProtoIgnore]
        public G2C_SocialListResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public string ViewMode { get; set; }
        [ProtoMember(3)]
        public string Keyword { get; set; }
    }
    [ProtoContract]
    public partial class G2C_SocialListResponse : AMessage, IResponse
    {
        public static G2C_SocialListResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_SocialListResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Players.Clear();
            FollowingCount = default;
            FollowerCount = default;
            ViewMode = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_SocialListResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_SocialListResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public List<SocialPlayerInfo> Players { get; set; } = new List<SocialPlayerInfo>();
        [ProtoMember(5)]
        public int FollowingCount { get; set; }
        [ProtoMember(6)]
        public int FollowerCount { get; set; }
        [ProtoMember(7)]
        public string ViewMode { get; set; }
    }
    /// <summary>
    /// 关注或取消关注玩家。
    /// </summary>
    [ProtoContract]
    public partial class C2G_FollowPlayerRequest : AMessage, IRequest
    {
        public static C2G_FollowPlayerRequest Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_FollowPlayerRequest>();
        }

        public override void Dispose()
        {
            Token = default;
            TargetPlayerId = default;
            Follow = default;
            ViewMode = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_FollowPlayerRequest>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_FollowPlayerRequest; } 
        [ProtoIgnore]
        public G2C_FollowPlayerResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public long TargetPlayerId { get; set; }
        [ProtoMember(3)]
        public bool Follow { get; set; }
        [ProtoMember(4)]
        public string ViewMode { get; set; }
    }
    [ProtoContract]
    public partial class G2C_FollowPlayerResponse : AMessage, IResponse
    {
        public static G2C_FollowPlayerResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_FollowPlayerResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Players.Clear();
            FollowingCount = default;
            FollowerCount = default;
            ViewMode = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_FollowPlayerResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_FollowPlayerResponse; } 
        [ProtoMember(1)]
        public uint ErrorCode { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
        [ProtoMember(4)]
        public List<SocialPlayerInfo> Players { get; set; } = new List<SocialPlayerInfo>();
        [ProtoMember(5)]
        public int FollowingCount { get; set; }
        [ProtoMember(6)]
        public int FollowerCount { get; set; }
        [ProtoMember(7)]
        public string ViewMode { get; set; }
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
            EquipmentSlots.Clear();
            Attack = default;
            AttackRange = default;
            AttackIntervalMs = default;
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
        [ProtoMember(13)]
        public List<BattleEquipmentSlotInfo> EquipmentSlots { get; set; } = new List<BattleEquipmentSlotInfo>();
        [ProtoMember(14)]
        public int Attack { get; set; }
        [ProtoMember(15)]
        public float AttackRange { get; set; }
        [ProtoMember(16)]
        public int AttackIntervalMs { get; set; }
    }
    /// <summary>
    /// 巨魔局内装备格。SlotIndex 固定 0-5，空格 ItemId 为 0。
    /// </summary>
    [ProtoContract]
    public partial class BattleEquipmentSlotInfo : AMessage
    {
        public static BattleEquipmentSlotInfo Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<BattleEquipmentSlotInfo>();
        }

        public override void Dispose()
        {
            SlotIndex = default;
            ItemId = default;
            GoodsId = default;
            ItemName = default;
            EffectDesc = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<BattleEquipmentSlotInfo>(this);
#endif
        }
        [ProtoMember(1)]
        public int SlotIndex { get; set; }
        [ProtoMember(2)]
        public int ItemId { get; set; }
        [ProtoMember(3)]
        public int GoodsId { get; set; }
        [ProtoMember(4)]
        public string ItemName { get; set; }
        [ProtoMember(5)]
        public string EffectDesc { get; set; }
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
            SourcePlayerId = default;
            TargetBuildingInstanceId = default;
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
        [ProtoMember(9)]
        public long SourcePlayerId { get; set; }
        [ProtoMember(10)]
        public long TargetBuildingInstanceId { get; set; }
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
    /// 巨魔购买地图局内商店装备。服务端用 TMX shop 层的 shoprange 校验距离。
    /// </summary>
    [ProtoContract]
    public partial class C2G_BuyBattleShopGoodsCommand : AMessage, IRequest
    {
        public static C2G_BuyBattleShopGoodsCommand Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<C2G_BuyBattleShopGoodsCommand>();
        }

        public override void Dispose()
        {
            Token = default;
            BattleId = default;
            ShopId = default;
            GoodsId = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<C2G_BuyBattleShopGoodsCommand>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.C2G_BuyBattleShopGoodsCommand; } 
        [ProtoIgnore]
        public G2C_BuyBattleShopGoodsCommandResponse ResponseType { get; set; }
        [ProtoMember(1)]
        public string Token { get; set; }
        [ProtoMember(2)]
        public int BattleId { get; set; }
        [ProtoMember(3)]
        public int ShopId { get; set; }
        [ProtoMember(4)]
        public int GoodsId { get; set; }
    }
    [ProtoContract]
    public partial class G2C_BuyBattleShopGoodsCommandResponse : AMessage, IResponse
    {
        public static G2C_BuyBattleShopGoodsCommandResponse Create(Scene scene)
        {
            return scene.MessagePoolComponent.Rent<G2C_BuyBattleShopGoodsCommandResponse>();
        }

        public override void Dispose()
        {
            Success = default;
            Message = default;
            Snapshot = default;
#if FANTASY_NET || FANTASY_UNITY
            GetScene().MessagePoolComponent.Return<G2C_BuyBattleShopGoodsCommandResponse>(this);
#endif
        }
public uint OpCode() { return OuterOpcode.G2C_BuyBattleShopGoodsCommandResponse; } 
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