using TEngine;

namespace GameLogic.SheepBattle.Event
{
    public sealed class LoginStatusChangedEvent : IEvent
    {
        public string Status { get; }
        public bool IsBusy { get; }

        public LoginStatusChangedEvent(string status, bool isBusy)
        {
            Status = status ?? string.Empty;
            IsBusy = isBusy;
        }
    }
}
