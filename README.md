# SheepBattle

商业化多人 PvP 非对称对抗手游工程。当前目标是先做可玩的核心版本：房间/地图选择、精灵带建筑卡组防守、幽灵局内装备成长进攻、服务端权威同步和结算闭环。

## 当前玩法方向

```text
精灵阵营：战前带 6 张建筑卡进局，建造防线、经济和功能建筑。
幽灵阵营：选择幽灵角色，局内购买 6 格装备，拆防线并击败/感染精灵。
地图：房主或匹配系统选择地图，地图配置决定人数范围、出生、阻挡和禁建规则。
外层：商店、任务、好友、邮件、公会、聊天逐步补齐，先只做解锁，不做卡片养成。
```

## 技术栈

```text
客户端：
  Unity 2022.3.62f1c1
  TEngine
  YooAsset
  HybridCLR
  Luban
  Fantasy Unity Runtime 2025.2.1402
  SuperTiled2Unity 2.3.0

服务端：
  .NET 9
  Fantasy-Net 2025.2.1402
  TCP Gate: 127.0.0.1:20000
  Main / Entity / Hotfix 三项目结构
```

## 工程目录

```text
Client.Unity/              # Unity + TEngine 客户端
Server.Fantasy/            # Fantasy .NET 9 服务端
Shared.Protocol/           # Fantasy proto 协议源
Shared.Config/             # 共享配置预留
Tools.Protocol/            # Fantasy 协议生成工具
Tools.Config/              # Luban 配表工具链
Tools.Map/                 # Tiled 地图编辑工作区
Deploy/                    # 部署配置预留
Docs/                      # 项目文档
```

## 快速运行

服务端：

```powershell
dotnet run --project Server.Fantasy\Main\Main.csproj --framework net9.0
```

客户端：

```text
用 Unity 2022.3.62f1c1 打开 Client.Unity
启动场景 Launcher
资源模式用 EditorMode 快速验证业务流程
```

双人同步测试：

```text
1. 启动服务端。
2. 打开两个客户端实例，使用不同账号登录。
3. A 创建房间。
4. B 加入房间并准备。
5. A 开始游戏。
6. 两端进入 BattleMainUI。
7. WASD 移动、选择建筑卡、建造/升级/回收，另一端应通过快照看到同步结果。
```

## 常用命令

客户端热更逻辑编译：

```powershell
dotnet build Client.Unity\GameLogic.csproj --no-restore
```

服务端编译：

```powershell
dotnet build Server.Fantasy\Server.sln --no-restore
```

协议生成：

```powershell
cd Tools.Protocol\ProtocolExportTool
dotnet .\Fantasy.ProtocolExportTool.dll export --silent
```

配置生成：

```powershell
cd Tools.Config\GameConfig
.\gen_code_bin_to_project.bat
.\gen_code_bin_to_server.bat
```

## 文档入口

先读这里：

- [文档入口](Docs/README.md)
- [当前状态](Docs/STATUS.md)
- [玩法与技术架构设计](Docs/GAME_DESIGN_AND_ARCHITECTURE.md)
- [玩法系统与首版数值设计](Docs/GAME_DESIGN_NUMBERS.md)
- [TEngine / Fantasy 框架使用体检](Docs/FRAMEWORK_USAGE_AUDIT.md)
- [下一步](Docs/NEXT_STEPS.md)

技术分册：

- [客户端架构](Docs/CLIENT_ARCHITECTURE.md)
- [服务端架构](Docs/SERVER_ARCHITECTURE.md)
- [协议设计](Docs/PROTOCOL_DESIGN.md)
- [Luban 配置流程](Docs/CONFIG_PIPELINE.md)
- [Tiled 地图管线](Docs/TILED_MAP_PIPELINE.md)

## 开发规则

- 不手改协议生成文件，只改 `Shared.Protocol/NetworkProtocol` 后运行协议生成。
- 不手改 Luban 生成文件，只改 `Tools.Config/GameConfig/Datas` 和 `Defines` 后运行导表。
- UI 不直接调网络，按钮走 `GameEvent.Get<IxxxCommand>()`，状态刷新走事件。
- 客户端资源加载走 TEngine ResourceModule，战斗表现对象逐步接 ObjectPool/自建表现池。
- 服务端 Handler 只做协议适配，领域状态逐步迁入 Fantasy Entity/Component，战斗 Tick 迁到 Battle Scene/System。
- 新增代码要给关键业务边界、状态机、框架接入点写注释。
