using Fantasy.Async;
using GameLogic.SheepBattle.Common;
using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Network;
using TEngine;

namespace GameLogic.SheepBattle.Social
{
    public sealed class SocialController
    {
        public static SocialController Instance { get; } = new();
        public SocialViewModel Model { get; } = new();

        private SocialController()
        {
        }

        public async FTask<SocialViewModel> RefreshAsync(string viewMode = SocialViewModel.FollowingMode, string keyword = "")
        {
            var response = await SheepNetworkService.Instance.RequestSocialListAsync(viewMode, keyword);
            if (!response.Success)
            {
                CommonNoticeService.Show(response.Message);
                return Model;
            }

            Model.Apply(response);
            GameEvent.Send(new SocialViewChangedEvent(Model));
            return Model;
        }

        public async FTask SetFollowAsync(long targetPlayerId, bool follow, string viewMode, string keyword = "")
        {
            var response = await SheepNetworkService.Instance.FollowPlayerAsync(targetPlayerId, follow, viewMode);
            if (!response.Success)
            {
                CommonNoticeService.Show(response.Message);
                return;
            }

            if (string.Equals(viewMode, SocialViewModel.SearchMode, System.StringComparison.OrdinalIgnoreCase))
            {
                await RefreshAsync(viewMode, keyword);
                CommonNoticeService.Show(response.Message);
                return;
            }

            Model.Apply(response);
            GameEvent.Send(new SocialViewChangedEvent(Model));
            CommonNoticeService.Show(response.Message);
        }
    }
}
