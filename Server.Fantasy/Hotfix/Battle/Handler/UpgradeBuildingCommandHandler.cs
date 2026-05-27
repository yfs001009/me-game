using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Battle.Handler;

public sealed class UpgradeBuildingCommandHandler : MessageRPC<C2G_UpgradeBuildingCommand, G2C_UpgradeBuildingCommandResponse>
{
    protected override async FTask Run(Session session, C2G_UpgradeBuildingCommand request, G2C_UpgradeBuildingCommandResponse response, Action reply)
    {
        var profile = SheepServices.Auth.RequireProfile(request.Token);
        var result = SheepServices.Battles.Upgrade(profile, request);
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Snapshot != null)
        {
            response.Snapshot = result.Snapshot;
        }

        await FTask.CompletedTask;
    }
}
