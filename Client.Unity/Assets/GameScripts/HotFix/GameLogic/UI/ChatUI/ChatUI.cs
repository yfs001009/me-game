using System.Collections.Generic;
using System.Linq;
using Fantasy;
using GameLogic.SheepBattle.Chat;
using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Network;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "ChatUI")]
    internal sealed class ChatUI : UIWindow
    {
        private RectTransform _drawerPanel;
        private RectTransform _messageListRect;
        private RectTransform _contactList;
        private Button _btnClose;
        private Button _btnComposite;
        private Button _btnPrivate;
        private Button _btnSend;
        private InputField _inputTarget;
        private InputField _inputMessage;
        private RectTransform _listRoot;
        private Text _messageTemplate;
        private Text _contactTemplate;
        private Text _txtEmpty;
        private Text _txtTargetLabel;
        private Text _txtPrivateHint;

        private readonly List<Text> _items = new();
        private readonly List<Text> _contactItems = new();
        private ChatViewModel _viewModel;
        private long _selectedPrivateTargetId;

        protected override void ScriptGenerator()
        {
            _drawerPanel = FindChild("m_drawerPanel") as RectTransform;
            _btnClose = FindChildComponent<Button>("m_drawerPanel/m_btnCollapse");
            _btnComposite = FindChildComponent<Button>("m_drawerPanel/m_tabs/m_btnComposite");
            _btnPrivate = FindChildComponent<Button>("m_drawerPanel/m_tabs/m_btnPrivate");
            _btnSend = FindChildComponent<Button>("m_drawerPanel/m_btnSend");
            _inputTarget = FindChildComponent<InputField>("m_drawerPanel/m_inputTarget");
            _inputMessage = FindChildComponent<InputField>("m_drawerPanel/m_inputMessage");
            _contactList = FindChild("m_drawerPanel/m_listContacts") as RectTransform;
            _contactTemplate = FindChildComponent<Text>("m_drawerPanel/m_listContacts/Viewport/Content/m_txtContactTemplate");
            _messageListRect = FindChild("m_drawerPanel/m_listMessages") as RectTransform;
            _listRoot = FindChild("m_drawerPanel/m_listMessages/Viewport/Content") as RectTransform;
            _messageTemplate = FindChildComponent<Text>("m_drawerPanel/m_listMessages/Viewport/Content/m_txtMessageTemplate");
            _txtEmpty = FindChildComponent<Text>("m_drawerPanel/m_listMessages/m_txtEmpty");
            _txtTargetLabel = FindChildComponent<Text>("m_drawerPanel/m_txtTargetLabel");
            _txtPrivateHint = FindChildComponent<Text>("m_drawerPanel/m_txtPrivateHint");
        }

        protected override void RegisterEvent()
        {
            AddUIEvent<ChatViewChangedEvent>(OnChatViewChanged);
        }

        protected override void OnCreate()
        {
            _messageTemplate?.gameObject.SetActive(false);
            _contactTemplate?.gameObject.SetActive(false);
            _btnClose?.onClick.AddListener(() => GameModule.UI.CloseUI<ChatUI>());
            _btnComposite?.onClick.AddListener(() => ChatController.Instance.SwitchMode(ChatConstants.CompositeMode));
            _btnPrivate?.onClick.AddListener(() => ChatController.Instance.SwitchMode(ChatConstants.PrivateMode));
            _btnSend?.onClick.AddListener(OnClickSend);
        }

        protected override void OnRefresh()
        {
            _viewModel = UserData as ChatViewModel ?? ChatController.Instance.Model;
            RefreshView();
        }

        private void OnChatViewChanged(ChatViewChangedEvent eventData)
        {
            _viewModel = eventData.ViewModel;
            RefreshView();
        }

        private void RefreshView()
        {
            var isPrivateMode = _viewModel?.IsPrivateMode == true;
            var privateTargets = BuildPrivateTargets();
            if (isPrivateMode && _selectedPrivateTargetId <= 0 && privateTargets.Count > 0)
            {
                _selectedPrivateTargetId = privateTargets[0].PlayerId;
            }

            var messages = isPrivateMode ? GetSelectedPrivateMessages() : (_viewModel?.CompositeMessages ?? new List<ChatMessageTreeInfo>());
            SetActive(_txtEmpty, messages.Count == 0);
            SetActive(_inputTarget, isPrivateMode);
            SetActive(_txtTargetLabel, isPrivateMode);
            SetActive(_contactList, isPrivateMode);
            SetActive(_txtPrivateHint, isPrivateMode && privateTargets.Count == 0);
            ApplyModeLayout(isPrivateMode);
            RefreshContacts(privateTargets);

            for (var i = 0; i < messages.Count; i++)
            {
                RefreshItem(i, messages[i]);
            }

            for (var i = messages.Count; i < _items.Count; i++)
            {
                _items[i].gameObject.SetActive(false);
            }
        }

        private void RefreshItem(int index, ChatMessageTreeInfo message)
        {
            var text = GetOrCreateItem(index);
            if (text == null)
            {
                return;
            }

            text.gameObject.SetActive(true);
            text.text = FormatMessage(message);
        }

        private Text GetOrCreateItem(int index)
        {
            if (_messageTemplate == null || _listRoot == null)
            {
                return null;
            }

            while (_items.Count <= index)
            {
                var instance = Object.Instantiate(_messageTemplate.gameObject, _listRoot, false);
                _items.Add(instance.GetComponent<Text>());
            }

            return _items[index];
        }

        private void OnClickSend()
        {
            var content = _inputMessage?.text ?? string.Empty;
            if (_viewModel?.IsPrivateMode == true)
            {
                var targetText = _inputTarget?.text ?? string.Empty;
                if (!long.TryParse(targetText, out var targetId) || targetId <= 0)
                {
                    targetId = _selectedPrivateTargetId;
                }

                if (targetId > 0)
                {
                    ChatController.Instance.SendPrivateAsync(targetId, content).Coroutine();
                }
            }
            else
            {
                ChatController.Instance.SendCompositeAsync(content).Coroutine();
            }

            if (_inputMessage != null)
            {
                _inputMessage.text = string.Empty;
            }
        }

        private void RefreshContacts(IReadOnlyList<PrivateTargetView> targets)
        {
            for (var i = 0; i < targets.Count; i++)
            {
                var item = GetOrCreateContact(i);
                if (item == null)
                {
                    continue;
                }

                var target = targets[i];
                item.gameObject.SetActive(true);
                item.text = target.PlayerId == _selectedPrivateTargetId ? $"■ {target.DisplayName}" : target.DisplayName;
                var button = item.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => SelectPrivateTarget(target.PlayerId));
                }
            }

            for (var i = targets.Count; i < _contactItems.Count; i++)
            {
                _contactItems[i].gameObject.SetActive(false);
            }
        }

        private Text GetOrCreateContact(int index)
        {
            if (_contactTemplate == null || _contactTemplate.transform.parent == null)
            {
                return null;
            }

            while (_contactItems.Count <= index)
            {
                var instance = Object.Instantiate(_contactTemplate.gameObject, _contactTemplate.transform.parent, false);
                instance.SetActive(true);
                if (instance.GetComponent<Button>() == null)
                {
                    var button = instance.AddComponent<Button>();
                    button.targetGraphic = instance.GetComponent<Text>();
                }

                var text = instance.GetComponent<Text>();
                text.raycastTarget = true;
                _contactItems.Add(text);
            }

            return _contactItems[index];
        }

        private void SelectPrivateTarget(long targetId)
        {
            _selectedPrivateTargetId = targetId;
            if (_inputTarget != null)
            {
                _inputTarget.text = targetId.ToString();
            }

            RefreshView();
        }

        private List<ChatMessageTreeInfo> GetSelectedPrivateMessages()
        {
            var result = new List<ChatMessageTreeInfo>();
            var messages = _viewModel?.PrivateMessages ?? new List<ChatMessageTreeInfo>();
            foreach (var message in messages)
            {
                if (GetPrivatePeerId(message) == _selectedPrivateTargetId)
                {
                    result.Add(message);
                }
            }

            return result;
        }

        private List<PrivateTargetView> BuildPrivateTargets()
        {
            var result = new List<PrivateTargetView>();
            var messages = _viewModel?.PrivateMessages ?? new List<ChatMessageTreeInfo>();
            foreach (var message in messages)
            {
                var peerId = GetPrivatePeerId(message);
                if (peerId <= 0 || result.Any(item => item.PlayerId == peerId))
                {
                    continue;
                }

                var name = message.UnitId == peerId && !string.IsNullOrWhiteSpace(message.UserName)
                    ? message.UserName
                    : $"玩家{peerId}";
                result.Add(new PrivateTargetView(peerId, name));
            }

            return result;
        }

        private static long GetPrivatePeerId(ChatMessageTreeInfo message)
        {
            if (message == null)
            {
                return 0;
            }

            var selfId = SheepNetworkService.Instance.Profile?.PlayerId ?? 0;
            if (message.UnitId == selfId)
            {
                return message.Targets?.FirstOrDefault(item => item > 0) ?? 0;
            }

            return message.UnitId;
        }

        private void ApplyModeLayout(bool isPrivateMode)
        {
            if (_messageListRect != null)
            {
                _messageListRect.anchorMin = new Vector2(0f, 0f);
                _messageListRect.anchorMax = new Vector2(1f, 1f);
                _messageListRect.offsetMin = new Vector2(isPrivateMode ? 144f : 16f, 92f);
                _messageListRect.offsetMax = new Vector2(-16f, -112f);
            }
        }

        private static string FormatMessage(ChatMessageTreeInfo message)
        {
            var channel = message?.ChannelType == ChatConstants.ChannelPrivate ? "私聊" : ChannelLabel(message?.ChannelType ?? 0);
            var sender = string.IsNullOrWhiteSpace(message?.UserName) ? message?.UnitId.ToString() : message.UserName;
            var content = message?.Nodes == null ? string.Empty : string.Concat(message.Nodes.Select(node => node?.Content ?? string.Empty));
            return $"[{channel}] {sender}: {content}";
        }

        private static string ChannelLabel(int channelType)
        {
            return channelType switch
            {
                ChatConstants.ChannelGuild => "公会",
                ChatConstants.ChannelTeam => "组队",
                ChatConstants.ChannelRoom => "房间",
                _ => "世界"
            };
        }

        private static void SetActive(Component component, bool active)
        {
            if (component != null)
            {
                component.gameObject.SetActive(active);
            }
        }

        private readonly struct PrivateTargetView
        {
            public PrivateTargetView(long playerId, string displayName)
            {
                PlayerId = playerId;
                DisplayName = displayName;
            }

            public long PlayerId { get; }
            public string DisplayName { get; }
        }
    }
}
