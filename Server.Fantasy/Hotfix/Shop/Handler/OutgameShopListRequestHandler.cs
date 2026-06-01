using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Shop.Handler;

public sealed class OutgameShopListRequestHandler : MessageRPC<C2G_OutgameShopListRequest, G2C_OutgameShopListResponse>
{
    protected override async FTask Run(Session session, C2G_OutgameShopListRequest request, G2C_OutgameShopListResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.ErrorCode = 401;
            response.Success = false;
            response.Message = message;
            return;
        }

        SheepServices.Assets.BindScene(session.Scene);
        var result = SheepServices.Shops.GetList(session.Scene, profile.PlayerId, request.ShopType, request.ActivityId, request.FeatureId);
        response.Success = result.Success;
        response.Message = result.Message;
        response.Shops.AddRange(result.Shops);
        await FTask.CompletedTask;
    }
}
