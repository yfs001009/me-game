using Cysharp.Threading.Tasks;
using GameLogic.SheepBattle.Config;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI, location: "TestUI")]
    internal sealed class SplashUI : UIWindow
    {
        private bool _closed;

        protected override void OnCreate()
        {
            DynamicUI.EnsureRaycaster(gameObject);
            DynamicUI.Clear(rectTransform);
            var bg = DynamicUI.Image("m_imgBg", rectTransform, new Color(0.08f, 0.12f, 0.13f, 1f));
            DynamicUI.Stretch(DynamicUI.Rect(bg));

            var logo = DynamicUI.Text("m_txtLogo", rectTransform, "SheepBattle", 56, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            DynamicUI.Center(DynamicUI.Rect(logo), new Vector2(560f, 90f), new Vector2(0f, 48f));

            var version = DynamicUI.Text("m_txtVersion", rectTransform, "v0.1.0", 22, TextAnchor.MiddleCenter, new Color(0.74f, 0.82f, 0.78f, 1f));
            DynamicUI.Center(DynamicUI.Rect(version), new Vector2(260f, 40f), new Vector2(0f, -48f));

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
