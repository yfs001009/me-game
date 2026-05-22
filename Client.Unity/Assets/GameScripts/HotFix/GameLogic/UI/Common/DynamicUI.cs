using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    internal static class DynamicUI
    {
        public static Font Font => Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        public static void Clear(RectTransform root)
        {
            for (var i = root.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(root.GetChild(i).gameObject);
            }
        }

        public static void EnsureRaycaster(GameObject root)
        {
            if (root.GetComponent<GraphicRaycaster>() == null)
            {
                root.AddComponent<GraphicRaycaster>();
            }
        }

        public static Image Image(string name, Transform parent, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.color = color;
            return image;
        }

        public static Text Text(string name, Transform parent, string content, int size, TextAnchor anchor, Color color, FontStyle style = FontStyle.Normal)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<Text>();
            text.font = Font;
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = anchor;
            text.color = color;
            text.text = content ?? string.Empty;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            return text;
        }

        public static Button Button(string name, Transform parent, string label, Color color)
        {
            var image = Image(name, parent, color);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            var text = Text("m_txtLabel", image.transform, label, 24, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            Stretch(Rect(text));
            return button;
        }

        public static InputField Input(string name, Transform parent, string placeholderText, int limit = 32, bool password = false)
        {
            var image = Image(name, parent, Color.white);
            var input = image.gameObject.AddComponent<InputField>();
            input.targetGraphic = image;
            input.characterLimit = limit;
            input.contentType = password ? InputField.ContentType.Password : InputField.ContentType.Standard;

            var text = Text("Text", image.transform, string.Empty, 24, TextAnchor.MiddleLeft, Color.black);
            Rect(text).offsetMin = new Vector2(18f, 0f);
            Rect(text).offsetMax = new Vector2(-18f, 0f);
            Stretch(Rect(text));

            var placeholder = Text("Placeholder", image.transform, placeholderText, 24, TextAnchor.MiddleLeft, new Color(0.35f, 0.35f, 0.35f, 0.65f), FontStyle.Italic);
            Rect(placeholder).offsetMin = new Vector2(18f, 0f);
            Rect(placeholder).offsetMax = new Vector2(-18f, 0f);
            Stretch(Rect(placeholder));

            input.textComponent = text;
            input.placeholder = placeholder;
            return input;
        }

        public static RectTransform Rect(Component component) => component.transform as RectTransform;

        public static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        public static void Center(RectTransform rect, Vector2 size, Vector2 position)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
        }
    }
}
