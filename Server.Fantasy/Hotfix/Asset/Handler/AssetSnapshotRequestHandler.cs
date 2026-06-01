using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Asset.Handler;

public sealed class AssetSnapshotRequestHandler : MessageRPC<C2G_AssetSnapshotRequest, G2C_AssetSnapshotResponse>
{
    protected override async FTask Run(Session session, C2G_AssetSnapshotRequest request, G2C_AssetSnapshotResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.ErrorCode = 401;
            response.Success = false;
            response.Message = message;
            return;
        }

        SheepServices.Assets.BindScene(session.Scene);
        response.Success = true;
        response.Message = "资产快照获取成功。";
        response.Snapshot = SheepServices.Assets.CreateSnapshot(profile.PlayerId);
        Log.Info($"Asset snapshot response: playerId={profile.PlayerId}, bagItems={response.Snapshot.BagItems.Count}, normalTicket={SheepServices.Assets.GetItemCount(profile.PlayerId, 1001)}, premiumTicket={SheepServices.Assets.GetItemCount(profile.PlayerId, 1002)}");
        await FTask.CompletedTask;
    }
}
