using GameLogic.SheepBattle.Config;
using GameLogic.SheepBattle.Lobby;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "CreateRoomUI")]
    internal sealed class CreateRoomUI : UIWindow
    {
        private InputField _inputRoomName;
        private InputField _inputMaxPlayers;
        private Toggle _togglePrivate;
        private InputField _inputPassword;
        private Text _txtHint;
        private Button _btnCreate;
        private Button _btnClose;

        protected override void ScriptGenerator()
        {
            _inputRoomName = FindChildComponent<InputField>("m_imgPanel/m_inputRoomName");
            _inputMaxPlayers = FindChildComponent<InputField>("m_imgPanel/m_inputMaxPlayers");
            _togglePrivate = FindChildComponent<Toggle>("m_imgPanel/m_togglePrivate");
            _inputPassword = FindChildComponent<InputField>("m_imgPanel/m_inputPassword");
            _txtHint = FindChildComponent<Text>("m_imgPanel/m_txtHint");
            _btnCreate = FindChildComponent<Button>("m_imgPanel/m_btnCreate");
            _btnClose = FindChildComponent<Button>("m_imgPanel/m_btnClose");
        }

        protected override void RegisterEvent()
        {
            _btnCreate?.onClick.AddListener(OnClickCreate);
            _btnClose?.onClick.AddListener(() => GameModule.UI.CloseUI<CreateRoomUI>());
            _togglePrivate?.onValueChanged.AddListener(OnPrivateChanged);
        }

        protected override void OnCreate()
        {
            var rules = GameRuleService.Instance;
            if (_inputRoomName != null)
            {
                _inputRoomName.text = "默认房间";
            }

            if (_inputMaxPlayers != null)
            {
                _inputMaxPlayers.text = rules.CustomRoomDefaultPlayers.ToString();
            }

            if (_togglePrivate != null)
            {
                _togglePrivate.isOn = false;
            }

            OnPrivateChanged(_togglePrivate != null && _togglePrivate.isOn);

            RefreshHint();
            ApplyArtSkin();
        }

        private void OnClickCreate()
        {
            var rules = GameRuleService.Instance;
            var roomName = string.IsNullOrWhiteSpace(_inputRoomName?.text) ? "默认房间" : _inputRoomName.text.Trim();
            var maxPlayers = rules.CustomRoomDefaultPlayers;
            if (!string.IsNullOrWhiteSpace(_inputMaxPlayers?.text) && int.TryParse(_inputMaxPlayers.text, out var parsed))
            {
                maxPlayers = Mathf.Clamp(parsed, rules.CustomRoomMinPlayers, rules.CustomRoomMaxPlayers);
            }

            var isPrivate = _togglePrivate != null && _togglePrivate.isOn;
            var password = isPrivate ? _inputPassword?.text ?? string.Empty : string.Empty;
            GameEvent.Get<ILobbyCommand>()?.OnCreateRoom(roomName, rules.DefaultMapId, maxPlayers, isPrivate, password);
        }

        private void RefreshHint()
        {
            if (_txtHint == null)
            {
                return;
            }

            var rules = GameRuleService.Instance;
            _txtHint.text = $"人数范围：{rules.CustomRoomMinPlayers}-{rules.CustomRoomMaxPlayers}";
        }

        private void OnPrivateChanged(bool isPrivate)
        {
            if (_inputPassword == null)
            {
                return;
            }

            _inputPassword.interactable = isPrivate;
            if (!isPrivate)
            {
                _inputPassword.text = string.Empty;
            }
        }

        private void ApplyArtSkin()
        {
            var panel = FindChildComponent<Image>("m_imgPanel");
            DynamicUI.ApplySprite(panel, DynamicUI.ArtPanelPopup);
            SkinInput(_inputRoomName);
            SkinInput(_inputMaxPlayers);
            SkinInput(_inputPassword);
            SkinButton(_btnCreate, DynamicUI.ArtButtonPrimary);
            SkinButton(_btnClose, DynamicUI.ArtButtonDanger);
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
    }
}
