using System.Collections.Generic;
using Fantasy;
using TEngine;
using UnityEngine;

namespace GameLogic.SheepBattle.Reward
{
    public static class RewardDisplayService
    {
        private static readonly Dictionary<int, Sprite> QualityFrameSprites = new();

        public static string GetQualityFrameAsset(int quality)
        {
            switch (ClampQuality(quality))
            {
                case 1:
                    return "Assets/AssetRaw/UI/Art/item_quality/quality_1_common.png";
                case 2:
                    return "Assets/AssetRaw/UI/Art/item_quality/quality_2_fine.png";
                case 3:
                    return "Assets/AssetRaw/UI/Art/item_quality/quality_3_rare.png";
                default:
                    return "Assets/AssetRaw/UI/Art/item_quality/quality_4_epic.png";
            }
        }

        public static Sprite GetQualityFrameSprite(int quality)
        {
            var clampedQuality = ClampQuality(quality);
            if (QualityFrameSprites.TryGetValue(clampedQuality, out var sprite))
            {
                return sprite;
            }

            var texture = GameModule.Resource.LoadAsset<Texture2D>(GetQualityFrameAsset(clampedQuality));
            if (texture == null)
            {
                return null;
            }

            sprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect);
            QualityFrameSprites[clampedQuality] = sprite;
            return sprite;
        }

        public static string GetQualityName(int quality)
        {
            switch (ClampQuality(quality))
            {
                case 1:
                    return "普通";
                case 2:
                    return "优秀";
                case 3:
                    return "稀有";
                default:
                    return "史诗";
            }
        }

        public static RewardPopupData FromReward(string title, RewardInfo reward)
        {
            var items = new List<RewardPopupItemData>();
            if (reward != null)
            {
                AddCurrencies(items, reward.Currencies);
                AddItems(items, reward.Items);
                AddIds(items, reward.CharacterIds, "角色", 4);
                AddIds(items, reward.BuildingCardIds, "建筑卡", 3);
            }

            return new RewardPopupData(title, items);
        }

        public static RewardPopupData FromLottery(string title, IReadOnlyList<LotteryDrawResultInfo> results)
        {
            var items = new List<RewardPopupItemData>();
            if (results != null)
            {
                for (var i = 0; i < results.Count; i++)
                {
                    var reward = results[i]?.Reward;
                    if (reward == null)
                    {
                        continue;
                    }

                    AddCurrencies(items, reward.Currencies);
                    AddItems(items, reward.Items);
                    AddIds(items, reward.CharacterIds, "角色", 4);
                    AddIds(items, reward.BuildingCardIds, "建筑卡", 3);
                }
            }

            return new RewardPopupData(title, items);
        }

        public static void Show(RewardPopupData data)
        {
            if (data == null || data.Items.Count == 0)
            {
                return;
            }

            GameModule.UI.ShowUIAsync<RewardPopupUI>(data);
        }

        private static void AddCurrencies(ICollection<RewardPopupItemData> output, IReadOnlyList<CurrencyBalanceInfo> currencies)
        {
            if (currencies == null)
            {
                return;
            }

            for (var i = 0; i < currencies.Count; i++)
            {
                var currency = currencies[i];
                if (currency == null)
                {
                    continue;
                }

                output.Add(new RewardPopupItemData(currency.Name, currency.Amount, 1, currency.IconAsset));
            }
        }

        private static void AddItems(ICollection<RewardPopupItemData> output, IReadOnlyList<BagItemInfo> items)
        {
            if (items == null)
            {
                return;
            }

            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item == null)
                {
                    continue;
                }

                output.Add(new RewardPopupItemData(item.Name, item.Count, item.Quality, item.IconAsset));
            }
        }

        private static void AddIds(ICollection<RewardPopupItemData> output, IReadOnlyList<int> ids, string prefix, int quality)
        {
            if (ids == null)
            {
                return;
            }

            for (var i = 0; i < ids.Count; i++)
            {
                output.Add(new RewardPopupItemData($"{prefix} {ids[i]}", 1, quality, string.Empty));
            }
        }

        private static int ClampQuality(int quality)
        {
            if (quality < 1)
            {
                return 1;
            }

            return quality > 4 ? 4 : quality;
        }
    }

    public sealed class RewardPopupData
    {
        public RewardPopupData(string title, IReadOnlyList<RewardPopupItemData> items)
        {
            Title = string.IsNullOrWhiteSpace(title) ? "获得奖励" : title;
            Items = items ?? new List<RewardPopupItemData>();
        }

        public string Title { get; }
        public IReadOnlyList<RewardPopupItemData> Items { get; }
    }

    public sealed class RewardPopupItemData
    {
        public RewardPopupItemData(string name, long count, int quality, string iconAsset)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "奖励" : name;
            Count = count;
            Quality = quality <= 0 ? 1 : quality;
            IconAsset = iconAsset ?? string.Empty;
        }

        public string Name { get; }
        public long Count { get; }
        public int Quality { get; }
        public string IconAsset { get; }
    }
}
