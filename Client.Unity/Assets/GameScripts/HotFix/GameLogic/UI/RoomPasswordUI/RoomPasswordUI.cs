using GameLogic.SheepBattle.Common;
using GameLogic.SheepBattle.Lobby;
using TEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "RoomPasswordUI")]
    internal sealed class RoomPasswordUI : UIWindow
    {
        private InputField _inputPassword;
        private Button _btnJoin;
        private Button _btnClose;
        private RoomSummaryViewModel _roomSummary;

        protected override void ScriptGenerator()
        {
            _inputPassword = FindChildComponent<InputField>("m_imgPanel/m_inputPassword");
            _btnJoin = FindChildComponent<Button>("m_imgPanel/m_btnJoin");
            _btnClose = FindChildComponent<Button>("m_imgPanel/m_btnClose");
        }

        protected override void OnCreate()
        {
            _btnJoin.onClick.AddListener(OnClickJoin);
            _btnClose.onClick.AddListener(() => GameModule.UI.CloseUI<RoomPasswordUI>());
        }

        protected override void OnRefresh()
        {
            _roomSummary = UserData as RoomSummaryViewModel;
            _inputPassword.text = string.Empty;
            _inputPassword.ActivateInputField();
        }

        private void OnClickJoin()
        {
            if (_roomSummary == null)
            {
                CommonNoticeService.Show("房间信息无效，请刷新后重试。");
                return;
            }

            GameEvent.Get<ILobbyCommand>()?.OnJoinRoom(_roomSummary.RoomId, _inputPassword?.text ?? string.Empty);
        }
    }
}
