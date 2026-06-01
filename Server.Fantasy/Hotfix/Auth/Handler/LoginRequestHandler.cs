using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Auth.Handler;

/// <summary>
/// 登录 RPC。成功后返回自定义 Token 和玩家资料。
/// </summary>
public sealed class LoginRequestHandler : MessageRPC<C2G_LoginRequest, G2C_LoginResponse>
{
    protected override async FTask Run(Session session, C2G_LoginRequest request, G2C_LoginResponse response, Action reply)
    {
        Log.Info($"收到登录请求：账号={request.Account}");
        var result = SheepServices.Auth.Login(session.Scene, request.Account, request.Password);
        response.Success = result.Success;
        response.Message = result.Message;
        response.Token = result.Token;
        response.Profile = result.Profile;
        if (result.Success)
        {
            SheepServices.Assets.BindScene(session.Scene);
            SheepServices.Tasks.AddProgress(result.Profile.PlayerId, "Login.Count", 1);
        }
        await FTask.CompletedTask;
    }
}
