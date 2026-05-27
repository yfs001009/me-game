using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Battle.Handler;

public sealed class RecycleBuildingCommandHandler : MessageRPC<C2G_RecycleBuildingCommand, G2C_RecycleBuildingCommandResponse>
{
    protected override async FTask Run(Session session, C2G_RecycleBuildingCommand request, G2C_RecycleBuildingCommandResponse response, Action reply)
    {
        var profile = SheepServices.Auth.RequireProfile(request.Token);
        var result = SheepServices.Battles.Recycle(profile, request);
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Snapshot != null)
        {
            response.Snapshot = result.Snapshot;
        }

        await FTask.CompletedTask;
    }
}
