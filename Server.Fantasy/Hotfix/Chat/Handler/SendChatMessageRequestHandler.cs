using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Chat.Handler;

public sealed class SendChatMessageRequestHandler : MessageRPC<C2G_SendChatMessageRequest, G2C_SendChatMessageResponse>
{
    protected override async FTask Run(Session session, C2G_SendChatMessageRequest request, G2C_SendChatMessageResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.ErrorCode = 401;
            response.Success = false;
            response.Message = message;
            return;
        }

        var result = SheepServices.Chat.Send(profile, request.MessageTree, session);
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Tree != null)
        {
            response.MessageTree = result.Tree;
        }
        await FTask.CompletedTask;
    }
}
