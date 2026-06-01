using Fantasy;
using Fantasy.Entitas;

namespace Fantasy;

/// <summary>
/// Per-player lottery progress such as draw count and pity counters.
/// </summary>
public sealed class PlayerLotteryEntity : Entity
{
    public long PlayerId { get; set; }
    public Dictionary<string, LotteryPoolProgress> Pools { get; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class LotteryPoolProgress
{
    public string Pool { get; set; } = string.Empty;
    public long DrawCount { get; set; }
    public long PityCount { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}

