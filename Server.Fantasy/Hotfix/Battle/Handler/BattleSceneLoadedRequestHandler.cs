using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Battle;
using Hotfix.Shared;

namespace Hotfix.Battle.Handler;

public sealed class BattleSceneLoadedRequestHandler : MessageRPC<C2G_BattleSceneLoadedRequest, G2C_BattleSceneLoadedResponse>
{
    protected override async FTask Run(Session session, C2G_BattleSceneLoadedRequest request, G2C_BattleSceneLoadedResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.ErrorCode = 401;
            response.Success = false;
            response.Message = message;
            return;
        }

        var result = await BattleSceneGateway.SceneLoaded(session.Scene, profile, request);
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Snapshot != null)
        {
            response.Snapshot = result.Snapshot;
        }
    }
}
