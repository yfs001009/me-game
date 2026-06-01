using Fantasy;
using Fantasy.Async;
using GameLogic.SheepBattle.Common;
using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Network;
using TEngine;

namespace GameLogic.SheepBattle.Asset
{
    public sealed class AssetController
    {
        public static AssetController Instance { get; } = new();
        public AssetViewModel Model { get; } = new();

        private AssetController()
        {
        }

        public async FTask<AssetViewModel> RefreshAsync()
        {
            var response = await SheepNetworkService.Instance.RequestAssetSnapshotAsync();
            if (!response.Success)
            {
                CommonNoticeService.Show(response.Message);
                return Model;
            }

            ApplySnapshot(response.Snapshot);
            return Model;
        }

        public void ApplySnapshot(AssetSnapshotInfo snapshot)
        {
            if (snapshot == null)
            {
                return;
            }

            Model.Apply(snapshot);
            GameEvent.Send(new AssetViewChangedEvent(Model));
        }

        public async FTask UseItemAsync(int itemId, int count = 1)
        {
            var response = await SheepNetworkService.Instance.UseItemAsync(itemId, count);
            if (!response.Success)
            {
                CommonNoticeService.Show(response.Message);
                return;
            }

            ApplySnapshot(response.Snapshot);
            CommonNoticeService.Show(response.Message);
        }
    }
}
