using System.Collections.Generic;
using System.Linq;
using Fantasy;

namespace GameLogic.SheepBattle.Chat
{
    public sealed class ChatViewModel
    {
        private readonly List<ChatMessageTreeInfo> compositeMessages = new();
        private readonly List<ChatMessageTreeInfo> privateMessages = new();

        public string ViewMode { get; private set; } = ChatConstants.CompositeMode;
        public IReadOnlyList<ChatMessageTreeInfo> CompositeMessages => compositeMessages;
        public IReadOnlyList<ChatMessageTreeInfo> PrivateMessages => privateMessages;
        public IReadOnlyList<ChatMessageTreeInfo> CurrentMessages => IsPrivateMode ? privateMessages : compositeMessages;
        public bool IsPrivateMode => ViewMode == ChatConstants.PrivateMode;

        public void SetMode(string mode)
        {
            ViewMode = mode == ChatConstants.PrivateMode ? ChatConstants.PrivateMode : ChatConstants.CompositeMode;
        }

        public void SetCompositeHistory(IEnumerable<ChatMessageTreeInfo> messages)
        {
            compositeMessages.Clear();
            compositeMessages.AddRange(messages ?? Enumerable.Empty<ChatMessageTreeInfo>());
        }

        public void AddMessage(ChatMessageTreeInfo message)
        {
            if (message == null)
            {
                return;
            }

            if (message.ChannelType == ChatConstants.ChannelPrivate)
            {
                AddBounded(privateMessages, message);
                return;
            }

            AddBounded(compositeMessages, message);
        }

        private static void AddBounded(List<ChatMessageTreeInfo> list, ChatMessageTreeInfo message)
        {
            if (list.Any(item => item.MessageId > 0 && item.MessageId == message.MessageId))
            {
                return;
            }

            list.Add(message);
            if (list.Count > 80)
            {
                list.RemoveRange(0, list.Count - 80);
            }
        }
    }
}
