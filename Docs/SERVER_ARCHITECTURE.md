# 服务端架构

## 1. 当前结构

```text
Server.Fantasy/
├── Server.sln
├── Main/      # 进程入口，启动 Fantasy
├── Entity/    # 协议生成和共享实体
└── Hotfix/    # 业务 Handler 和 Service
```

入口：

```text
Server.Fantasy/Main/Program.cs
```

启动：

```powershell
dotnet run --project Server.Fantasy\Main\Main.csproj --framework net9.0
```

本地监听：

```text
127.0.0.1:20000
```

## 2. 当前 Hotfix 模块

```text
Hotfix/
├── Auth/
│   ├── Handler/RegisterRequestHandler.cs
│   ├── Handler/LoginRequestHandler.cs
│   └── Service/AuthService.cs
├── Lobby/
│   ├── Handler/LobbyHomeRequestHandler.cs
│   ├── Handler/StartMatchRequestHandler.cs
│   └── Service/MatchService.cs
├── Room/
│   ├── Handler/CreateRoomRequestHandler.cs
│   └── Service/CustomRoomService.cs
└── Shared/
    └── SheepServices.cs
```

## 3. 当前数据状态

当前为了跑通链路，账号、Token、匹配票据、房间都在内存中。

```text
AuthService        # 内存账号和 token
MatchService       # 内存匹配状态
CustomRoomService  # 内存房间表
```

未来可替换为 Redis/DB，但当前不要在业务代码里提前写死 Redis/DB 假接口。

## 4. 当前 RPC

```text
C2G_RegisterRequest      -> RegisterRequestHandler
C2G_LoginRequest         -> LoginRequestHandler
C2G_LobbyHomeRequest     -> LobbyHomeRequestHandler
C2G_StartMatchRequest    -> StartMatchRequestHandler
C2G_CreateRoomRequest    -> CreateRoomRequestHandler
```

## 5. 开发规则

- Handler 只做协议适配。
- 业务规则放到 Service。
- 共享服务从 `SheepServices` 取。
- 协议类型来自生成代码，不手改生成文件。
- 新 RPC 先改 `Shared.Protocol/NetworkProtocol`，再运行协议生成工具。

