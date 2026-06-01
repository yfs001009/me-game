using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Auth.Handler;

/// <summary>
/// 账号注册 RPC。只负责协议适配，业务规则在 AuthService 中。
/// </summary>
public sealed class RegisterRequestHandler : MessageRPC<C2G_RegisterRequest, G2C_RegisterResponse>
{
    protected override async FTask Run(Session session, C2G_RegisterRequest request, G2C_RegisterResponse response, Action reply)
    {
        Log.Info($"收到注册请求：账号={request.Account}，昵称={request.Nickname}");
        var result = SheepServices.Auth.Register(session.Scene, request.Account, request.Password, request.Nickname);
        response.Success = result.Success;
        response.Message = result.Message;
        await FTask.CompletedTask;
    }
}
