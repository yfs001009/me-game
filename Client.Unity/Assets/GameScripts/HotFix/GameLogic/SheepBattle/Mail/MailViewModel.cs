using System.Collections.Generic;
using System.Linq;
using Fantasy;

namespace GameLogic.SheepBattle.Mail
{
    public sealed class MailViewModel
    {
        public List<MailEntryViewModel> Mails { get; } = new();

        public void Apply(IReadOnlyList<MailInfo> mails)
        {
            Mails.Clear();
            if (mails == null)
            {
                return;
            }

            Mails.AddRange(mails.Select(v => new MailEntryViewModel(v)));
        }
    }

    public sealed class MailEntryViewModel
    {
        public MailEntryViewModel(MailInfo info)
        {
            MailId = info?.MailId ?? 0;
            Title = info?.Title ?? string.Empty;
            Content = info?.Content ?? string.Empty;
            Sender = info?.Sender ?? string.Empty;
            SentAtUnixSeconds = info?.SentAtUnixSeconds ?? 0;
            IsRead = info?.IsRead ?? false;
            IsAttachmentClaimed = info?.IsAttachmentClaimed ?? false;
            Attachment = info?.Attachment;
        }

        public long MailId { get; }
        public string Title { get; }
        public string Content { get; }
        public string Sender { get; }
        public long SentAtUnixSeconds { get; }
        public bool IsRead { get; }
        public bool IsAttachmentClaimed { get; }
        public RewardInfo Attachment { get; }
        public bool HasAttachment => Attachment != null &&
                                     (Attachment.Currencies.Count > 0 ||
                                      Attachment.Items.Count > 0 ||
                                      Attachment.CharacterIds.Count > 0 ||
                                      Attachment.BuildingCardIds.Count > 0);
    }
}
