# 当前落地状态

## 已完成

- 根目录收敛为正式 `SheepBattle` 工程。
- 客户端基于 TEngine 工程结构：`Client.Unity`。
- 服务端基于 Fantasy 三项目结构：`Server.Fantasy/Main`、`Entity`、`Hotfix`。
- 协议源迁移到 `Shared.Protocol/NetworkProtocol`。
- 协议生成工具迁移到 `Tools.Protocol/ProtocolExportTool`。
- Fantasy 协议生成路径已指向服务端和 TEngine 热更协议目录。
- 服务端锁定 `Fantasy-Net 2025.2.1402`，与客户端 Unity 包版本保持一致。
- 服务端外网协议已改为 `TCP`，本地 Gate 端口为 `127.0.0.1:20000`。
- 已按 Fantasy `MessageRPC` 方式实现：注册、登录、大厅首页、开始匹配、创建房间。
- TEngine 热更入口已切换到 `SheepBattleApp.Start()`，默认打开 `LoginUI`。
- 客户端 `manifest.json` 已加入 `com.fantasy.unity@2025.2.1402` 的 OpenUPM scoped registry。
- 客户端热更层已建立 `SheepNetworkService`，按 Fantasy Runtime + 生成协议扩展方法调用 RPC。
- 客户端已补 TEngine 类事件能力：`IEvent`、`GameEvent.Send<TEvent>`、`AddUIEvent<TEvent>`。
- 客户端登录、大厅、房间已开始按 MVE 拆分：Model 保存状态，类事件通知 UI。
- 大厅和房间 UI 已接入创建房间、加入房间、离开房间、房间显示的最小流程。
- 协议已新增 `C2G_JoinRoomRequest`、`C2G_LeaveRoomRequest`，并生成到客户端和服务端。
- 服务端已实现 Join/Leave Room RPC 和内存房间人数/房主转移逻辑。
- 客户端 `SheepNetworkService` 已接入 Join/Leave Room RPC。
- Luban 工具已收敛到 `Tools/Luban`，删除临时导入的示例工程目录。
- 配表源已收敛为 SheepBattle 当前业务表：`common.TbGameRule`。
- `game_rule.xlsx` 已补 MVP 基础规则：启动加载、登录校验、大厅资源、匹配人数、房间人数、默认地图、战斗阶段、结算返回。
- 客户端和服务端 Luban 导表链路已跑通，只生成 `common_tbgamerule.bytes` 和 `common/TbGameRule` 相关代码。
- 服务端 Hotfix 已链接客户端 Luban runtime，并允许 unsafe 编译以支持 `ByteBuf`。
- 客户端 `GameRuleService` 已接入 `ConfigSystem.Instance.Tables.TbGameRule`，保留默认值兜底。
- `SplashUI -> VersionCheckUI -> LoadingUI -> LoginUI` 启动链路已按 `game_rule.xlsx` 的 Splash/Loading/VersionCheck 规则读取时长、开关和提示文案。
- 清理客户端 `GameProto.csproj` 的 demo 生成文件引用，命令行可构建 `GameLogic.csproj`。
- 已确认 TEngine `EditorMode` 不加载热更 dll bytes，但仍会从当前 AppDomain 反射调用 `GameApp.Entrance()`；可用于快速验证业务 UI，不能替代完整热更包验证。
- 已完成 HybridCLR 首次打包前置流程确认：`HybridCLR/Install...`、`Enable HybridCLR`、`Generate/All`、热更 DLL 构建与复制。
- 已打通 Windows 初包构建流程：`TEngine/Build/一键打包 Window` 会先构建热更 DLL 与 YooAsset AssetBundle，再输出 Windows Player。
- 已将资源运行模式调整为 `HostPlayMode`，用于验证真实远端版本检查、清单更新、资源下载和 `GameLogic.dll.bytes` 热更加载流程。
- 已创建本地热更资源目录：`D:\HotUpdateServer\Demo\Windows64`。
- 已同步最新 YooAsset 热更资源到本地资源目录，当前远端版本为 `2026-05-22-70`。
- 已创建 PowerShell 版本地静态资源服务器脚本：`D:\HotUpdateServer\StartHotUpdateServer.ps1`，用于无 Python 环境下提供 `http://127.0.0.1:8081` 资源服务。
- 已验证本地远端版本文件可访问：
  `http://127.0.0.1:8081/Demo/Windows64/DefaultPackage.version`。

## 服务端验证

```powershell
dotnet build SheepBattle\Server.Fantasy\Server.sln
```

结果：通过。

当前存在少量 warning：

- `Client.Unity/Assets/GameScripts/HotFix/GameLogic/Module/UIModule/UIBase.cs` 的 nullable 注解 warning，来自当前 Unity C# 项目 nullable 设置。

当前不影响构建和运行，后续可单独清理。

启动服务端：

```powershell
dotnet run --project SheepBattle\Server.Fantasy\Main\Main.csproj --framework net9.0
```

服务端可启动并监听 `127.0.0.1:20000`。

也可以在项目根目录双击：

```text
StartServer.bat
```

## 协议生成

```powershell
cd SheepBattle\Tools.Protocol\ProtocolExportTool
dotnet .\Fantasy.ProtocolExportTool.dll export --silent
```

协议源只改：

```text
SheepBattle/Shared.Protocol/NetworkProtocol
```

不要手改生成文件。

## Unity 验证步骤

1. 用 Unity `2022.3.62f1c1` 打开：
   `SheepBattle/Client.Unity`
2. 等 Package Manager 拉取：
   `com.fantasy.unity@2025.2.1402`
3. 确认 Scripting Define Symbols 包含：
   `FANTASY_UNITY`
4. 工具栏资源模式使用 `EditorMode` 可快速验证业务入口；完整热更包验证再切 `OfflinePlayMode`。
5. 启动服务端。
6. Play 运行 Launcher。
7. 编辑器模式下也应进入 `GameApp.Entrance()`，并显示 `SplashUI -> VersionCheckUI -> LoadingUI -> LoginUI`。

## 热更验证步骤

当前客户端热更栈：

```text
YooAsset # 资源清单、bundle、远端版本和下载
HybridCLR # GameProto.dll.bytes / GameLogic.dll.bytes 热更程序集加载
TEngine Procedure # 初始化资源、下载、加载程序集、启动 GameApp
```

本地资源服务目录：

```text
D:\HotUpdateServer\Demo\Windows64
```

本地资源服务启动：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File D:\HotUpdateServer\StartHotUpdateServer.ps1
```

服务验证：

```text
http://127.0.0.1:8081/Demo/Windows64/DefaultPackage.version
```

真实热更验证顺序：

1. 先保留一个 `HostPlayMode` 的旧 Windows 初包，不再覆盖它。
2. 修改 `Assets/GameScripts/HotFix/GameLogic` 中可见文案或日志，例如登录按钮文案。
3. 只执行 `TEngine/Build/一键打包 AssetBundle`，不要重新打 Window 包。
4. 将最新资源同步到 `D:\HotUpdateServer\Demo\Windows64`。
5. 运行旧 Windows 初包，确认它访问远端版本并下载新 bundle。
6. 若旧包中的 UI 文案变成新文案，则证明 `GameLogic.dll.bytes` 热更流程生效。

注意：如果重新打 `一键打包 Window`，新客户端本身就会内置新代码，不能证明旧包从远端热更成功。

## 当前客户端 UI

```text
SplashUI # 启动闪屏，占位/待按 MVP 规则完善
VersionCheckUI # 版本检查占位，一致时自动进入加载
LoadingUI # 加载界面，占位/待按 MVP 规则完善
LoginUI  # 登录入口
RegisterUI # 注册入口
NicknameUI # 首次昵称设置
LobbyUI  # 大厅、创建房间、加入房间
RoomListUI # 房间列表
RoomUI   # 房间信息、开始战斗、离开房间
BattleMainUI # 战斗占位界面
```

## 下一步建议

1. 用“旧 Windows 初包 + 新远端 AssetBundle”完成一次肉眼可见的热更验证，例如 `游客登录 -> 游客老弟登录`。
2. 让服务端 `GameRuleService` 读取 `TbGameRule`，与客户端规则保持一致。
3. 大厅拆分：补 `MatchUI`、`MatchQueueUI`、`CreateRoomUI`，让 `LobbyUI` 只做入口聚合。
4. 房间协议：新增准备、取消准备、开始游戏、房间状态推送。
5. 战斗 MVP：先做加载战斗、准备阶段、阵营分配、感染基础状态和结算返回。

## 注意

当前客户端 Fantasy Runtime API 是按 Fantasy Unity 2025.2.1402 的常用调用方式接入。若 Unity 编译提示 `Runtime.Connect` 参数签名不同，应以实际拉取到的 `com.fantasy.unity` 包源码为准微调 `SheepNetworkService`，不改业务层结构。
