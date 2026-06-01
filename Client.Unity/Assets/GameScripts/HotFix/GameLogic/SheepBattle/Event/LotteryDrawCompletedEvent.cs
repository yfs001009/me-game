using Fantasy;
using TEngine;

namespace GameLogic.SheepBattle.Event
{
    public sealed class LotteryDrawCompletedEvent : IEvent
    {
        public G2C_LotteryDrawResponse Response { get; }

        public LotteryDrawCompletedEvent(G2C_LotteryDrawResponse response)
        {
            Response = response;
        }
    }
}
