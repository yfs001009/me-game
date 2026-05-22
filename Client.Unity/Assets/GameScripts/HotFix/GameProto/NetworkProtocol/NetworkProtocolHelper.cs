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

   }
}