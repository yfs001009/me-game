using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Chat.Handler;

public sealed class ChatHistoryRequestHandler : MessageRPC<C2G_ChatHistoryRequest, G2C_ChatHistoryResponse>
{
    protected override async FTask Run(Session session, C2G_ChatHistoryRequest request, G2C_ChatHistoryResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.ErrorCode = 401;
            response.Success = false;
            response.Message = message;
            return;
        }

        SheepServices.Chat.RegisterOnline(profile.PlayerId, session);
        response.Success = true;
        response.Message = "聊天记录获取成功。";
        response.Messages.AddRange(SheepServices.Chat.GetHistory(profile.PlayerId, request.ChannelType, request.ChannelId, request.Limit));
        await FTask.CompletedTask;
    }
}
