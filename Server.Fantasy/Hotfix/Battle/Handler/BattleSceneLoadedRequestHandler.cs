using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Battle.Handler;

public sealed class BattleSceneLoadedRequestHandler : MessageRPC<C2G_BattleSceneLoadedRequest, G2C_BattleSceneLoadedResponse>
{
    protected override async FTask Run(Session session, C2G_BattleSceneLoadedRequest request, G2C_BattleSceneLoadedResponse response, Action reply)
    {
        var profile = SheepServices.Auth.RequireProfile(request.Token);
        var result = SheepServices.Battles.MarkSceneLoaded(profile, request.BattleId);
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Snapshot != null)
        {
            response.Snapshot = result.Snapshot;
        }

        await FTask.CompletedTask;
    }
}
