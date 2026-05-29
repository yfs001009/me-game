using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Battle;
using Hotfix.Shared;

namespace Hotfix.Battle.Handler;

public sealed class BattleMoveCommandHandler : MessageRPC<C2G_BattleMoveCommand, G2C_BattleMoveCommandResponse>
{
    protected override async FTask Run(Session session, C2G_BattleMoveCommand request, G2C_BattleMoveCommandResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.Success = false;
            response.Message = message;
            await FTask.CompletedTask;
            return;
        }

        var result = await BattleSceneGateway.Move(session.Scene, profile, request);
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Snapshot != null)
        {
            response.Snapshot = result.Snapshot;
        }
    }
}
