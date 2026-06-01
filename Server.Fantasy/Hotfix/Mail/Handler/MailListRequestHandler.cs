using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Mail.Handler;

public sealed class MailListRequestHandler : MessageRPC<C2G_MailListRequest, G2C_MailListResponse>
{
    protected override async FTask Run(Session session, C2G_MailListRequest request, G2C_MailListResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.ErrorCode = 401;
            response.Success = false;
            response.Message = message;
            return;
        }

        response.Success = true;
        response.Message = "邮件列表获取成功。";
        response.Mails.AddRange(SheepServices.Mails.GetList(profile.PlayerId));
        await FTask.CompletedTask;
    }
}
