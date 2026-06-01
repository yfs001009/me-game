using System;
using System.Collections.Generic;
using System.Linq;
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
            ValidateRegistrars();
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
                    typeof(ChatInfoNode),
                    typeof(ChatMessageTreeInfo),
                    typeof(C2G_SendChatMessageRequest),
                    typeof(G2C_SendChatMessageResponse),
                    typeof(C2G_ChatHistoryRequest),
                    typeof(G2C_ChatHistoryResponse),
                    typeof(G2C_ChatMessageNotify),
                    typeof(C2G_LobbyHomeRequest),
                    typeof(G2C_LobbyHomeResponse),
                    typeof(CharacterInfo),
                    typeof(C2G_CharacterListRequest),
                    typeof(G2C_CharacterListResponse),
                    typeof(C2G_SelectCharacterRequest),
                    typeof(G2C_SelectCharacterResponse),
                    typeof(CurrencyBalanceInfo),
                    typeof(BagItemInfo),
                    typeof(BuffStateInfo),
                    typeof(ProgressValueInfo),
                    typeof(AssetSnapshotInfo),
                    typeof(C2G_AssetSnapshotRequest),
                    typeof(G2C_AssetSnapshotResponse),
                    typeof(C2G_UseItemRequest),
                    typeof(G2C_UseItemResponse),
                    typeof(OutgameShopGoodsInfo),
                    typeof(OutgameShopInfo),
                    typeof(C2G_OutgameShopListRequest),
                    typeof(G2C_OutgameShopListResponse),
                    typeof(C2G_BuyOutgameShopGoodsRequest),
                    typeof(G2C_BuyOutgameShopGoodsResponse),
                    typeof(RewardInfo),
                    typeof(MailInfo),
                    typeof(C2G_MailListRequest),
                    typeof(G2C_MailListResponse),
                    typeof(C2G_ReadMailRequest),
                    typeof(G2C_ReadMailResponse),
                    typeof(C2G_ClaimMailAttachmentRequest),
                    typeof(G2C_ClaimMailAttachmentResponse),
                    typeof(LotteryDrawResultInfo),
                    typeof(C2G_LotteryDrawRequest),
                    typeof(G2C_LotteryDrawResponse),
                    typeof(OutgameTaskInfo),
                    typeof(C2G_OutgameTaskListRequest),
                    typeof(G2C_OutgameTaskListResponse),
                    typeof(C2G_ClaimOutgameTaskRewardRequest),
                    typeof(G2C_ClaimOutgameTaskRewardResponse),
                    typeof(SocialPlayerInfo),
                    typeof(C2G_SocialListRequest),
                    typeof(G2C_SocialListResponse),
                    typeof(C2G_FollowPlayerRequest),
                    typeof(G2C_FollowPlayerResponse),
                    typeof(MatchStatusInfo),
                    typeof(C2G_StartMatchRequest),
                    typeof(G2C_StartMatchResponse),
                    typeof(C2G_CreateRoomRequest),
                    typeof(G2C_CreateRoomResponse),
                    typeof(C2G_JoinRoomRequest),
                    typeof(G2C_JoinRoomResponse),
                    typeof(C2G_LeaveRoomRequest),
                    typeof(G2C_LeaveRoomResponse),
                    typeof(C2G_RoomDetailRequest),
                    typeof(G2C_RoomDetailResponse),
                    typeof(C2G_SetRoomReadyRequest),
                    typeof(G2C_SetRoomReadyResponse),
                    typeof(C2G_StartRoomRequest),
                    typeof(G2C_StartRoomResponse),
                    typeof(BattleStartInfo),
                    typeof(BattlePlayerStateInfo),
                    typeof(BattleEquipmentSlotInfo),
                    typeof(BattleBuildingStateInfo),
                    typeof(BattleAttackEventInfo),
                    typeof(BattleSnapshotInfo),
                    typeof(C2G_BattleSceneLoadedRequest),
                    typeof(G2C_BattleSceneLoadedResponse),
                    typeof(C2G_BattleSnapshotRequest),
                    typeof(G2C_BattleSnapshotResponse),
                    typeof(C2G_BattleMoveCommand),
                    typeof(G2C_BattleMoveCommandResponse),
                    typeof(C2G_BuildCommand),
                    typeof(G2C_BuildCommandResponse),
                    typeof(C2G_UpgradeBuildingCommand),
                    typeof(G2C_UpgradeBuildingCommandResponse),
                    typeof(C2G_RecycleBuildingCommand),
                    typeof(G2C_RecycleBuildingCommandResponse),
                    typeof(C2G_BuyBattleShopGoodsCommand),
                    typeof(G2C_BuyBattleShopGoodsCommandResponse),
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
                    OuterOpcode.C2G_SendChatMessageRequest,
                    OuterOpcode.G2C_SendChatMessageResponse,
                    OuterOpcode.C2G_ChatHistoryRequest,
                    OuterOpcode.G2C_ChatHistoryResponse,
                    OuterOpcode.G2C_ChatMessageNotify,
                    OuterOpcode.C2G_LobbyHomeRequest,
                    OuterOpcode.G2C_LobbyHomeResponse,
                    OuterOpcode.C2G_CharacterListRequest,
                    OuterOpcode.G2C_CharacterListResponse,
                    OuterOpcode.C2G_SelectCharacterRequest,
                    OuterOpcode.G2C_SelectCharacterResponse,
                    OuterOpcode.C2G_AssetSnapshotRequest,
                    OuterOpcode.G2C_AssetSnapshotResponse,
                    OuterOpcode.C2G_UseItemRequest,
                    OuterOpcode.G2C_UseItemResponse,
                    OuterOpcode.C2G_OutgameShopListRequest,
                    OuterOpcode.G2C_OutgameShopListResponse,
                    OuterOpcode.C2G_BuyOutgameShopGoodsRequest,
                    OuterOpcode.G2C_BuyOutgameShopGoodsResponse,
                    OuterOpcode.C2G_MailListRequest,
                    OuterOpcode.G2C_MailListResponse,
                    OuterOpcode.C2G_ReadMailRequest,
                    OuterOpcode.G2C_ReadMailResponse,
                    OuterOpcode.C2G_ClaimMailAttachmentRequest,
                    OuterOpcode.G2C_ClaimMailAttachmentResponse,
                    OuterOpcode.C2G_LotteryDrawRequest,
                    OuterOpcode.G2C_LotteryDrawResponse,
                    OuterOpcode.C2G_OutgameTaskListRequest,
                    OuterOpcode.G2C_OutgameTaskListResponse,
                    OuterOpcode.C2G_ClaimOutgameTaskRewardRequest,
                    OuterOpcode.G2C_ClaimOutgameTaskRewardResponse,
                    OuterOpcode.C2G_SocialListRequest,
                    OuterOpcode.G2C_SocialListResponse,
                    OuterOpcode.C2G_FollowPlayerRequest,
                    OuterOpcode.G2C_FollowPlayerResponse,
                    OuterOpcode.C2G_StartMatchRequest,
                    OuterOpcode.G2C_StartMatchResponse,
                    OuterOpcode.C2G_CreateRoomRequest,
                    OuterOpcode.G2C_CreateRoomResponse,
                    OuterOpcode.C2G_JoinRoomRequest,
                    OuterOpcode.G2C_JoinRoomResponse,
                    OuterOpcode.C2G_LeaveRoomRequest,
                    OuterOpcode.G2C_LeaveRoomResponse,
                    OuterOpcode.C2G_RoomDetailRequest,
                    OuterOpcode.G2C_RoomDetailResponse,
                    OuterOpcode.C2G_SetRoomReadyRequest,
                    OuterOpcode.G2C_SetRoomReadyResponse,
                    OuterOpcode.C2G_StartRoomRequest,
                    OuterOpcode.G2C_StartRoomResponse,
                    OuterOpcode.C2G_BattleSceneLoadedRequest,
                    OuterOpcode.G2C_BattleSceneLoadedResponse,
                    OuterOpcode.C2G_BattleSnapshotRequest,
                    OuterOpcode.G2C_BattleSnapshotResponse,
                    OuterOpcode.C2G_BattleMoveCommand,
                    OuterOpcode.G2C_BattleMoveCommandResponse,
                    OuterOpcode.C2G_BuildCommand,
                    OuterOpcode.G2C_BuildCommandResponse,
                    OuterOpcode.C2G_UpgradeBuildingCommand,
                    OuterOpcode.G2C_UpgradeBuildingCommandResponse,
                    OuterOpcode.C2G_RecycleBuildingCommand,
                    OuterOpcode.G2C_RecycleBuildingCommandResponse,
                    OuterOpcode.C2G_BuyBattleShopGoodsCommand,
                    OuterOpcode.G2C_BuyBattleShopGoodsCommandResponse
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
                    typeof(C2G_SendChatMessageRequest),
                    typeof(G2C_SendChatMessageResponse),
                    typeof(C2G_ChatHistoryRequest),
                    typeof(G2C_ChatHistoryResponse),
                    typeof(G2C_ChatMessageNotify),
                    typeof(C2G_LobbyHomeRequest),
                    typeof(G2C_LobbyHomeResponse),
                    typeof(C2G_CharacterListRequest),
                    typeof(G2C_CharacterListResponse),
                    typeof(C2G_SelectCharacterRequest),
                    typeof(G2C_SelectCharacterResponse),
                    typeof(C2G_AssetSnapshotRequest),
                    typeof(G2C_AssetSnapshotResponse),
                    typeof(C2G_UseItemRequest),
                    typeof(G2C_UseItemResponse),
                    typeof(C2G_OutgameShopListRequest),
                    typeof(G2C_OutgameShopListResponse),
                    typeof(C2G_BuyOutgameShopGoodsRequest),
                    typeof(G2C_BuyOutgameShopGoodsResponse),
                    typeof(C2G_MailListRequest),
                    typeof(G2C_MailListResponse),
                    typeof(C2G_ReadMailRequest),
                    typeof(G2C_ReadMailResponse),
                    typeof(C2G_ClaimMailAttachmentRequest),
                    typeof(G2C_ClaimMailAttachmentResponse),
                    typeof(C2G_LotteryDrawRequest),
                    typeof(G2C_LotteryDrawResponse),
                    typeof(C2G_OutgameTaskListRequest),
                    typeof(G2C_OutgameTaskListResponse),
                    typeof(C2G_ClaimOutgameTaskRewardRequest),
                    typeof(G2C_ClaimOutgameTaskRewardResponse),
                    typeof(C2G_SocialListRequest),
                    typeof(G2C_SocialListResponse),
                    typeof(C2G_FollowPlayerRequest),
                    typeof(G2C_FollowPlayerResponse),
                    typeof(C2G_StartMatchRequest),
                    typeof(G2C_StartMatchResponse),
                    typeof(C2G_CreateRoomRequest),
                    typeof(G2C_CreateRoomResponse),
                    typeof(C2G_JoinRoomRequest),
                    typeof(G2C_JoinRoomResponse),
                    typeof(C2G_LeaveRoomRequest),
                    typeof(G2C_LeaveRoomResponse),
                    typeof(C2G_RoomDetailRequest),
                    typeof(G2C_RoomDetailResponse),
                    typeof(C2G_SetRoomReadyRequest),
                    typeof(G2C_SetRoomReadyResponse),
                    typeof(C2G_StartRoomRequest),
                    typeof(G2C_StartRoomResponse),
                    typeof(C2G_BattleSceneLoadedRequest),
                    typeof(G2C_BattleSceneLoadedResponse),
                    typeof(C2G_BattleSnapshotRequest),
                    typeof(G2C_BattleSnapshotResponse),
                    typeof(C2G_BattleMoveCommand),
                    typeof(G2C_BattleMoveCommandResponse),
                    typeof(C2G_BuildCommand),
                    typeof(G2C_BuildCommandResponse),
                    typeof(C2G_UpgradeBuildingCommand),
                    typeof(G2C_UpgradeBuildingCommandResponse),
                    typeof(C2G_RecycleBuildingCommand),
                    typeof(G2C_RecycleBuildingCommandResponse),
                    typeof(C2G_BuyBattleShopGoodsCommand),
                    typeof(G2C_BuyBattleShopGoodsCommandResponse)
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
                    OuterOpcode.C2G_SendChatMessageRequest,
                    OuterOpcode.C2G_ChatHistoryRequest,
                    OuterOpcode.C2G_LobbyHomeRequest,
                    OuterOpcode.C2G_CharacterListRequest,
                    OuterOpcode.C2G_SelectCharacterRequest,
                    OuterOpcode.C2G_AssetSnapshotRequest,
                    OuterOpcode.C2G_UseItemRequest,
                    OuterOpcode.C2G_OutgameShopListRequest,
                    OuterOpcode.C2G_BuyOutgameShopGoodsRequest,
                    OuterOpcode.C2G_MailListRequest,
                    OuterOpcode.C2G_ReadMailRequest,
                    OuterOpcode.C2G_ClaimMailAttachmentRequest,
                    OuterOpcode.C2G_LotteryDrawRequest,
                    OuterOpcode.C2G_OutgameTaskListRequest,
                    OuterOpcode.C2G_ClaimOutgameTaskRewardRequest,
                    OuterOpcode.C2G_SocialListRequest,
                    OuterOpcode.C2G_FollowPlayerRequest,
                    OuterOpcode.C2G_StartMatchRequest,
                    OuterOpcode.C2G_CreateRoomRequest,
                    OuterOpcode.C2G_JoinRoomRequest,
                    OuterOpcode.C2G_LeaveRoomRequest,
                    OuterOpcode.C2G_RoomDetailRequest,
                    OuterOpcode.C2G_SetRoomReadyRequest,
                    OuterOpcode.C2G_StartRoomRequest,
                    OuterOpcode.C2G_BattleSceneLoadedRequest,
                    OuterOpcode.C2G_BattleSnapshotRequest,
                    OuterOpcode.C2G_BattleMoveCommand,
                    OuterOpcode.C2G_BuildCommand,
                    OuterOpcode.C2G_UpgradeBuildingCommand,
                    OuterOpcode.C2G_RecycleBuildingCommand,
                    OuterOpcode.C2G_BuyBattleShopGoodsCommand
                };
            }

            public Type[] Types()
            {
                return new[]
                {
                    typeof(G2C_RegisterResponse),
                    typeof(G2C_LoginResponse),
                    typeof(G2C_SetNicknameResponse),
                    typeof(G2C_SendChatMessageResponse),
                    typeof(G2C_ChatHistoryResponse),
                    typeof(G2C_LobbyHomeResponse),
                    typeof(G2C_CharacterListResponse),
                    typeof(G2C_SelectCharacterResponse),
                    typeof(G2C_AssetSnapshotResponse),
                    typeof(G2C_UseItemResponse),
                    typeof(G2C_OutgameShopListResponse),
                    typeof(G2C_BuyOutgameShopGoodsResponse),
                    typeof(G2C_MailListResponse),
                    typeof(G2C_ReadMailResponse),
                    typeof(G2C_ClaimMailAttachmentResponse),
                    typeof(G2C_LotteryDrawResponse),
                    typeof(G2C_OutgameTaskListResponse),
                    typeof(G2C_ClaimOutgameTaskRewardResponse),
                    typeof(G2C_SocialListResponse),
                    typeof(G2C_FollowPlayerResponse),
                    typeof(G2C_StartMatchResponse),
                    typeof(G2C_CreateRoomResponse),
                    typeof(G2C_JoinRoomResponse),
                    typeof(G2C_LeaveRoomResponse),
                    typeof(G2C_RoomDetailResponse),
                    typeof(G2C_SetRoomReadyResponse),
                    typeof(G2C_StartRoomResponse),
                    typeof(G2C_BattleSceneLoadedResponse),
                    typeof(G2C_BattleSnapshotResponse),
                    typeof(G2C_BattleMoveCommandResponse),
                    typeof(G2C_BuildCommandResponse),
                    typeof(G2C_UpgradeBuildingCommandResponse),
                    typeof(G2C_RecycleBuildingCommandResponse),
                    typeof(G2C_BuyBattleShopGoodsCommandResponse)
                };
            }
        }

        private static void ValidateRegistrars()
        {
#if DEBUG || UNITY_EDITOR
            var networkTypes = new NetworkProtocolRegistrar().GetNetworkProtocolTypes();
            var networkTypeSet = new HashSet<Type>(networkTypes);
            var opCodeRegistrar = new OpCodeRegistrar();
            var opCodes = opCodeRegistrar.TypeOpCodes();
            var opCodeTypes = opCodeRegistrar.OpCodeTypes();
            var responseRegistrar = new ResponseTypeRegistrar();
            var requestOpCodes = responseRegistrar.OpCodes();
            var responseTypes = responseRegistrar.Types();

            EnsureSameLength(opCodes.Length, opCodeTypes.Length, "opcode/type");
            EnsureSameLength(requestOpCodes.Length, responseTypes.Length, "request/response");
            EnsureNoDuplicates(networkTypes, "network protocol type");
            EnsureNoDuplicates(opCodes, "opcode");
            EnsureNoDuplicates(opCodeTypes, "opcode type");
            EnsureNoDuplicates(requestOpCodes, "request response opcode");

            foreach (var type in typeof(GameProtoFantasyRegistrar).Assembly.GetTypes()
                         .Where(type => type.IsClass
                                        && !type.IsAbstract
                                        && typeof(AMessage).IsAssignableFrom(type)
                                        && type.Namespace == typeof(GameProtoFantasyRegistrar).Namespace))
            {
                if (!networkTypeSet.Contains(type))
                {
                    throw new InvalidOperationException($"Network protocol type is not registered: {type.FullName}");
                }
            }

            for (var i = 0; i < opCodes.Length; i++)
            {
                var type = opCodeTypes[i];
                if (!typeof(IMessage).IsAssignableFrom(type))
                {
                    throw new InvalidOperationException($"Opcode type must implement IMessage: {type.FullName}");
                }

                if (!networkTypeSet.Contains(type))
                {
                    throw new InvalidOperationException($"Opcode type is not registered as a network protocol: {type.FullName}");
                }
            }

            var responseTypeByRequestOpcode = new Dictionary<uint, Type>();
            for (var i = 0; i < requestOpCodes.Length; i++)
            {
                responseTypeByRequestOpcode.Add(requestOpCodes[i], responseTypes[i]);
            }

            foreach (var requestType in opCodeTypes.Where(type => typeof(IRequest).IsAssignableFrom(type)))
            {
                var opCode = GetStaticOpCode(requestType);
                if (!responseTypeByRequestOpcode.TryGetValue(opCode, out var responseType))
                {
                    throw new InvalidOperationException($"Request response mapping is missing: {requestType.FullName}");
                }

                var declaredResponseType = requestType.GetProperty("ResponseType")?.PropertyType;
                if (declaredResponseType != null && declaredResponseType != responseType)
                {
                    throw new InvalidOperationException(
                        $"Request response mapping mismatch: {requestType.FullName} -> {responseType.FullName}, declared {declaredResponseType.FullName}");
                }
            }
#endif
        }

        private static uint GetStaticOpCode(Type type)
        {
            var field = typeof(OuterOpcode).GetField(type.Name);
            if (field == null || field.FieldType != typeof(uint))
            {
                throw new InvalidOperationException($"Opcode constant is missing: {type.FullName}");
            }

            return (uint)field.GetValue(null);
        }

        private static void EnsureSameLength(int left, int right, string name)
        {
            if (left != right)
            {
                throw new InvalidOperationException($"Protocol registrar {name} count mismatch: {left} != {right}");
            }
        }

        private static void EnsureNoDuplicates<T>(IEnumerable<T> values, string name)
        {
            var duplicates = values.GroupBy(value => value).Where(group => group.Count() > 1).Select(group => group.Key).ToArray();
            if (duplicates.Length > 0)
            {
                throw new InvalidOperationException($"Protocol registrar has duplicate {name}: {string.Join(", ", duplicates)}");
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
