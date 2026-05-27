using GameLogic.SheepBattle.Common;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "TestUI")]
    internal sealed class CommonNoticeUI : UIWindow
    {
        private Text _txtTitle;
        private Text _txtMessage;
        private Button _btnConfirm;

        protected override void OnCreate()
        {
            BuildView();
        }

        protected override void OnRefresh()
        {
            var data = UserData as CommonNoticeData;
            _txtTitle.text = string.IsNullOrWhiteSpace(data?.Title) ? "提示" : data.Title;
            _txtMessage.text = data?.Message ?? string.Empty;
        }

        private void BuildView()
        {
            EnsureGraphicRaycaster();
            ClearPrefabChildren();
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var mask = CreateImage("m_imgMask", rectTransform, new Color(0f, 0f, 0f, 0.55f));
            Stretch(((mask.transform) as RectTransform));

            var panel = DynamicUI.SpriteImage("m_imgPanel", rectTransform, DynamicUI.ArtPanelPopup, Color.white);
            ((panel.transform) as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
            ((panel.transform) as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
            ((panel.transform) as RectTransform).sizeDelta = new Vector2(560f, 300f);
            ((panel.transform) as RectTransform).anchoredPosition = Vector2.zero;

            _txtTitle = CreateText("m_txtTitle", ((panel.transform) as RectTransform), font, 30, FontStyle.Bold, TextAnchor.MiddleCenter, Color.black);
            ((_txtTitle.transform) as RectTransform).anchorMin = new Vector2(0f, 1f);
            ((_txtTitle.transform) as RectTransform).anchorMax = new Vector2(1f, 1f);
            ((_txtTitle.transform) as RectTransform).pivot = new Vector2(0.5f, 1f);
            ((_txtTitle.transform) as RectTransform).sizeDelta = new Vector2(-48f, 58f);
            ((_txtTitle.transform) as RectTransform).anchoredPosition = new Vector2(0f, -24f);

            _txtMessage = CreateText("m_txtMessage", ((panel.transform) as RectTransform), font, 24, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.12f, 0.12f, 0.12f, 1f));
            ((_txtMessage.transform) as RectTransform).anchorMin = new Vector2(0f, 0f);
            ((_txtMessage.transform) as RectTransform).anchorMax = new Vector2(1f, 1f);
            ((_txtMessage.transform) as RectTransform).offsetMin = new Vector2(48f, 92f);
            ((_txtMessage.transform) as RectTransform).offsetMax = new Vector2(-48f, -86f);

            _btnConfirm = CreateButton("m_btnConfirm", ((panel.transform) as RectTransform), font, "确定", DynamicUI.ArtButtonPrimary);
            ((_btnConfirm.transform) as RectTransform).anchorMin = new Vector2(0.5f, 0f);
            ((_btnConfirm.transform) as RectTransform).anchorMax = new Vector2(0.5f, 0f);
            ((_btnConfirm.transform) as RectTransform).sizeDelta = new Vector2(180f, 54f);
            ((_btnConfirm.transform) as RectTransform).anchoredPosition = new Vector2(0f, 36f);
            _btnConfirm.onClick.AddListener(() => GameModule.UI.CloseUI<CommonNoticeUI>());
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


