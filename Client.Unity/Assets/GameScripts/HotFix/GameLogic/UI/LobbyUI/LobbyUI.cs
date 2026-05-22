using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Lobby;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI, location: "LobbyUI")]
    internal sealed class LobbyUI : UIWindow
    {
        private Text _txtPlayer;
        private Text _txtLobby;
        private Text _txtStatus;
        private Button _btnRefresh;
        private Button _btnCreateRoom;
        private Button _btnJoinRoom;

        protected override void ScriptGenerator()
        {
            _txtPlayer = FindChildComponent<Text>("m_txtPlayer");
            _txtLobby = FindChildComponent<Text>("m_txtLobby");
            _txtStatus = FindChildComponent<Text>("m_txtStatus");
            _btnRefresh = FindChildComponent<Button>("m_btnRefresh");
            _btnCreateRoom = FindChildComponent<Button>("m_btnCreateRoom");
            _btnJoinRoom = FindChildComponent<Button>("m_btnEnterBattle");
        }

        protected override void RegisterEvent()
        {
            _btnRefresh?.onClick.AddListener(OnClickRefresh);
            _btnCreateRoom?.onClick.AddListener(OnClickCreateRoom);
            _btnJoinRoom?.onClick.AddListener(OnClickJoinRoom);
            AddUIEvent<LobbyViewChangedEvent>(OnLobbyViewChanged);
            AddUIEvent<LobbyStatusChangedEvent>(OnLobbyStatusChanged);
        }

        protected override void OnCreate()
        {
            SetButtonText(_btnRefresh, "刷新大厅", _txtStatus?.font);
            SetButtonText(_btnCreateRoom, "创建房间", _txtStatus?.font);
            SetButtonText(_btnJoinRoom, "加入房间", _txtStatus?.font);
        }

        protected override void OnRefresh()
        {
            ApplyView(UserData as LobbyViewModel ?? LobbyController.Instance.GetCurrentLobbyView());
        }

        private void ApplyView(LobbyViewModel viewModel)
        {
            if (_txtPlayer != null)
            {
                _txtPlayer.text = viewModel == null
                    ? "玩家：未登录"
                    : $"玩家：{viewModel.PlayerName}  ID:{viewModel.PlayerId}  Lv.{viewModel.Level}";
            }

            if (_txtLobby != null)
            {
                _txtLobby.text = viewModel == null
                    ? "大厅：等待数据"
                    : $"大厅：房间 {viewModel.RoomCount} 个  匹配中：{(viewModel.IsMatching ? "是" : "否")}\n{viewModel.RoomListText}";
            }

            SetStatus("状态：已进入大厅");
        }

        private void OnLobbyViewChanged(LobbyViewChangedEvent eventData)
        {
            ApplyView(eventData.ViewModel);
        }

        private void OnLobbyStatusChanged(LobbyStatusChangedEvent eventData)
        {
            SetStatus(eventData.Status);
        }

        private async void OnClickRefresh()
        {
            LobbyController.Instance.SetStatus("状态：刷新大厅中...");
            await LobbyController.Instance.RefreshLobbyAsync();
            LobbyController.Instance.SetStatus("状态：大厅刷新完成");
        }

        private async void OnClickCreateRoom()
        {
            LobbyController.Instance.SetStatus("状态：创建房间中...");
            var room = await LobbyController.Instance.CreateRoomAsync("默认房间");
            GameModule.UI.CloseUI<LobbyUI>();
            GameModule.UI.ShowUIAsync<RoomUI>(room);
        }

        private async void OnClickJoinRoom()
        {
            LobbyController.Instance.SetStatus("状态：刷新房间列表中...");
            var lobby = await LobbyController.Instance.RefreshLobbyAsync();
            GameModule.UI.ShowUIAsync<RoomListUI>(lobby);
        }

        private void SetStatus(string text)
        {
            if (_txtStatus != null)
            {
                _txtStatus.text = text;
            }

            Log.Info(text);
        }

        private static void SetButtonText(Button button, string text, Font font)
        {
            if (button == null)
            {
                return;
            }

            var label = button.GetComponentInChildren<Text>(true);
            if (label == null)
            {
                var labelGo = new GameObject("m_txtLabel", typeof(RectTransform), typeof(Text));
                labelGo.transform.SetParent(button.transform, false);
                var rect = labelGo.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                label = labelGo.GetComponent<Text>();
                label.alignment = TextAnchor.MiddleCenter;
                label.color = Color.black;
                label.raycastTarget = false;
                label.font = font;
            }

            label.text = text;
            if (label.font == null && font != null)
            {
                label.font = font;
            }
        }
    }
}
