using Fantasy;
using Hotfix.Asset.Model;
using Hotfix.Mail.Service;
using Hotfix.Shared;

namespace Hotfix.Lottery.Service;

public sealed class LotteryService
{
    private const int NormalTicketItemId = 1001;
    private const int PremiumTicketItemId = 1002;
    private const string DrawCountSuffix = ".DrawCount";
    private const string PitySuffix = ".Pity";

    public (bool Success, string Message, IReadOnlyList<LotteryDrawResultInfo> Results, AssetSnapshotInfo Snapshot) Draw(long playerId, string pool, int count)
    {
        count = Math.Clamp(count <= 0 ? 1 : count, 1, 10);
        pool = string.IsNullOrWhiteSpace(pool) ? "Normal" : pool.Trim();
        var ticketItemId = string.Equals(pool, "Premium", StringComparison.OrdinalIgnoreCase) ? PremiumTicketItemId : NormalTicketItemId;

        if (!SheepServices.Assets.TryCostItem(playerId, ticketItemId, count, "LotteryDraw", out var message))
        {
            return (false, message, Array.Empty<LotteryDrawResultInfo>(), SheepServices.Assets.CreateSnapshot(playerId));
        }

        var results = new List<LotteryDrawResultInfo>();
        for (var i = 0; i < count; i++)
        {
            var reward = CreateReward(pool);
            if (!SheepServices.Assets.TryGrantReward(playerId, reward, "LotteryDrawReward", out message))
            {
                return (false, message, results, SheepServices.Assets.CreateSnapshot(playerId));
            }

            results.Add(new LotteryDrawResultInfo
            {
                Pool = pool,
                Reward = MailService.ToRewardInfo(reward)
            });
            RecordLotteryProgress(playerId, pool);
        }

        return (true, "抽奖成功。", results, SheepServices.Assets.CreateSnapshot(playerId));
    }

    private static void RecordLotteryProgress(long playerId, string pool)
    {
        var normalizedPool = string.IsNullOrWhiteSpace(pool) ? "Normal" : pool.Trim();
        SheepServices.Assets.AddProgressValue(playerId, GetProgressKey(normalizedPool, DrawCountSuffix), 1);
        SheepServices.Assets.AddProgressValue(playerId, GetProgressKey(normalizedPool, PitySuffix), 1);
        SheepServices.Tasks.AddProgress(playerId, "Lottery.Any.DrawCount", 1);
    }

    private static string GetProgressKey(string pool, string suffix)
    {
        return $"Lottery.{pool}{suffix}";
    }

    private static AssetReward CreateReward(string pool)
    {
        var reward = new AssetReward();
        if (string.Equals(pool, "Premium", StringComparison.OrdinalIgnoreCase))
        {
            reward.Items.Add(new ItemAmount(1301, 10));
            return reward;
        }

        reward.Currencies.Add(new CurrencyAmount(1, 100));
        return reward;
    }
}
