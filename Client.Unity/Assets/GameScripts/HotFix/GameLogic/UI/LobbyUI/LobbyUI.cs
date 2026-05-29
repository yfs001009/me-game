using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Lobby;
using TEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI, location: "LobbyUI")]
    internal sealed class LobbyUI : UIWindow
    {
        private Text _txtPlayer;
        private Text _txtLevel;
        private Text _txtGold;
        private Text _txtGem;
        private Text _txtCrystal;
        private Text _txtLobby;
        private Text _txtStatus;
        private Text _txtRoomCount;
        private Text _txtMatchState;
        private Text _txtLoadout;
        private Button _btnRefresh;
        private Button _btnStartMatch;
        private Button _btnCreateRoom;
        private Button _btnJoinRoom;

        protected override void ScriptGenerator()
        {
            _txtPlayer = FindChildComponent<Text>("m_topBar/m_playerPanel/m_txtPlayerName");
            _txtLevel = FindChildComponent<Text>("m_topBar/m_playerPanel/m_txtLevel");
            _txtGold = FindChildComponent<Text>("m_topBar/m_currencyGold/m_txtValue");
            _txtGem = FindChildComponent<Text>("m_topBar/m_currencyGem/m_txtValue");
            _txtCrystal = FindChildComponent<Text>("m_topBar/m_currencyCrystal/m_txtValue");
            _txtLobby = FindChildComponent<Text>("m_mainPanel/m_txtLobbySummary");
            _txtStatus = FindChildComponent<Text>("m_mainPanel/m_txtStatus");
            _txtRoomCount = FindChildComponent<Text>("m_mainPanel/m_statRooms/m_txtValue");
            _txtMatchState = FindChildComponent<Text>("m_mainPanel/m_statMatch/m_txtValue");
            _txtLoadout = FindChildComponent<Text>("m_mainPanel/m_statLoadout/m_txtValue");
            _btnRefresh = FindChildComponent<Button>("m_bottomBar/m_btnRefresh");
            _btnStartMatch = FindChildComponent<Button>("m_bottomBar/m_btnStartMatch");
            _btnCreateRoom = FindChildComponent<Button>("m_bottomBar/m_btnCreateRoom");
            _btnJoinRoom = FindChildComponent<Button>("m_bottomBar/m_btnRoomList");
        }

        protected override void RegisterEvent()
        {
            _btnRefresh?.onClick.AddListener(OnClickRefreshCommand);
            _btnStartMatch?.onClick.AddListener(OnClickStartMatchCommand);
            _btnCreateRoom?.onClick.AddListener(OnClickCreateRoom);
            _btnJoinRoom?.onClick.AddListener(OnClickJoinRoomCommand);
            AddUIEvent<LobbyViewChangedEvent>(OnLobbyViewChanged);
            AddUIEvent<LobbyStatusChangedEvent>(OnLobbyStatusChanged);
        }

        protected override void OnCreate()
        {
            ApplyView(LobbyController.Instance.GetCurrentLobbyView());
        }

        protected override void OnRefresh()
        {
            ApplyView(UserData as LobbyViewModel ?? LobbyController.Instance.GetCurrentLobbyView());
        }

        private void ApplyView(LobbyViewModel viewModel)
        {
            var playerName = string.IsNullOrWhiteSpace(viewModel?.PlayerName) ? "未登录" : viewModel.PlayerName;
            var level = viewModel?.Level > 0 ? viewModel.Level : 1;

            SetText(_txtPlayer, playerName);
            SetText(_txtLevel, $"Lv.{level}");
            SetText(_txtGold, "12,800");
            SetText(_txtGem, "680");
            SetText(_txtCrystal, "96");
            SetText(_txtRoomCount, viewModel == null ? "- 个" : $"{viewModel.RoomCount} 个");
            SetText(_txtMatchState, viewModel?.IsMatching == true ? $"等待 {viewModel.MatchEstimatedSeconds}s" : "空闲");
            SetText(_txtLoadout, "默认 6 张");

            if (_txtLobby != null)
            {
                _txtLobby.text = viewModel == null
                    ? "大厅数据加载中"
                    : $"当前房间 {viewModel.RoomCount} 个  |  可加入 {FormatJoinableRoom(viewModel)}";
            }

            SetStatus(viewModel?.IsMatching == true ? "状态：正在匹配队列中" : "状态：已进入大厅");
        }

        private static string FormatJoinableRoom(LobbyViewModel viewModel)
        {
            if (viewModel.JoinableRoomId <= 0)
            {
                return "暂无";
            }

            return $"{viewModel.JoinableRoomName} {viewModel.JoinableRoomCurrentPlayers}/{viewModel.JoinableRoomMaxPlayers}";
        }

        private void OnLobbyViewChanged(LobbyViewChangedEvent eventData)
        {
            ApplyView(eventData.ViewModel);
        }

        private void OnLobbyStatusChanged(LobbyStatusChangedEvent eventData)
        {
            SetStatus(eventData.Status);
        }

        private void OnClickRefreshCommand()
        {
            GameEvent.Get<ILobbyCommand>()?.OnRefreshLobby();
        }

        private void OnClickStartMatchCommand()
        {
            GameEvent.Get<ILobbyCommand>()?.OnStartMatch();
        }

        private void OnClickJoinRoomCommand()
        {
            GameEvent.Get<ILobbyCommand>()?.OnOpenRoomList();
        }

        private void OnClickCreateRoom()
        {
            GameModule.UI.ShowUIAsync<CreateRoomUI>();
        }

        private void SetStatus(string text)
        {
            SetText(_txtStatus, text);
            Log.Info(text);
        }

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }
    }
}
