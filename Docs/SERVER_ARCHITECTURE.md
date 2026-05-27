# 服务端架构

最后整理：2026-05-26。

本文记录 Fantasy 服务端当前结构、已落地模块和目标演进方向。

## 1. 技术栈

```text
.NET 9
Fantasy-Net 2025.2.1402
TCP Gate: 127.0.0.1:20000
Main / Entity / Hotfix 三项目结构
```

启动：

```powershell
dotnet run --project Server.Fantasy\Main\Main.csproj --framework net9.0
```

编译：

```powershell
dotnet build Server.Fantasy\Server.sln --no-restore
```

## 2. 项目结构

```text
Server.Fantasy/
├── Server.sln
├── Main/      # 进程入口，启动 Fantasy，日志初始化，程序集加载
├── Entity/    # Fantasy.config、协议生成、目标领域 Entity/Component
└── Hotfix/    # MessageRPC Handler、System、过渡 Service、Config
```

当前 `Entity` 主要是协议生成代码。后续必须逐步放入 `Player/Room/Battle` 等领域 Entity/Component，不能长期只把 Fantasy 当 RPC 框架使用。

## 3. 当前运行形态

```text
Unity HotFix
-> Fantasy Runtime TCP Session
-> Server.Fantasy Hotfix MessageRPC Handler
-> SheepServices 过渡服务
-> 单进程内存状态
```

当前只有一个 Gate Scene：

```text
Server.Fantasy/Entity/Fantasy.config
sceneTypeString="Gate"
outerPort="20000"
```

这适合 MVP 调通。千人在线目标下，Battle/Room/Lobby/Chat/Guild/Data 需要逐步 Scene 化。

## 4. 当前 Hotfix 模块

```text
Auth/
  Handler/RegisterRequestHandler.cs
  Handler/LoginRequestHandler.cs
  Handler/SetNicknameRequestHandler.cs
  Service/AuthService.cs

Lobby/
  Handler/LobbyHomeRequestHandler.cs
  Handler/StartMatchRequestHandler.cs
  Service/MatchService.cs

Room/
  Handler/CreateRoomRequestHandler.cs
  Handler/JoinRoomRequestHandler.cs
  Handler/LeaveRoomRequestHandler.cs
  Handler/RoomDetailRequestHandler.cs
  Handler/SetRoomReadyRequestHandler.cs
  Handler/StartRoomRequestHandler.cs
  Service/CustomRoomService.cs

Battle/
  Handler/BattleSceneLoadedRequestHandler.cs
  Handler/BattleSnapshotRequestHandler.cs
  Handler/BattleMoveCommandHandler.cs
  Handler/BuildCommandHandler.cs
  Handler/UpgradeBuildingCommandHandler.cs
  Handler/RecycleBuildingCommandHandler.cs
  Service/BattleService.cs

Config/
  ConfigSystem.cs
  GameConfig/...

Shared/
  SheepServices.cs
  GameRuleService.cs
```

## 5. Handler 规则

Handler 只做协议适配：

```text
读取 Token
RequireProfile / 查找 PlayerEntity
字段映射
调用 Scene 内 System 或过渡 Service
填 response
```

不要在 Handler 里写业务规则。

当前兼容写法：

```text
C2G_BuildCommand
-> BuildCommandHandler
-> SheepServices.Auth.RequireProfile
-> SheepServices.Battles.Build
-> response.Snapshot
```

目标写法：

```text
C2G_BuildCommand
-> BuildCommandHandler
-> 根据 Player 找到 BattleEntity 地址
-> Route 到 Battle Scene
-> BuildSystem 校验卡组、资源、地图、占格、阵营
-> 修改 BattleBuildingEntity / ResourceWalletComponent
-> 返回响应或广播 S2C_BattleSnapshot
```

## 6. 当前数据状态

全部是单进程内存：

```text
AuthService
  账号、token、玩家资料。

MatchService
  匹配票据。

CustomRoomService
  房间表、准备状态、开始战斗。

BattleService
  战斗表、玩家状态、建筑状态、资源、Tick、攻击事件。

ConfigSystem
  Luban bytes。
```

这些是 MVP 过渡状态。新增功能时要避免继续扩大静态 Service，优先给未来 Entity/Scene 留边界。

## 7. 目标 Scene 规划

```text
Gate Scene
  连接、登录、Session、心跳、断线处理。

Account/Data Scene
  账号、货币、已解锁建筑卡、已解锁角色、任务、邮件、商城购买。

Lobby Scene
  在线状态、匹配、房间列表、地图池。

Room Scene
  房间、地图选择、阵营选择、精灵卡组选择、幽灵角色选择。

Battle Scene
  单局战斗 Tick、移动、建造、幽灵装备、伤害、胜负、快照广播。

Chat Scene
  世界聊天、房间聊天、公会聊天、私聊。

Guild Scene
  公会资料、成员、职位、申请、公告。
```

MVP 可先单进程同服，但代码结构要按这些边界组织。

## 8. 目标 Entity/Component

```text
PlayerEntity
  PlayerProfileComponent
  PlayerSessionComponent
  PlayerUnlockComponent
  PlayerCurrencyComponent

RoomEntity
  RoomMapComponent
  RoomPlayerListComponent
  RoomLoadoutComponent
  RoomReadyComponent

BattleEntity
  BattlePhaseComponent
  BattleMapComponent
  BattleSnapshotComponent
  BattleTimerComponent

BattlePlayerEntity
  CampComponent
  PositionComponent
  HealthComponent
  ResourceWalletComponent
  ElfLoadoutComponent
  GhostEquipmentComponent

BattleBuildingEntity
  GridFootprintComponent
  HealthComponent
  BuildingConfigComponent
  CooldownComponent

BattleGhostEntity
  EquipmentBagComponent
  CombatStatsComponent
  RespawnComponent
```

## 9. 战斗状态机

当前：

```text
StartRoom
-> CustomRoomService 校验房主、准备状态
-> BattleService.CreateFromRoom
-> Battle.State = Loading
-> 每个客户端 BattleSceneLoaded
-> 全员 SceneLoaded 后 Battle.State = Running
-> Move/Build/Upgrade/Recycle
-> BattleSnapshot 返回权威状态
```

目标：

```text
Created
-> Loading
-> Prepare
-> Running
-> Settling
-> Finished
```

后续幽灵装备、感染、结算都必须挂在这个状态机上。

## 10. Fantasy 使用原则

- `Fantasy.config` 放在直接引用 Fantasy 的 `Entity` 项目根目录。
- 每个运行时 Entity/Component 必须属于正确 Scene。
- 短生命周期实体优先用 Fantasy Entity 对象池。
- 定时逻辑走 TimerComponent/System，不手写散落后台循环。
- Scene 内部解耦用 Fantasy Event。
- 跨 Scene/跨服通信用 Route/Address/Roaming。
- Inner 协议用于服务器间通信，Outer 协议用于客户端通信。
- Handler 不写业务规则，业务进 System 或过渡 Service。

## 11. 下一步服务端切片

先做“地图选择 + 精灵 6 卡组”：

```text
1. 协议增加玩家卡组/阵营/角色选择字段。
2. Room 记录玩家选择。
3. StartRoom 固化卡组到 Battle。
4. BuildCommand 校验玩家是否带入该建筑卡。
5. 为 Room/Battle 状态补注释，标明后续迁移 Entity/Scene。
```

同时开始补 Entity 目录：

```text
Server.Fantasy/Entity/Battle
Server.Fantasy/Entity/Room
Server.Fantasy/Entity/Player
```

可以先定义结构，不急着全部替换 Service。

## 12. 代码注释要求

必须注释：

```text
Fantasy Scene / Entity / Component / System 的职责边界
协议 Handler 为什么只做适配、具体业务转到哪里
战斗状态机、Tick、结算、感染、装备购买等关键流程
临时兼容代码和未来迁移到 Scene/Entity 的过渡代码
```

不要给简单赋值、明显 if/return 写噪音注释。
