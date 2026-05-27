# Tools.Map

Tiled 地图编辑工作区。

## 打开方式

双击：

```text
OpenTiled.bat
```

它会启动：

```text
Tools.Map/Tiled/tiled.exe
```

并打开项目：

```text
Tools.Map/TiledProject/SheepBattle.tiled-project
```

## 目录

```text
Tools.Map/
├── Tiled/                  # Tiled 工具本体
├── OpenTiled.bat           # 启动 Tiled 并打开项目
├── ExportMaps.bat          # 批量导出 .tmx 到 Unity 运行时 JSON
└── TiledProject/
    ├── SheepBattle.tiled-project
    ├── Maps/               # .tmx 源地图
    ├── Tilesets/           # .tsx 图块集
    └── Exports/            # 可选临时导出目录
```

Unity 运行时读取的正式导出文件放在：

```text
Client.Unity/Assets/AssetRaw/Maps
```

地图图块素材放在：

```text
Client.Unity/Assets/AssetRaw/MapTiles
```

## 导出地图

编辑 `.tmx` 后双击：

```text
ExportMaps.bat
```

它会把：

```text
Tools.Map/TiledProject/Maps/*.tmx
```

导出到：

```text
Client.Unity/Assets/AssetRaw/Maps/*.json
```
