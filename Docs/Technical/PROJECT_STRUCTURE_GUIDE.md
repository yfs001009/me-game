# SheepBattle 项目结构解读

整理日期：2026-05-31。

本文用于新接手项目时快速判断“每个目录/文件是做什么的、哪些能改、哪些不要手改”。当前项目主体是 `SheepBattle`，`D:/my game` 是外层工作区目录。

## 1. 总览

`SheepBattle` 是一个商业化多人 PvP 非对称对抗手游工程。

核心链路：

```text
Unity/TEngine 客户端
-> Fantasy Unity Runtime TCP RPC
-> Fantasy .NET 9 服务端
-> Luban 配置 / Fantasy 协议 / Tiled 地图
```

当前可运行玩法链路：

```text
注册/登录
-> 大厅
-> 创建/加入房间
-> 准备/开始
-> 双人进入战斗
-> 服务端权威快照
-> 移动、建造、升级、回收
```

## 2. 根目录

```text
D:/my game/
├── .claude/
├── .codex/
├── openspec/
├── README.md
├── SheepBattle/
└── 聊天架构设计.xmind
```

| 文件/目录 | 用途 | 是否常改 |
|---|---|---|
| `.claude/` | Claude/项目辅助规则与技能文件，不属于游戏运行内容。 | 一般不改 |
| `.codex/` | Codex/项目辅助规则与技能文件，不属于游戏运行内容。 | 一般不改 |
| `openspec/` | OpenSpec 规格与变更工作区。 | 按变更流程修改 |
| `README.md` | 顶层项目说明，记录正式目录、服务端启动、协议生成、配置生成、客户端打开方式。 | 偶尔 |
| `SheepBattle/` | 正式项目主体，后续开发主要都在这里。 | 是 |
| `聊天架构设计.xmind` | 聊天架构脑图资料。 | 按需 |

## 3. SheepBattle 主目录

```text
SheepBattle/
├── README.md
├── Client.Unity/
├── Server.Fantasy/
├── Shared.Protocol/
├── Shared.Config/
├── Tools/
├── Tools.Config/
├── Tools.Map/
├── Tools.Protocol/
├── Deploy/
└── Docs/
```

| 目录/文件 | 用途 | 备注 |
|---|---|---|
| `README.md` | SheepBattle 项目说明，记录玩法方向、技术栈、目录、常用命令和开发规则。 | 新人先读 |
| `Client.Unity/` | Unity 2022.3.62f1c1 + TEngine 客户端工程。 | 客户端主战场 |
| `Server.Fantasy/` | Fantasy-Net .NET 9 服务端，`Main/Entity/Hotfix` 三项目结构。 | 服务端主战场 |
| `Shared.Protocol/` | Fantasy `.proto` 协议源。 | 只改源协议 |
| `Shared.Config/` | 共享配置预留目录，目前为空。 | 后续可放跨端配置源或说明 |
| `Tools/` | 外部工具本体，目前主要是 Luban 编译器和 UI 美术生成脚本。 | 工具目录 |
| `Tools.Config/` | Luban 配表工程，Excel 源表、定义、导出脚本都在这里。 | 配表主入口 |
| `Tools.Map/` | Tiled 地图编辑工作区。 | 地图主入口 |
| `Tools.Protocol/` | Fantasy 协议导出工具。 | 协议生成入口 |
| `Deploy/` | Docker/Nginx/环境配置预留，目前为空。 | 部署预留 |
| `Docs/` | 当前项目文档。 | 维护项目真实状态 |

## 4. 文档目录 `Docs/`

| 文件 | 用途 |
|---|---|
| `README.md` | 文档入口，给出推荐阅读顺序。 |
| `STATUS.md` | 当前真实状态：已落地功能、验证命令、风险和下一步。 |
| `GAME_DESIGN_AND_ARCHITECTURE.md` | 玩法与总体技术架构设计。 |
| `GAME_DESIGN_NUMBERS.md` | 首版玩法数值、建筑/幽灵/装备/经济/地图/外层系统规划。 |
| `FRAMEWORK_USAGE_AUDIT.md` | TEngine/Fantasy 框架使用体检，指出当前没用好的地方和改造方向。 |
| `NEXT_STEPS.md` | 短期开发顺序与测试重点。 |
| `CLIENT_ARCHITECTURE.md` | 客户端架构：启动链路、UI/MVE、战斗客户端、资源与池化规则。 |
| `SERVER_ARCHITECTURE.md` | 服务端架构：Fantasy 三项目结构、Handler/Service、Scene/Entity 演进方向。 |
| `PROTOCOL_DESIGN.md` | 协议源、生成命令、当前协议、下一批协议和兼容规则。 |
| `CONFIG_PIPELINE.md` | Luban 配置流程、源表、生成目标、访问方式、规则。 |
| `CONFIG_TABLE_DESIGN.md` | 配置表设计说明。 |
| `TILED_MAP_PIPELINE.md` | Tiled/SuperTiled2Unity 地图管线、层约定、导出规则。 |
| `PROJECT_STRUCTURE_GUIDE.md` | 本文件，项目结构解读。 |

## 5. 客户端 `Client.Unity/`

这是 Unity 客户端工程。用 Unity `2022.3.62f1c1` 打开此目录。

### 5.1 顶层工程文件

| 文件/目录 | 用途 |
|---|---|
| `Client.Unity.sln` / `UnityProject.sln` | IDE 解决方案文件。 |
| `Assembly-CSharp.csproj` / `Assembly-CSharp-Editor.csproj` | Unity 默认运行时代码和编辑器代码工程。 |
| `GameLogic.csproj` | 热更业务逻辑工程，对应 `Assets/GameScripts/HotFix/GameLogic`。 |
| `GameProto.csproj` | 协议和 Luban 生成代码工程，对应 `Assets/GameScripts/HotFix/GameProto`。 |
| `Launcher.csproj` | 首包启动器工程，对应 `Assets/Launcher`。 |
| `TEngine.Runtime.csproj` / `TEngine.Editor.csproj` | TEngine 框架工程。 |
| `YooAsset*.csproj` | YooAsset 资源系统相关工程。 |
| `UniTask*.csproj` | UniTask 异步库相关工程。 |
| `Packages/manifest.json` | Unity 包依赖配置，包含 Fantasy Unity、HybridCLR、SuperTiled2Unity、TextMeshPro、UGUI 等。 |
| `Packages/packages-lock.json` | Unity 包锁定文件。 |
| `ProjectSettings/` | Unity 项目设置。 |
| `Assets/` | 客户端所有源码、资源、热更 DLL、配置 bytes。 |
| `repowiki/` | TEngine/项目框架说明资料。参考用，不是运行时代码。 |
| `Library/` / `Temp/` / `obj/` / `bin/` | Unity 或 .NET 生成目录。不要手改、不要作为设计源。 |

### 5.2 `Assets/GameScripts`

| 文件/目录 | 用途 |
|---|---|
| `GameEntry.cs` | Unity 客户端入口，进入 TEngine/流程系统。 |
| `Procedure/` | TEngine 启动流程，负责资源初始化、下载、加载热更程序集、进入游戏。 |
| `ProcedureBase.cs` | 流程基类。 |
| `ProcedureLaunch.cs` | 启动流程。 |
| `ProcedureInitResources.cs` | 初始化 YooAsset/资源系统。 |
| `ProcedureLoadAssembly.cs` | 加载 HybridCLR 热更程序集。 |
| `ProcedureStartGame.cs` | 进入热更业务入口。 |
| `Editor/` | Unity 编辑器工具脚本。 |
| `Editor/SheepBattleUIPrefabBuilder.cs` | 构建/修复 SheepBattle UI prefab 的编辑器脚本。 |
| `Editor/SheepBattleRuntimeUIPrefabBuilder.cs` | 运行时 UI prefab 构建辅助。 |
| `Editor/SheepBattleUIListLayoutMigrator.cs` | UI 列表布局迁移工具。 |
| `Editor/SheepBattleChatUIPrefabMigrator.cs` | 聊天 UI prefab 迁移工具。 |
| `Editor/SheepBattleUIQualityFrameImporter.cs` | UI 品质框素材导入工具。 |
| `Editor/SheepBattleTiledTilesetImporter.cs` | Tiled tileset 导入辅助。 |
| `HotFix/GameLogic/` | 热更业务逻辑源码。 |
| `HotFix/GameProto/` | 协议和 Luban 配置生成代码。生成后使用，不建议手改。 |

### 5.3 热更业务 `Assets/GameScripts/HotFix/GameLogic`

| 文件/目录 | 用途 |
|---|---|
| `GameApp.cs` | 热更域入口，TEngine 加载后调用。 |
| `GameModule.cs` | 游戏模块访问入口。 |
| `GameLogic.asmdef` | Unity Assembly Definition，定义 GameLogic 热更程序集。 |
| `SingletonSystem/` | 简单单例系统。 |
| `Module/UIModule/` | UI 框架层，包含 `UIWindow`、`UIWidget`、资源加载接口、绑定组件等。 |
| `IEvent/` | UI 与业务层之间的命令接口。 |
| `IEvent/ILoginCommand.cs` | 登录/注册/昵称相关命令接口。 |
| `IEvent/ILobbyCommand.cs` | 大厅、房间、匹配相关命令接口。 |
| `IEvent/IBattleCommand.cs` | 战斗输入、建造、升级、回收等命令接口。 |
| `IEvent/ILoginUI.cs` | 登录 UI 相关接口。 |
| `SheepBattle/App/SheepBattleApp.cs` | SheepBattle 业务启动入口，注册协议、命令、初始化网络并打开初始 UI。 |
| `SheepBattle/Network/SheepNetworkService.cs` | Fantasy Runtime TCP Session 封装，客户端 RPC 统一入口。 |
| `SheepBattle/Common/CommonNoticeService.cs` | 通用提示/弹窗服务。 |
| `SheepBattle/Config/GameRuleService.cs` | 客户端游戏规则配置访问封装。 |
| `SheepBattle/Login/` | 登录模块：登录状态、登录控制器、账号流程。 |
| `SheepBattle/Lobby/` | 大厅/房间模块：大厅状态、房间列表、创建/加入/准备/开始。 |
| `SheepBattle/Battle/BattleController.cs` | 当前战斗客户端核心：地图加载、快照轮询、WASD 移动、建造预览、建造/升级/回收命令。后续应拆分。 |
| `SheepBattle/Battle/TiledMapData.cs` | Tiled JSON 地图数据结构。 |
| `SheepBattle/Battle/TiledMapLoader.cs` | Tiled JSON 地图加载器，prefab 不存在时用于 fallback。 |
| `SheepBattle/Asset/` | 资产/道具/货币视图与控制逻辑。 |
| `SheepBattle/Character/` | 角色列表、选择、角色状态视图。 |
| `SheepBattle/Chat/` | 聊天模块，含客户端推送 Handler 和聊天状态。 |
| `SheepBattle/Loadout/LoadoutService.cs` | 战前卡组/角色配置服务，目前用于后续“精灵 6 卡组”方向。 |
| `SheepBattle/Lottery/` | 抽奖模块。 |
| `SheepBattle/Mail/` | 邮件模块。 |
| `SheepBattle/Reward/RewardDisplayService.cs` | 奖励展示服务。 |
| `SheepBattle/Shop/` | 商店模块。 |
| `SheepBattle/Social/` | 好友/社交模块。 |
| `SheepBattle/Task/` | 任务模块。 |
| `SheepBattle/Event/` | 各模块 ViewChanged / StatusChanged 事件，Model 更新后通知 UI 刷新。 |
| `UI/` | 具体 UI 窗口脚本。 |
| `UI/SplashUI/` | 启动画面。 |
| `UI/VersionCheckUI/` | 版本检查界面。 |
| `UI/LoadingUI/` | 加载界面。 |
| `UI/LoginUI/` | 登录界面。 |
| `UI/RegisterUI/` | 注册界面。 |
| `UI/NicknameUI/` | 设置昵称界面。 |
| `UI/LobbyUI/` | 大厅界面。 |
| `UI/MatchQueueUI/` | 匹配队列界面。 |
| `UI/CreateRoomUI/` | 创建房间界面。 |
| `UI/RoomListUI/` | 房间列表界面。 |
| `UI/RoomPasswordUI/` | 房间密码界面。 |
| `UI/RoomUI/` | 房间详情和玩家槽位界面。 |
| `UI/BattleMainUI/` | 战斗主界面，建筑卡片、战斗状态等。 |
| `UI/BagUI/` | 背包界面。 |
| `UI/CharacterUI/` | 角色界面。 |
| `UI/ChatUI/` | 聊天界面。 |
| `UI/CommonNoticeUI/` | 通用提示界面。 |
| `UI/LotteryUI/` | 抽奖界面。 |
| `UI/MailUI/` | 邮件界面。 |
| `UI/RewardPopupUI/` | 奖励弹窗。 |
| `UI/ShopUI/` | 商店界面。 |
| `UI/SocialUI/` | 社交界面。 |

客户端业务规则：UI 不直接调网络，UI 通过 `GameEvent.Get<IxxxCommand>()` 发命令；Controller 调 Service；Model/ViewModel 发事件；UI 监听事件刷新。

### 5.4 生成代码 `Assets/GameScripts/HotFix/GameProto`

| 文件/目录 | 用途 |
|---|---|
| `GameProto.asmdef` | GameProto 程序集定义。 |
| `NetworkProtocol/` | Fantasy 协议生成代码。来源是 `Shared.Protocol/NetworkProtocol`。不要手改。 |
| `NetworkProtocol/OuterMessage.cs` | 客户端可见外网协议消息类型。 |
| `NetworkProtocol/OuterOpcode.cs` | 外网协议 opcode。 |
| `NetworkProtocol/RouteType.cs` | Fantasy 路由类型生成结果。 |
| `NetworkProtocol/GameProtoFantasyRegistrar.cs` | 协议注册器，客户端启动时注册。 |
| `NetworkProtocol/NetworkProtocolHelper.cs` | 协议辅助代码。 |
| `GameConfig/` | Luban 生成的配置表 C# 代码。来源是 `Tools.Config/GameConfig/Datas`。不要手改。 |
| `GameConfig/Tables.cs` | Luban 表集合入口。 |
| `GameConfig/asset/` | 货币、物品配置。 |
| `GameConfig/battle/` | 地图、建筑、建筑卡、建筑等级、局内商店、怪物配置。 |
| `GameConfig/common/` | 通用规则配置。 |
| `GameConfig/open/` | 功能开放配置。 |
| `GameConfig/role/` | 角色配置。 |
| `GameConfig/shop/` | 外层商店配置。 |
| `GameConfig/task/` | 任务配置。 |
| `LubanLib/` | Luban 二进制反序列化基础库。 |
| `ConfigSystem.cs` | 客户端配置系统生成/模板代码。 |
| `ExternalTypeUtil.cs` | Luban 外部类型辅助。 |

### 5.5 启动器 `Assets/Launcher`

| 文件/目录 | 用途 |
|---|---|
| `Launcher.asmdef` | Launcher 程序集定义。 |
| `Scripts/LauncherMgr.cs` | 首包启动管理。 |
| `Scripts/LoadTipsUI.cs` | 加载提示 UI 脚本。 |
| `Scripts/LoadUpdateUI.cs` | 资源更新 UI 脚本。 |
| `Scripts/LoadText.cs` | 加载文本辅助。 |
| `Scripts/UIBase.cs` | Launcher UI 基类。 |
| `Scripts/DisStripCode.cs` | 防止代码裁剪的引用保留脚本。 |
| `Resources/UIWindow/LoadTipsUI.prefab` | Launcher 加载提示 prefab。 |
| `Resources/UIWindow/LoadUpdateUI.prefab` | Launcher 更新 prefab。 |

### 5.6 资源 `Assets/AssetRaw`

| 目录/文件 | 用途 |
|---|---|
| `Actor/` | 角色/建筑/表现对象 prefab。目前有临时 `Cube.prefab`。 |
| `Audios/` | 音频资源预留。 |
| `Configs/about.txt` | 配置资源说明。 |
| `Configs/bytes/` | Luban 导出的客户端配置 bytes。生成产物，不手改。 |
| `DLL/` | HybridCLR 热更 DLL bytes，包括 `GameLogic.dll.bytes`、`GameProto.dll.bytes` 和补充元数据 DLL。 |
| `Effects/` | 特效资源，例如攻击范围图。 |
| `Fonts/` | 字体资源。 |
| `MapPrefabs/` | SuperTiled2Unity 导入/整理后的运行时地图 prefab。 |
| `Maps/` | Tiled 导出的 JSON fallback 地图。 |
| `MapTiles/` | 旧 JSON 地图预览链路使用的地图图块 PNG。 |
| `Materials/` | 材质资源。 |
| `Scenes/` | Unity 场景资源。 |
| `Shaders/` | Shader 资源。 |
| `TiledMaps/` | Unity 编辑器导入用 `.tmx/.tsx/png`。 |
| `UI/` | UI prefab 和项目 UI 美术资源。 |
| `UI/*.prefab` | 各业务 UI 的正式 prefab。 |
| `UI/Art/` | 欧美卡通风 UI 图、按钮、面板、图标、品质框等。 |
| `UIRaw/` | 原始 UI 图集/切图，供打图集或迁移使用。 |

所有 `.meta` 文件是 Unity 资源 GUID 文件。资源移动/改名时应通过 Unity 或保持 `.meta` 一起移动，避免引用丢失。

## 6. 服务端 `Server.Fantasy/`

```text
Server.Fantasy/
├── Server.sln
├── Main/
├── Entity/
└── Hotfix/
```

| 文件/目录 | 用途 |
|---|---|
| `Server.sln` | 服务端解决方案。 |
| `Main/` | 进程入口，启动 Fantasy，日志初始化，加载程序集。 |
| `Entity/` | Fantasy Entity/Component 定义、协议生成、Fantasy.config。 |
| `Hotfix/` | MessageRPC Handler、业务 Service、配置系统、后续 System。 |

### 6.1 `Main/`

| 文件 | 用途 |
|---|---|
| `Main.csproj` | 服务端主进程工程。 |
| `Fantasy.NLog.csproj` | Fantasy NLog 集成工程。 |
| `Program.cs` | 服务端启动入口。 |
| `NLog.cs` | 日志接入代码。 |
| `NLog.config` | NLog 配置。 |
| `Fantasy.config` | Main 工程的 Fantasy 配置文件。 |
| `Properties/launchSettings.json` | IDE 启动配置。 |

### 6.2 `Entity/`

| 文件/目录 | 用途 |
|---|---|
| `Entity.csproj` | Entity 工程，直接引用 Fantasy，承载 Entity/Component 和生成协议。 |
| `AssemblyHelper.cs` | Fantasy 程序集辅助/加载相关代码。 |
| `Fantasy.config` | 服务端 Scene/Process/World 等 Fantasy 配置，当前 Gate 外网端口为 `20000`。 |
| `Generate/NetworkProtocol/` | Fantasy 协议生成代码。来源是 `Shared.Protocol/NetworkProtocol`。不要手改。 |
| `Generate/NetworkProtocol/OuterMessage.cs` | 外网协议消息生成代码。 |
| `Generate/NetworkProtocol/OuterOpcode.cs` | 外网 opcode。 |
| `Generate/NetworkProtocol/InnerMessage.cs` | 内网协议消息生成代码。 |
| `Generate/NetworkProtocol/InnerOpcode.cs` | 内网 opcode。 |
| `Generate/NetworkProtocol/RouteType.cs` | 路由类型生成代码。 |
| `Player/PlayerEntities.cs` | 玩家领域 Entity/Component 定义。 |
| `Room/RoomEntities.cs` | 房间领域 Entity/Component 定义。 |
| `Battle/BattleEntities.cs` | 战斗领域 Entity/Component 定义。 |
| `Asset/AssetAmounts.cs` | 资产数量结构/模型。 |
| `Asset/AssetEntities.cs` | 资产领域 Entity/Component 定义。 |
| `Character/CharacterEntities.cs` | 角色领域 Entity/Component 定义。 |
| `Chat/ChatEntities.cs` | 聊天领域 Entity/Component 定义。 |
| `Feature/FeatureEntities.cs` | 功能开放领域 Entity/Component 定义。 |
| `Lottery/LotteryEntities.cs` | 抽奖领域 Entity/Component 定义。 |
| `Mail/MailEntities.cs` | 邮件领域 Entity/Component 定义。 |
| `Shop/ShopEntities.cs` | 商店领域 Entity/Component 定义。 |
| `Social/SocialEntities.cs` | 社交领域 Entity/Component 定义。 |
| `Task/TaskEntities.cs` | 任务领域 Entity/Component 定义。 |

### 6.3 `Hotfix/`

| 文件/目录 | 用途 |
|---|---|
| `Hotfix.csproj` | 热更业务工程。 |
| `Shared/SheepServices.cs` | 过渡期服务聚合入口，集中持有 Auth/Battle/Room 等内存服务。 |
| `Shared/GameRuleService.cs` | 服务端游戏规则访问封装。 |
| `Config/ConfigSystem.cs` | 服务端 Luban 配置加载入口。 |
| `Config/GameConfig/` | Luban 生成的服务端配置代码。生成产物，不手改。 |
| `Auth/Handler/*.cs` | 注册、登录、设置昵称等 RPC Handler。 |
| `Auth/Service/AuthService.cs` | 账号、token、玩家资料的过渡内存服务。 |
| `Lobby/Handler/*.cs` | 大厅首页、开始匹配等 RPC Handler。 |
| `Lobby/Service/MatchService.cs` | 匹配票据和匹配状态服务。 |
| `Room/Handler/*.cs` | 创建/加入/离开房间、详情、准备、开始等 RPC Handler。 |
| `Room/Handler/RoomSceneAddressHandlers.cs` | 房间 Scene/Address 通信相关 Handler。 |
| `Room/Service/CustomRoomService.cs` | 房间内存状态、准备状态、开始战斗逻辑。 |
| `Room/RoomSceneGateway.cs` | 房间 Scene 网关/过渡入口。 |
| `Battle/Handler/*.cs` | 战斗加载完成、快照、移动、建造、升级、回收等 RPC Handler。 |
| `Battle/Handler/BattleSceneAddressHandlers.cs` | 战斗 Scene/Address 通信相关 Handler。 |
| `Battle/Service/BattleService.cs` | 当前单进程战斗权威状态：玩家、建筑、资源、Tick、加载状态、运行状态。 |
| `Battle/BattleSceneGateway.cs` | 战斗 Scene 网关/过渡入口。 |
| `Asset/` | 资产快照、使用道具、奖励、道具容器、资产转移上下文。 |
| `Character/` | 角色列表与选择服务。 |
| `Chat/` | 聊天历史、发送消息、聊天常量和服务。 |
| `Feature/Service/FeatureGateService.cs` | 功能开放判断服务。 |
| `Lottery/` | 抽奖请求和抽奖服务。 |
| `Mail/` | 邮件列表、读取、领取附件、邮件记录和服务。 |
| `Shop/` | 外层商店列表、购买商品和商店服务。 |
| `Social/` | 关注/社交列表和社交服务。 |
| `Task/` | 外层任务列表、领奖和任务服务。 |

服务端当前仍有较多过渡内存 Service。长期目标是把 Room/Battle/Player 等状态逐步迁入 Fantasy Scene/Entity/System。

## 7. 协议 `Shared.Protocol/`

```text
Shared.Protocol/NetworkProtocol/
├── Outer/OuterMessage.proto
├── Inner/InnerMessage.proto
├── RouteType.Config
├── RoamingType.Config
├── OpCode.Cache
└── .DS_Store
```

| 文件 | 用途 |
|---|---|
| `Outer/OuterMessage.proto` | 客户端和服务端之间的外网协议源。新增 C2G/G2C/S2C 消息主要改这里。 |
| `Inner/InnerMessage.proto` | 服务端内部通信协议源。后续多 Scene/多进程会用到。 |
| `RouteType.Config` | Fantasy 路由类型配置。 |
| `RoamingType.Config` | Fantasy Roaming 类型配置。 |
| `OpCode.Cache` | 协议导出工具的 opcode 缓存。谨慎处理，避免 opcode 混乱。 |
| `.DS_Store` | macOS 目录元数据，无业务意义。 |

生成命令：

```powershell
cd SheepBattle/Tools.Protocol/ProtocolExportTool
dotnet .\Fantasy.ProtocolExportTool.dll export --silent
```

生成目标：

```text
Server.Fantasy/Entity/Generate/NetworkProtocol
Client.Unity/Assets/GameScripts/HotFix/GameProto/NetworkProtocol
```

规则：只改 `Shared.Protocol/NetworkProtocol` 下的源文件，不手改生成代码。

## 8. 配表 `Tools.Config/`

```text
Tools.Config/GameConfig/
├── Datas/
├── Defines/
├── CustomTemplate/
├── luban.conf
├── gen_code_bin_to_project.bat
├── gen_code_bin_to_server.bat
└── *.py
```

| 文件/目录 | 用途 |
|---|---|
| `GameConfig/luban.conf` | Luban 导表配置。 |
| `GameConfig/Datas/` | Excel 源表，只改这里和 `Defines`。 |
| `GameConfig/Datas/__beans__.xlsx` | Luban bean 结构定义。 |
| `GameConfig/Datas/__enums__.xlsx` | Luban 枚举定义。 |
| `GameConfig/Datas/__tables__.xlsx` | Luban 表注册定义。 |
| `GameConfig/Datas/game_rule.xlsx` | 通用游戏规则。 |
| `GameConfig/Datas/map.xlsx` | 地图配置：地图资源名、玩法、人数等。 |
| `GameConfig/Datas/building.xlsx` | 建筑基础配置。 |
| `GameConfig/Datas/building_card.xlsx` | 建筑卡配置。 |
| `GameConfig/Datas/building_level.xlsx` | 建筑等级配置。 |
| `GameConfig/Datas/monster.xlsx` | 怪物配置。 |
| `GameConfig/Datas/battle_shop.xlsx` | 局内商店配置。 |
| `GameConfig/Datas/battle_shop_goods.xlsx` | 局内商店商品配置。 |
| `GameConfig/Datas/character.xlsx` | 角色配置。 |
| `GameConfig/Datas/currency.xlsx` | 货币配置。 |
| `GameConfig/Datas/item.xlsx` | 道具配置。 |
| `GameConfig/Datas/open_feature.xlsx` | 功能开放配置。 |
| `GameConfig/Datas/shop.xlsx` | 外层商店配置。 |
| `GameConfig/Datas/shop_goods.xlsx` | 外层商店商品配置。 |
| `GameConfig/Datas/task.xlsx` | 任务配置。 |
| `GameConfig/Defines/builtin.xml` | Luban 内置/扩展类型定义。 |
| `GameConfig/CustomTemplate/` | 自定义导出模板。 |
| `gen_code_bin_to_project.bat/.sh` | 导出配置代码和 bytes 到 Unity 客户端。 |
| `gen_code_bin_to_project_lazyload.bat/.sh` | 客户端懒加载版本导出脚本。 |
| `gen_code_bin_to_server.bat/.sh` | 导出配置代码和 bytes 到服务端。 |
| `add_outgame_asset_tables.py` | 外层资产表辅助脚本。 |
| `fix_character_config_text.py` | 修复角色配置文本的辅助脚本。 |
| `fix_map_config_text.py` | 修复地图配置文本的辅助脚本。 |
| `inspect_config_tables.py` | 检查配置表的辅助脚本。 |
| `update_shop_task_tables.py` | 更新商店/任务表的辅助脚本。 |

生成命令：

```powershell
cd SheepBattle/Tools.Config/GameConfig
.\gen_code_bin_to_project.bat
.\gen_code_bin_to_server.bat
```

规则：不要手改客户端/服务端里的 Luban 生成代码和 bytes；修改 Excel 后要同时导出客户端和服务端。

## 9. 工具 `Tools/`

| 文件/目录 | 用途 |
|---|---|
| `generate_ui_art.py` | UI 美术资源生成/处理脚本。 |
| `Luban/Luban.dll` | Luban 编译器主程序。 |
| `Luban/Luban.exe` | Luban Windows 可执行入口。 |
| `Luban/*.dll` / `*.deps.json` / `*.runtimeconfig.json` | Luban 运行依赖。 |
| `Luban/Templates/` | Luban 官方模板。除非升级/定制 Luban，否则不要改。 |

## 10. 地图 `Tools.Map/`

```text
Tools.Map/
├── README.md
├── OpenTiled.bat
├── ExportMaps.bat
├── Tiled/
└── TiledProject/
```

| 文件/目录 | 用途 |
|---|---|
| `README.md` | 地图工具说明。 |
| `OpenTiled.bat` | 打开 Tiled 编辑器并加载项目。 |
| `ExportMaps.bat` | 批量把 `.tmx` 导出为 Unity 运行时 JSON。 |
| `Tiled/` | Tiled 工具本体，包含 `tiled.exe`、DLL、插件、示例。不要作为游戏资源打包。 |
| `TiledProject/SheepBattle.tiled-project` | Tiled 项目文件。 |
| `TiledProject/SheepBattle.tiled-session` | Tiled 会话状态。 |
| `TiledProject/Maps/battle_map_1.tmx` | 当前战斗地图源文件。 |
| `TiledProject/Tilesets/dikuai.tsx` | 当前地图图块集源文件。 |

地图正式运行资源会进入：

```text
Client.Unity/Assets/AssetRaw/TiledMaps
Client.Unity/Assets/AssetRaw/MapPrefabs
Client.Unity/Assets/AssetRaw/Maps
Client.Unity/Assets/AssetRaw/MapTiles
```

推荐视觉链路：`Tiled .tmx/.tsx/png -> SuperTiled2Unity -> Unity prefab -> YooAsset -> BattleController 加载 prefab`。

JSON 链路保留为 fallback 和规则导出参考。

## 11. 协议工具 `Tools.Protocol/`

| 文件/目录 | 用途 |
|---|---|
| `ProtocolExportTool/ExporterSettings.json` | 协议导出路径配置：源协议、服务端生成目录、客户端生成目录。 |
| `ProtocolExportTool/Fantasy.ProtocolExportTool.dll` | Fantasy 协议导出工具主程序。 |
| `ProtocolExportTool/Fantasy.ProtocolExportTool` | 非 Windows 可执行入口。 |
| `ProtocolExportTool/Run.bat` / `Run.sh` | 协议生成脚本。 |
| `ProtocolExportTool/*.dll` | 协议工具运行依赖。 |
| `ProtocolExportTool/*.deps.json` / `*.runtimeconfig.json` | .NET 运行配置。 |
| `ProtocolExportTool/runtimes/` | 工具运行时依赖。 |
| `ProtocolExportTool/zh-Hans/` | 工具中文资源。 |

## 12. 不要手改的内容

| 内容 | 原因 |
|---|---|
| `Server.Fantasy/Entity/Generate/NetworkProtocol` | 协议生成代码，改源 `.proto` 后重新生成。 |
| `Client.Unity/Assets/GameScripts/HotFix/GameProto/NetworkProtocol` | 客户端协议生成代码，改源 `.proto` 后重新生成。 |
| `Client.Unity/Assets/GameScripts/HotFix/GameProto/GameConfig` | Luban 生成配置代码，改 Excel 后重新导表。 |
| `Server.Fantasy/Hotfix/Config/GameConfig` | 服务端 Luban 生成配置代码，改 Excel 后重新导表。 |
| `Client.Unity/Assets/AssetRaw/Configs/bytes` | Luban 导出的配置 bytes，改 Excel 后重新导表。 |
| `Client.Unity/Assets/AssetRaw/DLL/*.dll.bytes` | HybridCLR 热更 DLL 产物，改代码后重新构建。 |
| `Library/` / `Temp/` / `obj/` / `bin/` | 本地生成目录。 |
| Unity `.meta` | Unity GUID 文件，不要随意删除；移动资源时跟随资源一起移动。 |
| 第三方包源码 | `Packages/YooAsset`、`Packages/UniTask`、`Assets/TEngine` 等，除非明确要升级或修框架。 |

## 13. 常用工作流

### 改客户端业务

```text
改 Assets/GameScripts/HotFix/GameLogic
-> dotnet build Client.Unity/GameLogic.csproj --no-restore
-> Unity EditorMode 验证
```

### 改服务端业务

```text
改 Server.Fantasy/Hotfix 或 Entity
-> dotnet build Server.Fantasy/Server.sln --no-restore
-> dotnet run --project Server.Fantasy/Main/Main.csproj --framework net9.0
```

### 改协议

```text
改 Shared.Protocol/NetworkProtocol
-> cd Tools.Protocol/ProtocolExportTool
-> dotnet .\Fantasy.ProtocolExportTool.dll export --silent
-> 编译客户端 GameLogic/GameProto 和服务端
```

### 改配置

```text
改 Tools.Config/GameConfig/Datas 或 Defines
-> cd Tools.Config/GameConfig
-> .\gen_code_bin_to_project.bat
-> .\gen_code_bin_to_server.bat
-> 编译客户端和服务端
```

### 改地图

```text
双击 Tools.Map/OpenTiled.bat
-> 编辑 TiledProject/Maps/*.tmx
-> ExportMaps.bat 导出 JSON fallback
-> Unity 内用 SuperTiled2Unity 导入 prefab
-> 确认 map.xlsx 的 mapAsset 与 prefab/json 文件名一致
```

## 14. 当前架构判断

当前项目已经从原型收敛到正式工程结构，但仍处于 MVP 到框架化演进阶段。

已比较清晰的边界：

```text
客户端：UI -> Command -> Controller -> Service/Model -> Event -> UI
服务端：MessageRPC Handler -> 过渡 Service -> 内存权威状态
协议：Shared.Protocol 源文件统一生成到两端
配置：Tools.Config Excel/Luban 统一生成到两端
地图：Tiled 源地图 + Unity prefab 优先 + JSON fallback
```

需要继续收敛的方向：

```text
1. 服务端 Room/Battle/Player 状态逐步迁入 Fantasy Entity/Scene/System。
2. 战斗快照从 200ms RPC 轮询升级为服务端推送。
3. 房间状态从 RoomUI 轮询升级为 S2C_RoomStatePush。
4. BattleController 拆分为同步、地图、实体表现、建造、输入、池化等子模块。
5. 地图规则完整接入服务端宽高、阻挡、禁建、出生区、怪物点和商店点。
```
