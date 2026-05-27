# 配置表设计蓝图

最后整理：2026-05-27。

本文把玩法系统和数值拆成可落地的 Luban 配置表。原则：能配置化的都配置化；服务端和客户端使用同一批配置 bytes；代码只负责读取配置、执行状态机和做权威校验。

## 1. 配置化原则

必须配置化：

```text
战斗模式、阶段时长、胜负条件
地图资源、人数范围、出生/阻挡/禁建规则
精灵/幽灵角色基础属性
建筑卡、建筑属性、建造消耗、冷却、升级
幽灵装备、价格、属性、出售比例
卡组槽位、默认卡组、解锁条件
商城商品、价格、上下架
任务条件、奖励、重置周期
邮件模板
聊天频道、等级限制、发言间隔、字数限制
```

不要写死：

```text
建筑 ID
装备 ID
角色属性
商品价格
地图人数
任务奖励
战斗时长
解锁条件
```

可以写代码：

```text
状态机推进
范围/碰撞/路径计算
服务端权威校验
协议收发
UI 展示逻辑
配置读取封装
```

## 2. 表优先级

### P0：做“地图选择 + 精灵 6 卡组”必须有

```text
battle_mode.xlsx
loadout_rule.xlsx
unlock.xlsx
```

复用已有：

```text
map.xlsx
building.xlsx
building_card.xlsx
building_level.xlsx
game_rule.xlsx
```

### P1：做“幽灵装备”必须有

```text
role.xlsx
ghost_equipment.xlsx
```

### P2：做“外层商店/任务/邮件”必须有

```text
shop.xlsx
shop_goods.xlsx
task.xlsx
mail_template.xlsx
```

### P3：做“聊天/公会/好友”必须有

```text
chat_rule.xlsx
guild_rule.xlsx
friend_rule.xlsx
```

## 3. 现有表继续使用

### game_rule.xlsx

用途：全局规则和临时开关。

适合放：

```text
DefaultMapId
BattleDurationSeconds
BattlePrepareSeconds
BuildRange
CustomRoomMinPlayers
CustomRoomMaxPlayers
CustomRoomDefaultPlayers
WaitingSoloRoomTtlSeconds
```

不适合放：

```text
大量建筑数值
大量装备数值
大量任务配置
大量商品配置
```

这些应该拆到业务表。

### map.xlsx

用途：地图基础配置。

现有字段：

```text
MapId
MapName
MapAsset
Mode
MinPlayers
MaxPlayers
RecommendedPlayers
TiledObjectLayers
Comment
EffectDesc
```

建议后续补充：

```text
Width
Height
MapPreviewAsset
MapDifficulty
DefaultBattleModeId
ElfSpawnLayer
GhostSpawnLayer
NoMoveLayer
NoBuildLayer
ShopLayer
ResourceLayer
```

### building.xlsx

用途：建筑基础属性。

建议字段：

```text
BuildingId
BuildingName
BuildingType
FootprintWidth
FootprintHeight
BaseHp
CanBlockPath
CanUpgrade
MaxLevel
Attack
AttackRange
AttackIntervalMs
RepairValue
EffectRadius
RecyclePercent
PrefabAsset
IconAsset
EffectDesc
```

### building_card.xlsx

用途：建筑卡显示、建造消耗、解锁规则、战前带卡。

现有字段：

```text
CardId
BuildingId
CardName
IconAsset
SortOrder
UnlockRule
CostGold
CostWood
CooldownMs
Description
EffectDesc
```

建议补充：

```text
Rarity
DefaultUnlocked
ShopGoodsId
RecommendedTag
AvailableModes
```

说明：

```text
卡片只决定“本局能造什么”。
建造时服务端用 BuildingId 找 card 和 building 校验。
```

### building_level.xlsx

用途：建筑升级。

建议字段：

```text
LevelId
BuildingId
Level
NextLevelId
UpgradeCostGold
UpgradeCostWood
Hp
Attack
AttackRange
AttackIntervalMs
RepairValue
EffectDesc
```

### shop.xlsx / shop_goods.xlsx

用途：商店和商品。

首版建议同时覆盖：

```text
外层商店：购买建筑卡/角色/皮肤。
局内幽灵商店：购买幽灵装备。
```

如果一张表太复杂，后续可拆成：

```text
mall_shop.xlsx
mall_goods.xlsx
battle_shop.xlsx
battle_shop_goods.xlsx
```

## 4. 新增表设计

### battle_mode.xlsx

用途：定义 PvP 模式。

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| ModeId | int | 模式 ID |
| ModeKey | string | 代码用 key，如 ClassicPvp |
| ModeName | string | 显示名 |
| MinPlayers | int | 最少人数 |
| MaxPlayers | int | 最大人数 |
| RecommendedElfCount | int | 推荐精灵数 |
| RecommendedGhostCount | int | 推荐幽灵数 |
| PrepareSeconds | int | 准备期 |
| BattleSeconds | int | 战斗时长 |
| SettleSeconds | int | 结算展示时长 |
| ElfWinCondition | string | 精灵胜利条件 |
| GhostWinCondition | string | 幽灵胜利条件 |
| DefaultMapPool | array<int> | 默认地图池 |
| DefaultLoadoutRuleId | int | 默认卡组规则 |
| Comment | string | 备注 |

首版数据：

| ModeId | ModeKey | ModeName | Min | Max | Elf | Ghost | Prepare | Battle |
| ---: | --- | --- | ---: | ---: | ---: | ---: | ---: | ---: |
| 1 | ClassicPvp | 经典对抗 | 4 | 6 | 4 | 1 | 45 | 600 |

### loadout_rule.xlsx

用途：定义战前带入规则。

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| RuleId | int | 规则 ID |
| Camp | string | Elf/Ghost |
| SlotCount | int | 槽位数 |
| DefaultCardIds | array<int> | 默认卡组 |
| MaxDuplicateCount | int | 同卡最多重复数，首版为 1 |
| AllowRoleIds | array<int> | 可选角色 |
| RequiredCardTags | array<string> | 可选，要求至少带某类卡 |
| Description | string | 说明 |

首版数据：

```text
RuleId = 1
Camp = Elf
SlotCount = 6
DefaultCardIds = 木墙, 基础箭塔, 农场, 维修站, 地刺, 侦察灯
MaxDuplicateCount = 1
```

### unlock.xlsx

用途：统一定义解锁条件。

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| UnlockId | int | 解锁 ID |
| UnlockType | string | Default/Level/Task/Shop/Event |
| TargetType | string | BuildingCard/ElfRole/GhostRole/Skin |
| TargetId | int | 目标 ID |
| RequireLevel | int | 等级要求 |
| RequireTaskId | int | 任务要求 |
| RequireGoodsId | int | 商品要求 |
| DefaultUnlocked | bool | 是否默认解锁 |
| Description | string | 说明 |

说明：

```text
客户端用于显示“已解锁/未解锁”。
服务端用于权威校验玩家是否拥有。
```

### role.xlsx

用途：精灵/幽灵角色。

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| RoleId | int | 角色 ID |
| RoleName | string | 显示名 |
| Camp | string | Elf/Ghost |
| UnlockId | int | 解锁配置 |
| ShopGoodsId | int | 商店商品 |
| Hp | int | 生命 |
| MoveSpeed | float | 移速 |
| Attack | int | 攻击 |
| AttackIntervalMs | int | 攻击间隔 |
| AttackRange | float | 攻击距离 |
| SkillIds | array<int> | 技能 ID |
| PrefabAsset | string | 表现资源 |
| IconAsset | string | 图标 |
| Description | string | 说明 |

首版：

```text
1001 默认精灵
2001 破墙者幽灵
```

### ghost_equipment.xlsx

用途：幽灵局内装备。

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| EquipmentId | int | 装备 ID |
| EquipmentName | string | 显示名 |
| Quality | int | 品质 |
| Price | int | 购买价格 |
| SellPercent | int | 出售返还比例 |
| SlotType | string | Attack/Defense/Move/Utility/Aura |
| AttackAdd | int | 攻击加值 |
| HpAdd | int | 生命加值 |
| MoveSpeedAdd | float | 移速加值 |
| AttackIntervalPercent | int | 攻击间隔百分比修正 |
| BuildingDamagePercent | int | 对建筑伤害百分比 |
| TowerDamageReducePercent | int | 受到塔伤害降低 |
| HpRegenPerSecond | int | 每秒回血 |
| AuraId | int | 光环 ID |
| IconAsset | string | 图标 |
| Description | string | 说明 |

首版装备：

```text
粗糙利爪
破墙斧
厚皮甲
疾行靴
再生血肉
腐蚀核心
抗塔披风
狂暴心脏
恐惧光环
巨力骨锤
```

### task.xlsx

用途：任务。

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| TaskId | int | 任务 ID |
| TaskName | string | 显示名 |
| TaskType | string | Daily/Achievement/Guide |
| ConditionType | string | Login/FinishBattle/Win/Build/Destroy/BuyEquipment |
| ConditionParam | string | 条件参数 |
| TargetCount | int | 目标次数 |
| RewardSoftCurrency | int | 软货币奖励 |
| RewardItemIds | array<int> | 奖励物品 |
| RewardItemCounts | array<int> | 奖励数量 |
| ResetType | string | None/Daily/Weekly |
| Description | string | 说明 |

### mail_template.xlsx

用途：系统邮件模板。

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| MailTemplateId | int | 模板 ID |
| Title | string | 标题 |
| Content | string | 内容 |
| ExpireDays | int | 过期天数 |
| AttachmentItemIds | array<int> | 附件 |
| AttachmentCounts | array<int> | 数量 |
| Description | string | 说明 |

### chat_rule.xlsx

用途：聊天规则。

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| Channel | string | World/Room/Guild/Private/System |
| MinLevel | int | 最低等级 |
| SendIntervalMs | int | 发言间隔 |
| DailyLimit | int | 每日上限 |
| MaxLength | int | 最大长度 |
| NeedFilter | bool | 是否敏感词过滤 |
| MuteSecondsOnSpam | int | 刷屏禁言秒数 |
| Description | string | 说明 |

### guild_rule.xlsx

用途：公会基础规则。

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| RuleId | int | 规则 ID |
| CreateCost | int | 创建费用 |
| MinCreateLevel | int | 创建等级 |
| MaxMembers | int | 初始人数上限 |
| ApplyExpireHours | int | 申请过期 |
| NoticeMaxLength | int | 公告长度 |
| Description | string | 说明 |

### friend_rule.xlsx

用途：好友基础规则。

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| RuleId | int | 规则 ID |
| MaxFriends | int | 好友上限 |
| ApplyExpireHours | int | 申请过期 |
| MaxPrivateMessageLength | int | 私聊长度 |
| InviteRoomCooldownSeconds | int | 邀请冷却 |
| Description | string | 说明 |

## 5. 生成和使用要求

新增表后要做：

```text
1. 更新 __tables__.xlsx。
2. 必要时更新 __beans__.xlsx / __enums__.xlsx。
3. 运行 gen_code_bin_to_project.bat。
4. 运行 gen_code_bin_to_server.bat。
5. 客户端通过 ConfigSystem / GameRuleService 或专用配置服务读取。
6. 服务端通过 ConfigSystem / SheepServices.Rules 或专用规则服务读取。
```

不要做：

```text
不要手改生成代码。
不要客户端和服务端用不同配置。
不要把配置 ID 写死在业务里，至少集中到默认规则配置。
不要把中文 Excel 用不安全脚本写成乱码。
```

## 6. 第一批落地顺序

```text
1. battle_mode.xlsx
2. loadout_rule.xlsx
3. unlock.xlsx
4. role.xlsx
5. ghost_equipment.xlsx
```

第一批完成后，就能支撑：

```text
地图选择
精灵 6 卡组
默认解锁
幽灵角色
幽灵装备
```

外层商店/任务/邮件/聊天/公会可以第二批再做。
