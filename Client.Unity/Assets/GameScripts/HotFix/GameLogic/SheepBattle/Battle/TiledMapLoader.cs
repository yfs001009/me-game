using Fantasy.Async;
using System.Collections.Generic;
using TEngine;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
#endif

namespace GameLogic.SheepBattle.Battle
{
    public static class TiledMapLoader
    {
        private static readonly Dictionary<string, Texture2D> TextureCache = new();

        public static async FTask<TiledMapData> LoadAsync(string assetName)
        {
            assetName = NormalizeAssetName(assetName);
            var textAsset = await LoadTextAssetAsync(assetName);
            if (textAsset == null)
            {
                Log.Warning($"Tiled map asset not found: {assetName}. Using fallback preview map.");
                return CreateFallback(assetName);
            }

            var map = JsonUtility.FromJson<TiledMapData>(textAsset.text);
            if (map == null || map.width <= 0 || map.height <= 0)
            {
                Log.Warning($"Tiled map asset invalid: {assetName}. Using fallback preview map.");
                return CreateFallback(assetName);
            }

            map.AssetName = assetName;
            return map;
        }

        public static async FTask<Texture2D> LoadTilesetTextureAsync(TiledTilesetData tileset)
        {
            if (tileset == null)
            {
                return null;
            }

            var assetName = tileset.ImageAssetName;
            if (string.IsNullOrWhiteSpace(assetName))
            {
                return null;
            }

            if (TextureCache.TryGetValue(assetName, out var cached) && cached != null)
            {
                return cached;
            }

#if UNITY_EDITOR
            var editorTexture = LoadEditorTexture(assetName);
            if (editorTexture != null)
            {
                TextureCache[assetName] = editorTexture;
                return editorTexture;
            }
#endif

            foreach (var location in GetTextureCandidateLocations(assetName))
            {
                if (!GameModule.Resource.CheckLocationValid(location))
                {
                    continue;
                }

                var texture = await GameModule.Resource.LoadAssetAsync<Texture2D>(location);
                if (texture != null)
                {
                    TextureCache[assetName] = texture;
                    return texture;
                }
            }

#if UNITY_EDITOR
            return LoadEditorTexture(assetName);
#else
            return null;
#endif
        }

        private static async FTask<TextAsset> LoadTextAssetAsync(string assetName)
        {
#if UNITY_EDITOR
            var editorAsset = LoadEditorTextAsset(assetName);
            if (editorAsset != null)
            {
                return editorAsset;
            }
#endif

            foreach (var location in GetCandidateLocations(assetName))
            {
                if (!GameModule.Resource.CheckLocationValid(location))
                {
                    continue;
                }

                var textAsset = await GameModule.Resource.LoadAssetAsync<TextAsset>(location);
                if (textAsset != null)
                {
                    return textAsset;
                }
            }

#if UNITY_EDITOR
            return LoadEditorTextAsset(assetName);
#else
            return null;
#endif
        }

#if UNITY_EDITOR
        private static TextAsset LoadEditorTextAsset(string assetName)
        {
            var fileName = assetName.EndsWith(".json") ? assetName : $"{assetName}.json";
            var path = Path.Combine(Application.dataPath, "AssetRaw", "Maps", fileName);
            if (File.Exists(path))
            {
                return new TextAsset(File.ReadAllText(path));
            }

            return null;
        }
#endif

        private static string[] GetCandidateLocations(string assetName)
        {
            assetName = NormalizeAssetName(assetName);
            var fileName = assetName.EndsWith(".json") ? assetName : $"{assetName}.json";
            var bareName = assetName.EndsWith(".json") ? assetName.Substring(0, assetName.Length - 5) : assetName;
            return new[]
            {
                fileName,
                $"Maps/{fileName}",
                $"Assets/AssetRaw/Maps/{fileName}",
                bareName
            };
        }

        private static string NormalizeAssetName(string assetName)
        {
            if (string.IsNullOrWhiteSpace(assetName))
            {
                return "battle_map_1";
            }

            var normalized = assetName.Replace("\\", "/").Trim();
            var slashIndex = normalized.LastIndexOf('/');
            if (slashIndex >= 0)
            {
                normalized = normalized.Substring(slashIndex + 1);
            }

            if (normalized.EndsWith(".json", System.StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring(0, normalized.Length - 5);
            }

            return string.IsNullOrWhiteSpace(normalized) ? "battle_map_1" : normalized;
        }

        private static string[] GetTextureCandidateLocations(string assetName)
        {
            var fileName = assetName.EndsWith(".png") ? assetName : $"{assetName}.png";
            var bareName = assetName.EndsWith(".png") ? assetName.Substring(0, assetName.Length - 4) : assetName;
            return new[]
            {
                assetName,
                fileName,
                $"MapTiles/{fileName}",
                $"Assets/AssetRaw/MapTiles/{fileName}",
                bareName
            };
        }

#if UNITY_EDITOR
        private static Texture2D LoadEditorTexture(string assetName)
        {
            var fileName = assetName.EndsWith(".png") ? assetName : $"{assetName}.png";
            var path = Path.Combine(Application.dataPath, "AssetRaw", "MapTiles", fileName);
            if (!File.Exists(path))
            {
                return null;
            }

            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            texture.name = assetName;
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.LoadImage(File.ReadAllBytes(path));
            return texture;
        }
#endif

        private static TiledMapData CreateFallback(string assetName)
        {
            return new TiledMapData
            {
                AssetName = assetName,
                width = 16,
                height = 9,
                tilewidth = 32,
                tileheight = 32,
                layers = new[]
                {
                    new TiledLayerData
                    {
                        name = "Ground",
                        type = "tilelayer",
                        width = 16,
                        height = 9,
                        data = CreateFallbackTiles(16, 9)
                    }
                }
            };
        }

        private static int[] CreateFallbackTiles(int width, int height)
        {
            var data = new int[width * height];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    data[y * width + x] = (x + y) % 3 + 1;
                }
            }

            return data;
        }
    }
}
