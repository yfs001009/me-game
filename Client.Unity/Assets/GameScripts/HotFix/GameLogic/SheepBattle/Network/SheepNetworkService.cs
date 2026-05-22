using Fantasy;
using Fantasy.Async;
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
        public string Token { get; private set; } = string.Empty;
        public PlayerProfileInfo Profile { get; private set; }
        public bool IsConnected { get; private set; }

        private SheepNetworkService()
        {
        }

        public void Initialize(string host, int port)
        {
            Host = host;
            Port = port;
            Log.Info($"网络模块初始化完成，目标服务器：{host}:{port}");
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

        public async FTask<G2C_CreateRoomResponse> CreateRoomAsync(string roomName, string mode, int mapId, int maxPlayers, bool isPrivate, string password)
        {
            await EnsureConnectedAsync();
            var response = await Runtime.Session.C2G_CreateRoomRequest(Token, roomName, mode, mapId, maxPlayers, isPrivate, password);
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

        public void Dispose()
        {
            Runtime.OnDestroy();
            IsConnected = false;
            Token = string.Empty;
            Profile = null;
            Log.Info("网络模块已释放。");
        }

        private async FTask EnsureConnectedAsync()
        {
            if (!IsConnected)
            {
                await ConnectAsync();
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
    }
}
