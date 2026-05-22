using TEngine;

namespace GameLogic.SheepBattle.Event
{
    public sealed class LobbyStatusChangedEvent : IEvent
    {
        public string Status { get; }

        public LobbyStatusChangedEvent(string status)
        {
            Status = status ?? string.Empty;
        }
    }
}
