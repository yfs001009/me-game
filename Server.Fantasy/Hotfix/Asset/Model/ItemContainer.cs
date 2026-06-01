using GameConfig.asset;

namespace Hotfix.Asset.Model;

public sealed class ItemContainer
{
    private readonly Dictionary<int, int> itemCounts = new();

    public IReadOnlyDictionary<int, int> ItemCounts => itemCounts;

    public int GetCount(int itemId)
    {
        return itemCounts.GetValueOrDefault(itemId);
    }

    public bool CanAdd(ItemConfig config, int count)
    {
        if (config == null || count <= 0)
        {
            return false;
        }

        var maxStack = config.MaxStack > 0 ? config.MaxStack : int.MaxValue;
        return GetCount(config.ItemId) <= maxStack - count;
    }

    public bool TryAdd(ItemConfig config, int count, out string message)
    {
        message = string.Empty;
        if (!CanAdd(config, count))
        {
            message = "道具数量已达上限。";
            return false;
        }

        itemCounts[config.ItemId] = GetCount(config.ItemId) + count;
        return true;
    }

    public bool TryRemove(int itemId, int count, out string message)
    {
        message = string.Empty;
        if (count <= 0)
        {
            message = "扣除数量必须大于 0。";
            return false;
        }

        var current = GetCount(itemId);
        if (current < count)
        {
            message = "道具数量不足。";
            return false;
        }

        var remaining = current - count;
        if (remaining == 0)
        {
            itemCounts.Remove(itemId);
        }
        else
        {
            itemCounts[itemId] = remaining;
        }

        return true;
    }
}
