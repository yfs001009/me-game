using Fantasy;
using Fantasy.Entitas;

namespace Fantasy;

/// <summary>
/// Player outgame asset aggregate. It owns bag, currencies, unlocks, buffs and
/// progress values as Fantasy components so persistence can later attach at the
/// aggregate boundary.
/// </summary>
public sealed class PlayerAssetEntity : Entity
{
    public long PlayerId { get; set; }
    public int AssetSchemaVersion { get; set; }
}

public sealed class PlayerCurrencyComponent : Entity
{
    public Dictionary<int, long> Currencies { get; } = new();
}

public sealed class PlayerBagComponent : Entity
{
    public Dictionary<int, int> ItemCounts { get; } = new();
}

public sealed class PlayerUnlockComponent : Entity
{
    public HashSet<int> CharacterIds { get; } = new();
    public HashSet<int> BuildingCardIds { get; } = new();
}

public sealed class PlayerBuffComponent : Entity
{
    public Dictionary<string, DateTimeOffset> BuffExpiresAtUtc { get; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class PlayerProgressComponent : Entity
{
    public Dictionary<string, long> Values { get; } = new(StringComparer.OrdinalIgnoreCase);
}
