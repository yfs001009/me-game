using Fantasy;
using Fantasy.Async;
using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Network;
using TEngine;

namespace GameLogic.SheepBattle.Chat
{
    public sealed class ChatController
    {
        public static ChatController Instance { get; } = new ChatController();

        public ChatViewModel Model { get; } = new ChatViewModel();

        private ChatController()
        {
        }

        public async FTask<ChatViewModel> RefreshCompositeAsync()
        {
            var response = await SheepNetworkService.Instance.RequestChatHistoryAsync(ChatConstants.ChannelWorld, ChatConstants.WorldChannelId);
            if (response.Success)
            {
                Model.SetCompositeHistory(response.Messages);
            }

            Model.SetMode(ChatConstants.CompositeMode);
            GameEvent.Send(new ChatViewChangedEvent(Model));
            return Model;
        }

        public void SwitchMode(string mode)
        {
            Model.SetMode(mode);
            GameEvent.Send(new ChatViewChangedEvent(Model));
        }

        public async FTask SendCompositeAsync(string content)
        {
            await SendAsync(ChatConstants.ChannelWorld, ChatConstants.WorldChannelId, 0, content);
        }

        public async FTask SendPrivateAsync(long targetPlayerId, string content)
        {
            await SendAsync(ChatConstants.ChannelPrivate, 0, targetPlayerId, content);
        }

        public void OnReceive(ChatMessageTreeInfo message)
        {
            Model.AddMessage(message);
            GameEvent.Send(new ChatViewChangedEvent(Model));
        }

        private static async FTask SendAsync(int channelType, long channelId, long targetPlayerId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            var tree = ChatMessageTreeInfo.Create(Runtime.Session.Scene);
            tree.ChannelType = channelType;
            tree.ChannelId = channelId;
            if (targetPlayerId > 0)
            {
                tree.Targets.Add(targetPlayerId);
            }

            tree.Nodes.Add(new ChatInfoNode
            {
                NodeType = ChatConstants.NodeText,
                NodeEvent = ChatConstants.NodeEventNone,
                Content = content.Trim()
            });

            var response = await SheepNetworkService.Instance.SendChatMessageAsync(tree);
            if (!response.Success)
            {
                TEngine.Log.Warning($"聊天发送失败：{response.Message}");
            }
        }
    }
}
