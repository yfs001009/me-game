using Cysharp.Threading.Tasks;
using GameLogic.SheepBattle.Config;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI, location: "TestUI")]
    internal sealed class LoadingUI : UIWindow
    {
        private Image _bar;
        private Text _progress;
        private bool _closed;

        protected override void OnCreate()
        {
            DynamicUI.EnsureRaycaster(gameObject);
            DynamicUI.Clear(rectTransform);
            var bg = DynamicUI.Image("m_imgBg", rectTransform, new Color(0.10f, 0.13f, 0.12f, 1f));
            DynamicUI.Stretch(DynamicUI.Rect(bg));

            var title = DynamicUI.Text("m_txtTitle", rectTransform, "加载中", 34, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            DynamicUI.Center(DynamicUI.Rect(title), new Vector2(360f, 60f), new Vector2(0f, 72f));

            var tip = DynamicUI.Text("m_txtTip", rectTransform, GameRuleService.Instance.GetLoadingTip(), 22, TextAnchor.MiddleCenter, new Color(0.77f, 0.84f, 0.78f, 1f));
            DynamicUI.Center(DynamicUI.Rect(tip), new Vector2(620f, 48f), new Vector2(0f, 14f));

            var track = DynamicUI.Image("m_imgTrack", rectTransform, new Color(0.24f, 0.28f, 0.27f, 1f));
            DynamicUI.Center(DynamicUI.Rect(track), new Vector2(520f, 22f), new Vector2(0f, -48f));
            _bar = DynamicUI.Image("m_imgBar", track.transform, new Color(0.18f, 0.62f, 0.42f, 1f));
            var barRect = DynamicUI.Rect(_bar);
            barRect.anchorMin = new Vector2(0f, 0f);
            barRect.anchorMax = new Vector2(0f, 1f);
            barRect.pivot = new Vector2(0f, 0.5f);
            barRect.sizeDelta = new Vector2(0f, 0f);
            barRect.anchoredPosition = Vector2.zero;

            _progress = DynamicUI.Text("m_txtProgress", rectTransform, "0%", 20, TextAnchor.MiddleCenter, Color.white);
            DynamicUI.Center(DynamicUI.Rect(_progress), new Vector2(160f, 32f), new Vector2(0f, -84f));

            LoadAsync().Forget();
        }

        private async UniTaskVoid LoadAsync()
        {
            var minLoadMs = Mathf.Max(300, GameRuleService.Instance.LoadingMinSeconds * 1000);
            const int steps = 20;
            var stepDelayMs = Mathf.Max(1, minLoadMs / steps);
            for (var i = 0; i <= 100; i += 5)
            {
                if (_closed)
                {
                    return;
                }

                var rect = DynamicUI.Rect(_bar);
                rect.sizeDelta = new Vector2(520f * i / 100f, 0f);
                _progress.text = $"{i}%";
                await UniTask.Delay(stepDelayMs);
            }

            GameModule.UI.CloseUI<LoadingUI>();
            GameModule.UI.ShowUIAsync<LoginUI>();
        }

        protected override void OnDestroy()
        {
            _closed = true;
        }
    }
}
