namespace Hotfix.Asset.Model;

public sealed class AssetReward
{
    public List<CurrencyAmount> Currencies { get; } = new();
    public List<ItemAmount> Items { get; } = new();
    public List<int> CharacterUnlocks { get; } = new();
    public List<int> BuildingCardUnlocks { get; } = new();
}

public readonly record struct CurrencyAmount(int CurrencyId, long Amount);

public readonly record struct ItemAmount(int ItemId, int Count);
