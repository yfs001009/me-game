using GameLogic.SheepBattle.Common;
using GameLogic.SheepBattle.Lobby;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "TestUI")]
    internal sealed class RoomPasswordUI : UIWindow
    {
        private InputField _inputPassword;
        private RoomSummaryViewModel _roomSummary;

        protected override void OnCreate()
        {
            BuildView();
        }

        protected override void OnRefresh()
        {
            _roomSummary = UserData as RoomSummaryViewModel;
            if (_inputPassword != null)
            {
                _inputPassword.text = string.Empty;
                _inputPassword.ActivateInputField();
            }
        }

        private void BuildView()
        {
            EnsureGraphicRaycaster();
            ClearPrefabChildren();
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var mask = CreateImage("m_imgMask", rectTransform, new Color(0f, 0f, 0f, 0.55f));
            Stretch(mask.transform as RectTransform);

            var panel = DynamicUI.SpriteImage("m_imgPanel", rectTransform, DynamicUI.ArtPanelPopup, Color.white);
            var panelRect = panel.transform as RectTransform;
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(520f, 300f);
            panelRect.anchoredPosition = Vector2.zero;

            var title = CreateText("m_txtTitle", panelRect, font, 28, FontStyle.Bold, TextAnchor.MiddleCenter, Color.black);
            SetRect(title.transform as RectTransform, 0f, 96f, 460f, 50f);
            title.text = "请输入房间密码";

            _inputPassword = CreateInput(panelRect, font, "房间密码", string.Empty);
            SetRect(_inputPassword.transform as RectTransform, 0f, 24f, 380f, 54f);

            var joinButton = CreateButton("m_btnJoin", panelRect, font, "加入", DynamicUI.ArtButtonPrimary);
            SetRect(joinButton.transform as RectTransform, -86f, -84f, 150f, 52f);
            joinButton.onClick.AddListener(OnClickJoin);

            var closeButton = CreateButton("m_btnClose", panelRect, font, "取消", DynamicUI.ArtButtonDanger);
            SetRect(closeButton.transform as RectTransform, 86f, -84f, 150f, 52f);
            closeButton.onClick.AddListener(() => GameModule.UI.CloseUI<RoomPasswordUI>());
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

        private static Image CreateImage(string name, Transform parent, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.color = color;
            return image;
        }

        private static Text CreateText(string name, Transform parent, Font font, int size, FontStyle style, TextAnchor anchor, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<Text>();
            text.font = font;
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = anchor;
            text.color = color;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        private static InputField CreateInput(Transform parent, Font font, string placeholder, string value)
        {
            var image = DynamicUI.SpriteImage("m_inputPassword", parent, DynamicUI.ArtInputFrame, Color.white);
            var input = image.gameObject.AddComponent<InputField>();
            var text = CreateText("m_txtText", image.transform, font, 22, FontStyle.Normal, TextAnchor.MiddleLeft, Color.black);
            StretchInset(text.transform as RectTransform, 16f, 0f);

            var holder = CreateText("m_txtPlaceholder", image.transform, font, 22, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.45f, 0.45f, 0.45f, 1f));
            StretchInset(holder.transform as RectTransform, 16f, 0f);
            holder.text = placeholder;

            input.textComponent = text;
            input.placeholder = holder;
            input.text = value;
            return input;
        }

        private static Button CreateButton(string name, Transform parent, Font font, string label, string spriteLocation)
        {
            var image = DynamicUI.SpriteImage(name, parent, spriteLocation, Color.white);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            var text = CreateText("m_txtLabel", image.transform, font, 22, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
            Stretch(text.transform as RectTransform);
            text.text = label;
            return button;
        }

        private static void SetRect(RectTransform rect, float x, float y, float width, float height)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = new Vector2(x, y);
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void StretchInset(RectTransform rect, float horizontal, float vertical)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(horizontal, vertical);
            rect.offsetMax = new Vector2(-horizontal, -vertical);
        }

        private void ClearPrefabChildren()
        {
            for (var i = rectTransform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(rectTransform.GetChild(i).gameObject);
            }
        }

        private void EnsureGraphicRaycaster()
        {
            if (gameObject.GetComponent<GraphicRaycaster>() == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }
        }
    }
}
