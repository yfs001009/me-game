using System.Collections.Generic;
using System.Linq;
using Fantasy;

namespace GameLogic.SheepBattle.Asset
{
    public sealed class AssetViewModel
    {
        public List<CurrencyEntryViewModel> Currencies { get; } = new();
        public List<BagItemEntryViewModel> BagItems { get; } = new();
        public List<BuffEntryViewModel> Buffs { get; } = new();
        public Dictionary<string, long> ProgressValues { get; } = new(System.StringComparer.OrdinalIgnoreCase);
        public HashSet<int> UnlockedCharacterIds { get; } = new();
        public HashSet<int> UnlockedBuildingCardIds { get; } = new();

        public void Apply(AssetSnapshotInfo snapshot)
        {
            if (snapshot == null)
            {
                return;
            }

            Currencies.Clear();
            BagItems.Clear();
            Buffs.Clear();
            ProgressValues.Clear();
            UnlockedCharacterIds.Clear();
            UnlockedBuildingCardIds.Clear();

            Currencies.AddRange(snapshot.Currencies.Select(v => new CurrencyEntryViewModel(v)));
            BagItems.AddRange(snapshot.BagItems.Select(v => new BagItemEntryViewModel(v)));
            Buffs.AddRange(snapshot.Buffs.Select(v => new BuffEntryViewModel(v)));
            foreach (var progress in snapshot.ProgressValues)
            {
                if (!string.IsNullOrWhiteSpace(progress.Key))
                {
                    ProgressValues[progress.Key] = progress.Value;
                }
            }

            foreach (var id in snapshot.UnlockedCharacterIds)
            {
                UnlockedCharacterIds.Add(id);
            }

            foreach (var id in snapshot.UnlockedBuildingCardIds)
            {
                UnlockedBuildingCardIds.Add(id);
            }
        }

        public long GetCurrencyAmount(string code)
        {
            var item = Currencies.FirstOrDefault(v => string.Equals(v.Code, code, System.StringComparison.OrdinalIgnoreCase));
            return item?.Amount ?? 0;
        }

        public int GetItemCount(int itemId)
        {
            var item = BagItems.FirstOrDefault(v => v.ItemId == itemId);
            return item?.Count ?? 0;
        }

        public long GetProgressValue(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return 0;
            }

            return ProgressValues.TryGetValue(key, out var value) ? value : 0;
        }
    }

    public sealed class BuffEntryViewModel
    {
        public BuffEntryViewModel(BuffStateInfo info)
        {
            BuffKey = info?.BuffKey ?? string.Empty;
            ExpiresAtUnixSeconds = info?.ExpiresAtUnixSeconds ?? 0;
        }

        public string BuffKey { get; }
        public long ExpiresAtUnixSeconds { get; }
    }

    public sealed class CurrencyEntryViewModel
    {
        public CurrencyEntryViewModel(CurrencyBalanceInfo info)
        {
            CurrencyId = info?.CurrencyId ?? 0;
            Code = info?.Code ?? string.Empty;
            Name = info?.Name ?? string.Empty;
            Amount = info?.Amount ?? 0;
            IconAsset = info?.IconAsset ?? string.Empty;
        }

        public int CurrencyId { get; }
        public string Code { get; }
        public string Name { get; }
        public long Amount { get; }
        public string IconAsset { get; }
    }

    public sealed class BagItemEntryViewModel
    {
        public BagItemEntryViewModel(BagItemInfo info)
        {
            ItemId = info?.ItemId ?? 0;
            ItemType = info?.ItemType ?? string.Empty;
            Name = info?.Name ?? string.Empty;
            Count = info?.Count ?? 0;
            MaxStack = info?.MaxStack ?? 0;
            UseType = info?.UseType ?? string.Empty;
            IconAsset = info?.IconAsset ?? string.Empty;
            Description = info?.Description ?? string.Empty;
            Quality = info?.Quality ?? 1;
        }

        public int ItemId { get; }
        public string ItemType { get; }
        public string Name { get; }
        public int Count { get; }
        public int MaxStack { get; }
        public string UseType { get; }
        public string IconAsset { get; }
        public string Description { get; }
        public int Quality { get; }
        public bool CanUse => !string.IsNullOrWhiteSpace(UseType) && !string.Equals(UseType, "None", System.StringComparison.OrdinalIgnoreCase);
    }
}
