using Cysharp.Threading.Tasks;
using GameLogic.SheepBattle.Config;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    [Window(UILayer.UI, location: "SplashUI")]
    internal sealed class SplashUI : UIWindow
    {
        private bool _closed;

        protected override void OnCreate()
        {
            GoNextAsync().Forget();
        }

        private async UniTaskVoid GoNextAsync()
        {
            var delayMs = Mathf.Max(0, GameRuleService.Instance.SplashDurationSeconds) * 1000;
            await UniTask.Delay(delayMs);
            if (_closed)
            {
                return;
            }

            GameModule.UI.CloseUI<SplashUI>();
            GameModule.UI.ShowUIAsync<VersionCheckUI>();
        }

        protected override void OnDestroy()
        {
            _closed = true;
        }
    }
}
