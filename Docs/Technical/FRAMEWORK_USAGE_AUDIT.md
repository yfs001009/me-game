# TEngine / Fantasy 框架使用体检

最后整理：2026-05-26。

本文记录当前代码里 TEngine 与 Fantasy 的使用情况、问题点和改造优先级。目标是先列清楚方向，再按功能切片逐步修，不做一次性大重构。

## 1. 总体结论

```text
客户端：
  TEngine 的 UI、事件、资源加载已经部分用上。
  主要短板是战斗表现和动态 UI 还在大量 Instantiate/Destroy，没有形成对象池和内存池边界。

服务端：
  Fantasy 的协议生成和 MessageRPC 已经用上。
  主要短板是 Entity 项目几乎只有协议生成代码，领域状态仍堆在 Hotfix 静态 Service 字典里。
```

下一步原则：

```text
新功能从一开始就按框架边界写。
旧代码先做清单，不急着全量重构。
每完成一个玩法切片，就顺手把相关框架用法补正。
```

## 2. 客户端 TEngine 体检

### 已经用得比较好的地方

```text
UI
  业务 UI 基本继承 UIWindow/UIWidget。
  UI 按钮大多通过 GameEvent.Get<IxxxCommand>() 进入 Controller。
  状态刷新已经使用 AddUIEvent<T>。

Network
  SheepNetworkService 封装 Fantasy Unity Runtime。
  UI 基本没有直接发 RPC，主要由 Controller 调网络。

Resource
  TiledMapLoader 和 BattleController 地图加载已经使用 GameModule.Resource。
  UI prefab 由 UIModule 加载。

MemoryPool
  TEngine UIBase 已经用 MemoryPool 管理 GameEventMgr。
```

### 主要问题

#### P0：战斗表现大量 new/destroy

位置：

```text
Client.Unity/Assets/GameScripts/HotFix/GameLogic/SheepBattle/Battle/BattleController.cs
```

现象：

```text
玩家快照刷新时销毁/重建玩家表现。
建筑快照刷新时销毁/重建建筑表现。
血条、弹道、范围圈、选中信息、建造预览大量 new GameObject。
弹道播放结束直接 Object.Destroy。
```

风险：

```text
战斗人数和建筑数量上来后 GC 压力会明显增加。
手机上会出现掉帧和卡顿。
后续幽灵、装备特效、伤害数字加入后问题会放大。
```

改造建议：

```text
新增 Battle/Pool 或 Battle/View：
  BattleViewPool
  BattleViewHandle
  BattlePlayerView
  BattleBuildingView
  BattleGhostView
  BattleHealthBarView
  BattleProjectileView
  BattleDamageTextView

资产加载交给 TEngine ResourceModule。
运行时实例复用交给 BattleViewPool。
短生命周期纯 C# 临时对象才交给 MemoryPool。
```

#### P0：BattleMainUI 列表项反复 Instantiate/Destroy

位置：

```text
Client.Unity/Assets/GameScripts/HotFix/GameLogic/UI/BattleMainUI/BattleMainUI.cs
```

现象：

```text
队友头像列表每次刷新会清理再创建。
建筑操作按钮会销毁重建。
建筑卡牌启动时创建一次，目前问题不大。
```

改造建议：

```text
队友头像、操作按钮改为 UIWidget 或轻量 ItemPool。
刷新时复用已有 item，超出部分隐藏。
建筑卡组改成“战前带入卡组”，进入战斗时构建一次。
```

#### P1：动态 UI 临时页面太多

位置：

```text
RoomListUI
RoomPasswordUI
MatchQueueUI
CommonNoticeUI
NicknameUI
```

现象：

```text
部分 UI 用 TestUI 或运行时 new GameObject 构建。
这适合 MVP，但长期不利于美术迭代和资源管理。
```

改造建议：

```text
高频/核心页面 prefab 化：
  RoomListUI
  LoadoutUI
  GhostEquipShopUI
  ShopUI
  TaskUI
  MailUI
  ChatUI

临时弹窗可以继续动态创建，但需要收敛到统一 DynamicUI 工具。
```

#### P1：资源卸载策略还不明确

现象：

```text
地图 prefab、战斗对象、特效资源已经通过 ResourceModule 加载，但退出战斗后的资源释放策略不清晰。
```

改造建议：

```text
BattleController.LeaveBattle:
  归还 BattleViewPool 活跃实例。
  清理战斗临时对象。
  按需调用 GameModule.Resource.UnloadUnusedAssets 或等待 TEngine 自动释放。

切换大场景或低内存：
  使用 ResourceModule 的 ForceUnloadUnusedAssets。
```

## 3. 服务端 Fantasy 体检

### 已经用得比较好的地方

```text
Protocol
  Shared.Protocol -> ProtocolExportTool -> Entity/Generate 和 Unity GameProto 已跑通。

RPC
  Handler 使用 MessageRPC<C2G, G2C>。
  Handler 多数只做 token/profile、调用服务、填 response，整体比较薄。

Config
  服务端 Luban bytes 和 ConfigSystem 已经接入。
```

### 主要问题

#### P0：Entity 项目没有承载领域模型

位置：

```text
Server.Fantasy/Entity
```

现状：

```text
只有 AssemblyHelper、Fantasy.config、协议生成代码。
没有 PlayerEntity、RoomEntity、BattleEntity、Component。
```

风险：

```text
Fantasy 被当成普通 RPC 框架使用。
后续要拆 Scene、Route、Battle Tick 时会很难。
千人在线时 Hotfix 静态 Service 会成为瓶颈。
```

改造建议：

```text
先新增 Entity/Battle：
  BattleEntity
  BattlePlayerEntity
  BattleBuildingEntity
  BattleMonsterEntity
  BattleMapComponent
  BattleSnapshotComponent

再新增 Entity/Room：
  RoomEntity
  RoomPlayerComponent
  RoomLoadoutComponent

再新增 Entity/Player：
  PlayerEntity
  PlayerUnlockComponent
  PlayerSessionComponent
```

#### P0：BattleService 是单体内存战斗

位置：

```text
Server.Fantasy/Hotfix/Battle/Service/BattleService.cs
```

现象：

```text
Dictionary<int, BattleRecord> 保存所有战斗。
lock(gate) 串行保护全部战斗。
AdvanceTick 由 RPC 请求顺手推进。
玩家、建筑、攻击事件、地图规则都在一个 Service 里。
```

风险：

```text
多人和多房间增长后锁竞争明显。
没有固定 Tick，快照依赖客户端轮询。
战斗逻辑越来越大，难以拆分。
```

改造建议：

```text
短期：
  保留 BattleService，但把 BattleRecord 拆成更接近 Entity 的结构。
  新增卡组校验、幽灵装备时不要继续扩大单方法。

中期：
  把 Tick、建造、移动、攻击拆成 System 类：
    BattleTickSystem
    BattleMoveSystem
    BuildSystem
    TowerAttackSystem
    GhostEquipmentSystem

长期：
  Battle Scene 内持有 BattleEntity。
  TimerComponent 固定 Tick。
  S2C_BattleSnapshot 广播替换 RPC 轮询。
```

#### P0：Room/Loadout 还没有服务端权威归属

现状：

```text
房间有 MapId。
还没有玩家阵营选择、精灵卡组、幽灵角色选择。
```

下一步要做：

```text
RoomPlayerInfo 或新增 RoomLoadoutInfo:
  Camp
  SelectedBuildingCardIds
  SelectedElfRoleId
  SelectedGhostRoleId

服务端 StartRoom 时把房间选择固化进 BattleEntity/BattleRecord。
BuildCommand 必须校验玩家是否带入该 BuildingCard。
```

#### P1：Fantasy.config 只有 Gate Scene

现状：

```text
sceneTypeString="Gate"
outerPort="20000"
```

建议：

```text
MVP 可继续单 Scene。
下一阶段先加 Battle Scene 配置，但可以仍在同进程。
再逐步加 Lobby/Room/Chat/Guild/Data Scene。
```

#### P1：Auth/Room/Match 仍是内存字典

现状：

```text
AuthService: accounts/token 字典
CustomRoomService: rooms 字典
MatchService: tickets 字典
```

建议：

```text
开发期可保留。
做商店/任务/邮件前，要先设计 DB/Redis 数据归属。
账号、解锁、货币、任务、邮件不应长期存在内存字典里。
```

## 4. 改造优先级

### P0：跟下一功能切片一起做

```text
1. 地图选择 + 6 卡组功能不要直接硬写 UI/Service。
2. 新增 Loadout 模块，客户端和服务端都保留边界。
3. BattleMainUI 显示带入卡组，不再显示全部 TbBuildingCard。
4. 服务端 BuildCommand 校验玩家带入卡组。
5. 给 Room/Battle 状态补注释，标明后续迁移 Entity/Scene。
```

### P1：战斗性能和框架化

```text
1. BattleViewPool 接管玩家、建筑、幽灵、血条、弹道、特效。
2. BattleController 拆出 BattleSyncService、BattleMapView、BattleEntityView。
3. BattleService 拆出 BattleTickSystem、BuildSystem、CombatSystem。
4. Entity 项目新增 Battle/Room/Player 领域实体。
```

### P2：千人在线准备

```text
1. Battle Scene 固定 Tick。
2. S2C_BattleSnapshot 广播。
3. Room/Lobby/Chat/Guild/Data Scene 拆分。
4. Redis 保存 token、在线状态、房间索引、聊天限频。
5. DB 保存账号、解锁、货币、任务、邮件、公会。
```

## 5. 下一步建议

先做“地图选择 + 精灵 6 卡组”切片，同时把框架边界带进去：

```text
客户端：
  LoadoutService
  LoadoutViewModel
  LoadoutUI 或 RoomUI 内的简化卡组选择
  BattleMainUI 只显示带入卡组

服务端：
  协议增加玩家带入卡组字段
  Room 记录玩家卡组
  StartRoom 固化到 Battle
  BuildCommand 校验卡组

文档：
  更新 PROTOCOL_DESIGN
  更新 CLIENT_ARCHITECTURE
  更新 SERVER_ARCHITECTURE
```

做这个切片时，新增代码需要补注释，特别是：

```text
卡组是 MVP 过渡实现，后续要接账号解锁和商城。
服务端校验是权威来源，客户端过滤只是 UI 提示。
Room/Battle 当前仍在 Service，后续迁移 Entity/Scene。
```
