# 协议设计

## 1. 当前协议源

协议源只改：

```text
Shared.Protocol/NetworkProtocol/
├── Outer/OuterMessage.proto
├── Inner/InnerMessage.proto
├── RouteType.Config
└── RoamingType.Config
```

不要手改生成文件。

## 2. 生成命令

```powershell
cd Tools.Protocol\ProtocolExportTool
dotnet .\Fantasy.ProtocolExportTool.dll export --silent
```

生成结果会进入服务端 `Entity/Generate` 和 Unity 热更协议目录。

## 3. 当前通信方式

当前使用 Fantasy TCP RPC，不是 HTTP/WebSocket 原型。

客户端：

```text
SheepNetworkService
-> Runtime.Connect(127.0.0.1, 20000, TCP)
-> Runtime.Session.C2G_xxxRequest(...)
```

服务端：

```text
MessageRPC<C2G_xxxRequest, G2C_xxxResponse>
```

## 4. 当前外网协议

当前 `OuterMessage.proto` 已包含：

```text
PlayerProfileInfo
C2G_RegisterRequest / G2C_RegisterResponse
C2G_LoginRequest / G2C_LoginResponse
C2G_LobbyHomeRequest / G2C_LobbyHomeResponse
MatchStatusInfo
C2G_StartMatchRequest / G2C_StartMatchResponse
C2G_CreateRoomRequest / G2C_CreateRoomResponse
RoomSummaryInfo
RoomPlayerInfo
RoomDetailInfo
```

## 5. 下一批建议协议

房间：

```text
C2G_JoinRoomRequest / G2C_JoinRoomResponse
C2G_LeaveRoomRequest / G2C_LeaveRoomResponse
C2G_SetReadyRequest / G2C_SetReadyResponse
C2G_StartRoomRequest / G2C_StartRoomResponse
S2C_RoomStatePush
```

战斗：

```text
C2G_BattleInputCommand
C2G_BuildCommand
C2G_UpgradeBuildingCommand
C2G_RecycleBuildingCommand
S2C_BattleSnapshot
S2C_BattleResultPush
```

## 6. 兼容规则

- 已生成并使用的字段只追加，不删除。
- 字段编号不复用。
- 消息命名使用 Fantasy 当前约定：`C2G_` / `G2C_` / `S2C_`。
- Handler 只做协议适配，业务逻辑放 Service。

