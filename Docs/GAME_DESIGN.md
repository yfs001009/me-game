# SheepBattle 玩法与技术架构设计

最后整理：2026-05-26。

本文先从策划视角定义游戏玩法、系统模块、商业化边界和版本路线，再从程序视角规划如何用好 TEngine 与 Fantasy。

## 1. 游戏定位

```text
类型：多人 PvP 非对称对抗手游
核心体验：精灵带建筑卡组防守，幽灵局内购买装备进攻
目标规模：千人同时在线，单局多人房间对战
当前阶段：先做 PvP，不做 PVE，不做卡牌养成，只做卡片解锁
```

一句话玩法：

```text
精灵玩家战前带 6 张建筑卡进局，在地图中建立防线和经济；幽灵玩家通过击杀、拆建筑和时间经济购买 6 格装备，突破防线并感染/击败精灵。
```

## 2. 阵营定位

### 精灵阵营

精灵是防守和经营方。

核心循环：

```text
选择地图/进入房间
-> 战前选择建筑卡组
-> 开局找据点
-> 放置核心/建立基础防线
-> 建造经济建筑
-> 建墙、塔、陷阱、功能建筑
-> 维修/升级/转移防线
-> 坚持到时间结束或击退幽灵
```

精灵关键体验：

- 像《植物大战僵尸》一样“带卡进局”。
- 卡片不养成，只通过账号进度、任务、商店解锁。
- 同一局内只能造带入的卡，形成战术差异。
- 防线不是一次性摆完，而是随着资源产出逐步铺开。
- 后期要面临幽灵装备成型后的压迫，需要撤退、重建、死守。

### 幽灵阵营

幽灵是进攻和成长方。

核心循环：

```text
选择幽灵角色
-> 观察精灵据点
-> 拆墙/绕后/偷袭
-> 获取金币
-> 购买装备
-> 强化破墙、机动、生存或控制能力
-> 击杀/感染精灵
-> 滚雪球压垮防线
```

幽灵关键体验：

- 像 MOBA 一样有 6 个装备格。
- 装备只在本局生效，结算后清空。
- 通过拆建筑、击杀、时间经济成长。
- 幽灵角色可以售卖/解锁，但装备和角色要控制平衡。
- 幽灵不建造，主要靠操作、装备路线和团队配合。

## 3. 单局流程

### 推荐第一版流程

第一版建议使用固定阵营，不先做“全员精灵后随机感染”。固定阵营更容易调平衡，也更容易让服务端状态机清晰。

```text
1. 大厅
   玩家选择匹配或房间。

2. 房间
   房主选择地图、人数、是否私密。
   玩家选择阵营，或由系统分配阵营。

3. 战前准备
   精灵选择 6 张已解锁建筑卡。
   幽灵选择一个已解锁幽灵角色。

4. 加载地图
   客户端加载地图 prefab。
   服务端创建 Battle Scene / BattleEntity。

5. 准备期 30-60 秒
   精灵找位置、放核心、建立初始防线。
   幽灵可等待出生或在限制区域观察。

6. 对抗期
   精灵建造/维修/升级。
   幽灵拆建筑/击杀/买装备。

7. 结算
   精灵胜利：坚持到时间结束，或幽灵击败次数达到阈值。
   幽灵胜利：全部精灵死亡/感染，或摧毁全部核心。

8. 返回大厅
   发放任务进度、货币、经验、邮件奖励等。
```

### 后续感染模式

等固定阵营 PvP 稳定后，再做感染模式：

```text
开局所有人都是精灵
-> 准备期后随机 1-2 人变成幽灵
-> 被击败的精灵转化为幽灵
-> 最后一名精灵获得英雄强化
```

这个模式更有特色，但也更难平衡，不建议第一阶段就做。

## 4. 地图设计

你计划配置多个不同地图，这很适合当前玩法。

地图需要同时服务视觉、规则和匹配。

### 地图配置字段

```text
MapId
MapName
MapAsset
Mode
MinPlayers
MaxPlayers
RecommendedPlayers
SpawnElfAreas
SpawnGhostAreas
NoMoveAreas
NoBuildAreas
ShopAreas
NeutralResourceAreas
Comment
```

### 地图类型建议

```text
1. 迷雾森林
   新手地图，通路清晰，适合 4-6 人。

2. 废弃矿坑
   多岔路、多狭窄路口，墙和陷阱价值高。

3. 古树圣地
   中央大据点，四周多入口，适合大房间。

4. 荒原峡谷
   开阔地多，塔和视野建筑更重要。
```

### 地图选择规则

- 房主创建房间时选择地图。
- 匹配模式可以轮换地图池。
- 地图限制最小/最大人数。
- 地图配置决定推荐玩家数和默认房间人数。
- 服务端用地图规则做移动、建造、出生、商店区校验。

## 5. 建筑卡片系统

### 第一阶段原则

```text
只有解锁，没有卡片升级。
卡片只决定“能造什么”，不决定“数值有多强”。
局内建造仍消耗金币/木材/能量。
每局带入 6 张建筑卡。
```

### 初始默认卡组

新手默认解锁：

```text
木墙
基础箭塔
农场
维修站
地刺
侦察灯
```

第一批可解锁卡：

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

### 卡片解锁来源

```text
账号等级解锁
任务奖励解锁
商店购买解锁
活动奖励解锁
```

不建议第一阶段做随机抽卡。先做明确购买和任务解锁，体验更稳，也更容易平衡。

### 战术流派

```text
经济发育流
  农场、高级农场、维修站、基础墙、基础塔、侦察灯

龟缩防守流
  木墙、高墙、护盾塔、维修站、减速塔、范围塔

陷阱控制流
  地刺、爆炸陷阱、减速塔、侦察灯、基础墙、基础塔

机动转移流
  传送门、维修站、基础墙、农场、侦察灯、基础塔
```

## 6. 幽灵角色与装备系统

### 幽灵角色

幽灵角色可以售卖或解锁，但要避免强付费角色破坏平衡。

角色差异建议：

```text
破墙者
  高血量，高破墙，低机动。

猎手
  高机动，擅长追击精灵，拆墙一般。

腐蚀者
  持续伤害和削弱建筑，正面能力中等。

潜行者
  短暂隐身或反侦察，适合绕后。
```

### 幽灵装备

装备为局内购买，6 个格子。

装备类型：

```text
攻击类
  破墙斧、腐蚀爪、狂暴核心

生存类
  厚皮甲、再生血肉、抗性披风

机动类
  疾行靴、冲刺核心、跳跃符文

功能类
  反侦察斗篷、破盾器、沉默尖刺

团队类
  恐惧光环、群体加速、建筑腐蚀光环
```

金币来源：

```text
随时间获得
攻击建筑获得
摧毁建筑获得
击杀/感染精灵获得
摧毁核心获得大量奖励
```

第一版装备系统可以很简单：

```text
幽灵商店 UI
6 个装备格
金币购买
装备只加属性
不做合成树
不做主动装备
```

## 7. 商店与外层系统

你提到商店、任务、好友、邮件、公会、聊天都希望完善进去。建议分成“核心外层”和“社交外层”。

### 商店

售卖内容：

```text
建筑卡解锁
精灵角色
幽灵角色
皮肤/外观
头像/头像框
表情/聊天气泡
```

平衡原则：

- 不卖直接数值碾压。
- 建筑卡可以卖，但必须保证默认卡组能正常获胜。
- 精灵/幽灵角色可有机制差异，但不应比免费角色全面更强。
- 推荐用横向差异和外观做商业化。

### 任务

任务先服务留存和引导：

```text
每日登录
完成 1 场对局
获胜 1 场
建造 20 个建筑
维修 10 次
作为幽灵摧毁 10 个建筑
购买 3 件局内装备
和好友组队 1 次
```

奖励：

```text
金币/软货币
建筑卡碎片或直接解锁
角色体验卡
头像/装饰
```

### 邮件

邮件用于：

```text
系统公告
维护补偿
任务/活动奖励补发
购买记录通知
封禁/处罚通知
```

### 好友

第一阶段功能：

```text
搜索玩家
发送好友申请
好友列表
在线状态
邀请进房间
私聊入口
```

### 公会

第一阶段功能：

```text
创建/加入/退出
成员列表
职位
公告
公会聊天
简单签到
```

公会战、捐献、科技树都可以后置。

### 聊天

频道：

```text
世界
房间
战斗快捷语
好友私聊
公会
系统
```

手游要注意：

- 文本过滤。
- 频率限制。
- 举报和禁言。
- 战斗中优先快捷语，减少打字负担。

## 8. 版本路线

### V0.1 核心可玩

```text
房间选择地图
固定阵营
精灵带 6 张建筑卡
幽灵固定角色
移动、建造、拆建筑、基础胜负
战斗结算返回大厅
```

### V0.2 幽灵局内成长

```text
幽灵金币
幽灵装备商店
6 格装备栏
基础装备属性
击杀/拆建筑奖励
```

### V0.3 外层闭环

```text
账号解锁建筑卡
商店购买卡片/角色
每日任务
邮件发奖励
多地图选择
```

### V0.4 社交

```text
好友
私聊
房间邀请
公会基础功能
世界/公会/房间聊天
```

### V0.5 规模化

```text
Battle Scene 化
房间/聊天/公会独立 Scene
Redis/DB 持久化
压测千人在线
日志和监控
```

## 9. TEngine 客户端架构

### 客户端模块划分

```text
SheepBattle/
├── App/
├── Network/
├── Config/
├── Account/
├── Lobby/
├── Room/
├── Loadout/          # 精灵卡组、幽灵角色选择
├── Battle/
│   ├── Input/
│   ├── Sync/
│   ├── View/
│   ├── Pool/
│   └── UI/
├── Shop/
├── Task/
├── Mail/
├── Friend/
├── Guild/
└── Chat/
```

### TEngine 应用方式

```text
UIWindow/UIWidget
  大厅、房间、卡组、商店、任务、邮件、公会、聊天、战斗 HUD。

GameEvent
  UI 命令接口和状态刷新事件。

ResourceModule/YooAsset
  地图、角色、建筑、幽灵、特效、UI 图标、音频、配置 bytes。

ObjectPoolModule
  战斗中的玩家、幽灵、建筑、血条、伤害数字、弹道、特效实例。

MemoryPool
  快照差异对象、临时刷新数据、UI 列表项数据、战斗事件包装。

HybridCLR
  玩法逻辑、UI 逻辑、网络协议处理、配置访问。
```

### 客户端池化重点

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

不建议池化：

```text
Controller/Service 单例
Model 长期状态
Luban 配置对象
网络响应对象
```

## 10. Fantasy 服务端架构

### 目标 Scene 划分

```text
Gate Scene
  连接、登录、Session、心跳、断线处理。

Account/Data Scene
  账号、货币、已解锁建筑卡、已解锁角色、任务、邮件、商城购买。

Lobby Scene
  在线状态、匹配、房间列表、地图池。

Room Scene
  房间、地图选择、阵营选择、精灵卡组选择、幽灵角色选择。

Battle Scene
  单局战斗 Tick、移动、建造、幽灵装备、伤害、胜负、快照广播。

Chat Scene
  世界聊天、房间聊天、公会聊天、私聊。

Guild Scene
  公会资料、成员、职位、申请、公告。
```

### Entity/Component 设计

```text
PlayerEntity
  PlayerProfileComponent
  PlayerSessionComponent
  PlayerUnlockComponent
  PlayerCurrencyComponent

RoomEntity
  RoomMapComponent
  RoomPlayerListComponent
  RoomLoadoutComponent
  RoomReadyComponent

BattleEntity
  BattlePhaseComponent
  BattleMapComponent
  BattleSnapshotComponent
  BattleTimerComponent

BattlePlayerEntity
  CampComponent
  PositionComponent
  HealthComponent
  ResourceWalletComponent
  ElfLoadoutComponent
  GhostEquipmentComponent

BattleBuildingEntity
  GridFootprintComponent
  HealthComponent
  BuildingConfigComponent
  CooldownComponent

BattleGhostEntity
  EquipmentBagComponent
  CombatStatsComponent
  RespawnComponent
```

### Fantasy 使用原则

- `Entity` 项目放协议生成、领域 Entity/Component、共享可序列化结构。
- `Hotfix` 项目放 Handler 和 System，不把所有状态塞静态 Service。
- 单局战斗必须逐步迁移到 Battle Scene，千人在线不能靠一个全局 `BattleService`。
- Tick 用 Fantasy TimerComponent/System，不手写散落后台循环。
- 跨 Scene 用 Route/Address/Roaming，特别是 Gate -> Room -> Battle。
- 聊天、邮件、任务、商城这些外层系统不要和 Battle Scene 耦合。

## 11. 服务端数据归属

```text
DB 持久化
  账号、角色、货币、解锁卡片、解锁角色、任务进度、邮件、公会。

Redis/缓存
  Token、在线状态、房间索引、聊天限频、临时匹配队列。

Battle Scene 内存
  单局状态、玩家位置、建筑、幽灵装备、伤害事件、快照。
```

## 12. 第一批要做的程序切片

建议按这个顺序做：

```text
1. 房间创建支持地图选择 UI。
2. RoomUI 显示地图名和推荐人数。
3. 战前精灵选择 6 张建筑卡。
4. BattleStartInfo 或 RoomPlayerInfo 携带玩家卡组。
5. BattleMainUI 只显示带入卡组。
6. 服务端 Build 校验玩家是否带入该建筑卡。
7. 幽灵角色选择。
8. 幽灵装备栏和局内商店。
9. 战斗胜负结算。
```

其中第 1-6 步是精灵卡组闭环，第 7-8 步是幽灵成长闭环，第 9 步让对局真正完整。

## 13. 不确定点

下面几个点需要你拍板：

```text
1. 第一版是固定阵营，还是生化感染模式？
   我建议先固定阵营。

2. 每局建筑卡数量是 6 张还是 8 张？
   我建议手游首版 6 张。

3. 幽灵装备是否第一版就做主动技能装备？
   我建议先只做被动属性装备。

4. 商店货币是否分软货币/付费货币？
   我建议先只做软货币，付费货币后置。

5. 单局目标人数先按几人？
   我建议先 4-6 人，稳定后扩到 8-12 人。
```

我的默认建议是：先做 4-6 人固定阵营 PvP，精灵 6 卡组，幽灵 6 装备格，地图房主选择，卡片只解锁不养成。

## 14. 代码注释约定

后续实现功能时需要补充必要注释，尤其是框架接入点和业务状态机。

必须注释：

```text
Fantasy Scene / Entity / Component / System 的职责边界
协议 Handler 为什么只做适配、具体业务转到哪里
战斗状态机、Tick、结算、感染、装备购买等关键流程
TEngine ResourceModule / ObjectPool / MemoryPool 的使用边界
地图规则、建造合法性、卡组校验等容易出错的规则
临时兼容代码和未来要迁移到 Scene/Entity 的过渡代码
```

不需要注释：

```text
简单赋值
明显的 UI 文本刷新
一眼可懂的 if/return
重复解释方法名已经说明的行为
```

注释风格：

```text
用中文写业务意图。
说明“为什么这样做”，少写“这一行在做什么”。
注释跟着代码一起维护，规则变了必须改注释。
```
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
