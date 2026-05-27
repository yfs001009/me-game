# 客户端架构

最后整理：2026-05-26。

本文记录 Unity/TEngine 客户端当前结构和后续改造方向。

## 1. 技术栈

```text
Unity 2022.3.62f1c1
TEngine
YooAsset
HybridCLR
Fantasy Unity Runtime 2025.2.1402
Luban
SuperTiled2Unity 2.3.0
```

## 2. 启动链路

热更入口：

```text
Assets/GameScripts/HotFix/GameLogic/GameApp.cs
Assets/GameScripts/HotFix/GameLogic/SheepBattle/App/SheepBattleApp.cs
```

启动流程：

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

`GameApp` 是热更域入口，`SheepBattleApp` 是项目业务入口。不要把业务初始化反向写进 TEngine Launcher。

## 3. TEngine 模式

```text
EditorMode
  编辑器快速验证业务，不加载真实热更 dll bytes。

OfflinePlayMode / HostPlayMode
  走 YooAsset + HybridCLR，必须重新构建热更 DLL 和 AssetBundle。
```

验证业务 UI 优先用 `EditorMode`。验证热更包、资源包和线上流程时才用 HostPlayMode。

## 4. 业务分层

```text
UIWindow/UIWidget
  展示数据、收集输入、监听事件。

Command Interface
  ILoginCommand / ILobbyCommand / IBattleCommand
  UI 通过 GameEvent.Get<T>() 发命令。

Controller
  编排业务流程，调用 Service，更新 Model。

Model/ViewModel
  保存状态，构造 UI 所需数据，发送 IEvent 状态事件。

Service
  封装外部系统：Fantasy 网络、Luban 配置、资源加载、公共提示。
```

UI 不直接调用网络。按钮示例：

```csharp
GameEvent.Get<ILobbyCommand>()?.OnStartMatch();
```

UI 状态刷新示例：

```csharp
AddUIEvent<LobbyViewChangedEvent>(OnLobbyViewChanged);
```

## 5. 当前 UI

```text
SplashUI
VersionCheckUI
LoadingUI
LoginUI
RegisterUI
NicknameUI
LobbyUI
MatchQueueUI
CreateRoomUI
RoomListUI
RoomPasswordUI
RoomUI
RoomPlayerSlot
BattleMainUI
CommonNoticeUI
```

已有 prefab：

```text
Assets/AssetRaw/UI/LoginUI.prefab
Assets/AssetRaw/UI/LobbyUI.prefab
Assets/AssetRaw/UI/CreateRoomUI.prefab
Assets/AssetRaw/UI/RoomUI.prefab
Assets/AssetRaw/UI/RoomPlayerSlot.prefab
Assets/AssetRaw/UI/BattleMainUI.prefab
```

部分临时弹窗仍动态创建。长期要把核心页面 prefab 化，尤其是 `LoadoutUI`、`GhostEquipShopUI`、`ShopUI`、`TaskUI`、`MailUI`、`ChatUI`。

## 6. 战斗客户端

`BattleController` 当前职责：

```text
进入/离开战斗
优先加载 Unity 地图 prefab
fallback 到 Tiled JSON 预览
上报 BattleSceneLoaded
启动 200ms BattleSnapshot 轮询
采样 WASD 并发送移动命令
刷新玩家/建筑占位
处理建造/升级/回收命令
显示建造预览、可建造范围和合法性颜色
```

当前问题是职责过重。后续按切片拆分：

```text
BattleController        # 对外 IBattleCommand 门面和生命周期编排
BattleSyncService       # 快照拉取/推送、移动命令限流、同步状态
BattleMapView           # 地图 prefab / Tiled fallback 加载和坐标转换
BattleEntityView        # 玩家、建筑、幽灵、攻击事件表现
BattleBuildPresenter    # 建造预览、范围圈、合法性颜色、点击确认
BattleInputService      # WASD、鼠标、UI 防穿透、镜头拖拽
BattleViewPool          # 玩家/建筑/幽灵/血条/弹道/特效实例复用
```

## 7. 资源与池化规则

### 资源资产走 ResourceModule

适用：

```text
UI prefab
地图 prefab
角色/建筑/幽灵 prefab
Sprite、图集、特效、音频
配置 bytes
```

规则：

- 只通过 `GameModule.Resource` / `UIModule` 加载。
- 不在业务层散落 `Resources.Load`。
- UI 图片用 `SetSprite` / `SetSubSprite`。
- GameObject prefab 用 `GameModule.Resource.LoadGameObjectAsync` 或 UI 模块创建。

### 运行时表现对象走池

优先池化：

```text
BattlePlayerView
BattleGhostView
BattleBuildingView
BattleHealthBar
BattleProjectile
BattleDamageText
BattleEffect
RoomListItem
ChatMessageItem
```

不要在快照刷新中反复 `Instantiate` / `Destroy`。

### 临时 C# 对象走 MemoryPool

适用：

```text
快照差异计算临时对象
攻击表现事件包装
伤害数字请求
血条显示请求
建造合法性检查结果
UI 列表刷新临时数据
```

不建议池化：

```text
Controller/Service 单例
Model 长期状态
Luban 配置对象
网络响应对象
```

## 8. 下一批客户端模块

```text
Loadout/
  精灵建筑卡组、幽灵角色选择。

Shop/
  建筑卡、精灵角色、幽灵角色、外观购买。

Task/
  每日/成就/引导任务。

Mail/
  系统奖励、补偿、公告。

Friend/
  好友申请、好友列表、邀请房间、私聊入口。

Guild/
  创建/加入/成员/公告/公会聊天。

Chat/
  世界、房间、公会、私聊、系统消息。
```

新增模块必须遵守：UI 只发命令和监听事件，Controller 调 Service，Service 封装网络/配置/资源。

## 9. 代码注释要求

必须注释：

```text
TEngine ResourceModule / ObjectPool / MemoryPool 的使用边界
战斗状态机、同步、卡组校验、装备购买等关键流程
临时兼容代码和后续迁移计划
```

不要给简单赋值、明显 if/return 写噪音注释。
