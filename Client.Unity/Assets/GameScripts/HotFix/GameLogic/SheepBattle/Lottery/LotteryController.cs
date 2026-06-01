using Fantasy.Async;
using Fantasy;
using GameLogic.SheepBattle.Asset;
using GameLogic.SheepBattle.Common;
using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Network;
using GameLogic.SheepBattle.Reward;
using TEngine;

namespace GameLogic.SheepBattle.Lottery
{
    public sealed class LotteryController
    {
        public static LotteryController Instance { get; } = new();

        private LotteryController()
        {
        }

        public async FTask<G2C_LotteryDrawResponse> DrawAsync(string pool, int count)
        {
            var response = await SheepNetworkService.Instance.LotteryDrawAsync(pool, count);
            if (!response.Success)
            {
                CommonNoticeService.Show(response.Message);
                return response;
            }

            AssetController.Instance.ApplySnapshot(response.Snapshot);
            GameEvent.Send(new LotteryDrawCompletedEvent(response));
            RewardDisplayService.Show(RewardDisplayService.FromLottery("抽奖获得", response.Results));
            return response;
        }
    }
}
