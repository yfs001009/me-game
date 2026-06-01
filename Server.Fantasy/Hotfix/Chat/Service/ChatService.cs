using Fantasy;
using Fantasy.Network;
using Hotfix.Shared;

namespace Hotfix.Chat.Service;

public sealed class ChatService
{
    private const int MaxHistoryCount = 80;
    private const int MaxContentLength = 120;

    private readonly object gate = new();
    private readonly Dictionary<long, Session> onlineSessions = new();
    private readonly List<ChatMessageTreeInfo> worldHistory = new();
    private readonly Dictionary<long, List<ChatMessageTreeInfo>> privateHistory = new();
    private long nextMessageId;

    public void RegisterOnline(long playerId, Session session)
    {
        if (playerId <= 0 || session == null)
        {
            return;
        }

        lock (gate)
        {
            onlineSessions[playerId] = session;
        }
    }

    public void Unregister(Session session)
    {
        if (session == null)
        {
            return;
        }

        lock (gate)
        {
            var removeIds = onlineSessions
                .Where(pair => ReferenceEquals(pair.Value, session) || pair.Value.IsDisposed)
                .Select(pair => pair.Key)
                .ToList();

            foreach (var playerId in removeIds)
            {
                onlineSessions.Remove(playerId);
            }
        }
    }

    public (bool Success, string Message, ChatMessageTreeInfo? Tree) Send(PlayerProfileInfo profile, ChatMessageTreeInfo requestTree, Session senderSession)
    {
        if (profile == null || profile.PlayerId <= 0)
        {
            return (false, "登录状态已失效。", null);
        }

        var tree = Normalize(profile, requestTree);
        var content = BuildPlainText(tree);
        if (string.IsNullOrWhiteSpace(content))
        {
            return (false, "聊天内容不能为空。", null);
        }

        if (content.Length > MaxContentLength)
        {
            return (false, $"聊天内容不能超过 {MaxContentLength} 个字符。", null);
        }

        lock (gate)
        {
            RegisterOnline(profile.PlayerId, senderSession);
            tree.MessageId = ++nextMessageId;
            tree.SendTimeUnixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            StoreHistory(tree);
            PushLocked(tree);
        }

        return (true, "发送成功。", tree);
    }

    public IReadOnlyList<ChatMessageTreeInfo> GetHistory(long playerId, int channelType, long channelId, int limit)
    {
        lock (gate)
        {
            limit = Math.Clamp(limit <= 0 ? 30 : limit, 1, MaxHistoryCount);
            if (channelType == ChatConstants.ChannelPrivate)
            {
                var key = GetPrivateHistoryKey(playerId, channelId);
                return privateHistory.TryGetValue(key, out var list)
                    ? list.TakeLast(limit).ToList()
                    : Array.Empty<ChatMessageTreeInfo>();
            }

            return worldHistory.TakeLast(limit).ToList();
        }
    }

    private ChatMessageTreeInfo Normalize(PlayerProfileInfo profile, ChatMessageTreeInfo requestTree)
    {
        var tree = new ChatMessageTreeInfo
        {
            ChannelType = requestTree?.ChannelType > 0 ? requestTree.ChannelType : ChatConstants.ChannelWorld,
            ChannelId = requestTree?.ChannelId > 0 ? requestTree.ChannelId : ChatConstants.WorldChannelId,
            UnitId = profile.PlayerId,
            UserName = string.IsNullOrWhiteSpace(profile.Nickname) ? profile.Account : profile.Nickname,
            IsHaveLinkItem = requestTree?.IsHaveLinkItem == true,
            SystemBroadcastId = requestTree?.SystemBroadcastId ?? 0
        };

        if (tree.ChannelType != ChatConstants.ChannelPrivate)
        {
            tree.ChannelType = ChatConstants.ChannelWorld;
            tree.ChannelId = ChatConstants.WorldChannelId;
        }

        if (requestTree?.Targets != null)
        {
            foreach (var target in requestTree.Targets.Where(id => id > 0).Distinct())
            {
                tree.Targets.Add(target);
            }
        }

        if (tree.ChannelType == ChatConstants.ChannelPrivate)
        {
            var targetId = tree.Targets.FirstOrDefault(id => id != profile.PlayerId);
            if (targetId <= 0)
            {
                tree.Targets.Clear();
                return tree;
            }

            tree.ChannelId = GetPrivateHistoryKey(profile.PlayerId, targetId);
            tree.Targets.Clear();
            tree.Targets.Add(profile.PlayerId);
            tree.Targets.Add(targetId);
        }

        if (requestTree?.Nodes != null && requestTree.Nodes.Count > 0)
        {
            foreach (var node in requestTree.Nodes)
            {
                AddNode(
                    tree,
                    node?.Content ?? string.Empty,
                    node?.NodeType ?? ChatConstants.NodeText,
                    node?.NodeEvent ?? ChatConstants.NodeEventNone,
                    node?.Color ?? string.Empty,
                    node?.Data ?? string.Empty);
            }
        }
        else
        {
            AddNode(tree, string.Empty, ChatConstants.NodeText, ChatConstants.NodeEventNone, string.Empty, string.Empty);
        }

        return tree;
    }

    private static void AddNode(ChatMessageTreeInfo tree, string content, int nodeType, int nodeEvent, string color, string data)
    {
        tree.Nodes.Add(new ChatInfoNode
        {
            NodeType = nodeType,
            NodeEvent = nodeEvent,
            Content = content?.Trim() ?? string.Empty,
            Color = color ?? string.Empty,
            Data = data ?? string.Empty
        });
    }

    private void StoreHistory(ChatMessageTreeInfo tree)
    {
        if (tree.ChannelType == ChatConstants.ChannelPrivate)
        {
            if (!privateHistory.TryGetValue(tree.ChannelId, out var list))
            {
                list = new List<ChatMessageTreeInfo>();
                privateHistory.Add(tree.ChannelId, list);
            }

            AddBounded(list, tree);
            return;
        }

        AddBounded(worldHistory, tree);
    }

    private static void AddBounded(List<ChatMessageTreeInfo> list, ChatMessageTreeInfo tree)
    {
        list.Add(tree);
        if (list.Count > MaxHistoryCount)
        {
            list.RemoveRange(0, list.Count - MaxHistoryCount);
        }
    }

    private void PushLocked(ChatMessageTreeInfo tree)
    {
        if (tree.ChannelType == ChatConstants.ChannelPrivate)
        {
            foreach (var target in tree.Targets.Distinct())
            {
                PushTo(target, tree);
            }

            return;
        }

        foreach (var playerId in onlineSessions.Keys.ToList())
        {
            PushTo(playerId, tree);
        }
    }

    private void PushTo(long playerId, ChatMessageTreeInfo tree)
    {
        if (!onlineSessions.TryGetValue(playerId, out var session) || session.IsDisposed)
        {
            onlineSessions.Remove(playerId);
            return;
        }

        session.Send(new G2C_ChatMessageNotify { MessageTree = tree });
    }

    private static string BuildPlainText(ChatMessageTreeInfo tree)
    {
        return tree?.Nodes == null
            ? string.Empty
            : string.Concat(tree.Nodes.Select(node => node?.Content ?? string.Empty)).Trim();
    }

    private static long GetPrivateHistoryKey(long left, long right)
    {
        var min = Math.Min(left, right);
        var max = Math.Max(left, right);
        return min * 10000000000L + max;
    }
}
