using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Battle.Handler;

public sealed class BattleSnapshotRequestHandler : MessageRPC<C2G_BattleSnapshotRequest, G2C_BattleSnapshotResponse>
{
    protected override async FTask Run(Session session, C2G_BattleSnapshotRequest request, G2C_BattleSnapshotResponse response, Action reply)
    {
        var profile = SheepServices.Auth.RequireProfile(request.Token);
        var result = SheepServices.Battles.GetSnapshot(profile, request.BattleId);
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Snapshot != null)
        {
            response.Snapshot = result.Snapshot;
        }

        await FTask.CompletedTask;
    }
}
