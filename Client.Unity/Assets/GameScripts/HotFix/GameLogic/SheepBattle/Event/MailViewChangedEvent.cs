using GameLogic.SheepBattle.Mail;
using TEngine;

namespace GameLogic.SheepBattle.Event
{
    public sealed class MailViewChangedEvent : IEvent
    {
        public MailViewModel ViewModel { get; }

        public MailViewChangedEvent(MailViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }
}
