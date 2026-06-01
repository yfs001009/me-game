using System.Collections.Generic;
using System.Linq;
using Fantasy;

namespace GameLogic.SheepBattle.Shop
{
    public sealed class ShopViewModel
    {
        public List<ShopEntryViewModel> Shops { get; } = new();
        public List<ShopGoodsEntryViewModel> Goods { get; } = new();
        public string ShopType { get; private set; } = string.Empty;
        public string ActivityId { get; private set; } = string.Empty;
        public string FeatureId { get; private set; } = string.Empty;

        public void Apply(IReadOnlyList<OutgameShopInfo> shops, string shopType, string activityId, string featureId)
        {
            ShopType = shopType ?? string.Empty;
            ActivityId = activityId ?? string.Empty;
            FeatureId = featureId ?? string.Empty;
            Shops.Clear();
            Goods.Clear();
            if (shops == null)
            {
                return;
            }

            Shops.AddRange(shops.Select(v => new ShopEntryViewModel(v)));
            Goods.AddRange(Shops.SelectMany(v => v.Goods));
        }
    }

    public sealed class ShopEntryViewModel
    {
        public ShopEntryViewModel(OutgameShopInfo info)
        {
            ShopId = info?.ShopId ?? 0;
            ShopName = info?.ShopName ?? string.Empty;
            ShopType = info?.ShopType ?? string.Empty;
            ActivityId = info?.ActivityId ?? string.Empty;
            RefreshGroup = info?.RefreshGroup ?? string.Empty;
            FeatureId = info?.FeatureId ?? string.Empty;
            if (info?.Goods != null)
            {
                Goods.AddRange(info.Goods.Select(v => new ShopGoodsEntryViewModel(v, ShopName)));
            }
        }

        public int ShopId { get; }
        public string ShopName { get; }
        public string ShopType { get; }
        public string ActivityId { get; }
        public string RefreshGroup { get; }
        public string FeatureId { get; }
        public List<ShopGoodsEntryViewModel> Goods { get; } = new();
    }

    public sealed class ShopGoodsEntryViewModel
    {
        public ShopGoodsEntryViewModel(OutgameShopGoodsInfo info, string shopName)
        {
            GoodsId = info?.GoodsId ?? 0;
            ShopId = info?.ShopId ?? 0;
            GoodsGroupId = info?.GoodsGroupId ?? 0;
            ShopName = shopName ?? string.Empty;
            Name = info?.Name ?? string.Empty;
            PriceCurrencyId = info?.PriceCurrencyId ?? 0;
            PriceAmount = info?.PriceAmount ?? 0;
            BuyLimit = info?.BuyLimit ?? 0;
            BoughtCount = info?.BoughtCount ?? 0;
            IsAvailable = info?.IsAvailable ?? false;
            UnlockRule = info?.UnlockRule ?? string.Empty;
            Description = info?.Description ?? string.Empty;
            Reward = info?.Reward;
            FeatureId = info?.FeatureId ?? string.Empty;
        }

        public int GoodsId { get; }
        public int ShopId { get; }
        public int GoodsGroupId { get; }
        public string ShopName { get; }
        public string Name { get; }
        public int PriceCurrencyId { get; }
        public long PriceAmount { get; }
        public int BuyLimit { get; }
        public int BoughtCount { get; }
        public bool IsAvailable { get; }
        public string UnlockRule { get; }
        public string Description { get; }
        public RewardInfo Reward { get; }
        public string FeatureId { get; }
        public bool SoldOut => BuyLimit > 0 && BoughtCount >= BuyLimit;
        public string LimitText => BuyLimit > 0 ? $"{BoughtCount}/{BuyLimit}" : "不限";
    }
}
