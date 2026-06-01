# Luban 配置流程

## 目录

```text
Tools.Config/GameConfig/
├── Datas/                         # Excel 源表
├── Defines/                       # Luban 定义
├── CustomTemplate/                # TEngine 配置模板
├── gen_code_bin_to_project.bat    # 导出到 Unity
├── gen_code_bin_to_project_lazyload.bat
└── gen_code_bin_to_server.bat     # 导出到服务端
```

Luban 编译器：

```text
Tools/Luban/Luban.dll
```

## 当前业务表

源表在：

```text
Tools.Config/GameConfig/Datas
```

当前已使用：

```text
game_rule.xlsx
map.xlsx
shop.xlsx
shop_goods.xlsx
monster.xlsx
building.xlsx
building_card.xlsx
building_level.xlsx
```

`__beans__.xlsx`、`__enums__.xlsx`、`__tables__.xlsx` 是 Luban 定义表。

## 当前建筑配置

建筑源表：

```text
Tools.Config/GameConfig/Datas/building.xlsx
Tools.Config/GameConfig/Datas/building_card.xlsx
Tools.Config/GameConfig/Datas/building_level.xlsx
```

当前 MVP 三类建筑：

```text
101 城墙   Wall     5 级满级，占 1x1，CanBlockPath=true
201 防御塔 Tower    5 级满级，占 1x1，有 Attack/AttackRange/AttackIntervalMs 配置
301 农场   Support  5 级满级，占 2x2，用 RepairValue 表示金币产量，用 AttackIntervalMs 表示产出间隔
```

当前服务端行为：

```text
Build: 校验战斗 Running、配置存在、资源足够、建筑占格不重叠
Upgrade: 按 building_level 的 NextLevelId / UpgradeCostGold / UpgradeCostWood 升级
Recycle: 按 building.RecyclePercent 返还建造卡资源
Farm: BuildingType=Support 时按等级周期给 owner 加 Gold
Wall: CanBlockPath=true 时阻挡玩家移动
Tower: 配置已存在，但等待巨魔实体/快照接入后再攻击
```

当前客户端行为：

```text
BattleMainUI: 从 TbBuildingCard 生成卡片
BattleController: 选择卡片后显示建造预览、范围圈、红/蓝合法提示
建筑表现: Unity Cube + TextMesh 建筑名
```

## 生成目标

客户端：

```text
Client.Unity/Assets/GameScripts/HotFix/GameProto/GameConfig
Client.Unity/Assets/AssetRaw/Configs/bytes
```

服务端：

```text
Server.Fantasy/Hotfix/Config/GameConfig
Server.Fantasy/GameConfig
```

## 生成命令

```powershell
cd Tools.Config\GameConfig
.\gen_code_bin_to_project.bat
.\gen_code_bin_to_server.bat
```

## 访问方式

客户端：

```text
GameLogic.SheepBattle.Config.ConfigSystem
GameRuleService.Instance
```

服务端：

```text
Hotfix.Config.ConfigSystem
SheepServices.Rules
```

## 规则

- 只改 `Datas` 和 `Defines`。
- 不手改生成代码和生成 bytes。
- 业务层优先通过服务封装访问配置，不让 UI 到处直接查 Luban 表。
- 导表后至少检查 id 唯一、引用存在、资源名非空、数值范围合理。
- 客户端和服务端需要使用同一批配置 bytes。
- 修改 Excel 中文时，注意 PowerShell 控制台编码可能把中文写成 `??`。如果用脚本改表，推荐 Python 源码里用 Unicode 转义写中文，或直接用 Excel/LibreOffice 修改。
- 修改建筑源表后必须同时运行客户端和服务端导表脚本，否则 UI 和服务端规则可能不一致。
