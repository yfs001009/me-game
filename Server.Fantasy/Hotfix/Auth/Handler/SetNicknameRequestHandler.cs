using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Auth.Handler;

/// <summary>
/// 首次登录后设置玩家昵称。
/// </summary>
public sealed class SetNicknameRequestHandler : MessageRPC<C2G_SetNicknameRequest, G2C_SetNicknameResponse>
{
    protected override async FTask Run(Session session, C2G_SetNicknameRequest request, G2C_SetNicknameResponse response, Action reply)
    {
        var result = SheepServices.Auth.SetNickname(request.Token, request.Nickname);
        response.Success = result.Success;
        response.Message = result.Message;
        response.Profile = result.Profile;
        await FTask.CompletedTask;
    }
}
