using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Mail.Handler;

public sealed class ClaimMailAttachmentRequestHandler : MessageRPC<C2G_ClaimMailAttachmentRequest, G2C_ClaimMailAttachmentResponse>
{
    protected override async FTask Run(Session session, C2G_ClaimMailAttachmentRequest request, G2C_ClaimMailAttachmentResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.ErrorCode = 401;
            response.Success = false;
            response.Message = message;
            return;
        }

        var result = SheepServices.Mails.ClaimAttachment(profile.PlayerId, request.MailId);
        response.Success = result.Success;
        response.Message = result.Message;
        response.Mails.AddRange(result.Mails);
        response.Snapshot = result.Snapshot;
        await FTask.CompletedTask;
    }
}
