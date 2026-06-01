using Fantasy.Async;
using GameLogic.SheepBattle.Asset;
using GameLogic.SheepBattle.Common;
using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Network;
using GameLogic.SheepBattle.Reward;
using TEngine;

namespace GameLogic.SheepBattle.Mail
{
    public sealed class MailController
    {
        public static MailController Instance { get; } = new();
        public MailViewModel Model { get; } = new();

        private MailController()
        {
        }

        public async FTask<MailViewModel> RefreshAsync()
        {
            var response = await SheepNetworkService.Instance.RequestMailListAsync();
            if (!response.Success)
            {
                CommonNoticeService.Show(response.Message);
                return Model;
            }

            Model.Apply(response.Mails);
            GameEvent.Send(new MailViewChangedEvent(Model));
            return Model;
        }

        public async FTask ReadAsync(long mailId)
        {
            var response = await SheepNetworkService.Instance.ReadMailAsync(mailId);
            if (!response.Success)
            {
                CommonNoticeService.Show(response.Message);
                return;
            }

            Model.Apply(response.Mails);
            GameEvent.Send(new MailViewChangedEvent(Model));
        }

        public async FTask ClaimAttachmentAsync(long mailId)
        {
            var response = await SheepNetworkService.Instance.ClaimMailAttachmentAsync(mailId);
            if (!response.Success)
            {
                CommonNoticeService.Show(response.Message);
                return;
            }

            Model.Apply(response.Mails);
            if (response.Snapshot != null)
            {
                AssetController.Instance.ApplySnapshot(response.Snapshot);
            }
            else
            {
                await AssetController.Instance.RefreshAsync();
            }

            GameEvent.Send(new MailViewChangedEvent(Model));
            var mail = Model.Mails.Find(v => v.MailId == mailId);
            RewardDisplayService.Show(RewardDisplayService.FromReward("获得附件", mail?.Attachment));
        }
    }
}
