using GameLogic.SheepBattle.Asset;
using TEngine;

namespace GameLogic.SheepBattle.Event
{
    public sealed class AssetViewChangedEvent : IEvent
    {
        public AssetViewModel ViewModel { get; }

        public AssetViewChangedEvent(AssetViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }
}
