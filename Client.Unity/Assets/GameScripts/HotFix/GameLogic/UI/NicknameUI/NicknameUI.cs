using GameLogic.SheepBattle.Common;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "TestUI")]
    internal sealed class NicknameUI : UIWindow
    {
        private InputField _inputNickname;
        private Button _btnConfirm;

        protected override void OnCreate()
        {
            BuildView();
        }

        private void BuildView()
        {
            EnsureGraphicRaycaster();
            ClearPrefabChildren();
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var mask = CreateImage("m_imgMask", rectTransform, new Color(0f, 0f, 0f, 0.62f));
            Stretch(((mask.transform) as RectTransform));

            var panel = DynamicUI.SpriteImage("m_imgPanel", rectTransform, DynamicUI.ArtPanelPopup, Color.white);
            ((panel.transform) as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
            ((panel.transform) as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
            ((panel.transform) as RectTransform).sizeDelta = new Vector2(560f, 330f);
            ((panel.transform) as RectTransform).anchoredPosition = Vector2.zero;

            var title = CreateText("m_txtTitle", ((panel.transform) as RectTransform), font, 32, FontStyle.Bold, TextAnchor.MiddleCenter, Color.black);
            ((title.transform) as RectTransform).anchorMin = new Vector2(0f, 1f);
            ((title.transform) as RectTransform).anchorMax = new Vector2(1f, 1f);
            ((title.transform) as RectTransform).pivot = new Vector2(0.5f, 1f);
            ((title.transform) as RectTransform).sizeDelta = new Vector2(-48f, 64f);
            ((title.transform) as RectTransform).anchoredPosition = new Vector2(0f, -28f);
            title.text = "取一个名字";

            var tip = CreateText("m_txtTip", ((panel.transform) as RectTransform), font, 22, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.2f, 0.2f, 0.2f, 1f));
            ((tip.transform) as RectTransform).anchorMin = new Vector2(0f, 1f);
            ((tip.transform) as RectTransform).anchorMax = new Vector2(1f, 1f);
            ((tip.transform) as RectTransform).pivot = new Vector2(0.5f, 1f);
            ((tip.transform) as RectTransform).sizeDelta = new Vector2(-56f, 42f);
            ((tip.transform) as RectTransform).anchoredPosition = new Vector2(0f, -92f);
            tip.text = "首次进入大厅前需要设置昵称";

            _inputNickname = CreateInput(((panel.transform) as RectTransform), font);
            ((_inputNickname.transform) as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
            ((_inputNickname.transform) as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
            ((_inputNickname.transform) as RectTransform).sizeDelta = new Vector2(420f, 58f);
            ((_inputNickname.transform) as RectTransform).anchoredPosition = new Vector2(0f, 12f);

            _btnConfirm = CreateButton("m_btnConfirm", ((panel.transform) as RectTransform), font, "进入大厅", DynamicUI.ArtButtonPrimary);
            ((_btnConfirm.transform) as RectTransform).anchorMin = new Vector2(0.5f, 0f);
            ((_btnConfirm.transform) as RectTransform).anchorMax = new Vector2(0.5f, 0f);
            ((_btnConfirm.transform) as RectTransform).sizeDelta = new Vector2(200f, 58f);
            ((_btnConfirm.transform) as RectTransform).anchoredPosition = new Vector2(0f, 42f);
            _btnConfirm.onClick.AddListener(OnClickConfirm);
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

        private static InputField CreateInput(Transform parent, Font font)
        {
            var image = DynamicUI.SpriteImage("m_inputNickname", parent, DynamicUI.ArtInputFrame, Color.white);
            var input = image.gameObject.AddComponent<InputField>();
            input.targetGraphic = image;
            var text = CreateText("Text", image.transform, font, 24, FontStyle.Normal, TextAnchor.MiddleLeft, Color.black);
            ((text.transform) as RectTransform).anchorMin = Vector2.zero;
            ((text.transform) as RectTransform).anchorMax = Vector2.one;
            ((text.transform) as RectTransform).offsetMin = new Vector2(18f, 0f);
            ((text.transform) as RectTransform).offsetMax = new Vector2(-18f, 0f);
            var placeholder = CreateText("Placeholder", image.transform, font, 24, FontStyle.Italic, TextAnchor.MiddleLeft, new Color(0.35f, 0.35f, 0.35f, 0.65f));
            ((placeholder.transform) as RectTransform).anchorMin = Vector2.zero;
            ((placeholder.transform) as RectTransform).anchorMax = Vector2.one;
            ((placeholder.transform) as RectTransform).offsetMin = new Vector2(18f, 0f);
            ((placeholder.transform) as RectTransform).offsetMax = new Vector2(-18f, 0f);
            placeholder.text = "请输入昵称";
            input.textComponent = text;
            input.placeholder = placeholder;
            input.characterLimit = 12;
            return input;
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

        private static Button CreateButton(string name, Transform parent, Font font, string label, string spriteLocation)
        {
            var image = DynamicUI.SpriteImage(name, parent, spriteLocation, Color.white);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            var text = CreateText("m_txtLabel", image.transform, font, 24, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
            Stretch(((text.transform) as RectTransform));
            text.text = label;
            return button;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
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


