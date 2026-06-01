using Fantasy;
using Fantasy.Entitas;

namespace Fantasy;

/// <summary>
/// Player social graph state. Hotfix/Social owns commands and queries; this
/// entity keeps following/follower sets ready for persistence or Scene routing.
/// </summary>
public sealed class PlayerSocialEntity : Entity
{
    public long PlayerId { get; set; }
    public HashSet<long> FollowingPlayerIds { get; } = new();
    public HashSet<long> FollowerPlayerIds { get; } = new();
}

