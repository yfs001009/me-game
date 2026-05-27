# 下一步

最后整理：2026-05-26。

当前优先目标：做出第一个完整 PvP 玩法切片。

```text
地图选择 + 精灵 6 卡组进战斗 + 服务端建造校验
```

## 1. 默认决策

```text
1. 第一版固定阵营 PvP，不做随机感染。
2. 首版按 4-6 人调试。
3. 精灵每局带 6 张建筑卡。
4. 幽灵装备首版只做被动属性，不做主动装备。
5. 商店先只做软货币解锁，不接真实付费。
```

## 2. 第一功能切片

### 客户端

```text
1. 新增 Loadout 模块。
2. 房间创建支持地图选择。
3. RoomUI 显示地图名、人数范围、玩家阵营/准备状态。
4. 战前精灵选择 6 张建筑卡。
5. BattleMainUI 只显示带入卡组，不显示全部 TbBuildingCard。
6. 给新增代码补关键注释，说明客户端过滤只是提示，服务端校验才权威。
```

### 服务端

```text
1. 协议增加 RoomLoadoutInfo / 玩家卡组字段。
2. Room 记录玩家阵营、精灵卡组、幽灵角色。
3. StartRoom 固化 Loadout 到 Battle。
4. BuildCommand 校验玩家是否带入对应建筑卡。
5. 保留当前 Service 过渡，但补注释说明后续迁移 Entity/Scene。
```

### 配置

```text
1. 复用 map.xlsx 的地图配置。
2. 复用 building_card.xlsx 的 UnlockRule 字段。
3. 如字段不够，再加 loadout 默认配置表，不先做卡牌养成表。
```

## 3. 第二功能切片

```text
幽灵角色选择
幽灵金币
6 格装备栏
局内装备商店
购买装备后属性生效
```

首版装备只做被动属性：

```text
攻击
血量
移速
破墙
抗性
冷却
```

## 4. 第三功能切片

```text
战斗胜负结算
S2C_BattleResultPush
返回大厅
任务进度结算
邮件发奖励预留
```

## 5. 框架改造穿插

每做一个切片同步检查：

```text
客户端是否用 TEngine UI/Event/Resource。
战斗表现是否避免反复 Instantiate/Destroy。
临时对象是否适合 MemoryPool。
服务端状态是否有 Entity/Component 归属。
Handler 是否只做协议适配。
Tick 是否朝 Timer/System 迁移。
```

短期先不做全量大重构，优先保证现有登录、房间、战斗链路不断。

## 6. 当前必测链路

```text
1. 启动服务端。
2. 两个客户端登录不同账号。
3. A 创建房间并选择地图。
4. B 加入房间并准备。
5. A 开始游戏。
6. 两端进入 BattleMainUI。
7. 验证只显示带入卡组。
8. 验证带入卡可建造，未带入卡服务端拒绝。
9. 验证移动、升级、回收仍正常。
```

如果失败，先看：

```text
服务端 BattleService / RoomService 日志
客户端 SheepNetworkService 日志
客户端 BattleController 日志
CommonNoticeUI 弹窗内容
```
