# Tiled 地图管线

## 目录约定

```text
Tools.Map/
├── Tiled/                  # Tiled 工具本体，不进入 Unity 打包
└── TiledProject/
    ├── Maps/               # .tmx 源地图
    └── Tilesets/           # .tsx 图块集源文件

Client.Unity/Assets/AssetRaw/
├── Maps/                   # Tiled 导出的 .json，运行时读取
├── MapTiles/               # 旧 JSON 预览链路使用的地图图块 PNG
├── TiledMaps/              # Unity 编辑器导入用 .tmx/.tsx/png
└── MapPrefabs/             # SuperTiled2Unity 导入/整理后的运行时地图 prefab
```

## 当前推荐管线

客户端视觉地图优先使用 Unity 资源，不再依赖运行时手动解析 JSON 拼地块：

```text
Tiled .tmx/.tsx/png
-> SuperTiled2Unity 在 Unity 编辑器内导入
-> Unity prefab/Tilemap 资源
-> YooAsset 打包
-> BattleController 运行时优先加载地图 prefab
```

当前已在 Unity 包中加入：

```text
com.seanba.super-tiled2unity@v2.3.0
```

Unity 内导入源文件放在：

```text
Client.Unity/Assets/AssetRaw/TiledMaps
```

运行时地图 prefab 建议放在：

```text
Client.Unity/Assets/AssetRaw/MapPrefabs
```

命名与 `map.xlsx` 的 `mapAsset` 一致，例如：

```text
mapAsset = battle_map_1
prefab = battle_map_1.prefab
```

`BattleController` 当前加载顺序：

1. 优先尝试 `MapPrefabs/battle_map_1.prefab`、`Assets/AssetRaw/MapPrefabs/battle_map_1.prefab` 等 Unity prefab 地址。
2. 找不到 prefab 时回退到 `Maps/battle_map_1.json` 旧预览链路。
3. 旧 JSON 链路仍可保留给规则解析、调试和服务端规则导出。

## 配表

- `map.xlsx`：地图 ID、地图资源名、玩法模式、最小人数、最大人数、推荐人数。
- `shop.xlsx`：局内商店定义。Tiled `shop` 对象层用 `shop_id` 引用。
- `shop_goods.xlsx`：商店售卖商品组。`shop.goodsGroupId` 关联这里的 `goodsGroupId`。
- `monster.xlsx`：怪物定义。Tiled `monster` 对象层用 `monster_id` 引用。

所有表都放在：

```text
Tools.Config/GameConfig/Datas
```

## Tiled 层约定

### Ground

图块层。普通地面显示层，运行时用于生成地图视觉占位。

### no_move

图块层。不可行走区域，同时也不可建造。

规则：

- 图层名必须是 `no_move`。
- `data` 中大于 0 的格子视为阻挡格。
- 运行时会把这些格子标记为不可行走，移动、寻路、碰撞都应以它为准。
- `no_move` 格子一定不可建造。
- 规则层本身不作为地图视觉显示。

### no_build

图块层。不可建造区域。

规则：

- 图层名必须是 `no_build`。
- `data` 中大于 0 的格子视为禁建格。
- 该层只限制建造，不限制移动。
- 规则层本身不作为地图视觉显示。

### birth_area

对象层。玩家出生和复活区域。

规则：

- 图层名必须是 `birth_area`。
- 对象建议用矩形。
- 玩家出生和复活时，在矩形区域内随机或按队伍规则选择点位。
- 当前运行时先解析并生成蓝色区域占位，后续接入真正出生点选择。

### shop

对象层。局内商店点。

规则：

- 图层名必须是 `shop`。
- 对象自定义属性：

```text
shop_id: int
```

兼容旧写法：

```text
shopid: int
```

运行时会用 `shop_id` 查 `shop.xlsx`，后续生成局内商店交互点。

### monster

对象层。怪物生成点。

规则：

- 图层名必须是 `monster`。
- 对象自定义属性：

```text
monster_id: int
```

兼容旧写法：

```text
monsterid: int
```

运行时会用 `monster_id` 查 `monster.xlsx`，后续生成对应怪物。

## JSON 导出格式

旧链路中，Tiled 地图导出为 JSON，放到：

```text
Client.Unity/Assets/AssetRaw/Maps
```

文件名与 `map.xlsx` 的 `mapAsset` 一致，例如：

```text
mapAsset = battle_map_1
JSON 文件 = battle_map_1.json
```

当前 JSON 链路先支持非压缩数据。对象层属性建议使用 Tiled 原生 int 类型。

## 当前接入状态

- `battle_map_1.json` 已作为当前战斗 MVP fallback 示例地图。
- `Client.Unity/Assets/AssetRaw/TiledMaps` 已保存一份 `.tmx/.tsx/png`，供 SuperTiled2Unity 在 Unity 编辑器内导入。
- 客户端 `TiledMapLoader` 会优先从资源系统读取地图 JSON，编辑器下也支持从 `Assets/AssetRaw/Maps` 兜底读取。
- `BattleController` 当前会优先加载 Unity 地图 prefab；找不到 prefab 时会生成快速 JSON 地图预览：
  - 地面：用整体 Quad 占位。
  - `no_move` / `no_build`：作为规则层读取，不生成可见方块。
  - `shop`：读取 `shop_id`，关联 `shop.xlsx`，生成商店点占位。
  - `monster`：读取 `monster_id`，关联 `monster.xlsx`，生成怪物点占位。
  - `birth_area`：生成蓝色出生区域占位。
- 服务端当前读取 TMX 的地图尺寸、出生区、`no_move` 和 `no_build` 规则层。移动会受 `no_move` 限制；建造会同时受 `no_move` 和 `no_build` 限制。

## 后续补齐

1. 在 Unity 编辑器内确认 SuperTiled2Unity 成功导入 `battle_map_1.tmx`，并把可运行地图 prefab 放到 `Assets/AssetRaw/MapPrefabs/battle_map_1.prefab`。
2. 地图导出时生成一份服务端可读的轻量地图数据，至少包含宽高、阻挡格、出生区。
3. 服务端创建战斗时按 `map.xlsx` 的 `mapAsset` 加载地图规则。
4. 玩家出生点从 `birth_area` 选择，不再使用临时坐标。
5. 怪物寻路统一使用 `no_move` 阻挡格。
