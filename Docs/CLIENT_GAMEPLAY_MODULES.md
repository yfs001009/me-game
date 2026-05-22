# 客户端业务模块划分

本文档从玩家视角划分客户端业务模块，用来指导 UI、事件、资源、配置和代码目录命名。TEngine 的 `UIModule`、`GameEvent`、`ResourceModule`、`ConfigSystem` 是技术模块；玩家实际感知的是登录、大厅、房间、战斗等业务模块。业务开发时优先按业务模块归类，再选择对应的 TEngine 技术能力。

## 1. 总原则

- 技术模块不等于业务模块：不要把所有事件都放成“UI 事件”，也不要只按资源类型组织功能。
- 业务模块按玩家流程划分：登录 -> 大厅 -> 房间 -> 战斗 -> 结算。
- UI 是 View，Model 保存状态，Event 通知变化，Controller/Service 执行业务动作。
- 事件名、UI 名、资源名、配置表名都应能看出属于哪个业务模块。

## 2. 业务模块地图

| 玩家视角模块 | 范围 | 典型 UI | 典型事件 | 典型资源 | 典型配置 |
| --- | --- | --- | --- | --- | --- |
| Login 登录 | 登录、注册、断线重登、账号状态 | `LoginUI`, `RegisterUI` | `LoginStatusChangedEvent` | 登录背景、Logo、按钮图 | 登录公告、服务器列表 |
| Lobby 大厅 | 玩家资料、入口聚合、匹配、房间列表、商店、好友、邮件、任务 | `LobbyUI`, `RoomListUI`, `ShopUI`, `FriendUI`, `MailUI`, `TaskUI` | `LobbyViewChangedEvent`, `ShopGoodsChangedEvent`, `FriendListChangedEvent` | 大厅背景、头像、入口图标 | 商品表、任务表、活动表 |
| Room 房间 | 创建房间、加入房间、玩家槽位、准备、房间设置 | `RoomUI`, `RoomSlotWidget`, `RoomSettingUI` | `RoomViewChangedEvent`, `RoomPlayerReadyChangedEvent` | 房间槽位、地图缩略图 | 地图表、模式表 |
| Battle 战斗 | 地图、单位、建造、技能、感染、同步、胜负 | `BattleMainUI`, `BuildPanelUI`, `BuildingInfoUI`, `SkillUI`, `BattleResultUI` | `BuildPreviewChangedEvent`, `BuildingPlacedEvent`, `CombatStateChangedEvent` | 战斗场景、单位、建筑、技能图标、特效 | 建筑表、单位表、技能表、Buff表 |
| Common 通用 | Toast、加载中、确认框、错误提示、通用道具展示 | `ToastUI`, `LoadingUI`, `ConfirmUI`, `ItemWidget` | `ToastShowEvent`, `LoadingStateChangedEvent` | 通用按钮、通用图标、字体 | 错误码表、本地化表 |

## 3. 事件归属

事件按业务模块命名，不按技术模块命名。带数据的业务事件优先使用类事件：无 `I` 前缀，实现 `TEngine.IEvent`，通过 `GameEvent.Send(new XxxEvent(...))` 派发，通过 `AddUIEvent<XxxEvent>(OnXxx)` 或 `GameEvent.AddEventListener<XxxEvent>` 监听。

推荐：

```text
LoginStatusChangedEvent
LobbyViewChangedEvent
LobbyStatusChangedEvent
RoomViewChangedEvent
ShopGoodsChangedEvent
FriendListChangedEvent
BuildPreviewChangedEvent
CombatStateChangedEvent
```

示例归属：

```text
商店打开、商品刷新、购买结果         -> Lobby/Shop
好友列表刷新、好友申请、好友上线     -> Lobby/Friend
房间创建、加入、玩家准备、房主变更   -> Room
建造预览、放置建筑、升级、拆除       -> Battle/Build
伤害、感染、Buff、死亡、胜负         -> Battle/Combat
```

UI 内监听事件必须使用 `AddUIEvent<TEvent>`，让 TEngine 在窗口销毁时自动清理。非 UI 类监听事件时使用 `GameEventMgr` 或明确释放。接口事件只保留给无数据的信号类场景，避免把复杂数据塞进接口方法参数里。

## 4. UI 归属

UI 类仍然继承 `UIWindow/UIWidget`，但命名和目录按业务模块组织。

建议目录：

```text
Assets/GameScripts/HotFix/GameLogic/UI/
├── Login/
├── Lobby/
├── Room/
├── Battle/
└── Common/
```

Widget 也跟随业务模块：

```text
Lobby/ShopItemWidget
Lobby/FriendItemWidget
Room/RoomPlayerSlotWidget
Battle/BuildItemWidget
Battle/BuffIconWidget
```

Prefab 放在 `Assets/AssetRaw/UI/` 下时，资源名应和窗口或 Widget 类名保持一致，方便 `GameModule.UI.ShowUIAsync<T>()` 和 `CreateWidgetByType<T>()` 寻址。

## 5. 资源归属

资源按业务模块和用途组织，加载时仍走 TEngine `ResourceModule`。

建议资源目录：

```text
Assets/AssetRaw/UI/Login/
Assets/AssetRaw/UI/Lobby/
Assets/AssetRaw/UI/Room/
Assets/AssetRaw/UI/Battle/
Assets/AssetRaw/Actor/Battle/
Assets/AssetRaw/Effects/Battle/
Assets/AssetRaw/Audio/Lobby/
Assets/AssetRaw/Audio/Battle/
```

使用规则：

- UI 图标、按钮图：`Image.SetSprite` / `SetSubSprite`
- UI prefab、战斗表现 prefab：`LoadGameObjectAsync`
- 配置、文本、ScriptableObject：`LoadAssetAsync` 后配对 `UnloadAsset`
- 不直接使用 `Resources.Load`

## 6. 配置归属

配置按业务模块命名，业务层通过封装访问，不让 UI 直接散落调用 Luban 表。

示例：

```text
Lobby: activity, shop, task
Room: game_mode, map
Battle: building, unit, skill, buff
Common: error_code, localization
```

访问建议：

```text
ConfigService.GetShopItem(id)
ConfigService.GetGameMode(id)
ConfigService.GetBuilding(id)
ConfigService.GetSkill(id)
```

## 7. 当前项目落点

当前已落地的最小模块：

```text
Login:
  UI: LoginUI
  Model: LoginModel
  Event: LoginStatusChangedEvent
  Controller: LoginController

Lobby:
  UI: LobbyUI
  Model: LobbyModel
  Event: LobbyViewChangedEvent, LobbyStatusChangedEvent
  Controller: LobbyController

Room:
  UI: RoomUI
  Model: 目前暂存在 LobbyModel.CurrentRoom，后续房间复杂后独立 RoomModel

Battle:
  UI: BattleMainUI
  Controller: BattleController
  后续建造、技能、同步、结算应拆成 Battle/Build/Combat/Sync 子模块
```
