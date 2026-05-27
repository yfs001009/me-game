# SheepBattle 文档入口

最后整理：2026-05-26。

这份目录用于新接手项目时快速上手。建议按顺序阅读，不需要上下文也能理解当前目标、现状和下一步。

## 阅读顺序

1. [当前状态](STATUS.md)  
   当前已经做完什么、能跑什么、验证命令和已知风险。

2. [玩法与技术架构设计](GAME_DESIGN_AND_ARCHITECTURE.md)  
   游戏玩法、阵营、卡组、幽灵装备、商店/任务/好友/邮件/公会/聊天，以及 TEngine/Fantasy 总体架构。

3. [玩法系统与首版数值设计](GAME_DESIGN_NUMBERS.md)  
   首版 PvP 规则、建筑/幽灵/装备/经济/地图/外层系统数值，以及配置表规划。

4. [TEngine / Fantasy 框架使用体检](FRAMEWORK_USAGE_AUDIT.md)  
   当前哪些地方没用好框架、哪些要优先改、下一功能切片该怎么带框架边界。

5. [下一步](NEXT_STEPS.md)  
   当前短期开发顺序和测试重点。

## 技术分册

- [客户端架构](CLIENT_ARCHITECTURE.md)  
  Unity/TEngine/HotFix 入口、UI/MVE、战斗客户端、资源规则。

- [服务端架构](SERVER_ARCHITECTURE.md)  
  Fantasy Main/Entity/Hotfix、当前 Handler/Service、后续 Scene/Entity 迁移方向。

- [协议设计](PROTOCOL_DESIGN.md)  
  Fantasy proto 源、生成命令、当前协议、下一批协议和兼容规则。

- [Luban 配置流程](CONFIG_PIPELINE.md)  
  Excel/Defines、生成目标、客户端/服务端访问方式、配置规则。

- [Tiled 地图管线](TILED_MAP_PIPELINE.md)  
  Tiled、SuperTiled2Unity、地图 prefab、地图规则层、服务端规则导出方向。

## 文档维护规则

- 文档要记录“当前真实状态”和“下一步计划”，不要保留过期幻想稿。
- 如果新文档覆盖了旧文档，直接合并或删除旧文档。
- 改协议、配置、地图、框架边界时，同步更新对应分册。
- 写代码时需要补关键注释，尤其是 Fantasy Scene/Entity/System、TEngine Resource/ObjectPool/MemoryPool、战斗状态机和规则校验。

## 当前默认开发方向

```text
1. 固定阵营 PvP，先不做随机感染。
2. 首版按 4-6 人调试。
3. 精灵每局带 6 张建筑卡。
4. 幽灵 6 个装备格，首版装备只做被动属性。
5. 商店先做软货币解锁建筑卡/角色，不接真实付费。
6. 下一功能切片：地图选择 + 精灵 6 卡组进战斗。
```
