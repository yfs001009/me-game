# 玩法系统与首版数值设计

最后整理：2026-05-27。

本文从策划视角定义首版 PvP 玩法、系统模块、数值框架和配置表规划。原则：能由配置表完成的内容都走 Luban 配置，代码只负责读取配置、执行规则和做服务端权威校验。

## 1. 设计目标

```text
首版目标：
  做出 4-6 人固定阵营 PvP。

核心体验：
  精灵带 6 张建筑卡防守。
  幽灵局内购买 6 格装备进攻。
  多地图可选。
  商店只做解锁，不做卡片养成。

数值目标：
  先可测、可调、可读，不追求最终平衡。
  所有战斗数值、经济数值、解锁规则、商品价格都配置化。
```

## 2. 首版 PvP 模式

### 模式定义

```text
模式名：ClassicPvp
人数：4-6 人
阵营：固定阵营
推荐比例：4 精灵 vs 1 幽灵，或 4 精灵 vs 2 幽灵
单局时长：10 分钟
准备期：45 秒
结算期：10 秒
```

### 胜负条件

精灵胜利：

```text
1. 坚持到单局倒计时结束。
2. 或幽灵死亡次数达到配置阈值。
```

幽灵胜利：

```text
1. 全部精灵死亡。
2. 或全部精灵核心被摧毁。
```

首版建议先用：

```text
精灵胜利：坚持 10 分钟。
幽灵胜利：摧毁全部精灵核心，或击杀全部精灵。
```

死亡转化感染可以后置。先把固定阵营打通。

## 3. 战斗阶段

```text
Loading
  客户端加载地图，服务端等待 BattleSceneLoaded。

Prepare
  45 秒准备期。精灵可以建造，幽灵不可进入精灵出生保护区。

Running
  正式对抗。幽灵可以攻击、买装备、拆建筑。

Settling
  胜负已定，停止输入，播放结算。

Finished
  回大厅，发放奖励和任务进度。
```

配置建议：

```text
BattlePrepareSeconds = 45
BattleDurationSeconds = 600
BattleSettleSeconds = 10
GhostKillLimitForElfWin = 12
```

## 4. 精灵系统

### 精灵基础数值

```text
Hp = 100
MoveSpeed = 4.0
InitialGold = 300
InitialWood = 180
BuildRange = 4
RepairRange = 3
RespawnSeconds = 15
```

首版精灵角色可以先只有一个默认角色。后续角色只做机制差异，不做纯强度碾压。

### 精灵建筑卡组

规则：

```text
每名精灵战前选择 6 张建筑卡。
未带入的建筑卡不能在本局建造。
客户端只做 UI 过滤。
服务端 BuildCommand 必须校验卡组。
卡片不升级，只解锁。
```

默认解锁 6 张：

```text
木墙
基础箭塔
农场
维修站
地刺
侦察灯
```

可解锁建筑：

```text
高墙
减速塔
范围塔
爆炸陷阱
护盾塔
高级农场
传送门
科技核心
```

## 5. 建筑首版数值

### 建筑卡表建议

| 卡片 | 类型 | 占格 | 金币 | 木材 | 冷却 | 默认解锁 | 作用 |
| --- | --- | ---: | ---: | ---: | ---: | --- | --- |
| 木墙 | Wall | 1x1 | 20 | 30 | 1s | 是 | 阻挡幽灵 |
| 基础箭塔 | Tower | 1x1 | 80 | 40 | 3s | 是 | 单体攻击 |
| 农场 | Economy | 2x2 | 100 | 60 | 5s | 是 | 周期产金币 |
| 维修站 | Support | 1x1 | 70 | 50 | 5s | 是 | 范围维修 |
| 地刺 | Trap | 1x1 | 60 | 40 | 4s | 是 | 触发伤害/减速 |
| 侦察灯 | Vision | 1x1 | 40 | 20 | 3s | 是 | 反隐/提供视野 |
| 高墙 | Wall | 1x1 | 45 | 75 | 2s | 否 | 更高生命 |
| 减速塔 | Tower | 1x1 | 120 | 70 | 5s | 否 | 低伤害减速 |
| 范围塔 | Tower | 1x1 | 160 | 90 | 6s | 否 | 群体攻击 |
| 爆炸陷阱 | Trap | 1x1 | 120 | 60 | 8s | 否 | 一次性范围伤害 |
| 护盾塔 | Support | 1x1 | 180 | 120 | 8s | 否 | 给附近建筑护盾 |
| 高级农场 | Economy | 2x2 | 180 | 120 | 8s | 否 | 更高产出 |

### 建筑属性建议

| 建筑 | HP | 攻击 | 射程 | 攻速 | 特殊 |
| --- | ---: | ---: | ---: | ---: | --- |
| 木墙 | 600 | 0 | 0 | 0 | 阻挡 |
| 高墙 | 1200 | 0 | 0 | 0 | 阻挡 |
| 基础箭塔 | 350 | 20 | 5 | 1.0s | 单体 |
| 减速塔 | 300 | 8 | 4 | 1.2s | 减速 25%，持续 2 秒 |
| 范围塔 | 280 | 14 | 4 | 1.5s | 半径 1.5 范围伤害 |
| 农场 | 300 | 0 | 0 | 10s | 每 10 秒 +20 金币 |
| 高级农场 | 450 | 0 | 0 | 10s | 每 10 秒 +40 金币 |
| 维修站 | 320 | 0 | 3 | 2s | 每次维修 20 HP |
| 地刺 | 180 | 35 | 0 | 0 | 触发后冷却 5 秒 |
| 爆炸陷阱 | 160 | 120 | 0 | 0 | 一次性爆炸 |
| 侦察灯 | 220 | 0 | 6 | 0 | 反隐/视野 |
| 护盾塔 | 300 | 0 | 3 | 5s | 给建筑 +80 护盾 |

## 6. 幽灵系统

### 幽灵基础数值

```text
Hp = 650
MoveSpeed = 4.5
Attack = 35
AttackInterval = 1.0s
AttackRange = 1.2
RespawnSeconds = 10
InitialGold = 300
GoldPerSecond = 3
BuildingDamageGoldRate = 0.05
KillElfGold = 250
DestroyBuildingGold = 60
DestroyCoreGold = 500
```

### 首版幽灵角色

| 角色 | 定位 | HP | 移速 | 攻击 | 特性 |
| --- | --- | ---: | ---: | ---: | --- |
| 破墙者 | 正面拆墙 | 800 | 4.0 | 45 | 对墙 +30% 伤害 |
| 猎手 | 追击精灵 | 520 | 5.0 | 30 | 击中精灵短暂加速 |
| 腐蚀者 | 削弱建筑 | 620 | 4.3 | 28 | 攻击附带建筑腐蚀 |
| 潜行者 | 绕后偷袭 | 480 | 4.8 | 32 | 短暂隐身，受侦察灯克制 |

首版可以先只实现 `破墙者`，其它角色配置好但不开放。

## 7. 幽灵装备系统

规则：

```text
幽灵有 6 个装备格。
装备只在本局生效。
首版装备只有被动属性，不做主动技能。
装备可以购买、替换、出售，出售返还 50% 金币。
```

### 首版装备表

| 装备 | 价格 | 类型 | 属性 |
| --- | ---: | --- | --- |
| 粗糙利爪 | 300 | 攻击 | 攻击 +15 |
| 破墙斧 | 500 | 破墙 | 对建筑伤害 +25% |
| 厚皮甲 | 450 | 生存 | HP +180 |
| 疾行靴 | 400 | 机动 | 移速 +0.4 |
| 再生血肉 | 600 | 生存 | 每秒回复 6 HP |
| 腐蚀核心 | 700 | 功能 | 攻击建筑附带 3 秒腐蚀 |
| 抗塔披风 | 650 | 抗性 | 受到塔伤害 -15% |
| 狂暴心脏 | 900 | 攻击 | 攻击 +30，HP -80 |
| 恐惧光环 | 1000 | 团队 | 附近幽灵移速 +0.2 |
| 巨力骨锤 | 1200 | 后期 | 攻击 +45，攻速 -10% |

### 装备购买节奏

目标：

```text
2 分钟：幽灵买到第 1-2 件小装。
5 分钟：幽灵有 3-4 件装备，开始强压防线。
8 分钟：幽灵接近神装，精灵进入死守阶段。
```

## 8. 资源经济

### 精灵经济

```text
InitialGold = 300
InitialWood = 180
BaseGoldPerSecond = 1
BaseWoodPerSecond = 0.5
FarmGoldPerCycle = 20
FarmCycleSeconds = 10
AdvancedFarmGoldPerCycle = 40
AdvancedFarmCycleSeconds = 10
RecycleDefaultPercent = 50
```

首版如果木材系统太复杂，可以先用金币 + 木材双资源保留，但数值主要看金币。

### 幽灵经济

```text
InitialGold = 300
GoldPerSecond = 3
DamageBuildingGoldPer100Damage = 5
DestroyWallGold = 30
DestroyTowerGold = 80
DestroyEconomyGold = 120
KillElfGold = 250
AssistGold = 80
```

## 9. 地图数值

### 地图首版建议

| 地图 | 推荐人数 | 精灵出生 | 幽灵出生 | 入口数量 | 特点 |
| --- | ---: | --- | --- | ---: | --- |
| 迷雾森林 | 4-6 | 中央偏左 | 右侧 | 3 | 新手图 |
| 废弃矿坑 | 6-8 | 中央 | 四角 | 4 | 多绕路 |
| 古树圣地 | 8-12 | 中央 | 外圈 | 5 | 大图 |

### 地图配置必须包含

```text
地图宽高
推荐人数
最小/最大人数
精灵出生区
幽灵出生区
no_move 阻挡格
no_build 禁建格
商店区
中立资源点
地图资源名
```

服务端必须用地图配置和 Tiled 规则做权威校验。

## 10. 外层系统

### 商店

售卖内容：

```text
建筑卡解锁
精灵角色解锁
幽灵角色解锁
皮肤/头像/头像框
```

首版价格建议：

| 商品 | 软货币价格 |
| --- | ---: |
| 普通建筑卡 | 1000 |
| 高级建筑卡 | 2500 |
| 精灵角色 | 3000 |
| 幽灵角色 | 3000 |
| 普通皮肤 | 2000 |

不卖直接数值升级。

### 任务

| 任务 | 条件 | 奖励 |
| --- | --- | ---: |
| 每日登录 | 登录 1 次 | 100 软货币 |
| 完成对局 | 完成 1 局 | 150 软货币 |
| 精灵建造 | 建造 20 个建筑 | 200 软货币 |
| 幽灵破坏 | 摧毁 10 个建筑 | 200 软货币 |
| 幽灵购物 | 购买 3 件局内装备 | 150 软货币 |
| 获胜 | 获胜 1 局 | 300 软货币 |

### 邮件

首版用途：

```text
系统公告
维护补偿
任务/活动奖励补发
购买失败补偿
```

### 好友/公会/聊天

首版只做基础：

```text
好友：申请、列表、在线状态、邀请进房间。
公会：创建、加入、成员、公告、公会聊天。
聊天：世界、房间、公会、私聊、系统消息。
```

聊天必须配置：

```text
发言间隔
每日发言上限
频道等级限制
敏感词开关
禁言时长
```

## 11. 配置表规划

### 已有表继续使用

```text
game_rule.xlsx
  全局规则和开关。

map.xlsx
  地图资源、人数范围、玩法模式。

building.xlsx
  建筑基础属性。

building_card.xlsx
  建筑卡显示、解锁规则、消耗、冷却。

building_level.xlsx
  建筑升级属性和消耗。

monster.xlsx
  后续 PVE/召唤物/中立单位可用。

shop.xlsx / shop_goods.xlsx
  可继续扩展为外层商店和局内商店。
```

### 建议新增表

#### battle_mode.xlsx

定义不同 PvP 模式。

```text
modeId
modeName
minPlayers
maxPlayers
recommendedElfCount
recommendedGhostCount
prepareSeconds
battleSeconds
settleSeconds
elfWinCondition
ghostWinCondition
defaultMapPool
```

#### role.xlsx

定义精灵/幽灵角色。

```text
roleId
roleName
camp
unlockRule
shopGoodsId
hp
moveSpeed
attack
attackIntervalMs
attackRange
skillIds
description
```

#### ghost_equipment.xlsx

定义幽灵局内装备。

```text
equipmentId
equipmentName
quality
price
sellPercent
slotType
attackAdd
hpAdd
moveSpeedAdd
attackSpeedAdd
buildingDamagePercent
towerDamageReducePercent
hpRegenPerSecond
auraId
description
```

#### loadout_rule.xlsx

定义卡组规则。

```text
ruleId
camp
slotCount
defaultCardIds
maxDuplicateCount
allowRoleIds
description
```

#### unlock.xlsx

定义解锁条件。

```text
unlockId
unlockType
targetType
targetId
requireLevel
requireTaskId
requireGoodsId
defaultUnlocked
description
```

#### task.xlsx

定义任务。

```text
taskId
taskName
taskType
conditionType
conditionParam
targetCount
rewardSoftCurrency
rewardItemIds
resetType
description
```

#### mail_template.xlsx

定义系统邮件模板。

```text
mailTemplateId
title
content
expireDays
attachmentItemIds
attachmentCounts
description
```

#### chat_rule.xlsx

定义聊天规则。

```text
channel
minLevel
sendIntervalMs
dailyLimit
maxLength
needFilter
description
```

## 12. 配置化原则

必须配置化：

```text
所有角色基础属性
所有建筑属性、消耗、冷却、升级
所有卡片解锁规则
所有幽灵装备价格和属性
所有地图规则、人数和资源名
所有任务条件和奖励
所有商店商品、价格、上下架
所有聊天频率限制
所有战斗阶段时长和胜负条件
```

可以写代码的：

```text
协议收发
状态机推进
公式执行
寻路/碰撞/范围查询
服务端权威校验
UI 展示逻辑
配置读取封装
```

不要写死：

```text
建筑 ID
装备价格
角色属性
地图人数
任务奖励
商店商品
战斗时长
解锁条件
```

## 13. 首版验收标准

```text
1. 房主可选择地图。
2. 精灵可选择 6 张建筑卡。
3. 进入战斗后只显示带入卡。
4. 未带入的建筑卡服务端拒绝建造。
5. 幽灵可获得金币并购买装备。
6. 装备属性影响战斗。
7. 胜负结算能返回大厅。
8. 对局结果能推进任务。
9. 商店可解锁建筑卡/角色。
10. 所有数值来自配置表。
```
