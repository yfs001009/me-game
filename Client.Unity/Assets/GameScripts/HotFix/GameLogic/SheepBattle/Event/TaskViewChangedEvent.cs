using GameLogic.SheepBattle.Task;
using TEngine;

namespace GameLogic.SheepBattle.Event
{
    public sealed class TaskViewChangedEvent : IEvent
    {
        public TaskViewModel ViewModel { get; }

        public TaskViewChangedEvent(TaskViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }
}
