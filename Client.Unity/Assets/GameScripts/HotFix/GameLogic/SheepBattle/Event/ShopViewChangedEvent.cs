using GameLogic.SheepBattle.Shop;
using TEngine;

namespace GameLogic.SheepBattle.Event
{
    public sealed class ShopViewChangedEvent : IEvent
    {
        public ShopViewModel ViewModel { get; }

        public ShopViewChangedEvent(ShopViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }
}
