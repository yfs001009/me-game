using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Shop.Handler;

public sealed class BuyOutgameShopGoodsRequestHandler : MessageRPC<C2G_BuyOutgameShopGoodsRequest, G2C_BuyOutgameShopGoodsResponse>
{
    protected override async FTask Run(Session session, C2G_BuyOutgameShopGoodsRequest request, G2C_BuyOutgameShopGoodsResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.ErrorCode = 401;
            response.Success = false;
            response.Message = message;
            return;
        }

        SheepServices.Assets.BindScene(session.Scene);
        var result = SheepServices.Shops.Buy(session.Scene, profile.PlayerId, request.GoodsId, request.Count);
        response.Success = result.Success;
        response.Message = result.Message;
        response.Goods = result.Goods;
        response.Snapshot = result.Snapshot;
        await FTask.CompletedTask;
    }
}
