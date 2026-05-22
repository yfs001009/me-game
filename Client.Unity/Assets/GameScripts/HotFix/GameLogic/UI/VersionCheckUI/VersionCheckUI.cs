using Cysharp.Threading.Tasks;
using GameLogic.SheepBattle.Config;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI, location: "TestUI")]
    internal sealed class VersionCheckUI : UIWindow
    {
        private Text _status;
        private Button _btnUpdate;
        private bool _closed;

        protected override void OnCreate()
        {
            DynamicUI.EnsureRaycaster(gameObject);
            DynamicUI.Clear(rectTransform);

            var bg = DynamicUI.Image("m_imgBg", rectTransform, new Color(0.09f, 0.12f, 0.13f, 1f));
            DynamicUI.Stretch(DynamicUI.Rect(bg));

            var title = DynamicUI.Text("m_txtTitle", rectTransform, "版本检查", 36, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            DynamicUI.Center(DynamicUI.Rect(title), new Vector2(360f, 64f), new Vector2(0f, 84f));

            _status = DynamicUI.Text("m_txtStatus", rectTransform, "正在检查资源版本...", 24, TextAnchor.MiddleCenter, new Color(0.78f, 0.86f, 0.80f, 1f));
            DynamicUI.Center(DynamicUI.Rect(_status), new Vector2(620f, 56f), new Vector2(0f, 20f));

            var version = DynamicUI.Text("m_txtVersion", rectTransform, "当前版本：v0.1.0", 20, TextAnchor.MiddleCenter, new Color(0.60f, 0.68f, 0.64f, 1f));
            DynamicUI.Center(DynamicUI.Rect(version), new Vector2(360f, 40f), new Vector2(0f, -34f));

            _btnUpdate = DynamicUI.Button("m_btnUpdate", rectTransform, "前往更新", new Color(0.20f, 0.48f, 0.36f, 1f));
            DynamicUI.Center(DynamicUI.Rect(_btnUpdate), new Vector2(260f, 56f), new Vector2(0f, -108f));
            _btnUpdate.gameObject.SetActive(false);

            CheckAsync().Forget();
        }

        private async UniTaskVoid CheckAsync()
        {
            if (!GameRuleService.Instance.VersionCheckEnabled)
            {
                GoLoading();
                return;
            }

            await UniTask.Delay(600);
            if (_closed)
            {
                return;
            }

            var needUpdate = false;
            if (needUpdate)
            {
                _status.text = "检测到新版本，请更新后继续。";
                _btnUpdate.gameObject.SetActive(true);
                return;
            }

            _status.text = "版本一致，准备加载资源...";
            await UniTask.Delay(500);
            if (!_closed)
            {
                GoLoading();
            }
        }

        private static void GoLoading()
        {
            GameModule.UI.CloseUI<VersionCheckUI>();
            GameModule.UI.ShowUIAsync<LoadingUI>();
        }

        protected override void OnDestroy()
        {
            _closed = true;
        }
    }
}
