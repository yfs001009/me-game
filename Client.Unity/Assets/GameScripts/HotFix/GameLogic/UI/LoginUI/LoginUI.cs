using GameLogic.SheepBattle.Event;
using TEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI)]
    internal sealed class LoginUI : UIWindow
    {
        private InputField _inputAccount;
        private InputField _inputPassword;
        private Button _btnLogin;
        private Button _btnGuest;

        protected override void ScriptGenerator()
        {
            _inputAccount = FindChildComponent<InputField>("m_imgPanel/m_inputAccount") ?? FindChildComponent<InputField>("m_inputAccount");
            _inputPassword = FindChildComponent<InputField>("m_imgPanel/m_inputPassword") ?? FindChildComponent<InputField>("m_inputPassword");
            _btnLogin = FindChildComponent<Button>("m_imgPanel/m_btnLogin") ?? FindChildComponent<Button>("m_btnLogin");
            _btnGuest = FindChildComponent<Button>("m_imgPanel/m_btnGuest") ?? FindChildComponent<Button>("m_btnGuest");
        }

        protected override void RegisterEvent()
        {
            _btnLogin?.onClick.AddListener(OnClickLogin);
            _btnGuest?.onClick.AddListener(OnClickGuestLogin);
            AddUIEvent<LoginStatusChangedEvent>(OnLoginStatusChanged);
        }

        protected override void OnCreate()
        {
        }

        public void OnClickLogin(string account, string password)
        {
            GameEvent.Get<ILoginCommand>()?.OnLogin(account, password);
        }

        private void OnClickLogin()
        {
            OnClickLogin(_inputAccount?.text ?? string.Empty, _inputPassword?.text ?? string.Empty);
        }

        private void OnClickGuestLogin()
        {
            var suffix = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() % 100000;
            OnClickLogin($"guest{suffix:D5}", "guest123");
        }

        private void OnLoginStatusChanged(LoginStatusChangedEvent eventData)
        {
            Log.Info($"Login UI status: {eventData.Status}, busy={eventData.IsBusy}");
        }

    }
}
