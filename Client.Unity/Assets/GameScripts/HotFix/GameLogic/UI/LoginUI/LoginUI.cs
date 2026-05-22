using GameLogic.SheepBattle.Common;
using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Login;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    /// <summary>
    /// 登录窗口。遵守 TEngine UIWindow 生命周期，只负责 UI 事件转发。
    /// </summary>
    [Window(UILayer.UI)]
    internal sealed class LoginUI : UIWindow
    {
        private InputField _inputAccount;
        private InputField _inputPassword;
        private Button _btnLogin;
        private Button _btnRegister;

        protected override void ScriptGenerator()
        {
            _inputAccount = FindChildComponent<InputField>("m_inputAccount");
            _inputPassword = FindChildComponent<InputField>("m_inputPassword");
            _btnLogin = FindChildComponent<Button>("m_btnLogin");
        }

        protected override void RegisterEvent()
        {
            _btnLogin?.onClick.AddListener(OnClickLogin);
            AddUIEvent<LoginStatusChangedEvent>(OnLoginStatusChanged);
        }

        protected override void OnCreate()
        {
            EnsureExtraButtons();
        }

        public void OnClickLogin(string account, string password)
        {
            LoginController.Instance.Login(account, password);
        }

        private void OnClickLogin()
        {
            OnClickLogin(_inputAccount?.text ?? string.Empty, _inputPassword?.text ?? string.Empty);
        }

        private void OnLoginStatusChanged(LoginStatusChangedEvent eventData)
        {
            Log.Info($"登录界面状态：{eventData.Status}，忙碌={eventData.IsBusy}");
        }

        private void EnsureExtraButtons()
        {
            if (_btnRegister != null)
            {
                return;
            }

            var font = _inputAccount?.textComponent?.font ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var loginRect = _btnLogin != null ? _btnLogin.transform as RectTransform : null;
            _btnRegister = CreateButton("m_btnRegister", "注册账号", font, loginRect, new Vector2(0f, -76f), new Color(0.24f, 0.46f, 0.38f, 1f));
            _btnRegister.onClick.AddListener(() => GameModule.UI.ShowUIAsync<RegisterUI>());

            var guest = CreateButton("m_btnGuest", "游客登录", font, loginRect, new Vector2(0f, -152f), new Color(0.35f, 0.37f, 0.36f, 1f));
            guest.onClick.AddListener(() => CommonNoticeService.Show("游客登录稍后接入。"));
        }

        private Button CreateButton(string name, string text, Font font, RectTransform reference, Vector2 offset, Color color)
        {
            var buttonGo = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            buttonGo.transform.SetParent(rectTransform, false);
            var rect = buttonGo.GetComponent<RectTransform>();
            rect.anchorMin = reference?.anchorMin ?? new Vector2(0.5f, 0.5f);
            rect.anchorMax = reference?.anchorMax ?? new Vector2(0.5f, 0.5f);
            rect.pivot = reference?.pivot ?? new Vector2(0.5f, 0.5f);
            rect.sizeDelta = reference?.sizeDelta ?? new Vector2(320f, 56f);
            rect.anchoredPosition = (reference?.anchoredPosition ?? new Vector2(0f, -75f)) + offset;

            var image = buttonGo.GetComponent<Image>();
            image.color = color;
            var button = buttonGo.GetComponent<Button>();
            button.targetGraphic = image;

            var labelGo = new GameObject("m_txtLabel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            labelGo.transform.SetParent(buttonGo.transform, false);
            var labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            var label = labelGo.GetComponent<Text>();
            label.font = font;
            label.fontSize = 24;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.raycastTarget = false;
            label.text = text;
            return button;
        }
    }
}
