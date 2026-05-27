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
        private Button _btnStartMatch;
        private Button _btnCreateRoom;
        private Button _btnJoinRoom;

        protected override void ScriptGenerator()
        {
            _txtPlayer = FindChildComponent<Text>("m_txtPlayer");
            _txtLobby = FindChildComponent<Text>("m_txtLobby");
            _txtStatus = FindChildComponent<Text>("m_txtStatus");
            _btnRefresh = FindChildComponent<Button>("m_btnRefresh");
            _btnStartMatch = FindChildComponent<Button>("m_btnStartMatch");
            _btnCreateRoom = FindChildComponent<Button>("m_btnCreateRoom");
            _btnJoinRoom = FindChildComponent<Button>("m_btnEnterBattle");
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
            SetButtonText(_btnRefresh, "刷新大厅", _txtStatus?.font);
            SetButtonText(_btnStartMatch, "直接匹配", _txtStatus?.font);
            SetButtonText(_btnCreateRoom, "创建房间", _txtStatus?.font);
            SetButtonText(_btnJoinRoom, "加入房间", _txtStatus?.font);
            ApplyLobbyLayout();
            ApplyArtSkin();
        }

        protected override void OnRefresh()
        {
            ApplyView(UserData as LobbyViewModel ?? LobbyController.Instance.GetCurrentLobbyView());
        }

        private void ApplyView(LobbyViewModel viewModel)
        {
            if (_txtPlayer != null)
            {
                _txtPlayer.text = viewModel == null ? "玩家：未登录" : $"{viewModel.PlayerName}  Lv.{viewModel.Level}";
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

        private void OnClickRefresh()
        {
            GameEvent.Get<ILobbyCommand>()?.OnRefreshLobby();
        }

        private void OnClickStartMatch()
        {
            GameEvent.Get<ILobbyCommand>()?.OnStartMatch();
        }

        private void OnClickCreateRoom()
        {
            GameModule.UI.ShowUIAsync<CreateRoomUI>();
        }

        private void OnClickJoinRoom()
        {
            GameEvent.Get<ILobbyCommand>()?.OnOpenRoomList();
        }

        private void SetStatus(string text)
        {
            if (_txtStatus != null)
            {
                _txtStatus.text = text;
            }

            Log.Info(text);
        }

        private void ApplyLobbyLayout()
        {
            SetTopRightText(_txtPlayer, -36f, -32f, 360f, 48f);
            SetCenterText(_txtLobby?.transform as RectTransform, 96f, 640f, 120f);
            SetCenterText(_txtStatus?.transform as RectTransform, -128f, 640f, 52f);
            SetButtonRect(_btnStartMatch, -170f, 12f);
            SetButtonRect(_btnCreateRoom, 170f, 12f);
            SetButtonRect(_btnJoinRoom, -170f, -72f);
            SetButtonRect(_btnRefresh, 170f, -72f);
        }

        private void ApplyArtSkin()
        {
            var rootImage = gameObject.GetComponent<Image>();
            if (rootImage == null)
            {
                rootImage = gameObject.AddComponent<Image>();
            }

            DynamicUI.ApplySprite(rootImage, DynamicUI.ArtLobbyBackground, Image.Type.Simple);
            SkinButton(_btnStartMatch, DynamicUI.ArtButtonPrimary);
            SkinButton(_btnCreateRoom, DynamicUI.ArtButtonSecondary);
            SkinButton(_btnJoinRoom, DynamicUI.ArtButtonSecondary);
            SkinButton(_btnRefresh, DynamicUI.ArtButtonSecondary);
        }

        private static void SkinButton(Button button, string spriteLocation)
        {
            if (button?.targetGraphic is Image image)
            {
                DynamicUI.ApplySprite(image, spriteLocation);
                image.color = Color.white;
            }
        }

        private static void SetTopRightText(Text text, float x, float y, float width, float height)
        {
            if (text == null)
            {
                return;
            }

            var rect = text.transform as RectTransform;
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = new Vector2(x, y);
            text.alignment = TextAnchor.MiddleRight;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Truncate;
        }

        private static void SetCenterText(RectTransform rect, float y, float width, float height)
        {
            if (rect == null)
            {
                return;
            }

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = new Vector2(0f, y);
        }

        private static void SetButtonRect(Button button, float x, float y)
        {
            var rect = button != null ? button.transform as RectTransform : null;
            if (rect == null)
            {
                return;
            }

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(220f, 64f);
            rect.anchoredPosition = new Vector2(x, y);
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
