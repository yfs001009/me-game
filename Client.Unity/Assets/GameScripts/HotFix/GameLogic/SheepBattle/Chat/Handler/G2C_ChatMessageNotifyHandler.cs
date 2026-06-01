using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;

namespace GameLogic.SheepBattle.Chat.Handler
{
    public sealed class G2C_ChatMessageNotifyHandler : Message<G2C_ChatMessageNotify>
    {
        protected override async FTask Run(Session session, G2C_ChatMessageNotify message)
        {
            ChatController.Instance.OnReceive(message.MessageTree);
            await FTask.CompletedTask;
        }
    }
}
