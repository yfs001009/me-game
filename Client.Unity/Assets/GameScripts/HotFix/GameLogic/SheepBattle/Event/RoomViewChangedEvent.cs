using GameLogic.SheepBattle.Lobby;
using TEngine;

namespace GameLogic.SheepBattle.Event
{
    public sealed class RoomViewChangedEvent : IEvent
    {
        public RoomViewModel ViewModel { get; }

        public RoomViewChangedEvent(RoomViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }
}
