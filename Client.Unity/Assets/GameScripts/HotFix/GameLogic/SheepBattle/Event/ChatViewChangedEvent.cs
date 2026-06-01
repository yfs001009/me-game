using GameLogic.SheepBattle.Chat;
using TEngine;

namespace GameLogic.SheepBattle.Event
{
    public sealed class ChatViewChangedEvent : IEvent
    {
        public ChatViewModel ViewModel { get; }

        public ChatViewChangedEvent(ChatViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }
}
