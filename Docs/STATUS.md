# 当前状态

最后整理：2026-05-24。

## 一句话现状

项目已经从原型收敛为 `SheepBattle` 正式工程。当前可运行链路是：注册/登录 -> 大厅 -> 创建/加入房间 -> 准备/开始 -> 双人进入战斗 -> 客户端 200ms 拉服务端权威快照 -> WASD 移动同步 -> 选择建筑卡片 -> 显示建造预览/范围 -> 点击合法格子建造 -> 升级/回收。

## 技术栈

客户端：

```text
Unity 2022.3.62f1c1
TEngine
YooAsset
HybridCLR
Luban
Fantasy Unity Runtime 2025.2.1402
SuperTiled2Unity 2.3.0
```

服务端：

```text
.NET 9
Fantasy-Net 2025.2.1402
TCP Gate: 127.0.0.1:20000
Main / Entity / Hotfix 三项目结构
```

## 已落地

- Fantasy TCP RPC 通信，不再走 HTTP/WebSocket 原型。
- 协议源在 `Shared.Protocol/NetworkProtocol`，生成到服务端 `Entity/Generate` 和 Unity `GameProto`。
- 客户端热更入口为 `SheepBattleApp.Start()`，默认启动 `SplashUI -> VersionCheckUI -> LoadingUI -> LoginUI`。
- `SheepNetworkService` 封装 Fantasy Runtime RPC。
- 登录、大厅、房间、战斗命令已接入 MVE：
  - UI 发 `ILoginCommand` / `ILobbyCommand` / `IBattleCommand`
  - Controller 调网络和更新 Model
  - Model 发 `IEvent` 类事件
  - UI 通过 `AddUIEvent<T>` 刷新
- 房间支持创建、加入、离开、详情轮询、准备、房主开始。
- 战斗支持加载完成、快照、移动、建造、升级、回收。
- 服务端 `BattleService` 是单进程内存权威状态，保存玩家、建筑、资源、Tick、加载状态和运行状态。
- 战斗移动输入已解耦 RPC 强阻塞：WASD 每 16ms 采样，移动命令 0.1 秒限流，不再因为上一条移动 RPC 未返回而吞掉后续按键。
- 战斗建筑 MVP 已接入：
  - 配置为三类建筑：农场 `301`、城墙 `101`、防御塔 `201`。
  - 每类建筑配置为 5 级满级，可通过 `building.xlsx` / `building_level.xlsx` 调整。
  - 客户端点击卡片进入建造模式，显示虚拟 Cube、红/蓝占地提示和本地玩家周围建造范围圈。
  - 鼠标在 UI 上时不会触发场景建造或镜头拖拽，避免点击卡片穿透到场景。
  - 建造失败会弹出服务端返回原因。
  - 建筑表现暂用 Unity 原生 Cube，中间用 `TextMesh` 显示建筑名。
  - 服务端农场按等级周期产金币，城墙会阻挡玩家移动。
- 防御塔配置已有攻击、射程、间隔字段，但巨魔战斗实体尚未进入快照，所以防御塔攻击巨魔尚未闭环。
- Luban 配表已扩展到：
  `game_rule`、`map`、`shop`、`shop_goods`、`monster`、`building`、`building_card`、`building_level`。
- 客户端和服务端配置生成链路已跑通。
- 非战斗 UI 已接入第一批欧美卡通风无文字美术资源，资源在 `Client.Unity/Assets/AssetRaw/UI/Art`。
- 地图视觉管线已切到 Unity prefab 优先：
  - `Client.Unity/Assets/AssetRaw/TiledMaps` 保存 `.tmx/.tsx/png`
  - SuperTiled2Unity 导入 Tiled
  - `Client.Unity/Assets/AssetRaw/MapPrefabs` 放运行时地图 prefab
  - `BattleController` 先加载地图 prefab，失败再回退旧 JSON 预览

## 当前验证命令

客户端热更逻辑：

```powershell
dotnet build Client.Unity\GameLogic.csproj --no-restore
```

服务端：

```powershell
dotnet build Server.Fantasy\Server.sln --no-restore
```

启动服务端：

```powershell
dotnet run --project Server.Fantasy\Main\Main.csproj --framework net9.0
```

最近一次验证：

```text
Client.Unity/GameLogic.csproj: 0 error
Server.Fantasy/Server.sln: 0 error
```

## 双人战斗同步测试步骤

```text
1. 启动服务端。
2. 打开两个客户端实例，使用不同账号登录。
3. A 创建房间。
4. B 在房间列表加入。
5. B 点击准备。
6. A 点击开始游戏。
7. A 会立即进入战斗；B 通过 RoomUI 每秒轮询拿到 BattleStartInfo，通常延迟不超过 1 秒进入战斗。
8. 两端都上报 BattleSceneLoaded 后，服务端 Battle.State 从 Loading 切到 Running。
9. A/B 分别 WASD 移动，另一端应看到位置变化。
10. 点击农场/城墙/防御塔卡片，鼠标移动到角色附近应看到建造预览；蓝色可建造，红色不可建造。
11. 点击蓝色格子后服务端建造成功，快照返回后场景出现 Cube 和建筑名。
```

关键日志：

```text
服务端：战斗全员加载完成，进入 Running
客户端：战斗快照：BattleId=...，Players=2
```

## 当前风险和下一步

- 房间状态仍是 RPC 返回刷新 + RoomUI 轮询，下一步应补 `S2C_RoomStatePush`。
- 战斗同步仍是 200ms RPC 快照轮询，下一步应补 `S2C_BattleSnapshot` 或 Battle Scene 广播。
- 服务端移动边界仍是临时 `0..99`，还没有读取地图宽高、出生区和阻挡格。
- 当前出生点临时设为 `(7,4.5)` 和 `(8.5,4.5)`，只是为了双人同步测试容易看到。
- 建造合法性目前客户端只判断建造范围和已有建筑占格；服务端也只判断资源和建筑占格，尚未接地图阻挡格、出生区、建筑离玩家距离等完整规则。
- 防御塔需要等巨魔实体、怪物快照、寻敌和扣血逻辑补齐后才能真正攻击。
- SuperTiled2Unity 需要 Unity Package Manager 拉包并在编辑器内导入 `.tmx`；命令行 dotnet build 不会生成地图 prefab。
- 若使用 HostPlayMode/热更包，改资源或热更 DLL 后需要重新打 AssetBundle 并同步到热更服务器。

## 不要踩的坑

- 不手改协议生成文件；只改 `Shared.Protocol/NetworkProtocol` 后运行协议生成。
- 不手改 Luban 生成文件；只改 `Tools.Config/GameConfig/Datas` 和 `Defines` 后运行导表脚本。
- 在 PowerShell 里批量写 Excel 中文时要小心控制台编码；如果必须脚本写中文，优先用 Python Unicode 转义或直接用 Excel/LibreOffice 编辑，避免把中文写成 `??`。
- UI 业务脚本不挂 prefab；prefab 只做节点和组件，业务在 `UIWindow/UIWidget`。
- UI 内不要直接调网络；按钮走接口事件，状态刷新走 Model 事件。
- 地图显示优先走 Unity prefab；JSON 仅作为 fallback/规则导出参考。
