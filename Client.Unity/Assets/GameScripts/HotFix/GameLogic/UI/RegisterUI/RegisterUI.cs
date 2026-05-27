using GameLogic.SheepBattle.Common;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "TestUI")]
    internal sealed class RegisterUI : UIWindow
    {
        private InputField _account;
        private InputField _password;
        private InputField _confirm;

        protected override void OnCreate()
        {
            DynamicUI.EnsureRaycaster(gameObject);
            DynamicUI.Clear(rectTransform);
            var mask = DynamicUI.Image("m_imgMask", rectTransform, new Color(0f, 0f, 0f, 0.56f));
            DynamicUI.Stretch(DynamicUI.Rect(mask));

            var panel = DynamicUI.SpriteImage("m_imgPanel", rectTransform, DynamicUI.ArtPanelPopup, Color.white);
            DynamicUI.Center(DynamicUI.Rect(panel), new Vector2(560f, 430f), Vector2.zero);

            var title = DynamicUI.Text("m_txtTitle", panel.transform, "注册新账号", 32, TextAnchor.MiddleCenter, Color.black, FontStyle.Bold);
            DynamicUI.Center(DynamicUI.Rect(title), new Vector2(420f, 56f), new Vector2(0f, 150f));

            _account = DynamicUI.Input("m_inputAccount", panel.transform, "用户名（3-12位）", 12);
            DynamicUI.Center(DynamicUI.Rect(_account), new Vector2(420f, 56f), new Vector2(0f, 74f));
            _password = DynamicUI.Input("m_inputPassword", panel.transform, "密码（6-20位）", 20, true);
            DynamicUI.Center(DynamicUI.Rect(_password), new Vector2(420f, 56f), new Vector2(0f, 8f));
            _confirm = DynamicUI.Input("m_inputConfirm", panel.transform, "确认密码", 20, true);
            DynamicUI.Center(DynamicUI.Rect(_confirm), new Vector2(420f, 56f), new Vector2(0f, -58f));

            var register = DynamicUI.SkinnedButton("m_btnRegister", panel.transform, "注册", DynamicUI.ArtButtonPrimary);
            DynamicUI.Center(DynamicUI.Rect(register), new Vector2(160f, 54f), new Vector2(-96f, -146f));
            register.onClick.AddListener(OnClickRegister);

            var back = DynamicUI.SkinnedButton("m_btnBack", panel.transform, "返回", DynamicUI.ArtButtonDanger);
            DynamicUI.Center(DynamicUI.Rect(back), new Vector2(160f, 54f), new Vector2(96f, -146f));
            back.onClick.AddListener(() => GameModule.UI.CloseUI<RegisterUI>());
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
