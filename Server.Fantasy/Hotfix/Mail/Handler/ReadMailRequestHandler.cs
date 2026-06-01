using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Mail.Handler;

public sealed class ReadMailRequestHandler : MessageRPC<C2G_ReadMailRequest, G2C_ReadMailResponse>
{
    protected override async FTask Run(Session session, C2G_ReadMailRequest request, G2C_ReadMailResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.ErrorCode = 401;
            response.Success = false;
            response.Message = message;
            return;
        }

        var result = SheepServices.Mails.Read(profile.PlayerId, request.MailId);
        response.Success = result.Success;
        response.Message = result.Message;
        response.Mails.AddRange(result.Mails);
        await FTask.CompletedTask;
    }
}
