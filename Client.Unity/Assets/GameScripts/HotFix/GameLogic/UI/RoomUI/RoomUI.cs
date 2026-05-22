using GameLogic.SheepBattle.Battle;
using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Lobby;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI, location: "RoomUI")]
    internal sealed class RoomUI : UIWindow
    {
        private Text _txtRoomTitle;
        private Text _txtRoomInfo;
        private Text _txtRoomState;
        private Text _txtPlayerList;
        private Button _btnStartBattle;
        private Button _btnLeaveRoom;

        protected override void ScriptGenerator()
        {
            _txtRoomTitle = FindChildComponent<Text>("m_txtRoomTitle");
            _txtRoomInfo = FindChildComponent<Text>("m_txtRoomInfo");
            _txtRoomState = FindChildComponent<Text>("m_txtRoomState");
            _txtPlayerList = FindChildComponent<Text>("m_txtPlayerList");
            _btnStartBattle = FindChildComponent<Button>("m_btnStartBattle");
            _btnLeaveRoom = FindChildComponent<Button>("m_btnLeaveRoom");
        }

        protected override void RegisterEvent()
        {
            _btnStartBattle?.onClick.AddListener(OnClickStartBattle);
            _btnLeaveRoom?.onClick.AddListener(OnClickLeaveRoom);
            AddUIEvent<RoomViewChangedEvent>(OnRoomViewChanged);
        }

        protected override void OnCreate()
        {
            SetButtonText(_btnStartBattle, "开始战斗", _txtRoomState?.font);
            SetButtonText(_btnLeaveRoom, "离开房间", _txtRoomState?.font);
            EnsurePlayerListText();
        }

        protected override void OnRefresh()
        {
            ApplyView(UserData as RoomViewModel);
        }

        private void ApplyView(RoomViewModel viewModel)
        {
            if (_txtRoomTitle != null)
            {
                _txtRoomTitle.text = viewModel == null ? "房间" : $"{viewModel.RoomName}  #{viewModel.RoomId}";
            }

            if (_txtRoomInfo != null)
            {
                _txtRoomInfo.text = viewModel == null
                    ? "房间信息待加载"
                    : $"模式：{viewModel.Mode}  地图：{viewModel.MapId}  人数：{viewModel.CurrentPlayers}/{viewModel.MaxPlayers}";
            }

            if (_txtRoomState != null)
            {
                _txtRoomState.text = viewModel == null ? "状态：等待进入房间" : $"状态：{viewModel.State}，等待玩家准备";
            }

            EnsurePlayerListText();
            if (_txtPlayerList != null)
            {
                _txtPlayerList.text = BuildPlayerListText(viewModel);
            }
        }

        private void OnRoomViewChanged(RoomViewChangedEvent eventData)
        {
            ApplyView(eventData.ViewModel);
        }

        private void OnClickStartBattle()
        {
            GameModule.UI.CloseUI<RoomUI>();
            BattleController.Instance.EnterBattle();
        }

        private async void OnClickLeaveRoom()
        {
            try
            {
                if (!await LobbyController.Instance.LeaveCurrentRoomAsync())
                {
                    return;
                }

                GameModule.UI.CloseUI<RoomUI>();
                GameModule.UI.ShowUIAsync<LobbyUI>(LobbyController.Instance.GetCurrentLobbyView());
            }
            catch (System.Exception exception)
            {
                LobbyController.Instance.SetStatus($"状态：离开房间失败：{exception.Message}");
                Log.Error($"离开房间失败：{exception}");
            }
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

        private void EnsurePlayerListText()
        {
            if (_txtPlayerList != null)
            {
                return;
            }

            var font = _txtRoomState != null ? _txtRoomState.font : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var labelGo = new GameObject("m_txtPlayerList", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            labelGo.transform.SetParent(rectTransform, false);
            var rect = labelGo.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(620f, 220f);
            rect.anchoredPosition = new Vector2(0f, -40f);
            _txtPlayerList = labelGo.GetComponent<Text>();
            _txtPlayerList.font = font;
            _txtPlayerList.fontSize = 24;
            _txtPlayerList.alignment = TextAnchor.UpperLeft;
            _txtPlayerList.color = Color.black;
            _txtPlayerList.raycastTarget = false;
            _txtPlayerList.horizontalOverflow = HorizontalWrapMode.Wrap;
            _txtPlayerList.verticalOverflow = VerticalWrapMode.Truncate;
        }

        private static string BuildPlayerListText(RoomViewModel viewModel)
        {
            if (viewModel == null)
            {
                return "玩家列表：暂无";
            }

            if (viewModel.Players == null || viewModel.Players.Count == 0)
            {
                return "玩家列表：暂无玩家信息";
            }

            var text = "玩家列表：\n";
            for (var i = 0; i < viewModel.Players.Count; i++)
            {
                var player = viewModel.Players[i];
                var owner = player.IsOwner ? " 房主" : string.Empty;
                var ready = player.IsReady ? " 已准备" : " 未准备";
                text += $"{i + 1}. {player.Nickname}  Lv.{player.Level}{owner}  {ready}\n";
            }

            return text.TrimEnd();
        }
    }
}

