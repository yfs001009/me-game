using GameLogic.SheepBattle.Event;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI)]
    internal sealed class LoginUI : UIWindow
    {
        private InputField _inputAccount;
        private InputField _inputPassword;
        private Button _btnLogin;
        private Button _btnRegister;
        private Button _btnGuest;

        protected override void ScriptGenerator()
        {
            _inputAccount = FindChildComponent<InputField>("m_inputAccount");
            _inputPassword = FindChildComponent<InputField>("m_inputPassword");
            _btnLogin = FindChildComponent<Button>("m_btnLogin");
            _btnRegister = FindChildComponent<Button>("m_btnRegister");
            _btnGuest = FindChildComponent<Button>("m_btnGuest");
        }

        protected override void RegisterEvent()
        {
            _btnLogin?.onClick.AddListener(OnClickLogin);
            _btnRegister?.onClick.AddListener(() => GameModule.UI.ShowUIAsync<RegisterUI>());
            _btnGuest?.onClick.AddListener(OnClickGuestLogin);
            AddUIEvent<LoginStatusChangedEvent>(OnLoginStatusChanged);
        }

        protected override void OnCreate()
        {
            ApplyCompactLayout();
            ApplyArtSkin();
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

        private void ApplyCompactLayout()
        {
            SetCenterItem(_inputAccount?.transform as RectTransform, 86f, 340f, 56f);
            SetCenterItem(_inputPassword?.transform as RectTransform, 18f, 340f, 56f);
            SetCenterItem(_btnLogin?.transform as RectTransform, -58f, 340f, 58f);
            SetCenterItem(_btnRegister?.transform as RectTransform, -132f, 160f, 52f, -92f);
            SetCenterItem(_btnGuest?.transform as RectTransform, -132f, 160f, 52f, 92f);
        }

        private void ApplyArtSkin()
        {
            var rootImage = gameObject.GetComponent<Image>();
            if (rootImage == null)
            {
                rootImage = gameObject.AddComponent<Image>();
            }

            DynamicUI.ApplySprite(rootImage, DynamicUI.ArtLoginBackground, Image.Type.Simple);
            SkinInput(_inputAccount);
            SkinInput(_inputPassword);
            SkinButton(_btnLogin, DynamicUI.ArtButtonPrimary);
            SkinButton(_btnRegister, DynamicUI.ArtButtonSecondary);
            SkinButton(_btnGuest, DynamicUI.ArtButtonSecondary);
        }

        private static void SkinInput(InputField input)
        {
            if (input?.targetGraphic is Image image)
            {
                DynamicUI.ApplySprite(image, DynamicUI.ArtInputFrame);
                image.color = Color.white;
            }
        }

        private static void SkinButton(Button button, string spriteLocation)
        {
            if (button?.targetGraphic is Image image)
            {
                DynamicUI.ApplySprite(image, spriteLocation);
                image.color = Color.white;
            }
        }

        private static void SetCenterItem(RectTransform rect, float y, float width, float height, float x = 0f)
        {
            if (rect == null)
            {
                return;
            }

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = new Vector2(x, y);
        }
    }
}
