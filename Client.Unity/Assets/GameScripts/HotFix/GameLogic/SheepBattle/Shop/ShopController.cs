using Fantasy.Async;
using GameLogic.SheepBattle.Asset;
using GameLogic.SheepBattle.Common;
using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Network;
using GameLogic.SheepBattle.Reward;
using TEngine;

namespace GameLogic.SheepBattle.Shop
{
    public sealed class ShopController
    {
        public static ShopController Instance { get; } = new();
        public ShopViewModel Model { get; } = new();

        private ShopController()
        {
        }

        public async FTask<ShopViewModel> RefreshAsync(string shopType = "", string activityId = "", string featureId = "")
        {
            var response = await SheepNetworkService.Instance.RequestOutgameShopListAsync(shopType, activityId, featureId);
            if (!response.Success)
            {
                CommonNoticeService.Show(response.Message);
                return Model;
            }

            Model.Apply(response.Shops, shopType, activityId, featureId);
            GameEvent.Send(new ShopViewChangedEvent(Model));
            return Model;
        }

        public async FTask BuyAsync(int goodsId, int count = 1)
        {
            var response = await SheepNetworkService.Instance.BuyOutgameShopGoodsAsync(goodsId, count);
            if (!response.Success)
            {
                CommonNoticeService.Show(response.Message);
                return;
            }

            if (response.Snapshot != null)
            {
                AssetController.Instance.ApplySnapshot(response.Snapshot);
            }
            else
            {
                await AssetController.Instance.RefreshAsync();
            }

            await RefreshAsync(Model.ShopType, Model.ActivityId, Model.FeatureId);
            RewardDisplayService.Show(RewardDisplayService.FromReward("购买获得", response.Goods?.Reward));
        }
    }
}
