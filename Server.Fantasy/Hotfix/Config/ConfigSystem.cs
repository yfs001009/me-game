using Fantasy;
using GameConfig;
using Luban;

namespace Hotfix.Config;

/// <summary>
/// Server-side Luban config loader.
/// </summary>
public sealed class ConfigSystem
{
    private const string ConfigDirectoryName = "GameConfig";

    private static readonly Lazy<ConfigSystem> LazyInstance = new(() => new ConfigSystem());

    private Tables? tables;

    public static ConfigSystem Instance => LazyInstance.Value;

    public Tables Tables => tables ??= LoadTables();

    private ConfigSystem()
    {
    }

    private static Tables LoadTables()
    {
        return new Tables(LoadByteBuf);
    }

    private static ByteBuf LoadByteBuf(string file)
    {
        var fileName = file.EndsWith(".bytes", StringComparison.OrdinalIgnoreCase) ? file : $"{file}.bytes";
        var path = ResolveConfigPath(fileName);
        Log.Info($"Load server config: {path}");
        return new ByteBuf(File.ReadAllBytes(path));
    }

    private static string ResolveConfigPath(string fileName)
    {
        foreach (var candidate in EnumerateCandidatePaths(fileName))
        {
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        throw new FileNotFoundException($"Config file not found: {fileName}");
    }

    private static IEnumerable<string> EnumerateCandidatePaths(string fileName)
    {
        foreach (var root in EnumerateSearchRoots())
        {
            yield return Path.Combine(root, ConfigDirectoryName, fileName);
            yield return Path.Combine(root, "Server.Fantasy", ConfigDirectoryName, fileName);
            yield return Path.Combine(root, "SheepBattle", "Server.Fantasy", ConfigDirectoryName, fileName);
        }
    }

    private static IEnumerable<string> EnumerateSearchRoots()
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            var directory = new DirectoryInfo(start);
            while (directory != null)
            {
                if (seen.Add(directory.FullName))
                {
                    yield return directory.FullName;
                }

                directory = directory.Parent;
            }
        }
    }
}
