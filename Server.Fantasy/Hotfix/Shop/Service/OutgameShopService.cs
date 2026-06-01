using Fantasy;
using Fantasy.Entitas;
using GameConfig.shop;
using Hotfix.Asset.Model;
using Hotfix.Config;
using Hotfix.Mail.Service;
using Hotfix.Shared;

namespace Hotfix.Shop.Service;

public sealed class OutgameShopService
{
    private const int GoldCurrencyId = 1;
    private const int DiamondCurrencyId = 2;
    private const int EventTokenCurrencyId = 3;
    private const int DefaultBuyLimit = 99;

    private readonly object gate = new();
    private readonly Dictionary<long, PlayerOutgameShopEntity> states = new();

    public G2C_OutgameShopListResponse GetList(Scene scene, long playerId, string shopType, string activityId, string featureId)
    {
        lock (gate)
        {
            var response = new G2C_OutgameShopListResponse
            {
                Success = true,
                Message = "商店列表获取成功。"
            };

            foreach (var shop in FilterShops(shopType, activityId, featureId))
            {
                response.Shops.Add(ToShopInfo(scene, playerId, shop));
            }

            return response;
        }
    }

    public G2C_BuyOutgameShopGoodsResponse Buy(Scene scene, long playerId, int goodsId, int count)
    {
        lock (gate)
        {
            count = Math.Clamp(count <= 0 ? 1 : count, 1, 99);
            var response = new G2C_BuyOutgameShopGoodsResponse();
            var goods = ConfigSystem.Instance.Tables.TbShopGoods.GetOrDefault(goodsId);
            if (goods == null)
            {
                response.Success = false;
                response.Message = "商品不存在。";
                response.Snapshot = SheepServices.Assets.CreateSnapshot(playerId);
                return response;
            }

            var shop = ConfigSystem.Instance.Tables.TbShop.DataList.FirstOrDefault(v => v.GoodsGroupId == goods.GoodsGroupId);
            if (shop == null)
            {
                response.Success = false;
                response.Message = "商店不存在。";
                response.Snapshot = SheepServices.Assets.CreateSnapshot(playerId);
                return response;
            }

            var shopFeatureId = GetFeatureId(shop);
            if (!SheepServices.Features.IsOpen(shopFeatureId))
            {
                response.Success = false;
                response.Message = "商店暂未开放。";
                response.Snapshot = SheepServices.Assets.CreateSnapshot(playerId);
                return response;
            }

            var state = GetOrCreateState(scene, playerId);
            var record = GetPurchaseRecord(state, goods);
            var buyLimit = GetBuyLimit(goods);
            if (record.BoughtCount + count > buyLimit)
            {
                response.Success = false;
                response.Message = "购买次数已达上限。";
                response.Goods = ToGoodsInfo(scene, playerId, shop, goods);
                response.Snapshot = SheepServices.Assets.CreateSnapshot(playerId);
                return response;
            }

            var currencyId = ResolveCurrencyId(goods.Currency);
            var reward = CreateGoodsReward(goods, count);
            if (!CanReceiveGoodsReward(playerId, goods, count, out var receiveMessage))
            {
                response.Success = false;
                response.Message = receiveMessage;
                response.Goods = ToGoodsInfo(scene, playerId, shop, goods);
                response.Snapshot = SheepServices.Assets.CreateSnapshot(playerId);
                return response;
            }

            var totalPrice = (long)goods.Price * count;
            if (!SheepServices.Assets.TryCostCurrency(playerId, currencyId, totalPrice, "OutgameShopBuy", out var message))
            {
                response.Success = false;
                response.Message = message;
                response.Goods = ToGoodsInfo(scene, playerId, shop, goods);
                response.Snapshot = SheepServices.Assets.CreateSnapshot(playerId);
                return response;
            }

            if (!SheepServices.Assets.TryTransferReward(
                    playerId,
                    reward,
                    new AssetTransferContext("OutgameShop", goods.GoodsId.ToString(), "OutgameShopBuy"),
                    out var snapshot,
                    out message))
            {
                response.Success = false;
                response.Message = message;
                response.Goods = ToGoodsInfo(scene, playerId, shop, goods);
                response.Snapshot = snapshot;
                return response;
            }

            record.BoughtCount += count;
            record.UpdatedAtUtc = DateTimeOffset.UtcNow;
            SheepServices.Tasks.AddProgress(playerId, "Shop.Buy.Count", count);
            SheepServices.Tasks.AddProgress(playerId, $"Shop.{NormalizeType(shop.ShopType)}.Buy.Count", count);

            response.Success = true;
            response.Message = "购买成功。";
            response.Goods = ToGoodsInfo(scene, playerId, shop, goods);
            response.Snapshot = snapshot;
            Log.Info($"局外商店购买：玩家ID={playerId}，商店={shop.ShopId}，商品={goods.GoodsId}，数量={count}");
            return response;
        }
    }

    private IEnumerable<ShopConfig> FilterShops(string shopType, string activityId, string featureId)
    {
        var normalizedType = NormalizeFilter(shopType);
        var normalizedActivity = NormalizeFilter(activityId);
        var normalizedFeature = NormalizeFilter(featureId);
        foreach (var shop in ConfigSystem.Instance.Tables.TbShop.DataList.OrderBy(v => v.ShopId))
        {
            var shopFeature = GetFeatureId(shop);
            if (!SheepServices.Features.IsOpen(shopFeature))
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(normalizedType) &&
                !string.Equals(NormalizeType(shop.ShopType), normalizedType, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(normalizedActivity) &&
                !string.Equals(GetActivityId(shop), normalizedActivity, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(normalizedFeature) &&
                !string.Equals(shopFeature, normalizedFeature, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            yield return shop;
        }
    }

    private OutgameShopInfo ToShopInfo(Scene scene, long playerId, ShopConfig shop)
    {
        var info = new OutgameShopInfo
        {
            ShopId = shop.ShopId,
            ShopName = shop.ShopName,
            ShopType = NormalizeType(shop.ShopType),
            ActivityId = GetActivityId(shop),
            RefreshGroup = GetRefreshGroup(shop),
            FeatureId = GetFeatureId(shop)
        };

        foreach (var goods in ConfigSystem.Instance.Tables.TbShopGoods.DataList
                     .Where(v => v.GoodsGroupId == shop.GoodsGroupId)
                     .OrderBy(v => v.GoodsId))
        {
            info.Goods.Add(ToGoodsInfo(scene, playerId, shop, goods));
        }

        return info;
    }

    private OutgameShopGoodsInfo ToGoodsInfo(Scene scene, long playerId, ShopConfig shop, ShopGoodsConfig goods)
    {
        var record = GetPurchaseRecord(GetOrCreateState(scene, playerId), goods);
        var reward = CreateGoodsReward(goods, 1);
        return new OutgameShopGoodsInfo
        {
            GoodsId = goods.GoodsId,
            ShopId = shop.ShopId,
            GoodsGroupId = goods.GoodsGroupId,
            Name = goods.ItemName,
            PriceCurrencyId = ResolveCurrencyId(goods.Currency),
            PriceAmount = goods.Price,
            BuyLimit = GetBuyLimit(goods),
            BoughtCount = record.BoughtCount,
            IsAvailable = string.IsNullOrWhiteSpace(goods.UnlockRule),
            UnlockRule = goods.UnlockRule,
            Description = string.IsNullOrWhiteSpace(goods.EffectDesc) ? goods.Comment : goods.EffectDesc,
            Reward = MailService.ToRewardInfo(reward),
            FeatureId = GetFeatureId(shop)
        };
    }

    private PlayerOutgameShopEntity GetOrCreateState(Scene scene, long playerId)
    {
        if (states.TryGetValue(playerId, out var state))
        {
            return state;
        }

        state = Entity.Create<PlayerOutgameShopEntity>(scene, isPool: false, isRunEvent: true);
        state.PlayerId = playerId;
        states.Add(playerId, state);
        return state;
    }

    private static OutgameShopPurchaseRecord GetPurchaseRecord(PlayerOutgameShopEntity state, ShopGoodsConfig goods)
    {
        if (state.PurchasesByGoodsId.TryGetValue(goods.GoodsId, out var record))
        {
            return record;
        }

        record = new OutgameShopPurchaseRecord
        {
            GoodsId = goods.GoodsId,
            RefreshGroup = $"GoodsGroup.{goods.GoodsGroupId}"
        };
        state.PurchasesByGoodsId.Add(goods.GoodsId, record);
        return record;
    }

    private static AssetReward CreateGoodsReward(ShopGoodsConfig goods, int count)
    {
        var reward = new AssetReward();
        reward.Items.Add(new ItemAmount(goods.ItemId, goods.RewardItemCount * count));
        return reward;
    }

    private static bool CanReceiveGoodsReward(long playerId, ShopGoodsConfig goods, int count, out string message)
    {
        message = string.Empty;
        var itemConfig = ConfigSystem.Instance.Tables.TbItem.GetOrDefault(goods.ItemId);
        if (itemConfig == null || count <= 0)
        {
            message = "商品奖励配置无效。";
            return false;
        }

        var current = SheepServices.Assets.GetItemCount(playerId, goods.ItemId);
        var maxStack = itemConfig.MaxStack > 0 ? itemConfig.MaxStack : int.MaxValue;
        var rewardCount = goods.RewardItemCount * count;
        if (current > maxStack - rewardCount)
        {
            message = "道具数量已达上限。";
            return false;
        }

        return true;
    }

    private static int ResolveCurrencyId(string currency)
    {
        return NormalizeType(currency) switch
        {
            "Diamond" => DiamondCurrencyId,
            "EventToken" => EventTokenCurrencyId,
            _ => GoldCurrencyId
        };
    }

    private static int GetBuyLimit(ShopGoodsConfig goods)
    {
        return goods.BuyLimit > 0 ? goods.BuyLimit : DefaultBuyLimit;
    }

    private static string GetActivityId(ShopConfig shop)
    {
        return string.IsNullOrWhiteSpace(shop.ActivityId) ? string.Empty : shop.ActivityId.Trim();
    }

    private static string GetRefreshGroup(ShopConfig shop)
    {
        return string.IsNullOrWhiteSpace(shop.RefreshGroup) ? $"{NormalizeType(shop.ShopType)}.{shop.ShopId}" : shop.RefreshGroup.Trim();
    }

    private static string GetFeatureId(ShopConfig shop)
    {
        return string.IsNullOrWhiteSpace(shop.FeatureId) ? $"Shop.{NormalizeType(shop.ShopType)}" : shop.FeatureId.Trim();
    }

    private static string NormalizeFilter(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
    }

    private static string NormalizeType(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? "Normal" : value.Trim();
    }
}
