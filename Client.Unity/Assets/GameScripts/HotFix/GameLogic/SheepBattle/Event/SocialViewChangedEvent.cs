using GameLogic.SheepBattle.Social;
using TEngine;

namespace GameLogic.SheepBattle.Event
{
    public sealed class SocialViewChangedEvent : IEvent
    {
        public SocialViewModel ViewModel { get; }

        public SocialViewChangedEvent(SocialViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }
}
