using System.Runtime.CompilerServices;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using System.Collections.Generic;
#pragma warning disable CS8618
namespace Fantasy
{
   public static class NetworkProtocolHelper
   {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_RegisterResponse> C2G_RegisterRequest(this Session session, C2G_RegisterRequest request)
		{
			return (G2C_RegisterResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_RegisterResponse> C2G_RegisterRequest(this Session session, string account, string password, string nickname)
		{
			using var request = Fantasy.C2G_RegisterRequest.Create(session.Scene);
			request.Account = account;
			request.Password = password;
			request.Nickname = nickname;
			return (G2C_RegisterResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_LoginResponse> C2G_LoginRequest(this Session session, C2G_LoginRequest request)
		{
			return (G2C_LoginResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_LoginResponse> C2G_LoginRequest(this Session session, string account, string password)
		{
			using var request = Fantasy.C2G_LoginRequest.Create(session.Scene);
			request.Account = account;
			request.Password = password;
			return (G2C_LoginResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_SetNicknameResponse> C2G_SetNicknameRequest(this Session session, C2G_SetNicknameRequest request)
		{
			return (G2C_SetNicknameResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_SetNicknameResponse> C2G_SetNicknameRequest(this Session session, string token, string nickname)
		{
			using var request = Fantasy.C2G_SetNicknameRequest.Create(session.Scene);
			request.Token = token;
			request.Nickname = nickname;
			return (G2C_SetNicknameResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_SendChatMessageResponse> C2G_SendChatMessageRequest(this Session session, C2G_SendChatMessageRequest request)
		{
			return (G2C_SendChatMessageResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_SendChatMessageResponse> C2G_SendChatMessageRequest(this Session session, string token, ChatMessageTreeInfo messageTree)
		{
			using var request = Fantasy.C2G_SendChatMessageRequest.Create(session.Scene);
			request.Token = token;
			request.MessageTree = messageTree;
			return (G2C_SendChatMessageResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_ChatHistoryResponse> C2G_ChatHistoryRequest(this Session session, C2G_ChatHistoryRequest request)
		{
			return (G2C_ChatHistoryResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_ChatHistoryResponse> C2G_ChatHistoryRequest(this Session session, string token, int channelType, long channelId, int limit)
		{
			using var request = Fantasy.C2G_ChatHistoryRequest.Create(session.Scene);
			request.Token = token;
			request.ChannelType = channelType;
			request.ChannelId = channelId;
			request.Limit = limit;
			return (G2C_ChatHistoryResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void G2C_ChatMessageNotify(this Session session, G2C_ChatMessageNotify message)
		{
			session.Send(message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void G2C_ChatMessageNotify(this Session session, ChatMessageTreeInfo messageTree)
		{
			using var message = Fantasy.G2C_ChatMessageNotify.Create(session.Scene);
			message.MessageTree = messageTree;
			session.Send(message);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_LobbyHomeResponse> C2G_LobbyHomeRequest(this Session session, C2G_LobbyHomeRequest request)
		{
			return (G2C_LobbyHomeResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_LobbyHomeResponse> C2G_LobbyHomeRequest(this Session session, string token)
		{
			using var request = Fantasy.C2G_LobbyHomeRequest.Create(session.Scene);
			request.Token = token;
			return (G2C_LobbyHomeResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_CharacterListResponse> C2G_CharacterListRequest(this Session session, C2G_CharacterListRequest request)
		{
			return (G2C_CharacterListResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_CharacterListResponse> C2G_CharacterListRequest(this Session session, string token)
		{
			using var request = Fantasy.C2G_CharacterListRequest.Create(session.Scene);
			request.Token = token;
			return (G2C_CharacterListResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_SelectCharacterResponse> C2G_SelectCharacterRequest(this Session session, C2G_SelectCharacterRequest request)
		{
			return (G2C_SelectCharacterResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_SelectCharacterResponse> C2G_SelectCharacterRequest(this Session session, string token, int characterId)
		{
			using var request = Fantasy.C2G_SelectCharacterRequest.Create(session.Scene);
			request.Token = token;
			request.CharacterId = characterId;
			return (G2C_SelectCharacterResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_AssetSnapshotResponse> C2G_AssetSnapshotRequest(this Session session, C2G_AssetSnapshotRequest request)
		{
			return (G2C_AssetSnapshotResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_AssetSnapshotResponse> C2G_AssetSnapshotRequest(this Session session, string token)
		{
			using var request = Fantasy.C2G_AssetSnapshotRequest.Create(session.Scene);
			request.Token = token;
			return (G2C_AssetSnapshotResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_UseItemResponse> C2G_UseItemRequest(this Session session, C2G_UseItemRequest request)
		{
			return (G2C_UseItemResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_UseItemResponse> C2G_UseItemRequest(this Session session, string token, int itemId, int count)
		{
			using var request = Fantasy.C2G_UseItemRequest.Create(session.Scene);
			request.Token = token;
			request.ItemId = itemId;
			request.Count = count;
			return (G2C_UseItemResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_OutgameShopListResponse> C2G_OutgameShopListRequest(this Session session, C2G_OutgameShopListRequest request)
		{
			return (G2C_OutgameShopListResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_OutgameShopListResponse> C2G_OutgameShopListRequest(this Session session, string token, string shopType, string activityId, string featureId)
		{
			using var request = Fantasy.C2G_OutgameShopListRequest.Create(session.Scene);
			request.Token = token;
			request.ShopType = shopType;
			request.ActivityId = activityId;
			request.FeatureId = featureId;
			return (G2C_OutgameShopListResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_BuyOutgameShopGoodsResponse> C2G_BuyOutgameShopGoodsRequest(this Session session, C2G_BuyOutgameShopGoodsRequest request)
		{
			return (G2C_BuyOutgameShopGoodsResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_BuyOutgameShopGoodsResponse> C2G_BuyOutgameShopGoodsRequest(this Session session, string token, int goodsId, int count)
		{
			using var request = Fantasy.C2G_BuyOutgameShopGoodsRequest.Create(session.Scene);
			request.Token = token;
			request.GoodsId = goodsId;
			request.Count = count;
			return (G2C_BuyOutgameShopGoodsResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_MailListResponse> C2G_MailListRequest(this Session session, C2G_MailListRequest request)
		{
			return (G2C_MailListResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_MailListResponse> C2G_MailListRequest(this Session session, string token)
		{
			using var request = Fantasy.C2G_MailListRequest.Create(session.Scene);
			request.Token = token;
			return (G2C_MailListResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_ReadMailResponse> C2G_ReadMailRequest(this Session session, C2G_ReadMailRequest request)
		{
			return (G2C_ReadMailResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_ReadMailResponse> C2G_ReadMailRequest(this Session session, string token, long mailId)
		{
			using var request = Fantasy.C2G_ReadMailRequest.Create(session.Scene);
			request.Token = token;
			request.MailId = mailId;
			return (G2C_ReadMailResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_ClaimMailAttachmentResponse> C2G_ClaimMailAttachmentRequest(this Session session, C2G_ClaimMailAttachmentRequest request)
		{
			return (G2C_ClaimMailAttachmentResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_ClaimMailAttachmentResponse> C2G_ClaimMailAttachmentRequest(this Session session, string token, long mailId)
		{
			using var request = Fantasy.C2G_ClaimMailAttachmentRequest.Create(session.Scene);
			request.Token = token;
			request.MailId = mailId;
			return (G2C_ClaimMailAttachmentResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_LotteryDrawResponse> C2G_LotteryDrawRequest(this Session session, C2G_LotteryDrawRequest request)
		{
			return (G2C_LotteryDrawResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_LotteryDrawResponse> C2G_LotteryDrawRequest(this Session session, string token, string pool, int count)
		{
			using var request = Fantasy.C2G_LotteryDrawRequest.Create(session.Scene);
			request.Token = token;
			request.Pool = pool;
			request.Count = count;
			return (G2C_LotteryDrawResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_OutgameTaskListResponse> C2G_OutgameTaskListRequest(this Session session, C2G_OutgameTaskListRequest request)
		{
			return (G2C_OutgameTaskListResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_OutgameTaskListResponse> C2G_OutgameTaskListRequest(this Session session, string token, string taskType, string activityId, string featureId)
		{
			using var request = Fantasy.C2G_OutgameTaskListRequest.Create(session.Scene);
			request.Token = token;
			request.TaskType = taskType;
			request.ActivityId = activityId;
			request.FeatureId = featureId;
			return (G2C_OutgameTaskListResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_ClaimOutgameTaskRewardResponse> C2G_ClaimOutgameTaskRewardRequest(this Session session, C2G_ClaimOutgameTaskRewardRequest request)
		{
			return (G2C_ClaimOutgameTaskRewardResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_ClaimOutgameTaskRewardResponse> C2G_ClaimOutgameTaskRewardRequest(this Session session, string token, int taskId)
		{
			using var request = Fantasy.C2G_ClaimOutgameTaskRewardRequest.Create(session.Scene);
			request.Token = token;
			request.TaskId = taskId;
			return (G2C_ClaimOutgameTaskRewardResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_SocialListResponse> C2G_SocialListRequest(this Session session, C2G_SocialListRequest request)
		{
			return (G2C_SocialListResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_SocialListResponse> C2G_SocialListRequest(this Session session, string token, string viewMode, string keyword)
		{
			using var request = Fantasy.C2G_SocialListRequest.Create(session.Scene);
			request.Token = token;
			request.ViewMode = viewMode;
			request.Keyword = keyword;
			return (G2C_SocialListResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_FollowPlayerResponse> C2G_FollowPlayerRequest(this Session session, C2G_FollowPlayerRequest request)
		{
			return (G2C_FollowPlayerResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_FollowPlayerResponse> C2G_FollowPlayerRequest(this Session session, string token, long targetPlayerId, bool follow, string viewMode)
		{
			using var request = Fantasy.C2G_FollowPlayerRequest.Create(session.Scene);
			request.Token = token;
			request.TargetPlayerId = targetPlayerId;
			request.Follow = follow;
			request.ViewMode = viewMode;
			return (G2C_FollowPlayerResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_StartMatchResponse> C2G_StartMatchRequest(this Session session, C2G_StartMatchRequest request)
		{
			return (G2C_StartMatchResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_StartMatchResponse> C2G_StartMatchRequest(this Session session, string token, string mode)
		{
			using var request = Fantasy.C2G_StartMatchRequest.Create(session.Scene);
			request.Token = token;
			request.Mode = mode;
			return (G2C_StartMatchResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_CreateRoomResponse> C2G_CreateRoomRequest(this Session session, C2G_CreateRoomRequest request)
		{
			return (G2C_CreateRoomResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_CreateRoomResponse> C2G_CreateRoomRequest(this Session session, string token, string roomName, string mode, int mapId, int maxPlayers, bool isPrivate, string password, List<int> selectedBuildingCardIds)
		{
			using var request = Fantasy.C2G_CreateRoomRequest.Create(session.Scene);
			request.Token = token;
			request.RoomName = roomName;
			request.Mode = mode;
			request.MapId = mapId;
			request.MaxPlayers = maxPlayers;
			request.IsPrivate = isPrivate;
			request.Password = password;
			request.SelectedBuildingCardIds = selectedBuildingCardIds;
			return (G2C_CreateRoomResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_JoinRoomResponse> C2G_JoinRoomRequest(this Session session, C2G_JoinRoomRequest request)
		{
			return (G2C_JoinRoomResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_JoinRoomResponse> C2G_JoinRoomRequest(this Session session, string token, int roomId, string password)
		{
			using var request = Fantasy.C2G_JoinRoomRequest.Create(session.Scene);
			request.Token = token;
			request.RoomId = roomId;
			request.Password = password;
			return (G2C_JoinRoomResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_LeaveRoomResponse> C2G_LeaveRoomRequest(this Session session, C2G_LeaveRoomRequest request)
		{
			return (G2C_LeaveRoomResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_LeaveRoomResponse> C2G_LeaveRoomRequest(this Session session, string token, int roomId)
		{
			using var request = Fantasy.C2G_LeaveRoomRequest.Create(session.Scene);
			request.Token = token;
			request.RoomId = roomId;
			return (G2C_LeaveRoomResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_RoomDetailResponse> C2G_RoomDetailRequest(this Session session, C2G_RoomDetailRequest request)
		{
			return (G2C_RoomDetailResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_RoomDetailResponse> C2G_RoomDetailRequest(this Session session, string token, int roomId)
		{
			using var request = Fantasy.C2G_RoomDetailRequest.Create(session.Scene);
			request.Token = token;
			request.RoomId = roomId;
			return (G2C_RoomDetailResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_SetRoomReadyResponse> C2G_SetRoomReadyRequest(this Session session, C2G_SetRoomReadyRequest request)
		{
			return (G2C_SetRoomReadyResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_SetRoomReadyResponse> C2G_SetRoomReadyRequest(this Session session, string token, int roomId, bool isReady)
		{
			using var request = Fantasy.C2G_SetRoomReadyRequest.Create(session.Scene);
			request.Token = token;
			request.RoomId = roomId;
			request.IsReady = isReady;
			return (G2C_SetRoomReadyResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_StartRoomResponse> C2G_StartRoomRequest(this Session session, C2G_StartRoomRequest request)
		{
			return (G2C_StartRoomResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_StartRoomResponse> C2G_StartRoomRequest(this Session session, string token, int roomId)
		{
			using var request = Fantasy.C2G_StartRoomRequest.Create(session.Scene);
			request.Token = token;
			request.RoomId = roomId;
			return (G2C_StartRoomResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_BattleSceneLoadedResponse> C2G_BattleSceneLoadedRequest(this Session session, C2G_BattleSceneLoadedRequest request)
		{
			return (G2C_BattleSceneLoadedResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_BattleSceneLoadedResponse> C2G_BattleSceneLoadedRequest(this Session session, string token, int battleId)
		{
			using var request = Fantasy.C2G_BattleSceneLoadedRequest.Create(session.Scene);
			request.Token = token;
			request.BattleId = battleId;
			return (G2C_BattleSceneLoadedResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_BattleSnapshotResponse> C2G_BattleSnapshotRequest(this Session session, C2G_BattleSnapshotRequest request)
		{
			return (G2C_BattleSnapshotResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_BattleSnapshotResponse> C2G_BattleSnapshotRequest(this Session session, string token, int battleId, long lastKnownTick)
		{
			using var request = Fantasy.C2G_BattleSnapshotRequest.Create(session.Scene);
			request.Token = token;
			request.BattleId = battleId;
			request.LastKnownTick = lastKnownTick;
			return (G2C_BattleSnapshotResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_BattleMoveCommandResponse> C2G_BattleMoveCommand(this Session session, C2G_BattleMoveCommand request)
		{
			return (G2C_BattleMoveCommandResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_BattleMoveCommandResponse> C2G_BattleMoveCommand(this Session session, string token, int battleId, float axisX, float axisY)
		{
			using var request = Fantasy.C2G_BattleMoveCommand.Create(session.Scene);
			request.Token = token;
			request.BattleId = battleId;
			request.AxisX = axisX;
			request.AxisY = axisY;
			return (G2C_BattleMoveCommandResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_BuildCommandResponse> C2G_BuildCommand(this Session session, C2G_BuildCommand request)
		{
			return (G2C_BuildCommandResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_BuildCommandResponse> C2G_BuildCommand(this Session session, string token, int battleId, int buildingId, int gridX, int gridY)
		{
			using var request = Fantasy.C2G_BuildCommand.Create(session.Scene);
			request.Token = token;
			request.BattleId = battleId;
			request.BuildingId = buildingId;
			request.GridX = gridX;
			request.GridY = gridY;
			return (G2C_BuildCommandResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_UpgradeBuildingCommandResponse> C2G_UpgradeBuildingCommand(this Session session, C2G_UpgradeBuildingCommand request)
		{
			return (G2C_UpgradeBuildingCommandResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_UpgradeBuildingCommandResponse> C2G_UpgradeBuildingCommand(this Session session, string token, int battleId, long buildingInstanceId)
		{
			using var request = Fantasy.C2G_UpgradeBuildingCommand.Create(session.Scene);
			request.Token = token;
			request.BattleId = battleId;
			request.BuildingInstanceId = buildingInstanceId;
			return (G2C_UpgradeBuildingCommandResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_RecycleBuildingCommandResponse> C2G_RecycleBuildingCommand(this Session session, C2G_RecycleBuildingCommand request)
		{
			return (G2C_RecycleBuildingCommandResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_RecycleBuildingCommandResponse> C2G_RecycleBuildingCommand(this Session session, string token, int battleId, long buildingInstanceId)
		{
			using var request = Fantasy.C2G_RecycleBuildingCommand.Create(session.Scene);
			request.Token = token;
			request.BattleId = battleId;
			request.BuildingInstanceId = buildingInstanceId;
			return (G2C_RecycleBuildingCommandResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_BuyBattleShopGoodsCommandResponse> C2G_BuyBattleShopGoodsCommand(this Session session, C2G_BuyBattleShopGoodsCommand request)
		{
			return (G2C_BuyBattleShopGoodsCommandResponse)await session.Call(request);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async FTask<G2C_BuyBattleShopGoodsCommandResponse> C2G_BuyBattleShopGoodsCommand(this Session session, string token, int battleId, int shopId, int goodsId)
		{
			using var request = Fantasy.C2G_BuyBattleShopGoodsCommand.Create(session.Scene);
			request.Token = token;
			request.BattleId = battleId;
			request.ShopId = shopId;
			request.GoodsId = goodsId;
			return (G2C_BuyBattleShopGoodsCommandResponse)await session.Call(request);
		}

   }
}