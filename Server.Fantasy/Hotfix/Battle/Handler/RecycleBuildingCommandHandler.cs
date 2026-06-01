using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Battle;
using Hotfix.Shared;

namespace Hotfix.Battle.Handler;

public sealed class RecycleBuildingCommandHandler : MessageRPC<C2G_RecycleBuildingCommand, G2C_RecycleBuildingCommandResponse>
{
    protected override async FTask Run(Session session, C2G_RecycleBuildingCommand request, G2C_RecycleBuildingCommandResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.ErrorCode = 401;
            response.Success = false;
            response.Message = message;
            return;
        }

        var result = await BattleSceneGateway.Recycle(session.Scene, profile, request);
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Snapshot != null)
        {
            response.Snapshot = result.Snapshot;
        }
    }
}
