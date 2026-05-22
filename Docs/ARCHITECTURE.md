# SheepBattle 当前架构

本文只描述当前工程正在使用的架构，不记录旧 Demo、旧 `UnityClient`、旧 HTTP 原型路线。

## 1. 工程结构

```text
SheepBattle/
├── Client.Unity/      # Unity + TEngine 客户端
├── Server.Fantasy/    # Fantasy .NET 9 服务端
├── Shared.Protocol/   # Fantasy proto 协议源
├── Tools.Protocol/    # Fantasy 协议生成工具
├── Tools.Config/      # Luban 配表工具链
├── Shared.Config/     # 配置共享预留
├── Deploy/            # 部署预留
└── Docs/              # 当前文档
```

## 2. 当前技术栈

客户端：

- Unity `2022.3.62f1c1`
- TEngine 热更工程
- HybridCLR
- YooAsset
- Luban
- Fantasy Unity Runtime

服务端：

- .NET `9`
- Fantasy-Net `2025.2.1402`
- 三项目结构：`Main`、`Entity`、`Hotfix`
- 本地 TCP 入口：`127.0.0.1:20000`

## 3. 当前通信方式

当前不是 HTTP/WebSocket 原型路线，而是 Fantasy TCP RPC：

```text
Unity HotFix
-> SheepNetworkService
-> Fantasy Runtime TCP Session
-> Server.Fantasy Hotfix MessageRPC Handler
```

已接入 RPC：

```text
C2G_RegisterRequest
C2G_LoginRequest
C2G_LobbyHomeRequest
C2G_StartMatchRequest
C2G_CreateRoomRequest
```

## 4. 客户端原则

客户端业务层按 TEngine 的 MVE 思路组织：

```text
Model 保存状态
View 由 UIWindow/UIWidget 展示
Event 通知状态变化
Controller/Service 执行业务动作
```

UI 不挂 MonoBehaviour 脚本。`LoginUI`、`LobbyUI`、`RoomUI` 这类窗口由 `GameModule.UI.ShowUIAsync<T>()` 创建纯 C# `UIWindow` 实例，再加载同名 prefab。

业务事件优先使用类事件：

```csharp
public sealed class LobbyViewChangedEvent : IEvent { ... }
GameEvent.Send(new LobbyViewChangedEvent(viewModel));
AddUIEvent<LobbyViewChangedEvent>(OnLobbyViewChanged);
```

## 5. 服务端原则

当前服务端先使用单进程内存实现，方便跑通登录、大厅、房间链路。

当前 Hotfix 业务模块：

```text
Auth   # 注册、登录、Token、玩家资料
Lobby  # 大厅首页、匹配状态
Room   # 创建房间、房间列表
Shared # 服务聚合和数据结构
```

未来 Redis/DB/多进程部署可以扩展，但当前文档和开发以现有 Fantasy TCP 单进程为准。

