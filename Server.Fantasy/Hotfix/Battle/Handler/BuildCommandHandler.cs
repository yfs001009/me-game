using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Battle.Handler;

public sealed class BuildCommandHandler : MessageRPC<C2G_BuildCommand, G2C_BuildCommandResponse>
{
    protected override async FTask Run(Session session, C2G_BuildCommand request, G2C_BuildCommandResponse response, Action reply)
    {
        var profile = SheepServices.Auth.RequireProfile(request.Token);
        var result = SheepServices.Battles.Build(profile, request);
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Snapshot != null)
        {
            response.Snapshot = result.Snapshot;
        }

        await FTask.CompletedTask;
    }
}
