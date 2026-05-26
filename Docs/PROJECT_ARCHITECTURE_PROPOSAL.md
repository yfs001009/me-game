# SheepBattle 项目架构方案

最后整理：2026-05-26。

本文基于当前工程文档、现有代码、Fantasy 官方文档/Docfork 摘要和 TEngine 本地使用文档整理。目标不是重写工程，而是在当前可运行 MVP 上，把客户端、服务端、协议、配置、地图和发布链路收敛成可继续扩展的项目架构。

## 1. 架构目标

```text
商业目标：多人非对称对抗手游，先完成可反复测试的核心玩法闭环。
工程目标：客户端热更、服务端权威、协议强类型、配置双端一致、地图规则可复用。
演进目标：当前单进程内存 MVP 可运行；后续平滑迁移到 Fantasy 多 Scene、Redis/DB 和服务端推送。
```

当前阶段优先保证：

- 注册/登录 -> 大厅 -> 房间 -> 战斗 -> 建造/移动/升级/回收链路稳定。
- 服务端保持权威状态，客户端只做表现、预测提示和输入提交。
- 所有玩法数值走 Luban，协议只改 proto 源，地图视觉和规则逐步统一。
- 不提前搭太重的分布式架子，但预留清晰的迁移边界。

## 2. 总体分层

```text
┌─────────────────────────────────────────────────────────────┐
│ Client.Unity / TEngine                                      │
│ Launcher / Procedure / YooAsset / HybridCLR                 │
│ HotFix GameLogic: UI + MVE + Controller + Service + Pool    │
│ Fantasy Unity Runtime TCP RPC                               │
└───────────────────────────────┬─────────────────────────────┘
                                │ Fantasy proto / TCP
┌───────────────────────────────▼─────────────────────────────┐
│ Server.Fantasy                                               │
│ Main: process bootstrap, Fantasy entry, logging              │
│ Entity: generated protocol + domain Entity/Component         │
│ Hotfix: MessageRPC Handler + Entity System                   │
└───────────────────────────────┬─────────────────────────────┘
                                │ generated data
┌───────────────────────────────▼─────────────────────────────┐
│ Shared Toolchain                                              │
│ Shared.Protocol: proto source                                │
│ Tools.Protocol: Fantasy ProtocolExportTool                   │
│ Tools.Config: Luban Excel/Defines/templates                  │
│ Tools.Map: Tiled source maps and import/export pipeline      │
└─────────────────────────────────────────────────────────────┘
```

### 工程目录定位

```text
SheepBattle/
├── Client.Unity/              # Unity 2022.3.62f1c1 + TEngine 客户端
├── Server.Fantasy/            # Fantasy .NET 9 服务端
├── Shared.Protocol/           # Fantasy proto 协议源
├── Shared.Config/             # 共享配置预留
├── Tools.Protocol/            # Fantasy 协议生成工具
├── Tools.Config/              # Luban 配表工具链
├── Tools.Map/                 # Tiled 地图工作区
├── Deploy/                    # 后续部署、Nginx、Docker、环境配置
└── Docs/                      # 项目文档
```

## 3. 客户端架构

客户端以 TEngine 为宿主，业务全部放在 HotFix GameLogic。TEngine 负责模块、资源、UI、事件、热更生命周期；SheepBattle 业务层只在框架入口之后启动。

### 3.1 启动链路

```text
Unity Launcher / Procedure
-> TEngine ResourceModule 初始化 YooAsset
-> HybridCLR 加载 GameProto.dll.bytes / GameLogic.dll.bytes
-> GameApp.Entrance
-> GameEventHelper.Init()
-> SheepBattleApp.Start()
-> GameProtoFantasyRegistrar.Register()
-> 注册 ILoginCommand / ILobbyCommand / IBattleCommand
-> SheepNetworkService.Initialize("127.0.0.1", 20000)
-> SplashUI -> VersionCheckUI -> LoadingUI -> LoginUI
```

建议保持一个原则：`GameApp` 是热更域入口，`SheepBattleApp` 是项目业务入口。后续新增系统时挂到 `SheepBattleApp.Start/Release` 或业务模块初始化，不反向侵入 TEngine Launcher。

### 3.2 客户端业务分层

```text
UIWindow/UIWidget
  只做显示、输入收集、事件监听

Command Interface
  ILoginCommand / ILobbyCommand / IBattleCommand
  UI 按钮通过 GameEvent.Get<T>() 调业务命令

Controller
  编排业务流程，调用 Service，更新 Model

Model/ViewModel
  保存状态，构造 UI 需要的数据，发送 IEvent 状态事件

Service
  封装外部系统：Fantasy 网络、Luban 配置、资源加载、公共提示
```

推荐目录继续收敛为：

```text
Assets/GameScripts/HotFix/GameLogic/SheepBattle/
├── App/
├── Common/
├── Config/
├── Network/
├── Login/
├── Lobby/
├── Room/             # 建议后续从 Lobby 中拆出
├── Battle/
│   ├── Input/
│   ├── View/
│   ├── Sync/
│   └── Rule/
└── Event/
```

`BattleController` 当前职责较重，后续建议按风险逐步拆成：

```text
BattleController        # 对外 IBattleCommand 门面和生命周期编排
BattleSyncService       # 快照拉取/推送、移动命令限流、同步状态
BattleMapView           # 地图 prefab / Tiled fallback 加载和坐标转换
BattleEntityView        # 玩家、建筑、怪物、攻击事件表现
BattleBuildPresenter    # 建造预览、范围圈、合法性颜色、点击确认
BattleInputService      # WASD、鼠标、UI 防穿透、镜头拖拽
```

先不要一次性大拆。每接一个新闭环，例如怪物/防御塔/感染，再把对应职责从 `BattleController` 中自然抽出去。

### 3.3 UI 规范

TEngine UI 已有 `UIWindow` / `UIWidget` / `UIModule` 分层，应继续遵守：

- 页面级使用 `UIWindow`，重复项和局部组件使用 `UIWidget`。
- UI prefab 只保留节点、组件、绑定引用；业务代码在窗口和组件类。
- UI 不直接调 `SheepNetworkService`，通过命令接口进入 Controller。
- 状态刷新走 `AddUIEvent<T>`，不要在 UI 里写轮询业务逻辑。
- 公共失败提示、确认弹窗、加载遮罩统一走 Common 层，不散落在每个 UI。

### 3.4 资源与热更

```text
EditorMode:
  用于快速验证业务逻辑和 UI，不依赖真实热更包。

OfflinePlayMode / HostPlayMode:
  走 YooAsset + HybridCLR，必须重新构建热更 DLL 和 AssetBundle。
```

资源约定：

```text
Assets/AssetRaw/UI              # UI prefab
Assets/AssetRaw/UI/Art          # UI 美术
Assets/AssetRaw/MapPrefabs      # 地图 prefab
Assets/AssetRaw/TiledMaps       # Tiled 导入源
Assets/AssetRaw/Configs/bytes   # Luban bytes
Assets/AssetRaw/Effects         # 战斗特效
```

业务层不散落 `Resources.Load`。UI 图片用 `SetSprite` / `SetSubSprite`；GameObject 用 `GameModule.Resource.LoadGameObjectAsync` 或 UI 模块加载。

### 3.5 TEngine 资源、对象池、内存池落点

当前项目已经通过 `UIModule` 和 `ResourceModule` 间接用到了 TEngine 的资源系统，但战斗层对反复创建/销毁对象的控制还不够明确。建议按对象生命周期分三类处理。

#### A. 资源资产：交给 ResourceModule/YooAsset

适用对象：

```text
UI prefab
地图 prefab
角色/建筑/怪物 prefab
Sprite、图集、特效、音频
配置 bytes
```

规则：

- 只通过 `GameModule.Resource` / `UIModule` 加载，不在业务里直接 `Resources.Load`。
- 同一个 location 的资源由 ResourceModule 命中缓存和 AssetObject 池。
- 战斗切场、退出房间、低内存时调用资源模块的卸载能力，而不是手动到处释放 AssetBundle。
- `LoadGameObjectAsync` 适合“加载并实例化”，但高频对象不能每次都加载/销毁。

#### B. 运行时 GameObject：战斗内做复用池

适用对象：

```text
玩家表现
建筑表现
怪物表现
血条
伤害数字
攻击弹道/命中特效
建造预览占格
选中框/范围圈
```

建议新增客户端战斗表现池：

```text
SheepBattle/Battle/View/
├── BattleViewPool.cs          # 按 prefab location 管理实例池
├── BattleViewHandle.cs        # 持有实例、location、归还逻辑
├── BattleEntityView.cs        # 玩家/建筑/怪物视图基类或轻包装
└── BattleEffectView.cs        # 短生命周期特效包装
```

用法策略：

```text
进入战斗:
  预热常用 prefab，比如玩家、基础建筑、血条、弹道。

快照刷新:
  新增实体 -> 从池里取实例。
  已存在实体 -> 更新位置/血量/状态。
  消失实体 -> 归还池，不 Destroy。

离开战斗:
  清空场景中的激活实例。
  可保留少量常用实例，或按内存压力释放。
```

这样可以避免 `BattleController` 每次刷新都 `new GameObject` / `Destroy`，也能让 TEngine 的资源缓存负责资产层，项目自己的池负责实例层。

#### C. 短生命周期 C# 对象：用 MemoryPool

适用对象：

```text
快照差异计算临时对象
攻击表现事件
伤害数字请求
血条显示请求
建造合法性检查结果
寻路/格子扫描临时列表包装
UI 列表项刷新数据
```

规则：

- 高频创建的纯 C# 对象实现 `IMemory` 或继承 `MemoryObject`，用 `MemoryPool.Acquire<T>()` / `MemoryPool.Release()`。
- 对象回收时必须 `Clear()`，清掉引用、List、Dictionary、状态字段。
- 不要把长期 Model 放进 MemoryPool；Model 是状态源，池化会让生命周期变得危险。
- List/Dictionary 如需复用，优先封装成可 Clear 的临时对象，不要把集合裸露给 UI 长期持有。

优先落地点：

```text
BattleController 中的攻击事件去重、血条显示、建造预览检查。
BattleMainUI 建筑卡片/房间玩家列表的临时刷新数据。
后续怪物、防御塔弹道、伤害数字上屏。
```

#### D. 不建议池化的对象

```text
UIWindow 生命周期对象
Controller/Model/Service 单例
网络响应协议对象
Luban 配置对象
需要清晰 Destroy 生命周期的地图根节点
```

这些对象更适合清晰创建/释放，不为了“用了池”而池化。

## 4. 服务端架构

服务端要按 Fantasy 的思想组织，而不是普通 Web 服务端的“Controller/Service/Repository”。Fantasy 的核心是 Scene 作为生命周期和并发边界，Entity/Component 承载领域状态，System/Handler/Event/Timer 承载行为，Route/Address/Roaming/SphereEvent 支撑分布式通信。

当前工程保持 Fantasy 推荐的 `Main / Entity / Hotfix` 三项目结构。

```text
Server.Fantasy/
├── Main/      # 进程入口，启动 Fantasy，日志初始化，程序集加载
├── Entity/    # 协议生成代码和共享实体
└── Hotfix/    # MessageRPC Handler、Service、Config、共享服务
```

### 4.1 Fantasy 分层职责

```text
Main
  进程入口，只负责程序集加载、日志、Entry.Start。

Entity
  Fantasy.config、协议生成代码、领域 Entity/Component 定义、可序列化共享数据。
  这里不应该只有 Generate，Player/Room/Battle/Building/Monster 等领域对象应逐步前移到这里。

Hotfix
  MessageRPC Handler、Awake/Destroy/Update/Timer/Transfer 等 System、领域行为扩展。
  Hotfix 可以热更行为，但不要把所有状态都塞成静态 Service 字典。
```

`Entity` 空，会导致项目看起来像“Fantasy 只被当作 RPC 框架使用”。目标结构应该让 `Entity` 成为服务端领域模型层。

### 4.2 Scene 设计

Fantasy.config 当前只有一个 `Gate` Scene，适合 MVP 连接调通。目标可以演进为：

```text
Gate Scene
  外网 TCP Session、登录鉴权、Session <-> Player 映射。

Lobby Scene
  大厅、在线玩家、匹配队列、房间索引。

Room Scene / Room SubScene
  房间 Entity，玩家准备状态，开始战斗前的编排。

Battle Scene / Battle SubScene
  单局战斗 Entity，固定 Tick，玩家/建筑/怪物实体，快照广播。

Data Scene
  DB/Redis 访问、账号资料、战绩、背包、商品数据。
```

单进程阶段也可以先只启动一个 Gate Scene，但内部仍按 Scene-owned Entity 写。等需要拆分时，才把 Room/Battle 搬到独立 Scene，而不是从静态单例硬拆。

### 4.3 Entity/Component 目标模型

建议在 `Server.Fantasy/Entity` 增加领域目录：

```text
Entity/
├── Generate/NetworkProtocol/
├── Gate/
│   ├── SessionPlayerComponent.cs
│   └── GateSessionComponent.cs
├── Player/
│   ├── PlayerEntity.cs
│   ├── PlayerProfileComponent.cs
│   └── PlayerSessionComponent.cs
├── Room/
│   ├── RoomEntity.cs
│   ├── RoomPlayerComponent.cs
│   └── RoomStateComponent.cs
└── Battle/
    ├── BattleEntity.cs
    ├── BattlePlayerEntity.cs
    ├── BattleBuildingEntity.cs
    ├── BattleMonsterEntity.cs
    ├── BattleMapComponent.cs
    ├── BattleResourceComponent.cs
    ├── BattleCombatComponent.cs
    └── BattleSnapshotComponent.cs
```

领域状态归属建议：

```text
PlayerEntity
  PlayerId、账号资料、在线 Session 引用、当前 Room/Battle 地址。

RoomEntity
  RoomId、Owner、Map、Mode、玩家列表、Ready 状态、Battle 地址。

BattleEntity
  BattleId、RoomId、Map、Phase、Tick、RunningStartedAt。

BattlePlayerEntity
  PlayerId、Camp、Hp、资源、位置、移动状态、复活/感染状态。

BattleBuildingEntity
  InstanceId、Owner、BuildingId、Level、Grid、Hp、状态、冷却。

BattleMonsterEntity
  MonsterId、Hp、位置、目标、复活/AI 状态。
```

组件适合放“可组合状态”：

```text
PositionComponent
HealthComponent
CampComponent
ResourceWalletComponent
GridFootprintComponent
CombatStatsComponent
CooldownComponent
MapRuleComponent
SnapshotDirtyComponent
```

System 负责行为：

```text
BattleTickSystem
BattleMoveSystem
BuildSystem
TowerAttackSystem
InfectionSystem
BattleResultSystem
RoomReadySystem
RoomStartSystem
```

这样 `BattleService` 不再是一个巨大的内存表，而是逐步变成 Entity/System 的编排薄层，甚至后续可以完全被 System 替代。

### 4.4 请求处理链路

当前兼容写法：

```text
Fantasy Gate Session
-> MessageRPC<C2G_xxxRequest, G2C_xxxResponse>
-> Handler
   - 读取 token
   - 找到 Scene 内 PlayerEntity / SessionPlayerComponent
   - 协议字段映射
   - 调用领域 System 或 Scene Component
   - 填充 response
```

推荐迁移后的写法：

```text
C2G_BuildCommand Handler
-> 根据 Player 找到 BattleEntity 地址
-> Route 到 Battle Scene
-> Battle Scene 内 BuildSystem 校验地图、资源、占格、阵营
-> 修改 BattleBuildingEntity / ResourceWalletComponent
-> 标记 SnapshotDirty
-> 返回 G2C_BuildCommandResponse 或广播 S2C_BattleSnapshot
```

Handler 继续保持薄层，但“业务规则放 Service”要修正为“业务规则放 Scene 内 Entity System / Component System”。Service 可以作为过渡适配器，但不是最终形态。

### 4.5 当前 Hotfix 模块

```text
Auth/
  账号、token、玩家资料

Lobby/
  大厅入口、匹配票据

Room/
  自定义房间、准备、开始、房间详情

Battle/
  单进程权威战斗、移动、建造、升级、回收、Tick

Config/
  Luban bytes 加载和强类型 Tables

Shared/
  SheepServices、GameRuleService、共享记录类型
```

### 4.6 当前到目标的迁移边界

当前 `SheepServices` 是进程内服务定位器，适合 MVP。后续不要直接替换成复杂 DI，而是先把边界写清楚：

```text
AuthService
  过渡保留；逐步改为 PlayerEntity + SessionPlayerComponent + Data Scene。

CustomRoomService
  过渡保留；逐步改为 RoomEntity + Room System。

BattleService
  过渡保留；逐步改为 BattleEntity + BattlePlayerEntity + BattleBuildingEntity + BattleMonsterEntity。

GameRuleService
  统一访问 Luban 和规则常量，双端规则对齐的服务端入口。

ConfigSystem
  服务端配置加载，不让业务直接读取文件。
```

迁移顺序建议：

```text
1. 先在 Entity 项目定义 BattleEntity / BattlePlayerEntity / BattleBuildingEntity。
2. BattleService 内部从 BattleRecord 切到创建 Scene-owned Entity。
3. 把 Tick、农场产出、塔攻击拆成 BattleTickSystem / TowerAttackSystem。
4. 补 Battle Scene 配置，把单局战斗迁移到 Battle Scene 或 SubScene。
5. Handler 由直接调 Service 改为 Route 到 Battle Scene。
6. 最后再考虑 Room/Lobby/Data Scene 拆分。
```

### 4.7 Fantasy 框架使用原则

- `Fantasy.config` 放在直接引用 Fantasy 的 `Entity` 项目根目录，保留给 Source Generator 生成注册代码。
- 每个运行时 Entity/Component 必须属于正确的 Scene，不跨 Scene 长期持有运行时对象引用。
- 短生命周期实体优先用 Fantasy Entity 对象池创建，销毁父实体时自动清理子实体/组件。
- 定时逻辑走 Scene 的 TimerComponent，不在业务里手写后台线程或无限 Task。
- Scene 内部解耦用 Fantasy Event；跨 Scene/跨服再考虑 Address/Route/SphereEvent。
- 玩家从 Gate 到 Battle 的迁移，后续可研究 Roaming/Terminus，而不是手工复制一堆静态状态。
- Inner 协议用于服务器间通信，Outer 协议面向客户端，不要混用。

### 4.8 战斗状态机

建议将战斗状态机明确为：

```text
Created
-> Loading
-> Running
-> Settling
-> Finished
```

当前已落地：

```text
StartRoom
-> CreateFromRoom
-> Loading
-> 客户端 BattleSceneLoaded
-> 全员 loaded 后 Running
-> Move / Build / Upgrade / Recycle
-> Snapshot 返回权威状态
```

下一步补：

```text
Running
-> TrollSelecting        # 到达感染时间点
-> InfectionRunning      # 阵营对抗
-> Settling              # 胜负结算
-> Finished
```

不要让客户端决定阵营转换、胜负、资源产出、塔攻击、感染结果。客户端只能展示服务端快照和本地可交互提示。

## 5. 协议架构

协议源只改：

```text
Shared.Protocol/NetworkProtocol/
├── Outer/OuterMessage.proto
├── Inner/InnerMessage.proto
├── RouteType.Config
└── RoamingType.Config
```

生成目标：

```text
Server.Fantasy/Entity/Generate
Client.Unity/Assets/GameScripts/HotFix/GameProto/NetworkProtocol
```

生成命令：

```powershell
cd Tools.Protocol\ProtocolExportTool
dotnet .\Fantasy.ProtocolExportTool.dll export --silent
```

### 5.1 协议设计原则

- 客户端请求命名 `C2G_`，服务端响应 `G2C_`，服务端推送 `S2C_`。
- 已使用字段只追加，不删除、不复用编号。
- RPC 响应中带 `Success`、`Message` 和必要快照，便于 UI 处理失败。
- 高频状态不要长期依赖 RPC 轮询，逐步迁移为 S2C 推送或 Battle Scene 广播。
- 协议对象是传输 DTO，不在客户端/服务端直接塞复杂业务逻辑。

### 5.2 同步演进路线

当前：

```text
RoomUI 每秒 RoomDetail 轮询
BattleController 每 200ms BattleSnapshot RPC 轮询
移动/建造等 RPC 返回最新 Snapshot
```

建议演进：

```text
阶段 1:
  保持 RPC 快照，先补齐地图规则、建造规则、怪物/感染闭环。

阶段 2:
  在 Entity 项目补 Player/Room/Battle 领域实体，让 Handler 面向 Entity/System，而不是静态字典。

阶段 3:
  增加 S2C_RoomStatePush，替换房间轮询。

阶段 4:
  增加 Battle Scene + S2C_BattleSnapshot 广播，替换 200ms RPC 快照轮询。

阶段 5:
  Battle Scene 内做固定 Tick，按房间成员广播快照，必要时再做增量或 AOI。
```

## 6. 配置架构

Luban 是玩法数值的唯一来源。

```text
Tools.Config/GameConfig/
├── Datas/             # Excel 源表，只改这里
├── Defines/           # Luban 定义
├── CustomTemplate/    # TEngine 配置模板
├── gen_code_bin_to_project.bat
└── gen_code_bin_to_server.bat
```

导出目标：

```text
客户端:
Client.Unity/Assets/GameScripts/HotFix/GameProto/GameConfig
Client.Unity/Assets/AssetRaw/Configs/bytes

服务端:
Server.Fantasy/Hotfix/Config/GameConfig
Server.Fantasy/GameConfig
```

### 6.1 配置访问边界

客户端：

```text
UI/Controller
-> GameRuleService / ConfigSystem
-> Luban Tables
```

服务端：

```text
Handler
-> Service
-> SheepServices.Rules
-> ConfigSystem / Luban Tables
```

不要让 UI 到处直接查表，也不要让 Handler 直接写数值规则。

### 6.2 建议补齐的表

当前已有：

```text
game_rule
map
shop
shop_goods
monster
building
building_card
building_level
```

建议下一批：

```text
skill                 # 精灵/巨魔技能
buff                  # 感染、强化、减速、修理等
battle_phase          # 准备期、感染点、结算参数
spawn_rule            # 出生区、巨魔复活点、权重
drop_or_reward        # 局内奖励和结算
```

## 7. 地图架构

地图分两层：视觉层给 Unity，规则层给服务端和客户端预判。

```text
Tiled .tmx/.tsx/png
-> SuperTiled2Unity
-> Unity Map Prefab
-> YooAsset 加载显示

Tiled ObjectLayer / Custom Properties
-> 地图规则导出
-> Luban map 或独立 map_rule bytes
-> 服务端权威校验
-> 客户端建造/移动预提示
```

### 7.1 地图规则类型

建议固定 Tiled 对象/图层命名：

```text
no_move        # 移动阻挡
no_build       # 禁建
birth_elf      # 精灵出生区
birth_troll    # 巨魔出生/复活区
resource       # 资源点/商店/互动点
waypoint       # 怪物或巨魔 AI 路径点，后续预留
```

服务端必须使用同一份规则判断：

- 地图边界。
- 移动阻挡。
- 建筑占地。
- 禁建区。
- 出生区。
- 建筑与玩家距离。
- 巨魔占格和感染/攻击范围。

客户端本地判断只用于提示，结果以服务端返回为准。

## 8. 玩法模块架构

项目玩法核心是“精灵建造防线，巨魔感染扩散”。建议按领域模块拆：

```text
PlayerModule
  玩家阵营、血量、移动、死亡、复活、感染转换。

BuildModule
  建造、占格、升级、回收、供能、维修。

EconomyModule
  金币/木材/产出/消耗/商店。

CombatModule
  普攻、塔攻击、伤害、仇恨、攻击事件。

InfectionModule
  感染时间点、巨魔选择、精灵转化、最后精灵强化。

MapRuleModule
  阻挡、出生、禁建、距离、区域查询。

ResultModule
  胜负判断、结算、战绩、奖励。
```

MVP 不必真的创建这么多项目或程序集，但 `BattleService` 内部方法和记录类型要朝这些边界收敛。等某个模块复杂到影响阅读，再抽成独立 Service。

## 9. 数据持久化和部署演进

### 9.1 当前 MVP

```text
AuthService        内存账号/token/profile
CustomRoomService  内存房间
BattleService      内存战斗
ConfigSystem       本地 bytes
```

适合本地开发和双人联调。

### 9.2 下一阶段

```text
Redis:
  token、在线状态、房间索引、短期匹配票据。

DB:
  账号、玩家基础资料、战绩、背包、付费/商品记录。

Fantasy Scene:
  Gate Scene          连接和鉴权
  Lobby/Room Scene    大厅、匹配、房间
  Battle Scene        单局 Tick、快照、广播
```

迁移时优先迁 Battle，而不是先拆 Auth。战斗是状态量和 Tick 压力最大的地方，也最需要 Scene 隔离。

## 10. 开发优先级

### P0：把当前 MVP 稳住

- 双人房间和战斗反复测试。
- 建造规则双端一致：地图边界、禁建、阻挡、距离、占格。
- 客户端失败提示清晰，所有服务端拒绝都有可展示 Message。
- BattleController 只做小步拆分，避免大改破坏闭环。
- 客户端先引入战斗表现池，减少玩家、建筑、血条、弹道等对象的反复创建/销毁。
- 服务端先把 BattleRecord 方向的领域对象迁到 `Entity/Battle`，不要让 `Entity` 长期只放协议生成代码。

### P1：补核心玩法闭环

- 巨魔实体进入快照。
- 感染时间点和阵营转换。
- 防御塔寻敌、攻击事件、扣血。
- 玩家死亡、复活、精灵转巨魔。
- 胜负结算和 `S2C_BattleResultPush`。
- Battle Tick 改成 Fantasy TimerComponent/System 形态。

### P2：提升同步形态

- `S2C_RoomStatePush` 替换房间轮询。
- `S2C_BattleSnapshot` 或 Battle Scene 广播替换 200ms RPC 轮询。
- 客户端快照插值、攻击事件表现去重。
- Handler 从直接调静态 Service 逐步改为 Route 到对应 Scene/Entity。

### P3：商业化和长期工程

- 账号持久化、战绩、匹配、商店。
- 热更发布流程标准化。
- CI 跑协议生成、导表、客户端热更逻辑编译、服务端编译。
- Fantasy 多 Scene 部署、DB/Redis、Docker/配置中心/日志采集。

## 11. 关键规则清单

- 不手改协议生成文件，只改 `Shared.Protocol/NetworkProtocol`。
- 不手改 Luban 生成文件，只改 `Tools.Config/GameConfig/Datas` 和 `Defines`。
- 客户端 UI 不直接调网络。
- 服务端 Handler 不写业务规则。
- 服务端领域状态优先放 Fantasy Entity/Component，不长期堆在静态 Service 字典里。
- Fantasy Scene 是运行边界；跨 Scene 用 Route/Address/Roaming 等框架能力，不手写全局引用。
- 定时 Tick 走 Fantasy Timer/System，不手写散落的后台循环。
- 服务端是权威状态源，客户端本地判断只做体验提示。
- 客户端资产加载走 TEngine ResourceModule，运行时表现对象走战斗池，临时 C# 对象才走 MemoryPool。
- 地图视觉用 Unity prefab，地图规则要能被服务端读取。
- 改配置后必须同时导出客户端和服务端。
- 使用 HostPlayMode 或真实热更包时，资源和热更 DLL 修改后必须重新构建 AssetBundle。

## 12. 建议文档分工

```text
Docs/STATUS.md
  记录当前真实状态和验证命令。

Docs/PROJECT_ARCHITECTURE_PROPOSAL.md
  记录目标架构、分层和演进路线。

Docs/CLIENT_ARCHITECTURE.md
  记录客户端当前实现细节。

Docs/SERVER_ARCHITECTURE.md
  记录服务端当前实现细节。

Docs/PROTOCOL_DESIGN.md
  记录协议范围、生成方式、兼容规则。

Docs/CONFIG_PIPELINE.md
  记录 Luban 表和生成链路。

Docs/TILED_MAP_PIPELINE.md
  记录地图视觉和规则导出流程。
```

一句话总结：当前工程最合适的路线是“TEngine 承载客户端热更、资源缓存和运行时对象复用，Fantasy 承载服务端 Scene、Entity/Component 和权威 Tick，Luban/Tiled 做双端共享规则来源”。先把单进程 MVP 做扎实，但从现在开始让 Entity 真正承载领域模型，再把房间和战斗从轮询平滑迁到推送/Scene 广播。
