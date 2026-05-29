using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Lobby;
using GameLogic.SheepBattle.Network;
using System.Collections.Generic;
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
        private Transform _playerSlotRoot;
        private Button _btnStartBattle;
        private Button _btnLeaveRoom;
        private readonly List<RoomPlayerSlot> _playerSlots = new();
        private const string PlayerSlotAsset = "RoomPlayerSlot";
        private RoomViewModel _viewModel;

        protected override void ScriptGenerator()
        {
            _txtRoomTitle = FindChildComponent<Text>("m_txtRoomTitle");
            _txtRoomInfo = FindChildComponent<Text>("m_txtRoomInfo");
            _txtRoomState = FindChildComponent<Text>("m_txtRoomState");
            _playerSlotRoot = FindChild("m_panelCenter/m_scrollPlayers/Viewport/Content");
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
            SetButtonText(_btnLeaveRoom, "离开房间", _txtRoomState?.font);
            ApplyArtSkin();
        }

        protected override void OnRefresh()
        {
            ApplyView(UserData as RoomViewModel);
        }

        private void ApplyView(RoomViewModel viewModel)
        {
            _viewModel = viewModel;
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

            RefreshPlayerSlots(viewModel);
            RefreshActionButtons(viewModel);
        }

        private void OnRoomViewChanged(RoomViewChangedEvent eventData)
        {
            ApplyView(eventData.ViewModel);
        }

        private void OnClickStartBattle()
        {
            GameEvent.Get<ILobbyCommand>()?.OnRoomPrimaryAction();
        }

        private void OnClickLeaveRoom()
        {
            GameEvent.Get<ILobbyCommand>()?.OnLeaveRoom();
        }

        private void RefreshPlayerSlots(RoomViewModel viewModel)
        {
            var maxPlayers = Mathf.Max(viewModel?.MaxPlayers ?? 4, 1);
            EnsurePlayerSlotCount(maxPlayers);
            for (var i = 0; i < _playerSlots.Count; i++)
            {
                var player = viewModel?.Players != null && i < viewModel.Players.Count ? viewModel.Players[i] : null;
                _playerSlots[i].Refresh(player, i);
            }
        }

        private void RefreshActionButtons(RoomViewModel viewModel)
        {
            if (_btnStartBattle == null)
            {
                return;
            }

            if (IsLocalOwner(viewModel))
            {
                SetButtonText(_btnStartBattle, "开始游戏", _txtRoomState?.font);
                return;
            }

            var localPlayer = GetLocalPlayer(viewModel);
            SetButtonText(_btnStartBattle, localPlayer != null && localPlayer.IsReady ? "取消准备" : "准备", _txtRoomState?.font);
        }

        private static bool IsLocalOwner(RoomViewModel viewModel)
        {
            return GetLocalPlayer(viewModel)?.IsOwner ?? false;
        }

        private static RoomPlayerViewModel GetLocalPlayer(RoomViewModel viewModel)
        {
            var playerId = SheepNetworkService.Instance.Profile?.PlayerId ?? 0;
            if (playerId <= 0 || viewModel?.Players == null)
            {
                return null;
            }

            for (var i = 0; i < viewModel.Players.Count; i++)
            {
                var player = viewModel.Players[i];
                if (player != null && player.PlayerId == playerId)
                {
                    return player;
                }
            }

            return null;
        }

        private void EnsurePlayerSlotCount(int count)
        {
            if (_playerSlotRoot == null)
            {
                return;
            }

            DisablePlayerSlotLayoutGroups();
            while (_playerSlots.Count < count)
            {
                var slot = CreateWidgetByPath<RoomPlayerSlot>(_playerSlotRoot, PlayerSlotAsset);
                if (slot == null)
                {
                    return;
                }

                slot.gameObject.name = $"m_itemRoomPlayerSlot{_playerSlots.Count + 1}";
                slot.gameObject.SetActive(true);
                _playerSlots.Add(slot);
            }

            for (var i = 0; i < _playerSlots.Count; i++)
            {
                _playerSlots[i].Visible = i < count;
            }

            LayoutPlayerSlots(count);
        }

        private void DisablePlayerSlotLayoutGroups()
        {
            if (_playerSlotRoot == null)
            {
                return;
            }

            var layoutGroups = _playerSlotRoot.GetComponents<LayoutGroup>();
            for (var i = 0; i < layoutGroups.Length; i++)
            {
                if (layoutGroups[i] != null)
                {
                    layoutGroups[i].enabled = false;
                }
            }
        }

        private void LayoutPlayerSlots(int visibleCount)
        {
            var contentRect = _playerSlotRoot as RectTransform;
            if (contentRect == null)
            {
                return;
            }

            const float paddingX = 16f;
            const float paddingY = 16f;
            const float spacingX = 10f;
            const float spacingY = 10f;
            const float cellHeight = 72f;
            const float preferredCellWidth = 320f;
            var contentWidth = contentRect.rect.width > 0f ? contentRect.rect.width : 688f;
            var usableWidth = Mathf.Max(preferredCellWidth, contentWidth - paddingX * 2f);
            var columns = Mathf.Max(1, Mathf.FloorToInt((usableWidth + spacingX) / (preferredCellWidth + spacingX)));
            var cellWidth = Mathf.Max(240f, (contentWidth - paddingX * 2f - Mathf.Max(0, columns - 1) * spacingX) / columns);
            var rows = Mathf.CeilToInt(visibleCount / (float)columns);

            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, paddingY * 2f + rows * cellHeight + Mathf.Max(0, rows - 1) * spacingY);

            for (var i = 0; i < _playerSlots.Count; i++)
            {
                var slot = _playerSlots[i];
                if (slot == null || slot.rectTransform == null)
                {
                    continue;
                }

                var column = i % columns;
                var row = i / columns;
                var rect = slot.rectTransform;
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0f, 1f);
                rect.sizeDelta = new Vector2(cellWidth, cellHeight);
                rect.anchoredPosition = new Vector2(paddingX + column * (cellWidth + spacingX), -paddingY - row * (cellHeight + spacingY));
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
                Log.Warning($"按钮缺少 m_txtLabel 文本节点：{button.name}");
                return;
            }

            label.text = text;
            if (label.font == null && font != null)
            {
                label.font = font;
            }
        }

        protected override void OnDestroy()
        {
            _playerSlots.Clear();
        }

        protected override void OnUpdate()
        {
            PollRoomDetail();
        }

        private float _nextPollTime;

        private void PollRoomDetail()
        {
            if (_viewModel == null || Time.unscaledTime < _nextPollTime)
            {
                return;
            }

            _nextPollTime = Time.unscaledTime + 1f;
            GameEvent.Get<ILobbyCommand>()?.OnRefreshCurrentRoom();
        }

        private void ApplyArtSkin()
        {
            var background = FindChildComponent<Image>("m_imgBackground");
            DynamicUI.ApplySprite(background, DynamicUI.ArtPanelRoom);
            SkinButton(_btnStartBattle, DynamicUI.ArtButtonPrimary);
            SkinButton(_btnLeaveRoom, DynamicUI.ArtButtonDanger);
        }

        private static void SkinButton(Button button, string spriteLocation)
        {
            if (button?.targetGraphic is Image image)
            {
                DynamicUI.ApplySprite(image, spriteLocation);
                image.color = Color.white;
            }
        }
    }
}
