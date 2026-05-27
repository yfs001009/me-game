using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Battle.Handler;

public sealed class BattleMoveCommandHandler : MessageRPC<C2G_BattleMoveCommand, G2C_BattleMoveCommandResponse>
{
    protected override async FTask Run(Session session, C2G_BattleMoveCommand request, G2C_BattleMoveCommandResponse response, Action reply)
    {
        var profile = SheepServices.Auth.RequireProfile(request.Token);
        var result = SheepServices.Battles.Move(profile, request);
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Snapshot != null)
        {
            response.Snapshot = result.Snapshot;
        }

        await FTask.CompletedTask;
    }
}
