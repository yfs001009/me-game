using System;
using System.Collections.Generic;
using Fantasy.Assembly;
using Fantasy.Async;
using Fantasy.DataStructure.Collection;
using Fantasy.Entitas;
using Fantasy.Event;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Fantasy.Pool;

namespace Fantasy
{
    public static class GameProtoFantasyRegistrar
    {
        private const long ManifestId = 0x4750524F544F;
        private static bool registered;

        public static void Register()
        {
            if (registered)
            {
                return;
            }

            registered = true;
            AssemblyManifest.Register(
                ManifestId,
                typeof(GameProtoFantasyRegistrar).Assembly,
                new NetworkProtocolRegistrar(),
                new EmptyEventSystemRegistrar(),
                new EmptyEntitySystemRegistrar(),
                new EmptyMessageHandlerResolver(),
                new EmptyEntityTypeCollectionRegistrar(),
                new OpCodeRegistrar(),
                new ResponseTypeRegistrar(),
                new EmptyCustomInterfaceRegistrar(),
                new EmptyPoolCreatorGenerator());
        }

        private sealed class NetworkProtocolRegistrar : INetworkProtocolRegistrar
        {
            public List<Type> GetNetworkProtocolTypes()
            {
                return new List<Type>
                {
                    typeof(PlayerProfileInfo),
                    typeof(C2G_RegisterRequest),
                    typeof(G2C_RegisterResponse),
                    typeof(C2G_LoginRequest),
                    typeof(G2C_LoginResponse),
                    typeof(C2G_SetNicknameRequest),
                    typeof(G2C_SetNicknameResponse),
                    typeof(C2G_LobbyHomeRequest),
                    typeof(G2C_LobbyHomeResponse),
                    typeof(MatchStatusInfo),
                    typeof(C2G_StartMatchRequest),
                    typeof(G2C_StartMatchResponse),
                    typeof(C2G_CreateRoomRequest),
                    typeof(G2C_CreateRoomResponse),
                    typeof(C2G_JoinRoomRequest),
                    typeof(G2C_JoinRoomResponse),
                    typeof(C2G_LeaveRoomRequest),
                    typeof(G2C_LeaveRoomResponse),
                    typeof(RoomSummaryInfo),
                    typeof(RoomPlayerInfo),
                    typeof(RoomDetailInfo)
                };
            }
        }

        private sealed class OpCodeRegistrar : IOpCodeRegistrar
        {
            public uint[] TypeOpCodes()
            {
                return new[]
                {
                    OuterOpcode.C2G_RegisterRequest,
                    OuterOpcode.G2C_RegisterResponse,
                    OuterOpcode.C2G_LoginRequest,
                    OuterOpcode.G2C_LoginResponse,
                    OuterOpcode.C2G_SetNicknameRequest,
                    OuterOpcode.G2C_SetNicknameResponse,
                    OuterOpcode.C2G_LobbyHomeRequest,
                    OuterOpcode.G2C_LobbyHomeResponse,
                    OuterOpcode.C2G_StartMatchRequest,
                    OuterOpcode.G2C_StartMatchResponse,
                    OuterOpcode.C2G_CreateRoomRequest,
                    OuterOpcode.G2C_CreateRoomResponse,
                    OuterOpcode.C2G_JoinRoomRequest,
                    OuterOpcode.G2C_JoinRoomResponse,
                    OuterOpcode.C2G_LeaveRoomRequest,
                    OuterOpcode.G2C_LeaveRoomResponse
                };
            }

            public Type[] OpCodeTypes()
            {
                return new[]
                {
                    typeof(C2G_RegisterRequest),
                    typeof(G2C_RegisterResponse),
                    typeof(C2G_LoginRequest),
                    typeof(G2C_LoginResponse),
                    typeof(C2G_SetNicknameRequest),
                    typeof(G2C_SetNicknameResponse),
                    typeof(C2G_LobbyHomeRequest),
                    typeof(G2C_LobbyHomeResponse),
                    typeof(C2G_StartMatchRequest),
                    typeof(G2C_StartMatchResponse),
                    typeof(C2G_CreateRoomRequest),
                    typeof(G2C_CreateRoomResponse),
                    typeof(C2G_JoinRoomRequest),
                    typeof(G2C_JoinRoomResponse),
                    typeof(C2G_LeaveRoomRequest),
                    typeof(G2C_LeaveRoomResponse)
                };
            }

            public uint[] CustomRouteTypeOpCodes() => Array.Empty<uint>();

            public int[] CustomRouteTypes() => Array.Empty<int>();
        }

        private sealed class ResponseTypeRegistrar : IResponseTypeRegistrar
        {
            public uint[] OpCodes()
            {
                return new[]
                {
                    OuterOpcode.C2G_RegisterRequest,
                    OuterOpcode.C2G_LoginRequest,
                    OuterOpcode.C2G_SetNicknameRequest,
                    OuterOpcode.C2G_LobbyHomeRequest,
                    OuterOpcode.C2G_StartMatchRequest,
                    OuterOpcode.C2G_CreateRoomRequest,
                    OuterOpcode.C2G_JoinRoomRequest,
                    OuterOpcode.C2G_LeaveRoomRequest
                };
            }

            public Type[] Types()
            {
                return new[]
                {
                    typeof(G2C_RegisterResponse),
                    typeof(G2C_LoginResponse),
                    typeof(G2C_SetNicknameResponse),
                    typeof(G2C_LobbyHomeResponse),
                    typeof(G2C_StartMatchResponse),
                    typeof(G2C_CreateRoomResponse),
                    typeof(G2C_JoinRoomResponse),
                    typeof(G2C_LeaveRoomResponse)
                };
            }
        }

        private sealed class EmptyMessageHandlerResolver : IMessageHandlerResolver
        {
            public uint[] MessageHandlerOpCodes() => Array.Empty<uint>();

            public Func<Session, uint, object, FTask>[] MessageHandlers() => Array.Empty<Func<Session, uint, object, FTask>>();
        }

        private sealed class EmptyEventSystemRegistrar : IEventSystemRegistrar
        {
            public RuntimeTypeHandle[] EventTypeHandles() => Array.Empty<RuntimeTypeHandle>();

            public IEvent[] Events() => Array.Empty<IEvent>();

            public RuntimeTypeHandle[] AsyncEventTypeHandles() => Array.Empty<RuntimeTypeHandle>();

            public IEvent[] AsyncEvents() => Array.Empty<IEvent>();
        }

        private sealed class EmptyEntitySystemRegistrar : IEntitySystemRegistrar
        {
            public RuntimeTypeHandle[] AwakeTypeHandles() => Array.Empty<RuntimeTypeHandle>();

            public Action<Entity>[] AwakeHandles() => Array.Empty<Action<Entity>>();

            public RuntimeTypeHandle[] UpdateTypeHandles() => Array.Empty<RuntimeTypeHandle>();

            public Action<Entity>[] UpdateHandles() => Array.Empty<Action<Entity>>();

            public RuntimeTypeHandle[] DestroyTypeHandles() => Array.Empty<RuntimeTypeHandle>();

            public Action<Entity>[] DestroyHandles() => Array.Empty<Action<Entity>>();

            public RuntimeTypeHandle[] DeserializeTypeHandles() => Array.Empty<RuntimeTypeHandle>();

            public Action<Entity>[] DeserializeHandles() => Array.Empty<Action<Entity>>();

            public RuntimeTypeHandle[] LateUpdateTypeHandles() => Array.Empty<RuntimeTypeHandle>();

            public Action<Entity>[] LateUpdateHandles() => Array.Empty<Action<Entity>>();
        }

        private sealed class EmptyEntityTypeCollectionRegistrar : IEntityTypeCollectionRegistrar
        {
            public List<Type> GetEntityTypes() => new();
        }

        private sealed class EmptyCustomInterfaceRegistrar : ICustomInterfaceRegistrar
        {
            public void Register(OneToManyList<RuntimeTypeHandle, Type> customRegistrar)
            {
            }

            public void UnRegister(OneToManyList<RuntimeTypeHandle, Type> customRegistrar)
            {
            }
        }

        private sealed class EmptyPoolCreatorGenerator : IPoolCreatorGenerator
        {
            public RuntimeTypeHandle[] RuntimeTypeHandles() => Array.Empty<RuntimeTypeHandle>();

            public Func<IPool>[] Generators() => Array.Empty<Func<IPool>>();
        }
    }
}
