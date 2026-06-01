using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Battle.Handler;

/// <summary>
/// Room Scene 创建战斗的 Address Handler。这里只做消息适配，战斗状态写入 BattleService/BattleEntity。
/// </summary>
public sealed class CreateBattleAddressHandler : AddressRPC<Scene, Room2Battle_CreateBattleRequest, Battle2Room_CreateBattleResponse>
{
    protected override async FTask Run(Scene scene, Room2Battle_CreateBattleRequest request, Battle2Room_CreateBattleResponse response, Action reply)
    {
        response.Battle = SheepServices.Battles.CreateFromRoom(scene, request.Room, request.Battle);
        await FTask.CompletedTask;
    }
}

/// <summary>
/// Battle Scene 内处理玩家加载完成事件，并返回当前权威快照。
/// </summary>
public sealed class BattleSceneLoadedAddressHandler : AddressRPC<Scene, G2Battle_SceneLoadedRequest, Battle2G_SceneLoadedResponse>
{
    protected override async FTask Run(Scene scene, G2Battle_SceneLoadedRequest request, Battle2G_SceneLoadedResponse response, Action reply)
    {
        var result = SheepServices.Battles.MarkSceneLoaded(request.Profile, request.BattleId);
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Snapshot != null)
        {
            response.Snapshot = result.Snapshot;
        }
        await FTask.CompletedTask;
    }
}

/// <summary>
/// Battle Scene 内处理快照查询。当前是轮询式同步，后续可替换为 Battle Scene 主动广播。
/// </summary>
public sealed class BattleSnapshotAddressHandler : AddressRPC<Scene, G2Battle_SnapshotRequest, Battle2G_SnapshotResponse>
{
    protected override async FTask Run(Scene scene, G2Battle_SnapshotRequest request, Battle2G_SnapshotResponse response, Action reply)
    {
        var result = SheepServices.Battles.GetSnapshot(request.Profile, request.BattleId);
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Snapshot != null)
        {
            response.Snapshot = result.Snapshot;
        }
        await FTask.CompletedTask;
    }
}

/// <summary>
/// Battle Scene 内处理移动输入，确保坐标与碰撞由服务端权威计算。
/// </summary>
public sealed class BattleMoveAddressHandler : AddressRPC<Scene, G2Battle_MoveRequest, Battle2G_MoveResponse>
{
    protected override async FTask Run(Scene scene, G2Battle_MoveRequest request, Battle2G_MoveResponse response, Action reply)
    {
        var result = SheepServices.Battles.Move(request.Profile, new C2G_BattleMoveCommand
        {
            BattleId = request.BattleId,
            AxisX = request.AxisX,
            AxisY = request.AxisY
        });
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Snapshot != null)
        {
            response.Snapshot = result.Snapshot;
        }
        await FTask.CompletedTask;
    }
}

/// <summary>
/// Battle Scene 内处理建造命令，服务端负责资源、位置和阵营校验。
/// </summary>
public sealed class BattleBuildAddressHandler : AddressRPC<Scene, G2Battle_BuildRequest, Battle2G_BuildResponse>
{
    protected override async FTask Run(Scene scene, G2Battle_BuildRequest request, Battle2G_BuildResponse response, Action reply)
    {
        var result = SheepServices.Battles.Build(request.Profile, new C2G_BuildCommand
        {
            BattleId = request.BattleId,
            BuildingId = request.BuildingId,
            GridX = request.GridX,
            GridY = request.GridY
        });
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Snapshot != null)
        {
            response.Snapshot = result.Snapshot;
        }
        await FTask.CompletedTask;
    }
}

/// <summary>
/// Battle Scene 内处理建筑升级命令。
/// </summary>
public sealed class BattleUpgradeAddressHandler : AddressRPC<Scene, G2Battle_UpgradeBuildingRequest, Battle2G_UpgradeBuildingResponse>
{
    protected override async FTask Run(Scene scene, G2Battle_UpgradeBuildingRequest request, Battle2G_UpgradeBuildingResponse response, Action reply)
    {
        var result = SheepServices.Battles.Upgrade(request.Profile, new C2G_UpgradeBuildingCommand
        {
            BattleId = request.BattleId,
            BuildingInstanceId = request.BuildingInstanceId
        });
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Snapshot != null)
        {
            response.Snapshot = result.Snapshot;
        }
        await FTask.CompletedTask;
    }
}

/// <summary>
/// Battle Scene 内处理建筑回收命令。
/// </summary>
public sealed class BattleRecycleAddressHandler : AddressRPC<Scene, G2Battle_RecycleBuildingRequest, Battle2G_RecycleBuildingResponse>
{
    protected override async FTask Run(Scene scene, G2Battle_RecycleBuildingRequest request, Battle2G_RecycleBuildingResponse response, Action reply)
    {
        var result = SheepServices.Battles.Recycle(request.Profile, new C2G_RecycleBuildingCommand
        {
            BattleId = request.BattleId,
            BuildingInstanceId = request.BuildingInstanceId
        });
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Snapshot != null)
        {
            response.Snapshot = result.Snapshot;
        }
        await FTask.CompletedTask;
    }
}

/// <summary>
/// Battle Scene 内处理巨魔局内商店购买，商店距离和装备栏容量由服务端权威校验。
/// </summary>
public sealed class BattleBuyShopGoodsAddressHandler : AddressRPC<Scene, G2Battle_BuyShopGoodsRequest, Battle2G_BuyShopGoodsResponse>
{
    protected override async FTask Run(Scene scene, G2Battle_BuyShopGoodsRequest request, Battle2G_BuyShopGoodsResponse response, Action reply)
    {
        var result = SheepServices.Battles.BuyBattleShopGoods(request.Profile, new C2G_BuyBattleShopGoodsCommand
        {
            BattleId = request.BattleId,
            ShopId = request.ShopId,
            GoodsId = request.GoodsId
        });
        response.Success = result.Success;
        response.Message = result.Message;
        if (result.Snapshot != null)
        {
            response.Snapshot = result.Snapshot;
        }
        await FTask.CompletedTask;
    }
}
