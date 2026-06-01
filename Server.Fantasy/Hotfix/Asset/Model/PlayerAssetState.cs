namespace Hotfix.Asset.Model;

public sealed class PlayerAssetState
{
    public Dictionary<int, long> Currencies { get; } = new();
    public ItemContainer Bag { get; } = new();
    public HashSet<int> UnlockedCharacterIds { get; } = new();
    public HashSet<int> UnlockedBuildingCardIds { get; } = new();
    public Dictionary<string, DateTimeOffset> BuffExpiresAtUtc { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, long> ProgressValues { get; } = new(StringComparer.OrdinalIgnoreCase);
    public int AssetSchemaVersion { get; set; }
}
