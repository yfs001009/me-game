# 协议设计

## 协议源

只改：

```text
Shared.Protocol/NetworkProtocol/
├── Outer/OuterMessage.proto
├── Inner/InnerMessage.proto
├── RouteType.Config
└── RoamingType.Config
```

不要手改生成文件。

## 生成命令

```powershell
cd Tools.Protocol\ProtocolExportTool
dotnet .\Fantasy.ProtocolExportTool.dll export --silent
```

生成结果进入：

```text
Server.Fantasy/Entity/Generate
Client.Unity/Assets/GameScripts/HotFix/GameProto/NetworkProtocol
```

## 当前通信方式

```text
Unity HotFix
-> SheepNetworkService
-> Fantasy Runtime TCP Session
-> Runtime.Session.C2G_xxxRequest(...)
-> Server MessageRPC<C2G_xxxRequest, G2C_xxxResponse>
```

不是 HTTP/WebSocket 原型。

## 当前外网协议范围

账号：

```text
C2G_RegisterRequest / G2C_RegisterResponse
C2G_LoginRequest / G2C_LoginResponse
C2G_SetNicknameRequest / G2C_SetNicknameResponse
PlayerProfileInfo
```

大厅/房间：

```text
C2G_LobbyHomeRequest / G2C_LobbyHomeResponse
C2G_StartMatchRequest / G2C_StartMatchResponse
C2G_CreateRoomRequest / G2C_CreateRoomResponse
C2G_JoinRoomRequest / G2C_JoinRoomResponse
C2G_LeaveRoomRequest / G2C_LeaveRoomResponse
C2G_RoomDetailRequest / G2C_RoomDetailResponse
C2G_SetRoomReadyRequest / G2C_SetRoomReadyResponse
C2G_StartRoomRequest / G2C_StartRoomResponse
MatchStatusInfo
RoomSummaryInfo
RoomPlayerInfo
RoomDetailInfo
BattleStartInfo
```

战斗：

```text
BattlePlayerStateInfo
BattleBuildingStateInfo
BattleSnapshotInfo
C2G_BattleSceneLoadedRequest / G2C_BattleSceneLoadedResponse
C2G_BattleSnapshotRequest / G2C_BattleSnapshotResponse
C2G_BattleMoveCommand / G2C_BattleMoveCommandResponse
C2G_BuildCommand / G2C_BuildCommandResponse
C2G_UpgradeBuildingCommand / G2C_UpgradeBuildingCommandResponse
C2G_RecycleBuildingCommand / G2C_RecycleBuildingCommandResponse
```

## 当前同步方式

房间：

```text
创建/加入/离开/准备/开始 RPC 返回 RoomDetailInfo
RoomUI 当前每秒 RoomDetail 轮询，用于非房主发现 BattleStartInfo 并进入战斗
```

战斗：

```text
BattleSceneLoaded 上报加载完成
服务端等全员 loaded 后 State=Running
客户端 200ms 轮询 BattleSnapshot
移动/建造/升级/回收 RPC 返回最新 BattleSnapshotInfo
```

移动：

```text
客户端只提交方向 AxisX/AxisY
服务端权威更新 PosX/PosY
客户端用快照刷新表现
```

## 下一批协议

- `RoomLoadoutInfo`：房间内玩家阵营、精灵建筑卡组、幽灵角色选择。
- `C2G_UpdateRoomLoadoutRequest` / `G2C_UpdateRoomLoadoutResponse`：战前更新卡组/阵营/角色。
- `BattlePlayerLoadoutInfo`：进入战斗时固化玩家带入卡组和角色。
- `BattleGhostEquipmentInfo`：幽灵局内 6 格装备。
- `C2G_BuyGhostEquipmentRequest` / `G2C_BuyGhostEquipmentResponse`：幽灵局内购买装备。
- `S2C_RoomStatePush`：替换房间轮询。
- `S2C_BattleSnapshot`：替换 200ms RPC 快照轮询。
- `S2C_BattleResultPush`：战斗结算。
- `C2G_BattleInteractCommand`：商店、怪物、采集等局内交互。
- `C2G_BattleSkillCommand`：技能释放和目标选择。

## 兼容规则

- 字段只追加，不删除已使用字段。
- 字段编号不复用。
- 命名用 `C2G_` / `G2C_` / `S2C_`。
- Handler 只做协议适配，业务逻辑放 Service。
