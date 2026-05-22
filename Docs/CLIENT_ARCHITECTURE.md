# 客户端架构

## 1. 当前入口

Unity 用 `2022.3.62f1c1` 打开：

```text
Client.Unity/
```

热更入口：

```text
Assets/GameScripts/HotFix/GameLogic/GameApp.cs
Assets/GameScripts/HotFix/GameLogic/SheepBattle/App/SheepBattleApp.cs
```

启动后：

```text
GameEventHelper.Init()
-> SheepBattleApp.Start()
-> Fantasy.GameProtoFantasyRegistrar.Register()
-> SheepNetworkService.Initialize("127.0.0.1", 20000)
-> GameModule.UI.ShowUIAsync<LoginUI>()
```

### 编辑器模式与热更模式

TEngine 工程在 Unity 工具栏里有资源运行模式：

```text
EditorMode (编辑器下的模拟模式)
OfflinePlayMode (单机模式)
HostPlayMode (联机运行模式)
WebPlayMode (WebGL运行模式)
```

当前流程差异：

- `EditorMode`：`ProcedureLoadAssembly` 不加载热更 dll bytes，而是从当前 AppDomain 查找 `GameLogic.dll`，再反射调用 `GameApp.Entrance()`。因此可以在编辑器里验证业务入口和 UI 流程，但这不是完整热更加载验证。
- 非 `EditorMode` 且启用 HybridCLR：`ProcedureLoadAssembly` 会从资源系统加载 `GameProto.dll.bytes`、`GameLogic.dll.bytes`，再调用 `GameApp.Entrance()`。这是更接近真机/发布包的热更验证路径。

验证 UI 业务流程时，`EditorMode` 足够快；验证热更包、dll bytes、资源清单和 AOT 元数据时，应切到 `OfflinePlayMode` 或后续发布流程。

## 2. TEngine UI 模式

本项目 UI 不把业务脚本挂到 prefab 上。

```text
GameModule.UI.ShowUIAsync<LoginUI>()
-> UIModule new LoginUI()
-> 加载 Assets/AssetRaw/UI/LoginUI.prefab
-> ScriptGenerator()
-> RegisterEvent()
-> OnCreate()
-> OnRefresh()
```

Prefab 只负责节点结构和 Unity 原生组件；`UIWindow/UIWidget` 负责绑定节点、监听事件和刷新显示。

## 3. 当前业务模块

```text
SheepBattle/
├── App/
├── Network/
├── Login/
├── Lobby/
├── Battle/
└── Event/
```

当前已落地：

```text
LoginUI -> LoginController -> LoginModel -> LoginStatusChangedEvent
LobbyUI -> LobbyController -> LobbyModel -> LobbyViewChangedEvent/LobbyStatusChangedEvent
RoomUI  -> LobbyModel.CurrentRoom -> RoomViewChangedEvent
```

## 4. MVE 规则

- Model：保存业务状态，派发类事件。
- View：`UIWindow/UIWidget`，只展示数据、收集输入、监听事件。
- Event：带数据事件使用类事件，实现 `TEngine.IEvent`。
- Controller：处理按钮触发的业务动作，调用网络服务，更新 Model。
- Service：封装外部系统，比如 Fantasy 网络、资源加载、配置访问。

## 5. 资源规则

- UI prefab 放在 `Assets/AssetRaw/UI/`。
- Sprite 用 `SetSprite` / `SetSubSprite`。
- GameObject prefab 用 `LoadGameObjectAsync` 或 UI 模块创建。
- 非 GameObject 资源用 `LoadAssetAsync`，并配对 `UnloadAsset`。
- 不直接使用 `Resources.Load` 做业务资源加载。

## 6. 事件规则

带数据业务事件：

```csharp
public sealed class LobbyViewChangedEvent : IEvent
{
    public LobbyViewModel ViewModel { get; }
}
```

派发：

```csharp
GameEvent.Send(new LobbyViewChangedEvent(viewModel));
```

UI 监听：

```csharp
AddUIEvent<LobbyViewChangedEvent>(OnLobbyViewChanged);
```

UI 监听会随窗口销毁自动清理。

## 7. 当前 UI

```text
Assets/GameScripts/HotFix/GameLogic/UI/LoginUI/LoginUI.cs
Assets/GameScripts/HotFix/GameLogic/UI/LobbyUI/LobbyUI.cs
Assets/GameScripts/HotFix/GameLogic/UI/RoomUI/RoomUI.cs
Assets/GameScripts/HotFix/GameLogic/UI/BattleMainUI/BattleMainUI.cs
```

后续可按业务目录继续拆：

```text
UI/Login/
UI/Lobby/
UI/Room/
UI/Battle/
UI/Common/
```
