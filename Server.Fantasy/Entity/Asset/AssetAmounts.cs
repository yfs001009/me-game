namespace Fantasy;

public readonly record struct AssetCurrencyAmount(int CurrencyId, long Amount);

public readonly record struct AssetItemAmount(int ItemId, int Count);

