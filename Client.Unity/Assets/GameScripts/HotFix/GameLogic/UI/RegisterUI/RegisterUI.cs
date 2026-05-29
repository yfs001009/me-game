using GameLogic.SheepBattle.Common;
using TEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "RegisterUI")]
    internal sealed class RegisterUI : UIWindow
    {
        private InputField _account;
        private InputField _password;
        private InputField _confirm;
        private Button _btnRegister;
        private Button _btnBack;

        protected override void ScriptGenerator()
        {
            _account = FindChildComponent<InputField>("m_imgPanel/m_inputAccount");
            _password = FindChildComponent<InputField>("m_imgPanel/m_inputPassword");
            _confirm = FindChildComponent<InputField>("m_imgPanel/m_inputConfirm");
            _btnRegister = FindChildComponent<Button>("m_imgPanel/m_btnRegister");
            _btnBack = FindChildComponent<Button>("m_imgPanel/m_btnBack");
        }

        protected override void OnCreate()
        {
            _btnRegister.onClick.AddListener(OnClickRegister);
            _btnBack.onClick.AddListener(() => GameModule.UI.CloseUI<RegisterUI>());
        }

        protected override void OnRefresh()
        {
            _account.text = string.Empty;
            _password.text = string.Empty;
            _confirm.text = string.Empty;
            _account.ActivateInputField();
        }

        private void OnClickRegister()
        {
            var account = _account.text.Trim();
            var password = _password.text;
            if (account.Length < 3 || account.Length > 12)
            {
                CommonNoticeService.Show("用户名需要 3-12 位。");
                return;
            }

            if (password.Length < 6 || password.Length > 20)
            {
                CommonNoticeService.Show("密码需要 6-20 位。");
                return;
            }

            if (password != _confirm.text)
            {
                CommonNoticeService.Show("两次输入的密码不一致。");
                return;
            }

            GameEvent.Get<ILoginCommand>()?.OnRegister(account, password, string.Empty);
        }
    }
}
