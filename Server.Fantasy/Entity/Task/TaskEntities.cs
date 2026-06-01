using Fantasy;
using Fantasy.Entitas;

namespace Fantasy;

/// <summary>
/// Per-player outgame task state. Progress values live in PlayerProgressComponent;
/// this aggregate stores claim state and refresh ownership.
/// </summary>
public sealed class PlayerOutgameTaskEntity : Entity
{
    public long PlayerId { get; set; }
    public Dictionary<int, OutgameTaskRecord> TasksById { get; } = new();
}

public sealed class OutgameTaskRecord
{
    public int TaskId { get; set; }
    public string RefreshGroup { get; set; } = string.Empty;
    public string State { get; set; } = "Doing";
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}

