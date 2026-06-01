using Fantasy;
using Fantasy.Entitas;
using Fantasy.Network;

namespace Fantasy;

/// <summary>
/// Chat Scene target state. Session values are transient and should not be
/// persisted; histories are bounded by Hotfix/Chat policy.
/// </summary>
public sealed class ChatChannelEntity : Entity
{
    public int ChannelType { get; set; }
    public long ChannelId { get; set; }
    public List<ChatMessageTreeInfo> History { get; } = new();
}

public sealed class ChatOnlineComponent : Entity
{
    public Dictionary<long, Session> SessionsByPlayerId { get; } = new();
}

