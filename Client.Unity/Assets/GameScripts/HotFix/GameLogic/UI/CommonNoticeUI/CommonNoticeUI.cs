using GameLogic.SheepBattle.Common;
using TEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "CommonNoticeUI")]
    internal sealed class CommonNoticeUI : UIWindow
    {
        private Text _txtTitle;
        private Text _txtMessage;
        private Button _btnConfirm;

        protected override void ScriptGenerator()
        {
            _txtTitle = FindChildComponent<Text>("m_imgPanel/m_txtTitle");
            _txtMessage = FindChildComponent<Text>("m_imgPanel/m_txtMessage");
            _btnConfirm = FindChildComponent<Button>("m_imgPanel/m_btnConfirm");
        }

        protected override void OnCreate()
        {
            _btnConfirm.onClick.AddListener(() => GameModule.UI.CloseUI<CommonNoticeUI>());
        }

        protected override void OnRefresh()
        {
            var data = UserData as CommonNoticeData;
            _txtTitle.text = string.IsNullOrWhiteSpace(data?.Title) ? "提示" : data.Title;
            _txtMessage.text = data?.Message ?? string.Empty;
        }
    }
}
