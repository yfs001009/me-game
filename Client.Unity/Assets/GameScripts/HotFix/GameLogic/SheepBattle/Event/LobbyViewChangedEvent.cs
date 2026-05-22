using GameLogic.SheepBattle.Lobby;
using TEngine;

namespace GameLogic.SheepBattle.Event
{
    public sealed class LobbyViewChangedEvent : IEvent
    {
        public LobbyViewModel ViewModel { get; }

        public LobbyViewChangedEvent(LobbyViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }
}
