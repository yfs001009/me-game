using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Lobby;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "TestUI")]
    internal sealed class MatchQueueUI : UIWindow
    {
        private Text _txtStatus;
        private LobbyViewModel _viewModel;

        protected override void RegisterEvent()
        {
            AddUIEvent<LobbyViewChangedEvent>(OnLobbyViewChanged);
        }

        protected override void OnCreate()
        {
            BuildView();
        }

        protected override void OnRefresh()
        {
            _viewModel = UserData as LobbyViewModel ?? LobbyController.Instance.GetCurrentLobbyView();
            ApplyView();
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
            panelRect.sizeDelta = new Vector2(600f, 360f);
            panelRect.anchoredPosition = Vector2.zero;

            var title = CreateText("m_txtTitle", panelRect, font, 30, FontStyle.Bold, TextAnchor.MiddleCenter, Color.black);
            SetRect(title.transform as RectTransform, 0f, 112f, 520f, 58f);
            title.text = "匹配队列";

            _txtStatus = CreateText("m_txtStatus", panelRect, font, 24, FontStyle.Normal, TextAnchor.MiddleCenter, Color.black);
            SetRect(_txtStatus.transform as RectTransform, 0f, 20f, 520f, 120f);

            var refreshButton = CreateButton("m_btnRefresh", panelRect, font, "刷新状态", DynamicUI.ArtButtonSecondary);
            SetRect(refreshButton.transform as RectTransform, -96f, -116f, 160f, 54f);
            refreshButton.onClick.AddListener(OnClickRefresh);

            var closeButton = CreateButton("m_btnClose", panelRect, font, "返回大厅", DynamicUI.ArtButtonDanger);
            SetRect(closeButton.transform as RectTransform, 96f, -116f, 160f, 54f);
            closeButton.onClick.AddListener(() => GameModule.UI.CloseUI<MatchQueueUI>());
        }

        private void ApplyView()
        {
            if (_txtStatus == null)
            {
                return;
            }

            if (_viewModel == null || !_viewModel.IsMatching)
            {
                _txtStatus.text = "尚未进入匹配。";
                return;
            }

            _txtStatus.text = _viewModel.MatchRoomId > 0
                ? $"已分配房间：#{_viewModel.MatchRoomId}"
                : $"匹配中...\n预计等待 {_viewModel.MatchEstimatedSeconds} 秒";
        }

        private void OnLobbyViewChanged(LobbyViewChangedEvent eventData)
        {
            _viewModel = eventData.ViewModel;
            ApplyView();
        }

        private void OnClickRefresh()
        {
            GameEvent.Get<ILobbyCommand>()?.OnRefreshLobby();
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
