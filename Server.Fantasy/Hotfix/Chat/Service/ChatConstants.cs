namespace Hotfix.Chat.Service;

public static class ChatConstants
{
    public const int ChannelNone = 0;
    public const int ChannelWorld = 1;
    public const int ChannelPrivate = 2;
    public const int ChannelRoom = 3;
    public const int ChannelGuild = 4;
    public const int ChannelTeam = 5;
    public const int ChannelSystem = 6;

    public const int NodeText = 0;
    public const int NodeLink = 1;
    public const int NodeEmoji = 2;
    public const int NodeItem = 3;
    public const int NodeSystem = 4;

    public const int NodeEventNone = 0;
    public const int NodeEventOpenPlayer = 1;
    public const int NodeEventOpenItem = 2;

    public const long WorldChannelId = 1;
}
