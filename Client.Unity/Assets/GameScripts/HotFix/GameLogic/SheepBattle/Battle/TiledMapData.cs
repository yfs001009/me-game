using System;

namespace GameLogic.SheepBattle.Battle
{
    [Serializable]
    public sealed class TiledMapData
    {
        public string AssetName;
        public int width;
        public int height;
        public int tilewidth;
        public int tileheight;
        public TiledLayerData[] layers;
        public TiledTilesetData[] tilesets;

        public int Width => width;
        public int Height => height;
        public int TileWidth => tilewidth;
        public int TileHeight => tileheight;

        public int GetVisualTileId(int x, int y)
        {
            if (layers == null || layers.Length == 0)
            {
                return 0;
            }

            for (var i = 0; i < layers.Length; i++)
            {
                var layer = layers[i];
                if (layer == null || layer.data == null || layer.data.Length == 0)
                {
                    continue;
                }

                if (IsRuleLayer(layer.name))
                {
                    continue;
                }

                var layerWidth = layer.width > 0 ? layer.width : width;
                var index = y * layerWidth + x;
                if (index >= 0 && index < layer.data.Length && layer.data[index] > 0)
                {
                    return layer.data[index];
                }
            }

            return 0;
        }

        public bool IsWallTile(int x, int y)
        {
            return IsNoMoveTile(x, y);
        }

        public bool IsNoMoveTile(int x, int y)
        {
            return GetLayerTileId(GetLayer("no_move"), x, y) > 0;
        }

        public bool IsNoBuildTile(int x, int y)
        {
            return GetLayerTileId(GetLayer("no_build"), x, y) > 0;
        }

        public bool IsBuildForbiddenTile(int x, int y)
        {
            return IsNoMoveTile(x, y) || IsNoBuildTile(x, y);
        }

        public TiledLayerData GetLayer(string layerName)
        {
            if (layers == null)
            {
                return null;
            }

            for (var i = 0; i < layers.Length; i++)
            {
                var layer = layers[i];
                if (layer != null && layer.name == layerName)
                {
                    return layer;
                }
            }

            return null;
        }

        private static bool IsRuleLayer(string layerName)
        {
            return layerName == "no_move" || layerName == "no_build";
        }

        private int GetLayerTileId(TiledLayerData layer, int x, int y)
        {
            if (layer?.data == null || layer.data.Length == 0)
            {
                return 0;
            }

            var layerWidth = layer.width > 0 ? layer.width : width;
            var index = y * layerWidth + x;
            return index >= 0 && index < layer.data.Length ? layer.data[index] : 0;
        }

        public TiledTilesetData GetTileset(int globalTileId)
        {
            if (tilesets == null || globalTileId <= 0)
            {
                return null;
            }

            TiledTilesetData result = null;
            for (var i = 0; i < tilesets.Length; i++)
            {
                var tileset = tilesets[i];
                if (tileset != null && globalTileId >= tileset.firstgid)
                {
                    result = tileset;
                }
            }

            return result;
        }
    }

    [Serializable]
    public sealed class TiledLayerData
    {
        public string name;
        public string type;
        public bool visible = true;
        public float opacity = 1f;
        public int width;
        public int height;
        public int[] data;
        public TiledObjectData[] objects;

        public int GetTileId(int mapWidth, int x, int y)
        {
            if (data == null || data.Length == 0)
            {
                return 0;
            }

            var layerWidth = width > 0 ? width : mapWidth;
            var index = y * layerWidth + x;
            return index >= 0 && index < data.Length ? data[index] : 0;
        }
    }

    [Serializable]
    public sealed class TiledObjectData
    {
        public int id;
        public string name;
        public string type;
        public float x;
        public float y;
        public float width;
        public float height;
        public TiledPropertyData[] properties;

        public int GetIntProperty(string propertyName, int defaultValue = 0)
        {
            if (properties == null)
            {
                return defaultValue;
            }

            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                if (property != null && property.name == propertyName && int.TryParse(property.ValueText, out var value))
                {
                    return value;
                }
            }

            return defaultValue;
        }
    }

    [Serializable]
    public sealed class TiledTilesetData
    {
        public int firstgid;
        public string name;
        public string image;
        public int imagewidth;
        public int imageheight;
        public int tilewidth;
        public int tileheight;
        public int tilecount;
        public int columns;
        public int margin;
        public int spacing;

        public int LocalTileId(int globalTileId)
        {
            return Math.Max(0, globalTileId - firstgid);
        }

        public string ImageAssetName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(image))
                {
                    return name;
                }

                var normalized = image.Replace("\\", "/");
                var slashIndex = normalized.LastIndexOf("/", StringComparison.Ordinal);
                var fileName = slashIndex >= 0 ? normalized.Substring(slashIndex + 1) : normalized;
                return fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                    ? fileName.Substring(0, fileName.Length - 4)
                    : fileName;
            }
        }
    }

    [Serializable]
    public sealed class TiledPropertyData
    {
        public string name;
        public string type;
        public int value;

        public string ValueText => value.ToString();
    }
}
