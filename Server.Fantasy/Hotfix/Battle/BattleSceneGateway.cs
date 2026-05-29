using Fantasy;
using Fantasy.Async;
using Fantasy.Platform.Net;

namespace Hotfix.Battle;

/// <summary>
/// Gate/Room Scene 到 Battle Scene 的 Address 调用门面。
/// 外层 Handler 不直接访问 BattleService，保证战斗状态始终归属 Battle Scene，后续做分服或按 BattleId 分片时只需要替换寻址策略。
/// </summary>
internal static class BattleSceneGateway
{
    public static async FTask<Battle2Room_CreateBattleResponse> Create(Scene sourceScene, RoomDetailInfo room, BattleStartInfo battle)
    {
        return (Battle2Room_CreateBattleResponse)await sourceScene.Call(BattleSceneAddress(), new Room2Battle_CreateBattleRequest
        {
            Room = room,
            Battle = battle
        });
    }

    public static async FTask<Battle2G_SceneLoadedResponse> SceneLoaded(Scene gateScene, PlayerProfileInfo profile, C2G_BattleSceneLoadedRequest request)
    {
        return (Battle2G_SceneLoadedResponse)await gateScene.Call(BattleSceneAddress(), new G2Battle_SceneLoadedRequest
        {
            Profile = profile,
            BattleId = request.BattleId
        });
    }

    public static async FTask<Battle2G_SnapshotResponse> Snapshot(Scene gateScene, PlayerProfileInfo profile, C2G_BattleSnapshotRequest request)
    {
        return (Battle2G_SnapshotResponse)await gateScene.Call(BattleSceneAddress(), new G2Battle_SnapshotRequest
        {
            Profile = profile,
            BattleId = request.BattleId,
            LastKnownTick = request.LastKnownTick
        });
    }

    public static async FTask<Battle2G_MoveResponse> Move(Scene gateScene, PlayerProfileInfo profile, C2G_BattleMoveCommand request)
    {
        return (Battle2G_MoveResponse)await gateScene.Call(BattleSceneAddress(), new G2Battle_MoveRequest
        {
            Profile = profile,
            BattleId = request.BattleId,
            AxisX = request.AxisX,
            AxisY = request.AxisY
        });
    }

    public static async FTask<Battle2G_BuildResponse> Build(Scene gateScene, PlayerProfileInfo profile, C2G_BuildCommand request)
    {
        return (Battle2G_BuildResponse)await gateScene.Call(BattleSceneAddress(), new G2Battle_BuildRequest
        {
            Profile = profile,
            BattleId = request.BattleId,
            BuildingId = request.BuildingId,
            GridX = request.GridX,
            GridY = request.GridY
        });
    }

    public static async FTask<Battle2G_UpgradeBuildingResponse> Upgrade(Scene gateScene, PlayerProfileInfo profile, C2G_UpgradeBuildingCommand request)
    {
        return (Battle2G_UpgradeBuildingResponse)await gateScene.Call(BattleSceneAddress(), new G2Battle_UpgradeBuildingRequest
        {
            Profile = profile,
            BattleId = request.BattleId,
            BuildingInstanceId = request.BuildingInstanceId
        });
    }

    public static async FTask<Battle2G_RecycleBuildingResponse> Recycle(Scene gateScene, PlayerProfileInfo profile, C2G_RecycleBuildingCommand request)
    {
        return (Battle2G_RecycleBuildingResponse)await gateScene.Call(BattleSceneAddress(), new G2Battle_RecycleBuildingRequest
        {
            Profile = profile,
            BattleId = request.BattleId,
            BuildingInstanceId = request.BuildingInstanceId
        });
    }

    private static long BattleSceneAddress()
    {
        var battleScenes = SceneConfigData.Instance.GetSceneBySceneType(SceneType.Battle);
        if (battleScenes.Count <= 0)
        {
            Log.Error("Battle Scene is not configured.");
            return 0;
        }

        // MVP 当前只有一个 Battle Scene；多战斗分片时应按 BattleId 或房间分配结果选择目标地址。
        return battleScenes[0].Address;
    }
}
