using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Battle;
using Hotfix.Shared;

namespace Hotfix.Battle.Handler;

public sealed class BuyBattleShopGoodsCommandHandler : MessageRPC<C2G_BuyBattleShopGoodsCommand, G2C_BuyBattleShopGoodsCommandResponse>
{
    protected override async FTask Run(Session session, C2G_BuyBattleShopGoodsCommand request, G2C_BuyBattleShopGoodsCommandResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.ErrorCode = 401;
            response.Success = false;
            response.Message = message;
            return;
        }

        var result = await BattleSceneGateway.BuyShopGoods(session.Scene, profile, request);
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Snapshot != null)
        {
            response.Snapshot = result.Snapshot;
        }
        if (response.Success)
        {
            SheepServices.Tasks.AddProgress(profile.PlayerId, "Battle.Troll.BuyEquipment.Count", 1);
        }
    }
}
