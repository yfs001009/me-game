using System.Collections.Generic;
using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Lobby;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "RoomListUI")]
    internal sealed class RoomListUI : UIWindow
    {
        private RectTransform _listRoot;
        private Text _txtEmpty;
        private Button _btnClose;
        private Button _btnRefresh;
        private Button _roomTemplate;
        private LobbyViewModel _viewModel;
        private readonly List<Button> _roomItems = new();

        protected override void ScriptGenerator()
        {
            _listRoot = FindChild("m_imgPanel/m_listRooms") as RectTransform;
            _txtEmpty = FindChildComponent<Text>("m_imgPanel/m_listRooms/m_txtEmpty");
            _roomTemplate = FindChildComponent<Button>("m_imgPanel/m_listRooms/m_btnRoomTemplate");
            _btnClose = FindChildComponent<Button>("m_imgPanel/m_btnClose");
            _btnRefresh = FindChildComponent<Button>("m_imgPanel/m_btnRefresh");
        }

        protected override void RegisterEvent()
        {
            AddUIEvent<LobbyViewChangedEvent>(OnLobbyViewChanged);
        }

        protected override void OnCreate()
        {
            _roomTemplate.gameObject.SetActive(false);
            _btnClose.onClick.AddListener(() => GameModule.UI.CloseUI<RoomListUI>());
            _btnRefresh.onClick.AddListener(OnClickRefresh);
        }

        protected override void OnRefresh()
        {
            _viewModel = UserData as LobbyViewModel ?? LobbyController.Instance.GetCurrentLobbyView();
            RefreshList();
        }

        private void RefreshList()
        {
            var rooms = _viewModel?.Rooms;
            var count = rooms?.Count ?? 0;
            _txtEmpty.gameObject.SetActive(count == 0);

            for (var i = 0; i < count; i++)
            {
                RefreshRoomItem(i, rooms[i]);
            }

            for (var i = count; i < _roomItems.Count; i++)
            {
                _roomItems[i].gameObject.SetActive(false);
            }
        }

        private void RefreshRoomItem(int index, RoomSummaryViewModel room)
        {
            var button = GetOrCreateRoomItem(index);
            button.gameObject.SetActive(true);
            button.name = $"m_btnRoom_{room.RoomId}";
            var rect = button.transform as RectTransform;
            rect.anchoredPosition = new Vector2(0f, -index * 78f);

            var label = button.GetComponentInChildren<Text>();
            var privacy = room.IsPrivate ? "私密" : "公开";
            label.text = $"{room.RoomName}  #{room.RoomId}    {room.CurrentPlayers}/{room.MaxPlayers}    地图:{room.MapId}    {privacy}    状态:{room.State}";
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnClickRoom(room));
        }

        private Button GetOrCreateRoomItem(int index)
        {
            while (_roomItems.Count <= index)
            {
                var instance = Object.Instantiate(_roomTemplate.gameObject, _listRoot, false);
                var button = instance.GetComponent<Button>();
                _roomItems.Add(button);
            }

            return _roomItems[index];
        }

        private void OnLobbyViewChanged(LobbyViewChangedEvent eventData)
        {
            _viewModel = eventData.ViewModel;
            RefreshList();
        }

        private void OnClickRefresh()
        {
            GameEvent.Get<ILobbyCommand>()?.OnRefreshLobby();
        }

        private void OnClickRoom(RoomSummaryViewModel roomSummary)
        {
            if (roomSummary == null)
            {
                return;
            }

            if (roomSummary.IsPrivate)
            {
                GameModule.UI.ShowUIAsync<RoomPasswordUI>(roomSummary);
                return;
            }

            GameEvent.Get<ILobbyCommand>()?.OnJoinRoom(roomSummary.RoomId, string.Empty);
        }
    }
}
