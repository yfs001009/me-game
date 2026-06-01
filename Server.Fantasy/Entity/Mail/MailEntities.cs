using Fantasy;
using Fantasy.Entitas;

namespace Fantasy;

/// <summary>
/// Player mailbox aggregate. Mail behavior stays in Hotfix/Mail; this model is
/// the persistence and ownership boundary for per-player mail state.
/// </summary>
public sealed class PlayerMailboxEntity : Entity
{
    public long PlayerId { get; set; }
    public Dictionary<long, PlayerMailEntity> MailsByTemplateId { get; } = new();
}

public sealed class PlayerMailEntity : Entity
{
    public long MailId { get; set; }
    public long TemplateId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Sender { get; set; } = string.Empty;
    public DateTimeOffset SentAtUtc { get; set; }
    public bool IsRead { get; set; }
    public bool IsAttachmentClaimed { get; set; }
    public List<AssetCurrencyAmount> AttachmentCurrencies { get; } = new();
    public List<AssetItemAmount> AttachmentItems { get; } = new();
    public List<int> AttachmentCharacterUnlocks { get; } = new();
    public List<int> AttachmentBuildingCardUnlocks { get; } = new();
}

public sealed class MailTemplateEntity : Entity
{
    public long TemplateId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Sender { get; set; } = "System";
    public DateTimeOffset SentAtUtc { get; set; }
    public MailDeliveryTargetType TargetType { get; set; }
    public HashSet<long> TargetPlayerIds { get; } = new();
    public int MinLevel { get; set; }
    public int MaxLevel { get; set; }
    public DateTimeOffset? RegisteredAfterUtc { get; set; }
    public DateTimeOffset? RegisteredBeforeUtc { get; set; }
    public List<AssetCurrencyAmount> AttachmentCurrencies { get; } = new();
    public List<AssetItemAmount> AttachmentItems { get; } = new();
    public List<int> AttachmentCharacterUnlocks { get; } = new();
    public List<int> AttachmentBuildingCardUnlocks { get; } = new();
}

public enum MailDeliveryTargetType
{
    All = 0,
    Players = 1,
    Filter = 2
}
