using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

public sealed class SheepBattleTiledTilesetImporter : AssetPostprocessor
{
    private const string TiledMapsFolder = "Assets/AssetRaw/TiledMaps";
    private const string TiledMapFilesFolder = "Assets/AssetRaw/TiledMaps/Maps";
    private const string TilesetsFolder = "Assets/AssetRaw/TiledMaps/Tilesets";
    private const string MapPrefabsFolder = "Assets/AssetRaw/MapPrefabs";

    private void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(TilesetsFolder) || !assetPath.EndsWith(".png"))
        {
            return;
        }

        var settings = FindTilesetSettings(assetPath);
        if (settings == null)
        {
            return;
        }

        var importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.spritePixelsPerUnit = settings.TileWidth;
        importer.spritesheet = CreateSprites(Path.GetFileNameWithoutExtension(assetPath), settings);
    }

    [MenuItem("SheepBattle/Maps/Reimport Tiled Tilesets")]
    public static void ReimportTiledTilesets()
    {
        var textures = AssetDatabase.FindAssets("t:Texture2D", new[] { TilesetsFolder });
        foreach (var guid in textures)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }

    [MenuItem("SheepBattle/Maps/Create Runtime Map Prefabs")]
    public static void CreateRuntimeMapPrefabs()
    {
        EnsureFolder("Assets/AssetRaw", "MapPrefabs");

        var mapGuids = AssetDatabase.FindAssets("t:GameObject", new[] { TiledMapFilesFolder });
        foreach (var guid in mapGuids)
        {
            var mapPath = AssetDatabase.GUIDToAssetPath(guid);
            if (!mapPath.EndsWith(".tmx"))
            {
                continue;
            }

            var mapAsset = AssetDatabase.LoadAssetAtPath<GameObject>(mapPath);
            if (mapAsset == null)
            {
                Debug.LogWarning($"Tiled map asset is not ready: {mapPath}");
                continue;
            }

            var prefabPath = $"{MapPrefabsFolder}/{Path.GetFileNameWithoutExtension(mapPath)}.prefab";
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(mapAsset);
            try
            {
                instance.name = Path.GetFileNameWithoutExtension(mapPath);
                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                Debug.Log($"Created runtime map prefab: {prefabPath}");
            }
            finally
            {
                Object.DestroyImmediate(instance);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void EnsureFolder(string parent, string folderName)
    {
        var path = $"{parent}/{folderName}";
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder(parent, folderName);
        }
    }

    private static SpriteMetaData[] CreateSprites(string assetName, TilesetImportSettings settings)
    {
        var sprites = new SpriteMetaData[settings.TileCount];
        for (var index = 0; index < settings.TileCount; index++)
        {
            var x = index % settings.Columns;
            var y = index / settings.Columns;
            sprites[index] = new SpriteMetaData
            {
                name = $"{assetName}_{index}",
                rect = new Rect(
                    x * settings.TileWidth,
                    y * settings.TileHeight,
                    settings.TileWidth,
                    settings.TileHeight),
                alignment = (int)SpriteAlignment.Custom,
                pivot = Vector2.zero
            };
        }

        return sprites;
    }

    private static TilesetImportSettings FindTilesetSettings(string textureAssetPath)
    {
        var textureFileName = Path.GetFileName(textureAssetPath);
        var result = FindTilesetSettingsInTmx(textureAssetPath, textureFileName);
        if (result != null)
        {
            return result;
        }

        return FindTilesetSettingsInTsx(textureFileName);
    }

    private static TilesetImportSettings FindTilesetSettingsInTmx(string textureAssetPath, string textureFileName)
    {
        var mapDirectory = Path.Combine(Application.dataPath, "AssetRaw", "TiledMaps", "Maps");
        if (!Directory.Exists(mapDirectory))
        {
            return null;
        }

        foreach (var mapPath in Directory.GetFiles(mapDirectory, "*.tmx", SearchOption.TopDirectoryOnly))
        {
            var doc = new XmlDocument();
            doc.Load(mapPath);
            var tilesets = doc.SelectNodes("//tileset");
            if (tilesets == null)
            {
                continue;
            }

            foreach (XmlElement tileset in tilesets)
            {
                var image = tileset.SelectSingleNode("image") as XmlElement;
                var imageSource = image?.GetAttribute("source");
                if (string.IsNullOrEmpty(imageSource) || Path.GetFileName(imageSource) != textureFileName)
                {
                    continue;
                }

                var resolvedPath = ResolveUnityAssetPath(Path.GetDirectoryName(mapPath), imageSource);
                if (resolvedPath != textureAssetPath)
                {
                    continue;
                }

                return ReadSettings(tileset);
            }
        }

        return null;
    }

    private static TilesetImportSettings FindTilesetSettingsInTsx(string textureFileName)
    {
        var tilesetDirectory = Path.Combine(Application.dataPath, "AssetRaw", "TiledMaps", "Tilesets");
        if (!Directory.Exists(tilesetDirectory))
        {
            return null;
        }

        foreach (var tilesetPath in Directory.GetFiles(tilesetDirectory, "*.tsx", SearchOption.TopDirectoryOnly))
        {
            var doc = new XmlDocument();
            doc.Load(tilesetPath);
            var tileset = doc.DocumentElement;
            var image = tileset?.SelectSingleNode("image") as XmlElement;
            var imageSource = image?.GetAttribute("source");
            if (string.IsNullOrEmpty(imageSource) || Path.GetFileName(imageSource) != textureFileName)
            {
                continue;
            }

            return ReadSettings(tileset);
        }

        return null;
    }

    private static string ResolveUnityAssetPath(string sourceDirectory, string relativePath)
    {
        var fullPath = Path.GetFullPath(Path.Combine(sourceDirectory, relativePath)).Replace('\\', '/');
        var dataPath = Application.dataPath.Replace('\\', '/');
        if (!fullPath.StartsWith(dataPath))
        {
            return string.Empty;
        }

        return "Assets" + fullPath.Substring(dataPath.Length);
    }

    private static TilesetImportSettings ReadSettings(XmlElement tileset)
    {
        return new TilesetImportSettings
        {
            TileWidth = ReadInt(tileset, "tilewidth"),
            TileHeight = ReadInt(tileset, "tileheight"),
            TileCount = ReadInt(tileset, "tilecount"),
            Columns = ReadInt(tileset, "columns")
        };
    }

    private static int ReadInt(XmlElement element, string attributeName)
    {
        return int.TryParse(element.GetAttribute(attributeName), out var value) ? value : 0;
    }

    private sealed class TilesetImportSettings
    {
        public int TileWidth;
        public int TileHeight;
        public int TileCount;
        public int Columns;
    }
}
