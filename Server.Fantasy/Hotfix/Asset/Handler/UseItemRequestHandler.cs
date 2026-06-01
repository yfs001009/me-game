using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Asset.Handler;

public sealed class UseItemRequestHandler : MessageRPC<C2G_UseItemRequest, G2C_UseItemResponse>
{
    protected override async FTask Run(Session session, C2G_UseItemRequest request, G2C_UseItemResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.ErrorCode = 401;
            response.Success = false;
            response.Message = message;
            return;
        }

        SheepServices.Assets.BindScene(session.Scene);
        response.Success = SheepServices.Assets.TryUseItem(profile.PlayerId, request.ItemId, request.Count, out message);
        response.Message = message;
        response.Snapshot = SheepServices.Assets.CreateSnapshot(profile.PlayerId);
        if (response.Success)
        {
            SheepServices.Tasks.AddProgress(profile.PlayerId, "Item.Use.Count", Math.Max(1, request.Count));
        }
        await FTask.CompletedTask;
    }
}
