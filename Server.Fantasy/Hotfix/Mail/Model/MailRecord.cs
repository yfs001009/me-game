using Hotfix.Asset.Model;

namespace Hotfix.Mail.Model;

public sealed class MailRecord
{
    public long MailId { get; init; }
    public long TemplateId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string Sender { get; init; } = string.Empty;
    public DateTimeOffset SentAtUtc { get; init; }
    public bool IsRead { get; set; }
    public bool IsAttachmentClaimed { get; set; }
    public AssetReward Attachment { get; init; } = new();
}

public sealed class MailTemplate
{
    public long TemplateId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string Sender { get; init; } = "系统";
    public DateTimeOffset SentAtUtc { get; init; }
    public AssetReward Attachment { get; init; } = new();
    public MailTargetRule TargetRule { get; init; } = MailTargetRule.All();
}

public sealed class MailTargetRule
{
    public MailTargetType Type { get; init; }
    public HashSet<long> PlayerIds { get; init; } = new();
    public int MinLevel { get; init; }
    public int MaxLevel { get; init; }
    public DateTimeOffset? RegisteredAfterUtc { get; init; }
    public DateTimeOffset? RegisteredBeforeUtc { get; init; }

    public static MailTargetRule All() => new() { Type = MailTargetType.All };

    public static MailTargetRule Players(IEnumerable<long> playerIds)
    {
        return new MailTargetRule
        {
            Type = MailTargetType.Players,
            PlayerIds = playerIds.ToHashSet()
        };
    }
}

public enum MailTargetType
{
    All = 0,
    Players = 1,
    Filter = 2
}

public sealed class MailRecipientProfile
{
    public long PlayerId { get; init; }
    public int Level { get; init; }
    public DateTimeOffset RegisteredAtUtc { get; init; }
}
