using Cysharp.Threading.Tasks;
using GameLogic.SheepBattle.Config;
using TEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI, location: "VersionCheckUI")]
    internal sealed class VersionCheckUI : UIWindow
    {
        private Text _status;
        private Button _btnUpdate;
        private bool _closed;

        protected override void ScriptGenerator()
        {
            _status = FindChildComponent<Text>("m_txtStatus");
            _btnUpdate = FindChildComponent<Button>("m_btnUpdate");
        }

        protected override void OnCreate()
        {
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
