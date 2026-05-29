using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Battle;
using Hotfix.Shared;

namespace Hotfix.Battle.Handler;

public sealed class UpgradeBuildingCommandHandler : MessageRPC<C2G_UpgradeBuildingCommand, G2C_UpgradeBuildingCommandResponse>
{
    protected override async FTask Run(Session session, C2G_UpgradeBuildingCommand request, G2C_UpgradeBuildingCommandResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.Success = false;
            response.Message = message;
            await FTask.CompletedTask;
            return;
        }

        var result = await BattleSceneGateway.Upgrade(session.Scene, profile, request);
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Snapshot != null)
        {
            response.Snapshot = result.Snapshot;
        }
    }
}
