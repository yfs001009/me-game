using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Lobby.Handler;

/// <summary>
/// 开始匹配 RPC。当前只登记匹配状态，后续接 Match Scene 分配房间。
/// </summary>
public sealed class StartMatchRequestHandler : MessageRPC<C2G_StartMatchRequest, G2C_StartMatchResponse>
{
    protected override async FTask Run(Session session, C2G_StartMatchRequest request, G2C_StartMatchResponse response, Action reply)
    {
        var profile = SheepServices.Auth.RequireProfile(request.Token);
        response.Status = SheepServices.Match.Start(profile, request.Mode);
        await FTask.CompletedTask;
    }
}
