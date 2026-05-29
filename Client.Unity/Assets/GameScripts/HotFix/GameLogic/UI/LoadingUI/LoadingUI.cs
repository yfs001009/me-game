using Cysharp.Threading.Tasks;
using GameLogic.SheepBattle.Config;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI, location: "LoadingUI")]
    internal sealed class LoadingUI : UIWindow
    {
        private Image _bar;
        private Text _tip;
        private Text _progress;
        private bool _closed;

        protected override void ScriptGenerator()
        {
            _tip = FindChildComponent<Text>("m_txtTip");
            _bar = FindChildComponent<Image>("m_imgTrack/m_imgBar");
            _progress = FindChildComponent<Text>("m_txtProgress");
        }

        protected override void OnCreate()
        {
            _tip.text = GameRuleService.Instance.GetLoadingTip();
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

                var rect = _bar.rectTransform;
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
