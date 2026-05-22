using TEngine;

namespace GameLogic.SheepBattle.Battle
{
    public sealed class BattleController
    {
        public static BattleController Instance { get; } = new BattleController();

        private BattleController()
        {
        }

        public void EnterBattle()
        {
            Log.Info("进入战斗流程。场景资源接入后，这里统一调用 GameModule.Scene 切换战斗场景。");
            GameModule.UI.ShowUIAsync<BattleMainUI>();
        }

        public void LeaveBattle()
        {
            GameModule.UI.CloseUI<BattleMainUI>();
            GameModule.UI.ShowUIAsync<LobbyUI>();
        }
    }
}
