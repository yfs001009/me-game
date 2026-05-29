using GameLogic.SheepBattle.Common;
using TEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "NicknameUI")]
    internal sealed class NicknameUI : UIWindow
    {
        private InputField _inputNickname;
        private Button _btnConfirm;

        protected override void ScriptGenerator()
        {
            _inputNickname = FindChildComponent<InputField>("m_imgPanel/m_inputNickname");
            _btnConfirm = FindChildComponent<Button>("m_imgPanel/m_btnConfirm");
        }

        protected override void OnCreate()
        {
            _btnConfirm.onClick.AddListener(OnClickConfirm);
        }

        protected override void OnRefresh()
        {
            _inputNickname.text = string.Empty;
            _inputNickname.ActivateInputField();
        }

        private void OnClickConfirm()
        {
            var nickname = _inputNickname?.text?.Trim() ?? string.Empty;
            if (nickname.Length < 2 || nickname.Length > 12)
            {
                CommonNoticeService.Show("昵称需要 2-12 个字符。");
                return;
            }

            GameEvent.Get<ILoginCommand>()?.OnSubmitNickname(nickname);
        }
    }
}
