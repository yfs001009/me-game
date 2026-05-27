using GameLogic.SheepBattle.Battle;
using GameLogic.SheepBattle.Login;
using GameLogic.SheepBattle.Lobby;
using GameLogic.SheepBattle.Network;
using TEngine;

namespace GameLogic.SheepBattle.App
{
    /// <summary>
    /// SheepBattle 热更业务入口。
    /// 负责串起 TEngine UI/资源/流程模块，不替代 TEngine 框架生命周期。
    /// </summary>
    public static class SheepBattleApp
    {
        public static void Start()
        {
            Log.Info("SheepBattle hotfix app started.");
            Fantasy.GameProtoFantasyRegistrar.Register();
            GameEvent.EventMgr.RegWrapInterface<ILoginCommand>(LoginController.Instance);
            GameEvent.EventMgr.RegWrapInterface<ILobbyCommand>(LobbyController.Instance);
            GameEvent.EventMgr.RegWrapInterface<IBattleCommand>(BattleController.Instance);
            SheepNetworkService.Instance.Initialize("127.0.0.1", 20000);
            GameModule.UI.ShowUIAsync<SplashUI>();
        }

        public static void Release()
        {
            ReleaseAsync().Coroutine();
        }

        private static async Fantasy.Async.FTask ReleaseAsync()
        {
            await LobbyController.Instance.TryLeaveCurrentRoomOnShutdownAsync();
            SheepNetworkService.Instance.Dispose();
            Log.Info("SheepBattle hotfix app released.");
        }

    }
}
