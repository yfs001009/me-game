using Fantasy;
using Fantasy.Entitas;

namespace Fantasy;

/// <summary>
/// Unified feature/open definition. Shops, tasks and activity entries reference
/// this FeatureId, so one open-condition definition controls all attached content.
/// </summary>
public sealed class FeatureGateEntity : Entity
{
    public string FeatureId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string OpenCondition { get; set; } = string.Empty;
    public bool IsOpen { get; set; }
    public DateTimeOffset OpensAtUtc { get; set; }
    public DateTimeOffset ClosesAtUtc { get; set; }
}
