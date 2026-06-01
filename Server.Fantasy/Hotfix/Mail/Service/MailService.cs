using Fantasy;
using GameConfig.asset;
using Hotfix.Asset.Model;
using Hotfix.Asset.Service;
using Hotfix.Config;
using Hotfix.Mail.Model;
using Hotfix.Shared;

namespace Hotfix.Mail.Service;

public sealed class MailService
{
    private readonly object gate = new();
    private readonly List<MailTemplate> templates = new();
    private readonly Dictionary<long, Dictionary<long, MailRecord>> mailboxByPlayerId = new();
    private long nextMailId = 100000;
    private long nextTemplateId = 1000;

    public IReadOnlyList<MailInfo> GetList(long playerId)
    {
        lock (gate)
        {
            var records = GetOrCreateMailsUnsafe(playerId);
            return records
                .OrderByDescending(v => v.SentAtUtc)
                .Select(ToInfo)
                .ToList();
        }
    }

    public (bool Success, string Message, IReadOnlyList<MailInfo> Mails) Read(long playerId, long mailId)
    {
        lock (gate)
        {
            var records = GetOrCreateMailsUnsafe(playerId);
            var mail = records.FirstOrDefault(v => v.MailId == mailId);
            if (mail == null)
            {
                return (false, "邮件不存在。", records.Select(ToInfo).ToList());
            }

            mail.IsRead = true;
            return (true, "邮件已读。", records.OrderByDescending(v => v.SentAtUtc).Select(ToInfo).ToList());
        }
    }

    public (bool Success, string Message, IReadOnlyList<MailInfo> Mails, AssetSnapshotInfo Snapshot) ClaimAttachment(long playerId, long mailId)
    {
        lock (gate)
        {
            var records = GetOrCreateMailsUnsafe(playerId);
            var mail = records.FirstOrDefault(v => v.MailId == mailId);
            if (mail == null)
            {
                return (false, "邮件不存在。", records.Select(ToInfo).ToList(), SheepServices.Assets.CreateSnapshot(playerId));
            }

            if (mail.IsAttachmentClaimed)
            {
                return (false, "附件已领取。", records.Select(ToInfo).ToList(), SheepServices.Assets.CreateSnapshot(playerId));
            }

            var context = AssetTransferContext.FromMailAttachment(mail.MailId);
            if (!SheepServices.Assets.TryTransferReward(playerId, mail.Attachment, context, out var snapshot, out var message))
            {
                return (false, message, records.Select(ToInfo).ToList(), snapshot);
            }

            mail.IsRead = true;
            mail.IsAttachmentClaimed = true;
            return (true, "附件领取成功。", records.OrderByDescending(v => v.SentAtUtc).Select(ToInfo).ToList(), snapshot);
        }
    }

    public void Send(long playerId, string title, string content, AssetReward attachment, string sender = "系统")
    {
        SendToPlayers(new[] { playerId }, title, content, attachment, sender);
    }

    public long SendToPlayers(IEnumerable<long> playerIds, string title, string content, AssetReward attachment, string sender = "系统")
    {
        return SendByRule(MailTargetRule.Players(playerIds), title, content, attachment, sender);
    }

    public long SendToAll(string title, string content, AssetReward attachment, string sender = "系统")
    {
        return SendByRule(MailTargetRule.All(), title, content, attachment, sender);
    }

    public long SendByRule(MailTargetRule rule, string title, string content, AssetReward attachment, string sender = "系统")
    {
        lock (gate)
        {
            var template = new MailTemplate
            {
                TemplateId = ++nextTemplateId,
                Title = title,
                Content = content,
                Sender = sender,
                SentAtUtc = DateTimeOffset.UtcNow,
                Attachment = attachment,
                TargetRule = rule
            };
            templates.Add(template);

            foreach (var playerId in rule.PlayerIds)
            {
                MaterializeTemplateUnsafe(playerId, template);
            }

            if (rule.Type != MailTargetType.Players)
            {
                foreach (var profile in SheepServices.Auth.GetMailRecipients())
                {
                    if (IsTarget(rule, profile))
                    {
                        MaterializeTemplateUnsafe(profile.PlayerId, template);
                    }
                }
            }

            return template.TemplateId;
        }
    }

    private List<MailRecord> GetOrCreateMailsUnsafe(long playerId)
    {
        var mailbox = GetMailboxUnsafe(playerId);
        foreach (var template in templates)
        {
            if (IsTarget(template.TargetRule, new MailRecipientProfile { PlayerId = playerId }))
            {
                MaterializeTemplateUnsafe(playerId, template);
            }
        }

        if (templates.Count == 0)
        {
            CreateInitialMailsUnsafe();
            foreach (var template in templates)
            {
                if (IsTarget(template.TargetRule, new MailRecipientProfile { PlayerId = playerId }))
                {
                    MaterializeTemplateUnsafe(playerId, template);
                }
            }
        }

        return mailbox.Values.ToList();
    }

    private Dictionary<long, MailRecord> GetMailboxUnsafe(long playerId)
    {
        if (mailboxByPlayerId.TryGetValue(playerId, out var mailbox))
        {
            return mailbox;
        }

        mailbox = new Dictionary<long, MailRecord>();
        mailboxByPlayerId.Add(playerId, mailbox);
        return mailbox;
    }

    private void CreateInitialMailsUnsafe()
    {
        var reward = new AssetReward();
        reward.Currencies.Add(new CurrencyAmount(1, 1000));
        reward.Items.Add(new ItemAmount(1001, 3));
        reward.Items.Add(new ItemAmount(1101, 1));

        SendToAll(
            "新手补给",
            "欢迎来到羊了个羊战场，附件里有初始金币、抽奖券和双倍经验卡。",
            reward);
    }

    private void MaterializeTemplateUnsafe(long playerId, MailTemplate template)
    {
        var mailbox = GetMailboxUnsafe(playerId);
        if (mailbox.ContainsKey(template.TemplateId))
        {
            return;
        }

        mailbox.Add(template.TemplateId, new MailRecord
        {
            MailId = ++nextMailId,
            TemplateId = template.TemplateId,
            Title = template.Title,
            Content = template.Content,
            Sender = template.Sender,
            SentAtUtc = template.SentAtUtc,
            Attachment = template.Attachment
        });
    }

    private static bool IsTarget(MailTargetRule rule, MailRecipientProfile profile)
    {
        return rule.Type switch
        {
            MailTargetType.All => true,
            MailTargetType.Players => rule.PlayerIds.Contains(profile.PlayerId),
            MailTargetType.Filter => MatchFilter(rule, profile),
            _ => false
        };
    }

    private static bool MatchFilter(MailTargetRule rule, MailRecipientProfile profile)
    {
        if (rule.MinLevel > 0 && profile.Level < rule.MinLevel)
        {
            return false;
        }

        if (rule.MaxLevel > 0 && profile.Level > rule.MaxLevel)
        {
            return false;
        }

        if (rule.RegisteredAfterUtc != null && profile.RegisteredAtUtc < rule.RegisteredAfterUtc)
        {
            return false;
        }

        return rule.RegisteredBeforeUtc == null || profile.RegisteredAtUtc <= rule.RegisteredBeforeUtc;
    }

    private static MailInfo ToInfo(MailRecord record)
    {
        return new MailInfo
        {
            MailId = record.MailId,
            Title = record.Title,
            Content = record.Content,
            Sender = record.Sender,
            SentAtUnixSeconds = record.SentAtUtc.ToUnixTimeSeconds(),
            IsRead = record.IsRead,
            IsAttachmentClaimed = record.IsAttachmentClaimed,
            Attachment = ToRewardInfo(record.Attachment)
        };
    }

    public static RewardInfo ToRewardInfo(AssetReward reward)
    {
        var info = new RewardInfo();
        foreach (var currency in reward.Currencies)
        {
            var config = ConfigSystem.Instance.Tables.TbCurrency.GetOrDefault(currency.CurrencyId);
            if (config != null)
            {
                info.Currencies.Add(AssetService.ToCurrencyInfo(config, currency.Amount));
            }
        }

        foreach (var item in reward.Items)
        {
            ItemConfig config = ConfigSystem.Instance.Tables.TbItem.GetOrDefault(item.ItemId);
            if (config != null)
            {
                info.Items.Add(AssetService.ToBagItemInfo(config, item.Count));
            }
        }

        info.CharacterIds.AddRange(reward.CharacterUnlocks);
        info.BuildingCardIds.AddRange(reward.BuildingCardUnlocks);
        return info;
    }
}
