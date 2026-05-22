# Luban 配置流程

## 1. 当前目录

配置工具链位于：

```text
Tools.Config/GameConfig/
├── Datas/                         # Excel 源表
├── Defines/                       # Luban 定义
├── CustomTemplate/                # TEngine 配置模板
├── gen_code_bin_to_project.bat    # 导出到 Unity
├── gen_code_bin_to_project_lazyload.bat
└── gen_code_bin_to_server.bat     # 导出到服务端
```

Luban 编译器本体位于：

```text
Tools/Luban/Luban.dll
```

当前已有源表：

```text
__beans__.xlsx
__enums__.xlsx
__tables__.xlsx
game_rule.xlsx
```

当前已清理掉 Luban 示例表，`__beans__.xlsx` 和 `__enums__.xlsx` 仅保留表头；业务配置先从 `common.TbGameRule` 开始。

## 2. 生成目标

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

具体以 `Tools.Config/GameConfig/*.bat` 中的输出路径为准。

## 3. 开发规则

- 配置源只改 `Tools.Config/GameConfig/Datas` 和 `Defines`。
- 不手改生成代码和生成二进制。
- 业务层不要散落直接访问表对象，后续统一收敛到 `ConfigService`。
- UI 只展示配置转换后的 ViewModel。
- 没有业务使用的 demo 表、demo bean、demo enum 不保留。

## 4. 后续表规划

按业务模块逐步添加：

```text
Lobby: activity, shop, task
Room: game_mode, map
Battle: building, building_level, unit, skill, buff
Common: error_code, localization
```

## 5. 校验规则

导表后至少校验：

- Id 唯一
- 引用存在
- 图标和 prefab 资源名非空
- 数值范围合法
- 客户端和服务端配置版本一致

## 6. 当前验证

```powershell
cd Tools.Config\GameConfig
.\gen_code_bin_to_project.bat
.\gen_code_bin_to_server.bat
```

两条导表链路已通过。当前生成内容只包含：

```text
common.TbGameRule
common_tbgamerule.bytes
```
