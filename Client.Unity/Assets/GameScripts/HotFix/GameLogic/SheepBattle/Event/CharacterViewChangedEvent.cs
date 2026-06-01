using GameLogic.SheepBattle.Character;
using TEngine;

namespace GameLogic.SheepBattle.Event
{
    public sealed class CharacterViewChangedEvent : IEvent
    {
        public CharacterViewModel ViewModel { get; }

        public CharacterViewChangedEvent(CharacterViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }
}
