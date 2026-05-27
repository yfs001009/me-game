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
		public static async FTask<G2C_CreateRoomResponse> C2G_CreateRoomRequest(this Session session, string token, string roomName, string mode, int mapId, int maxPlayers, bool isPrivate, string password)
		{
			using var request = Fantasy.C2G_CreateRoomRequest.Create(session.Scene);
			request.Token = token;
			request.RoomName = roomName;
			request.Mode = mode;
			request.MapId = mapId;
			request.MaxPlayers = maxPlayers;
			request.IsPrivate = isPrivate;
			request.Password = password;
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

   }
}