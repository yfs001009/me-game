using Fantasy;
using Fantasy.Entitas;
using GameConfig.asset;
using Hotfix.Asset.Model;
using Hotfix.Config;

namespace Hotfix.Asset.Service;

public sealed class AssetService
{
    private const int CurrentAssetSchemaVersion = 1;
    private const int InitialBuildingCardCount = 6;
    private const int GoldCurrencyId = 1;
    private const int DiamondCurrencyId = 2;
    private const int EventTokenCurrencyId = 3;
    private const int NormalTicketItemId = 1001;
    private const int PremiumTicketItemId = 1002;
    private const int DoubleExpCardItemId = 1101;
    private const int DoubleGoldCardItemId = 1102;

    private readonly object gate = new();
    private readonly Dictionary<long, PlayerAssetEntity> states = new();
    private Scene? ownerScene;

    public void BindScene(Scene scene)
    {
        if (scene == null)
        {
            return;
        }

        lock (gate)
        {
            ownerScene ??= scene;
        }
    }

    public PlayerAssetEntity GetOrCreateState(long playerId)
    {
        lock (gate)
        {
            return GetOrCreateStateUnsafe(playerId);
        }
    }

    public long GetCurrency(long playerId, int currencyId)
    {
        lock (gate)
        {
            return Currencies(GetOrCreateStateUnsafe(playerId)).Currencies.GetValueOrDefault(currencyId);
        }
    }

    public int GetItemCount(long playerId, int itemId)
    {
        lock (gate)
        {
            return GetItemCount(Bag(GetOrCreateStateUnsafe(playerId)), itemId);
        }
    }

    public bool TryGrantReward(long playerId, AssetReward reward, string reason, out string message)
    {
        var context = new AssetTransferContext("Reward", string.Empty, reason);
        return TryTransferReward(playerId, reward, context, out _, out message);
    }

    public bool TryTransferReward(
        long playerId,
        AssetReward reward,
        AssetTransferContext context,
        out AssetSnapshotInfo snapshot,
        out string message)
    {
        lock (gate)
        {
            message = string.Empty;
            var state = GetOrCreateStateUnsafe(playerId);
            if (!CanGrantReward(state, reward, out message))
            {
                snapshot = ToSnapshot(state);
                return false;
            }

            foreach (var currency in reward.Currencies)
            {
                AddCurrencyUnsafe(state, currency.CurrencyId, currency.Amount);
            }

            foreach (var item in reward.Items)
            {
                var config = GetItemConfig(item.ItemId);
                TryAddItemUnsafe(Bag(state), config, item.Count, out _);
            }

            foreach (var characterId in reward.CharacterUnlocks)
            {
                Unlocks(state).CharacterIds.Add(characterId);
            }

            foreach (var cardId in reward.BuildingCardUnlocks)
            {
                Unlocks(state).BuildingCardIds.Add(cardId);
            }

            snapshot = ToSnapshot(state);
            Log.Info($"Asset transfer committed: playerId={playerId}, source={context.SourceContainerType}:{context.SourceContainerId}, reason={context.Reason}, currencies={reward.Currencies.Count}, items={reward.Items.Count}");
            return true;
        }
    }

    public AssetSnapshotInfo CreateSnapshot(long playerId)
    {
        lock (gate)
        {
            return ToSnapshot(GetOrCreateStateUnsafe(playerId));
        }
    }

    public bool TryUseItem(long playerId, int itemId, int count, out string message)
    {
        lock (gate)
        {
            count = count <= 0 ? 1 : count;
            var config = GetItemConfig(itemId);
            if (config == null)
            {
                message = "道具配置不存在。";
                return false;
            }

            var useType = config.UseType ?? string.Empty;
            if (useType.Equals("ActivateBuff", StringComparison.OrdinalIgnoreCase))
            {
                var seconds = 0;
                _ = int.TryParse(config.EffectParam, out seconds);
                if (seconds <= 0)
                {
                    message = "道具效果参数无效。";
                    return false;
                }
            }
            else if (useType.Equals("OpenChest", StringComparison.OrdinalIgnoreCase))
            {
                message = "宝箱奖励包尚未配置。";
                return false;
            }
            else if (useType.Equals("Exchange", StringComparison.OrdinalIgnoreCase))
            {
                message = "兑换规则尚未配置。";
                return false;
            }
            else if (useType.Equals("UseDirectly", StringComparison.OrdinalIgnoreCase) &&
                     config.EffectType.Equals("CharacterTrial", StringComparison.OrdinalIgnoreCase))
            {
                var parts = (config.EffectParam ?? string.Empty).Split(':', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0 || !int.TryParse(parts[0], out var characterId))
                {
                    message = "角色体验卡参数无效。";
                    return false;
                }
            }
            else if (useType.Equals("UseDirectly", StringComparison.OrdinalIgnoreCase) &&
                     config.EffectType.Equals("Lottery", StringComparison.OrdinalIgnoreCase))
            {
                message = "抽奖券请在抽奖界面使用。";
                return false;
            }
            else if (useType.Equals("None", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(useType))
            {
                message = "该道具不能直接使用。";
                return false;
            }

            var state = GetOrCreateStateUnsafe(playerId);
            if (!TryRemoveItemUnsafe(Bag(state), itemId, count, out message))
            {
                return false;
            }

            if (useType.Equals("ActivateBuff", StringComparison.OrdinalIgnoreCase))
            {
                _ = int.TryParse(config.EffectParam, out var seconds);
                if (!TryActivateBuffUnsafe(playerId, config.EffectType, TimeSpan.FromSeconds(seconds), out message))
                {
                    return false;
                }
            }
            else if (useType.Equals("UseDirectly", StringComparison.OrdinalIgnoreCase) &&
                     config.EffectType.Equals("CharacterTrial", StringComparison.OrdinalIgnoreCase))
            {
                var parts = (config.EffectParam ?? string.Empty).Split(':', StringSplitOptions.RemoveEmptyEntries);
                _ = int.TryParse(parts[0], out var characterId);
                Unlocks(state).CharacterIds.Add(characterId);
            }

            message = "道具使用成功。";
            Log.Info($"道具使用：玩家ID={playerId}，道具ID={itemId}，数量={count}");
            return true;
        }
    }

    public bool TryAddCurrency(long playerId, int currencyId, long amount, string reason, out string message)
    {
        lock (gate)
        {
            message = string.Empty;
            if (amount <= 0)
            {
                message = "增加货币数量必须大于 0。";
                return false;
            }

            if (GetCurrencyConfig(currencyId) == null)
            {
                message = "货币配置不存在。";
                return false;
            }

            AddCurrencyUnsafe(GetOrCreateStateUnsafe(playerId), currencyId, amount);
            Log.Info($"货币增加：玩家ID={playerId}，货币ID={currencyId}，数量={amount}，来源={reason}");
            return true;
        }
    }

    public bool TryCostCurrency(long playerId, int currencyId, long amount, string reason, out string message)
    {
        lock (gate)
        {
            message = string.Empty;
            if (amount <= 0)
            {
                message = "扣除货币数量必须大于 0。";
                return false;
            }

            var state = GetOrCreateStateUnsafe(playerId);
            var currencies = Currencies(state);
            var current = currencies.Currencies.GetValueOrDefault(currencyId);
            if (current < amount)
            {
                message = "货币不足。";
                return false;
            }

            currencies.Currencies[currencyId] = current - amount;
            Log.Info($"货币扣除：玩家ID={playerId}，货币ID={currencyId}，数量={amount}，来源={reason}");
            return true;
        }
    }

    public bool TryAddItem(long playerId, int itemId, int count, string reason, out string message)
    {
        lock (gate)
        {
            message = string.Empty;
            var config = GetItemConfig(itemId);
            if (config == null)
            {
                message = "道具配置不存在。";
                return false;
            }

            if (!TryAddItemUnsafe(Bag(GetOrCreateStateUnsafe(playerId)), config, count, out message))
            {
                return false;
            }

            Log.Info($"道具增加：玩家ID={playerId}，道具ID={itemId}，数量={count}，来源={reason}");
            return true;
        }
    }

    public bool TryCostItem(long playerId, int itemId, int count, string reason, out string message)
    {
        lock (gate)
        {
            var state = GetOrCreateStateUnsafe(playerId);
            if (!TryRemoveItemUnsafe(Bag(state), itemId, count, out message))
            {
                return false;
            }

            Log.Info($"道具扣除：玩家ID={playerId}，道具ID={itemId}，数量={count}，来源={reason}");
            return true;
        }
    }

    public bool TryActivateBuff(long playerId, string buffKey, TimeSpan duration, out string message)
    {
        lock (gate)
        {
            return TryActivateBuffUnsafe(playerId, buffKey, duration, out message);
        }
    }

    public long GetProgressValue(long playerId, string key)
    {
        lock (gate)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return 0;
            }

            return ProgressValues(GetOrCreateStateUnsafe(playerId)).Values.GetValueOrDefault(key);
        }
    }

    public long AddProgressValue(long playerId, string key, long delta)
    {
        lock (gate)
        {
            if (string.IsNullOrWhiteSpace(key) || delta == 0)
            {
                return string.IsNullOrWhiteSpace(key) ? 0 : ProgressValues(GetOrCreateStateUnsafe(playerId)).Values.GetValueOrDefault(key);
            }

            var state = GetOrCreateStateUnsafe(playerId);
            var progress = ProgressValues(state);
            var value = progress.Values.GetValueOrDefault(key) + delta;
            progress.Values[key] = value;
            return value;
        }
    }

    public void SetProgressValue(long playerId, string key, long value)
    {
        lock (gate)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            ProgressValues(GetOrCreateStateUnsafe(playerId)).Values[key] = Math.Max(0, value);
        }
    }

    public bool HasCharacter(long playerId, int characterId)
    {
        lock (gate)
        {
            return Unlocks(GetOrCreateStateUnsafe(playerId)).CharacterIds.Contains(characterId);
        }
    }

    public bool EnsureCharacterUnlocked(long playerId, int characterId)
    {
        lock (gate)
        {
            return Unlocks(GetOrCreateStateUnsafe(playerId)).CharacterIds.Add(characterId);
        }
    }

    private PlayerAssetEntity GetOrCreateStateUnsafe(long playerId)
    {
        if (states.TryGetValue(playerId, out var state))
        {
            MigrateAssetStateUnsafe(state);
            return state;
        }

        if (ownerScene == null)
        {
            throw new InvalidOperationException("AssetService scene is not bound. Call BindScene before accessing player assets.");
        }

        state = Entity.Create<PlayerAssetEntity>(ownerScene, isPool: false, isRunEvent: true);
        state.PlayerId = playerId;
        _ = Currencies(state);
        _ = Bag(state);
        _ = Unlocks(state);
        _ = Buffs(state);
        _ = ProgressValues(state);
        InitializeNewPlayerAssetsUnsafe(state);
        states.Add(playerId, state);
        return state;
    }

    private static void InitializeNewPlayerAssetsUnsafe(PlayerAssetEntity state)
    {
        // Currency stays as pure numeric asset; starter tickets/cards go into the bag container.
        AddInitialCurrencyUnsafe(state, GoldCurrencyId, 1000);
        AddInitialCurrencyUnsafe(state, DiamondCurrencyId, 100);
        AddInitialCurrencyUnsafe(state, EventTokenCurrencyId, 0);
        GrantStarterBagItemsUnsafe(state);

        foreach (var config in ConfigSystem.Instance.Tables.TbCharacter.DataList)
        {
            if (config.IsInitial)
            {
                Unlocks(state).CharacterIds.Add(config.CharacterId);
            }
        }

        foreach (var card in ConfigSystem.Instance.Tables.TbBuildingCard.DataList
                     .Where(card => string.IsNullOrWhiteSpace(card.UnlockRule))
                     .OrderBy(card => card.SortOrder)
                     .ThenBy(card => card.CardId)
                     .Take(InitialBuildingCardCount))
        {
            Unlocks(state).BuildingCardIds.Add(card.CardId);
        }

        state.AssetSchemaVersion = CurrentAssetSchemaVersion;
    }

    private static void MigrateAssetStateUnsafe(PlayerAssetEntity state)
    {
        if (state.AssetSchemaVersion >= CurrentAssetSchemaVersion)
        {
            return;
        }

        GrantStarterBagItemsUnsafe(state);
        state.AssetSchemaVersion = CurrentAssetSchemaVersion;
    }

    private static void GrantStarterBagItemsUnsafe(PlayerAssetEntity state)
    {
        AddInitialItemIfMissingUnsafe(state, NormalTicketItemId, 3);
        AddInitialItemIfMissingUnsafe(state, PremiumTicketItemId, 1);
        AddInitialItemIfMissingUnsafe(state, DoubleExpCardItemId, 1);
        AddInitialItemIfMissingUnsafe(state, DoubleGoldCardItemId, 1);
    }

    private static void AddInitialCurrencyUnsafe(PlayerAssetEntity state, int currencyId, long amount)
    {
        if (amount < 0 || GetCurrencyConfig(currencyId) == null)
        {
            return;
        }

        Currencies(state).Currencies[currencyId] = amount;
    }

    private static void AddInitialItemUnsafe(PlayerAssetEntity state, int itemId, int count)
    {
        var config = GetItemConfig(itemId);
        if (config == null || count <= 0)
        {
            return;
        }

        TryAddItemUnsafe(Bag(state), config, count, out _);
    }

    private static void AddInitialItemIfMissingUnsafe(PlayerAssetEntity state, int itemId, int count)
    {
        if (GetItemCount(Bag(state), itemId) > 0)
        {
            return;
        }

        AddInitialItemUnsafe(state, itemId, count);
    }

    private static bool CanGrantReward(PlayerAssetEntity state, AssetReward reward, out string message)
    {
        message = string.Empty;
        foreach (var currency in reward.Currencies)
        {
            if (currency.Amount <= 0 || GetCurrencyConfig(currency.CurrencyId) == null)
            {
                message = "奖励货币配置无效。";
                return false;
            }
        }

        foreach (var item in reward.Items)
        {
            var config = GetItemConfig(item.ItemId);
            if (config == null || !CanAddItem(Bag(state), config, item.Count))
            {
                message = "奖励道具配置无效或数量已达上限。";
                return false;
            }
        }

        return true;
    }

    private static CurrencyConfig GetCurrencyConfig(int currencyId)
    {
        return ConfigSystem.Instance.Tables.TbCurrency.GetOrDefault(currencyId);
    }

    private static ItemConfig GetItemConfig(int itemId)
    {
        return ConfigSystem.Instance.Tables.TbItem.GetOrDefault(itemId);
    }

    private static PlayerCurrencyComponent Currencies(PlayerAssetEntity state)
    {
        return state.GetOrAddComponent<PlayerCurrencyComponent>();
    }

    private static PlayerBagComponent Bag(PlayerAssetEntity state)
    {
        return state.GetOrAddComponent<PlayerBagComponent>();
    }

    private static PlayerUnlockComponent Unlocks(PlayerAssetEntity state)
    {
        return state.GetOrAddComponent<PlayerUnlockComponent>();
    }

    private static PlayerBuffComponent Buffs(PlayerAssetEntity state)
    {
        return state.GetOrAddComponent<PlayerBuffComponent>();
    }

    private static PlayerProgressComponent ProgressValues(PlayerAssetEntity state)
    {
        return state.GetOrAddComponent<PlayerProgressComponent>();
    }

    private static int GetItemCount(PlayerBagComponent bag, int itemId)
    {
        return bag.ItemCounts.GetValueOrDefault(itemId);
    }

    private static bool CanAddItem(PlayerBagComponent bag, ItemConfig config, int count)
    {
        if (config == null || count <= 0)
        {
            return false;
        }

        var maxStack = config.MaxStack > 0 ? config.MaxStack : int.MaxValue;
        return GetItemCount(bag, config.ItemId) <= maxStack - count;
    }

    private static bool TryAddItemUnsafe(PlayerBagComponent bag, ItemConfig config, int count, out string message)
    {
        message = string.Empty;
        if (!CanAddItem(bag, config, count))
        {
            message = "道具数量已达上限。";
            return false;
        }

        bag.ItemCounts[config.ItemId] = GetItemCount(bag, config.ItemId) + count;
        return true;
    }

    private static bool TryRemoveItemUnsafe(PlayerBagComponent bag, int itemId, int count, out string message)
    {
        message = string.Empty;
        if (count <= 0)
        {
            message = "扣除数量必须大于 0。";
            return false;
        }

        var current = GetItemCount(bag, itemId);
        if (current < count)
        {
            message = "道具数量不足。";
            return false;
        }

        var remaining = current - count;
        if (remaining == 0)
        {
            bag.ItemCounts.Remove(itemId);
        }
        else
        {
            bag.ItemCounts[itemId] = remaining;
        }

        return true;
    }

    private static void AddCurrencyUnsafe(PlayerAssetEntity state, int currencyId, long amount)
    {
        var currencies = Currencies(state).Currencies;
        currencies[currencyId] = currencies.GetValueOrDefault(currencyId) + amount;
    }

    private bool TryActivateBuffUnsafe(long playerId, string buffKey, TimeSpan duration, out string message)
    {
        message = string.Empty;
        if (string.IsNullOrWhiteSpace(buffKey) || duration <= TimeSpan.Zero)
        {
            message = "Buff 参数无效。";
            return false;
        }

        var state = GetOrCreateStateUnsafe(playerId);
        var now = DateTimeOffset.UtcNow;
        var buffs = Buffs(state).BuffExpiresAtUtc;
        var current = buffs.GetValueOrDefault(buffKey);
        var start = current > now ? current : now;
        buffs[buffKey] = start.Add(duration);
        return true;
    }

    private static AssetSnapshotInfo ToSnapshot(PlayerAssetEntity state)
    {
        var snapshot = new AssetSnapshotInfo();
        var currencies = Currencies(state);
        var bag = Bag(state);
        var unlocks = Unlocks(state);
        var buffs = Buffs(state);
        var progress = ProgressValues(state);
        foreach (var currencyConfig in ConfigSystem.Instance.Tables.TbCurrency.DataList.OrderBy(v => v.SortOrder))
        {
            snapshot.Currencies.Add(ToCurrencyInfo(currencyConfig, currencies.Currencies.GetValueOrDefault(currencyConfig.CurrencyId)));
        }

        foreach (var pair in bag.ItemCounts.OrderBy(v => v.Key))
        {
            var config = GetItemConfig(pair.Key);
            if (config != null)
            {
                snapshot.BagItems.Add(ToBagItemInfo(config, pair.Value));
            }
        }

        snapshot.UnlockedCharacterIds.AddRange(unlocks.CharacterIds.OrderBy(v => v));
        snapshot.UnlockedBuildingCardIds.AddRange(unlocks.BuildingCardIds.OrderBy(v => v));
        foreach (var pair in buffs.BuffExpiresAtUtc.OrderBy(v => v.Key))
        {
            snapshot.Buffs.Add(new BuffStateInfo
            {
                BuffKey = pair.Key,
                ExpiresAtUnixSeconds = pair.Value.ToUnixTimeSeconds()
            });
        }

        foreach (var pair in progress.Values.OrderBy(v => v.Key))
        {
            snapshot.ProgressValues.Add(new ProgressValueInfo
            {
                Key = pair.Key,
                Value = pair.Value
            });
        }

        return snapshot;
    }

    public static CurrencyBalanceInfo ToCurrencyInfo(CurrencyConfig config, long amount)
    {
        return new CurrencyBalanceInfo
        {
            CurrencyId = config.CurrencyId,
            Code = config.Code,
            Name = config.Name,
            Amount = amount,
            IconAsset = config.IconAsset
        };
    }

    public static BagItemInfo ToBagItemInfo(ItemConfig config, int count)
    {
        return new BagItemInfo
        {
            ItemId = config.ItemId,
            ItemType = config.ItemType,
            Name = config.Name,
            Count = count,
            MaxStack = config.MaxStack,
            UseType = config.UseType,
            IconAsset = config.IconAsset,
            Description = config.Description,
            Quality = config.Quality
        };
    }
}
