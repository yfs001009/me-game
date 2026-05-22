using GameLogic.SheepBattle.Common;
using GameLogic.SheepBattle.Lobby;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "TestUI")]
    internal sealed class RoomListUI : UIWindow
    {
        private RectTransform _listRoot;
        private Text _txtEmpty;
        private LobbyViewModel _viewModel;

        protected override void OnCreate()
        {
            BuildView();
        }

        protected override void OnRefresh()
        {
            _viewModel = UserData as LobbyViewModel ?? LobbyController.Instance.GetCurrentLobbyView();
            RefreshList();
        }

        private void BuildView()
        {
            EnsureGraphicRaycaster();
            ClearPrefabChildren();
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var mask = CreateImage("m_imgMask", rectTransform, new Color(0f, 0f, 0f, 0.55f));
            Stretch(((mask.transform) as RectTransform));

            var panel = CreateImage("m_imgPanel", rectTransform, new Color(0.94f, 0.96f, 0.94f, 1f));
            ((panel.transform) as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
            ((panel.transform) as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
            ((panel.transform) as RectTransform).sizeDelta = new Vector2(760f, 520f);
            ((panel.transform) as RectTransform).anchoredPosition = Vector2.zero;

            var title = CreateText("m_txtTitle", ((panel.transform) as RectTransform), font, 30, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black);
            ((title.transform) as RectTransform).anchorMin = new Vector2(0f, 1f);
            ((title.transform) as RectTransform).anchorMax = new Vector2(1f, 1f);
            ((title.transform) as RectTransform).pivot = new Vector2(0.5f, 1f);
            ((title.transform) as RectTransform).offsetMin = new Vector2(32f, -72f);
            ((title.transform) as RectTransform).offsetMax = new Vector2(-180f, -24f);
            title.text = "房间列表";

            var closeButton = CreateButton("m_btnClose", ((panel.transform) as RectTransform), font, "关闭", new Color(0.35f, 0.37f, 0.36f, 1f));
            ((closeButton.transform) as RectTransform).anchorMin = new Vector2(1f, 1f);
            ((closeButton.transform) as RectTransform).anchorMax = new Vector2(1f, 1f);
            ((closeButton.transform) as RectTransform).pivot = new Vector2(1f, 1f);
            ((closeButton.transform) as RectTransform).sizeDelta = new Vector2(120f, 48f);
            ((closeButton.transform) as RectTransform).anchoredPosition = new Vector2(-28f, -24f);
            closeButton.onClick.AddListener(() => GameModule.UI.CloseUI<RoomListUI>());

            _listRoot = new GameObject("m_listRooms", typeof(RectTransform)).GetComponent<RectTransform>();
            _listRoot.SetParent(((panel.transform) as RectTransform), false);
            _listRoot.anchorMin = new Vector2(0f, 0f);
            _listRoot.anchorMax = new Vector2(1f, 1f);
            _listRoot.offsetMin = new Vector2(32f, 34f);
            _listRoot.offsetMax = new Vector2(-32f, -96f);

            _txtEmpty = CreateText("m_txtEmpty", _listRoot, font, 24, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.18f, 0.18f, 0.18f, 1f));
            Stretch(((_txtEmpty.transform) as RectTransform));
            _txtEmpty.text = "暂无可加入房间";
        }

        private void RefreshList()
        {
            for (var i = _listRoot.childCount - 1; i >= 0; i--)
            {
                var child = _listRoot.GetChild(i);
                if (child != _txtEmpty.transform)
                {
                    Object.Destroy(child.gameObject);
                }
            }

            var rooms = _viewModel?.Rooms;
            _txtEmpty.gameObject.SetActive(rooms == null || rooms.Count == 0);
            if (rooms == null)
            {
                return;
            }

            for (var i = 0; i < rooms.Count; i++)
            {
                CreateRoomItem(rooms[i], i);
            }
        }

        private void CreateRoomItem(RoomSummaryViewModel room, int index)
        {
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var button = CreateButton($"m_btnRoom_{room.RoomId}", _listRoot, font, string.Empty, index % 2 == 0 ? new Color(0.84f, 0.89f, 0.86f, 1f) : new Color(0.78f, 0.84f, 0.81f, 1f));
            ((button.transform) as RectTransform).anchorMin = new Vector2(0f, 1f);
            ((button.transform) as RectTransform).anchorMax = new Vector2(1f, 1f);
            ((button.transform) as RectTransform).pivot = new Vector2(0.5f, 1f);
            ((button.transform) as RectTransform).sizeDelta = new Vector2(0f, 68f);
            ((button.transform) as RectTransform).anchoredPosition = new Vector2(0f, -index * 78f);

            var label = button.GetComponentInChildren<Text>();
            label.alignment = TextAnchor.MiddleLeft;
            label.color = Color.black;
            label.fontStyle = FontStyle.Normal;
            ((label.transform) as RectTransform).offsetMin = new Vector2(18f, 0f);
            ((label.transform) as RectTransform).offsetMax = new Vector2(-18f, 0f);
            label.text = $"{room.RoomName}  #{room.RoomId}    {room.CurrentPlayers}/{room.MaxPlayers}    地图:{room.MapId}    状态:{room.State}";
            button.onClick.AddListener(() => OnClickRoom(room.RoomId));
        }

        private async void OnClickRoom(int roomId)
        {
            var room = await LobbyController.Instance.JoinRoomAsync(roomId);
            if (room == null || room.RoomId <= 0)
            {
                CommonNoticeService.Show("加入房间失败，请刷新后重试。");
                return;
            }

            GameModule.UI.CloseUI<RoomListUI>();
            GameModule.UI.CloseUI<LobbyUI>();
            GameModule.UI.ShowUIAsync<RoomUI>(room);
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

        private static Button CreateButton(string name, Transform parent, Font font, string label, Color color)
        {
            var image = CreateImage(name, parent, color);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            var text = CreateText("m_txtLabel", image.transform, font, 22, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
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


