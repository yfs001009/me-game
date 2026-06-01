using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using System.Collections.Generic;
using TEngine;
using Log = TEngine.Log;

namespace GameLogic.SheepBattle.Network
{
    /// <summary>
    /// TEngine 热更层 Fantasy 网络门面。
    /// 使用 Fantasy Unity Runtime 建立连接，并通过协议导出的扩展方法发起 RPC。
    /// </summary>
    public sealed class SheepNetworkService
    {
        public static SheepNetworkService Instance { get; } = new SheepNetworkService();

        public string Host { get; private set; } = string.Empty;
        public int Port { get; private set; }
        public int BattlePort { get; private set; }
        public string BattleHost { get; private set; } = string.Empty;
        public string BattleProtocol { get; private set; } = "KCP";
        public string Token { get; private set; } = string.Empty;
        public PlayerProfileInfo Profile { get; private set; }
        public bool IsConnected { get; private set; }
        public bool IsSessionAvailable => IsConnected && !IsRuntimeSessionDisposed();
        public bool IsBattleSessionAvailable => battleSession != null && !battleSession.IsDisposed;

        private Scene battleScene;
        private Session battleSession;
        private bool isBattleConnected;

        private SheepNetworkService()
        {
        }

        public void Initialize(string host, int port)
        {
            Host = host;
            Port = port;
            BattleHost = host;
            BattlePort = port + 1;
            Log.Info($"网络模块初始化完成，目标服务器：{host}:{port}，战斗服务器：{host}:{BattlePort}");
        }

        public async FTask ConnectAsync()
        {
            if (IsConnected)
            {
                return;
            }

            Log.Info($"正在连接服务器：{Host}:{Port} ...");
            await Runtime.Connect(
                remoteIP: Host,
                remotePort: Port,
                protocol: FantasyRuntime.NetworkProtocolType.TCP,
                isHttps: false,
                connectTimeout: 5000,
                enableHeartbeat: true,
                heartbeatInterval: 2000,
                heartbeatTimeOut: 30000,
                heartbeatTimeOutInterval: 5000,
                maxPingSamples: 4,
                onConnectComplete: OnConnected,
                onConnectFail: OnConnectFailed,
                onConnectDisconnect: OnDisconnected
            );
        }

        public async FTask<G2C_RegisterResponse> RegisterAsync(string account, string password, string nickname)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_RegisterRequest(account, password, nickname);
            Log.Info($"收到注册结果：成功={response.Success}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_LoginResponse> LoginAsync(string account, string password)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_LoginRequest(account, password);
            if (response.Success)
            {
                Token = response.Token;
                Profile = response.Profile;
                Log.Info($"登录成功：玩家ID={Profile?.PlayerId}，昵称={Profile?.Nickname}");
            }
            else
            {
                Token = string.Empty;
                Profile = null;
                Log.Warning($"登录失败：{response.Message}");
            }

            return response;
        }

        public async FTask<G2C_SetNicknameResponse> SetNicknameAsync(string nickname)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_SetNicknameRequest(Token, nickname);
            if (response.Success)
            {
                Profile = response.Profile;
            }

            Log.Info($"设置昵称结果：成功={response.Success}，昵称={response.Profile?.Nickname}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_LobbyHomeResponse> RequestLobbyHomeAsync()
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_LobbyHomeRequest(Token);
            Log.Info($"大厅数据获取成功：房间数量={response.Rooms.Count}，匹配中={response.MatchStatus?.IsMatching}");
            return response;
        }

        public async FTask<G2C_CharacterListResponse> RequestCharacterListAsync()
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_CharacterListRequest(Token);
            Log.Info($"角色列表获取结果：成功={response.Success}，数量={response.Characters.Count}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_SelectCharacterResponse> SelectCharacterAsync(int characterId)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_SelectCharacterRequest(Token, characterId);
            Log.Info($"选择角色结果：成功={response.Success}，角色ID={characterId}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_AssetSnapshotResponse> RequestAssetSnapshotAsync()
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_AssetSnapshotRequest(Token);
            Log.Info($"资产快照结果：成功={response.Success}，货币={response.Snapshot?.Currencies.Count ?? 0}，道具={response.Snapshot?.BagItems.Count ?? 0}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_UseItemResponse> UseItemAsync(int itemId, int count = 1)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_UseItemRequest(Token, itemId, count);
            Log.Info($"使用道具结果：成功={response.Success}，道具ID={itemId}，数量={count}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_OutgameShopListResponse> RequestOutgameShopListAsync(string shopType = "", string activityId = "", string featureId = "")
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_OutgameShopListRequest(Token, shopType, activityId, featureId);
            Log.Info($"局外商店列表结果：成功={response.Success}，数量={response.Shops.Count}，类型={shopType}，活动={activityId}，开放={featureId}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_BuyOutgameShopGoodsResponse> BuyOutgameShopGoodsAsync(int goodsId, int count = 1)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_BuyOutgameShopGoodsRequest(Token, goodsId, count);
            Log.Info($"局外商店购买结果：成功={response.Success}，商品ID={goodsId}，数量={count}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_MailListResponse> RequestMailListAsync()
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_MailListRequest(Token);
            Log.Info($"邮件列表结果：成功={response.Success}，数量={response.Mails.Count}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_ReadMailResponse> ReadMailAsync(long mailId)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_ReadMailRequest(Token, mailId);
            Log.Info($"读取邮件结果：成功={response.Success}，邮件ID={mailId}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_ClaimMailAttachmentResponse> ClaimMailAttachmentAsync(long mailId)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_ClaimMailAttachmentRequest(Token, mailId);
            Log.Info($"领取邮件附件结果：成功={response.Success}，邮件ID={mailId}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_LotteryDrawResponse> LotteryDrawAsync(string pool, int count)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_LotteryDrawRequest(Token, pool, count);
            Log.Info($"抽奖结果：成功={response.Success}，奖池={pool}，次数={count}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_OutgameTaskListResponse> RequestOutgameTaskListAsync(string taskType = "", string activityId = "", string featureId = "")
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_OutgameTaskListRequest(Token, taskType, activityId, featureId);
            Log.Info($"局外任务列表结果：成功={response.Success}，数量={response.Tasks.Count}，类型={taskType}，活动={activityId}，开放={featureId}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_ClaimOutgameTaskRewardResponse> ClaimOutgameTaskRewardAsync(int taskId)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_ClaimOutgameTaskRewardRequest(Token, taskId);
            Log.Info($"局外任务领奖结果：成功={response.Success}，任务ID={taskId}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_SocialListResponse> RequestSocialListAsync(string viewMode, string keyword)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_SocialListRequest(Token, viewMode, keyword);
            Log.Info($"社交列表结果：成功={response.Success}，视图={viewMode}，数量={response.Players.Count}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_FollowPlayerResponse> FollowPlayerAsync(long targetPlayerId, bool follow, string viewMode)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_FollowPlayerRequest(Token, targetPlayerId, follow, viewMode);
            Log.Info($"关注操作结果：成功={response.Success}，目标={targetPlayerId}，关注={follow}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_ChatHistoryResponse> RequestChatHistoryAsync(int channelType, long channelId, int limit = 50)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_ChatHistoryRequest(Token, channelType, channelId, limit);
            Log.Info($"聊天记录结果：成功={response.Success}，频道={channelType}:{channelId}，数量={response.Messages.Count}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_SendChatMessageResponse> SendChatMessageAsync(ChatMessageTreeInfo messageTree)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_SendChatMessageRequest(Token, messageTree);
            Log.Info($"发送聊天结果：成功={response.Success}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_StartMatchResponse> StartMatchAsync(string mode)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_StartMatchRequest(Token, mode);
            Log.Info($"开始匹配结果：匹配中={response.Status?.IsMatching}，预计={response.Status?.EstimatedSeconds}");
            return response;
        }

        public async FTask<G2C_CreateRoomResponse> CreateRoomAsync(string roomName, string mode, int mapId, int maxPlayers, bool isPrivate, string password, IReadOnlyList<int> selectedBuildingCardIds)
        {
            await EnsureConnectedAsync();
            var cards = selectedBuildingCardIds == null ? new List<int>() : new List<int>(selectedBuildingCardIds);
            var response = await Runtime.Session.C2G_CreateRoomRequest(Token, roomName, mode, mapId, maxPlayers, isPrivate, password, cards);
            Log.Info($"创建房间成功：房间ID={response.Room?.Summary?.RoomId}");
            return response;
        }

        public async FTask<G2C_JoinRoomResponse> JoinRoomAsync(int roomId, string password)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_JoinRoomRequest(Token, roomId, password);
            Log.Info($"加入房间结果：成功={response.Success}，房间ID={response.Room?.Summary?.RoomId}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_LeaveRoomResponse> LeaveRoomAsync(int roomId)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_LeaveRoomRequest(Token, roomId);
            Log.Info($"离开房间结果：成功={response.Success}，房间ID={roomId}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_RoomDetailResponse> RequestRoomDetailAsync(int roomId)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_RoomDetailRequest(Token, roomId);
            CacheBattleEndpoint(response.Battle);
            Log.Info($"房间详情结果：成功={response.Success}，房间ID={roomId}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_SetRoomReadyResponse> SetRoomReadyAsync(int roomId, bool isReady)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_SetRoomReadyRequest(Token, roomId, isReady);
            Log.Info($"准备状态结果：成功={response.Success}，房间ID={roomId}，IsReady={isReady}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_StartRoomResponse> StartRoomAsync(int roomId)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_StartRoomRequest(Token, roomId);
            CacheBattleEndpoint(response.Battle);
            Log.Info($"开始房间结果：成功={response.Success}，房间ID={roomId}，地图={response.Battle?.MapAsset}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_BattleSceneLoadedResponse> BattleSceneLoadedAsync(int battleId)
        {
            await EnsureBattleConnectedAsync();
            var response = await battleSession.C2G_BattleSceneLoadedRequest(Token, battleId);
            Log.Info($"战斗加载完成上报：成功={response.Success}，BattleId={battleId}，状态={response.Snapshot?.State}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_BattleSnapshotResponse> RequestBattleSnapshotAsync(int battleId, long lastKnownTick)
        {
            await EnsureBattleConnectedAsync();
            var response = await battleSession.C2G_BattleSnapshotRequest(Token, battleId, lastKnownTick);
            return response;
        }

        public async FTask<G2C_BattleMoveCommandResponse> MoveBattlePlayerAsync(int battleId, float axisX, float axisY)
        {
            await EnsureBattleConnectedAsync();
            var response = await battleSession.C2G_BattleMoveCommand(Token, battleId, axisX, axisY);
            return response;
        }

        public async FTask<G2C_BuildCommandResponse> BuildAsync(int battleId, int buildingId, int gridX, int gridY)
        {
            await EnsureBattleConnectedAsync();
            var response = await battleSession.C2G_BuildCommand(Token, battleId, buildingId, gridX, gridY);
            Log.Info($"建造命令结果：成功={response.Success}，BattleId={battleId}，BuildingId={buildingId}，Grid={gridX},{gridY}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_UpgradeBuildingCommandResponse> UpgradeBuildingAsync(int battleId, long buildingInstanceId)
        {
            await EnsureBattleConnectedAsync();
            var response = await battleSession.C2G_UpgradeBuildingCommand(Token, battleId, buildingInstanceId);
            Log.Info($"升级建筑结果：成功={response.Success}，BattleId={battleId}，InstanceId={buildingInstanceId}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_RecycleBuildingCommandResponse> RecycleBuildingAsync(int battleId, long buildingInstanceId)
        {
            await EnsureBattleConnectedAsync();
            var response = await battleSession.C2G_RecycleBuildingCommand(Token, battleId, buildingInstanceId);
            Log.Info($"回收建筑结果：成功={response.Success}，BattleId={battleId}，InstanceId={buildingInstanceId}，消息={response.Message}");
            return response;
        }

        public async FTask<G2C_BuyBattleShopGoodsCommandResponse> BuyBattleShopGoodsAsync(int battleId, int shopId, int goodsId)
        {
            await EnsureBattleConnectedAsync();
            var response = await battleSession.C2G_BuyBattleShopGoodsCommand(Token, battleId, shopId, goodsId);
            Log.Info($"局内商店购买结果：成功={response.Success}，BattleId={battleId}，ShopId={shopId}，GoodsId={goodsId}，消息={response.Message}");
            return response;
        }

        public void Dispose()
        {
            DisposeBattleSession();
            Runtime.OnDestroy();
            IsConnected = false;
            Token = string.Empty;
            Profile = null;
            Log.Info("网络模块已释放。");
        }

        private async FTask EnsureConnectedAsync()
        {
            if (!IsConnected || IsRuntimeSessionDisposed())
            {
                IsConnected = false;
                await ConnectAsync();
            }
        }

        private async FTask EnsureBattleConnectedAsync()
        {
            await EnsureConnectedAsync();
            if (IsBattleSessionAvailable)
            {
                return;
            }

            DisposeBattleSession();
            battleScene = await Scene.Create();
            isBattleConnected = false;
            battleSession = Runtime.CreateSession(
                battleScene,
                BattleHost,
                BattlePort,
                GetBattleNetworkProtocol(),
                false,
                5000,
                OnBattleConnected,
                OnBattleConnectFailed,
                OnBattleDisconnected);
        }

        public void DisposeBattleSession()
        {
            if (battleScene != null)
            {
                battleScene.Dispose();
                battleScene = null;
                battleSession = null;
            }

            isBattleConnected = false;
        }

        private void CacheBattleEndpoint(BattleStartInfo battle)
        {
            if (battle == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(battle.BattleHost))
            {
                BattleHost = battle.BattleHost;
            }

            if (battle.BattlePort > 0)
            {
                BattlePort = battle.BattlePort;
            }

            if (!string.IsNullOrWhiteSpace(battle.BattleProtocol))
            {
                BattleProtocol = battle.BattleProtocol;
            }
        }

        private FantasyRuntime.NetworkProtocolType GetBattleNetworkProtocol()
        {
            return string.Equals(BattleProtocol, "TCP", System.StringComparison.OrdinalIgnoreCase)
                ? FantasyRuntime.NetworkProtocolType.TCP
                : FantasyRuntime.NetworkProtocolType.KCP;
        }

        private static bool IsRuntimeSessionDisposed()
        {
            try
            {
                return Runtime.Session == null || Runtime.Session.IsDisposed;
            }
            catch
            {
                return true;
            }
        }

        private void OnConnected()
        {
            IsConnected = true;
            Log.Info($"连接服务器成功：{Host}:{Port}");
        }

        private void OnConnectFailed()
        {
            IsConnected = false;
            Log.Error($"连接服务器失败：{Host}:{Port}，请确认服务端是否已启动。");
        }

        private void OnDisconnected()
        {
            IsConnected = false;
            Log.Warning("服务器连接已断开。");
        }

        private void OnBattleConnected()
        {
            isBattleConnected = true;
            Log.Info($"连接战斗服务器成功：{BattleHost}:{BattlePort}");
        }

        private void OnBattleConnectFailed()
        {
            isBattleConnected = false;
            Log.Error($"连接战斗服务器失败：{BattleHost}:{BattlePort}，战斗请求将不可用。");
        }

        private void OnBattleDisconnected()
        {
            isBattleConnected = false;
            Log.Warning("战斗服务器连接已断开。");
        }
    }
}
