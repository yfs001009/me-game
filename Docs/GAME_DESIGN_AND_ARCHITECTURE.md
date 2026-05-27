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
