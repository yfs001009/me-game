using Fantasy;
using Fantasy.Entitas;

namespace Fantasy;

/// <summary>
/// Per-player outgame shop state. Static goods come from config; this aggregate
/// stores purchase limits and refresh ownership.
/// </summary>
public sealed class PlayerOutgameShopEntity : Entity
{
    public long PlayerId { get; set; }
    public Dictionary<int, OutgameShopPurchaseRecord> PurchasesByGoodsId { get; } = new();
}

public sealed class OutgameShopPurchaseRecord
{
    public int GoodsId { get; set; }
    public string RefreshGroup { get; set; } = string.Empty;
    public int BoughtCount { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}

