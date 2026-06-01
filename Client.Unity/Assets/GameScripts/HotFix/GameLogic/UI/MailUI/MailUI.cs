using System.Collections.Generic;
using Fantasy;
using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Mail;
using GameLogic.SheepBattle.Reward;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "MailUI")]
    internal sealed class MailUI : UIWindow
    {
        private Button _btnClose;
        private Button _btnRefresh;
        private RectTransform _listRoot;
        private Button _mailTemplate;
        private Text _txtEmpty;
        private Text _txtTitle;
        private Text _txtContent;
        private Text _txtAttachment;
        private RectTransform _attachmentRoot;
        private Button _attachmentTemplate;
        private Button _btnRead;
        private Button _btnClaim;

        private readonly List<MailItemWidget> _items = new();
        private readonly List<Button> _attachmentItems = new();
        private MailViewModel _viewModel;
        private MailEntryViewModel _current;
        private const string MailItemAsset = "MailItemWidget";

        protected override void ScriptGenerator()
        {
            _btnClose = FindChildComponent<Button>("m_imgPanel/m_btnClose");
            _btnRefresh = FindChildComponent<Button>("m_imgPanel/m_btnRefresh");
            _listRoot = FindListContent("m_imgPanel/m_listMails");
            _mailTemplate = FindListComponent<Button>("m_imgPanel/m_listMails", "m_item_MailTemplate")
                            ?? FindListComponent<Button>("m_imgPanel/m_listMails", "m_btnMailTemplate");
            _txtEmpty = FindChildComponent<Text>("m_imgPanel/m_listMails/m_txtEmpty");
            _txtTitle = FindChildComponent<Text>("m_imgPanel/m_detailPanel/m_txtTitle");
            _txtContent = FindChildComponent<Text>("m_imgPanel/m_detailPanel/m_txtContent");
            _txtAttachment = FindChildComponent<Text>("m_imgPanel/m_detailPanel/m_txtAttachment");
            _attachmentRoot = FindListContent("m_imgPanel/m_detailPanel/m_listAttachments");
            _attachmentTemplate = FindListComponent<Button>("m_imgPanel/m_detailPanel/m_listAttachments", "m_btnAttachmentTemplate");
            _btnRead = FindChildComponent<Button>("m_imgPanel/m_detailPanel/m_btnRead");
            _btnClaim = FindChildComponent<Button>("m_imgPanel/m_detailPanel/m_btnClaim");
        }

        protected override void RegisterEvent()
        {
            AddUIEvent<MailViewChangedEvent>(OnMailViewChanged);
        }

        protected override void OnCreate()
        {
            if (_mailTemplate != null)
            {
                _mailTemplate.gameObject.SetActive(false);
            }

            if (_attachmentTemplate != null)
            {
                _attachmentTemplate.gameObject.SetActive(false);
            }

            _btnClose?.onClick.AddListener(() => GameModule.UI.CloseUI<MailUI>());
            _btnRefresh?.onClick.AddListener(() => MailController.Instance.RefreshAsync().Coroutine());
            _btnRead?.onClick.AddListener(OnClickRead);
            _btnClaim?.onClick.AddListener(OnClickClaim);
        }

        protected override void OnRefresh()
        {
            _viewModel = UserData as MailViewModel ?? MailController.Instance.Model;
            RefreshList();
        }

        private void OnMailViewChanged(MailViewChangedEvent eventData)
        {
            _viewModel = eventData.ViewModel;
            RefreshList();
        }

        private void RefreshList()
        {
            var entries = _viewModel?.Mails ?? new List<MailEntryViewModel>();
            SetActive(_txtEmpty, entries.Count == 0);
            for (var i = 0; i < entries.Count; i++)
            {
                RefreshItem(i, entries[i]);
            }

            for (var i = entries.Count; i < _items.Count; i++)
            {
                _items[i].gameObject.SetActive(false);
            }

            _current = entries.Count > 0 ? entries[0] : null;
            RefreshDetail();
        }

        private void RefreshItem(int index, MailEntryViewModel entry)
        {
            var widget = GetOrCreateItem(index);
            if (widget == null)
            {
                return;
            }

            widget.Refresh(entry, OnMailItemClicked);
        }

        private void OnMailItemClicked(MailEntryViewModel entry)
        {
            _current = entry;
            RefreshDetail();
        }

        private MailItemWidget GetOrCreateItem(int index)
        {
            if (_mailTemplate == null || _listRoot == null)
            {
                return null;
            }

            while (_items.Count <= index)
            {
                var widget = CreateWidgetByPath<MailItemWidget>(_listRoot, MailItemAsset, false)
                             ?? CreateWidgetByPrefab<MailItemWidget>(_mailTemplate.gameObject, _listRoot, false);
                if (widget == null)
                {
                    return null;
                }

                _items.Add(widget);
            }

            return _items[index];
        }

        private void RefreshDetail()
        {
            SetText(_txtTitle, _current?.Title ?? string.Empty);
            SetText(_txtContent, _current?.Content ?? string.Empty);
            SetText(_txtAttachment, _current?.HasAttachment == true ? "附件" : "无附件");
            RefreshAttachments(_current?.Attachment);
            if (_btnRead != null)
            {
                _btnRead.interactable = _current != null && !_current.IsRead;
            }

            if (_btnClaim != null)
            {
                _btnClaim.interactable = _current?.HasAttachment == true && !_current.IsAttachmentClaimed;
            }
        }

        private void RefreshAttachments(RewardInfo reward)
        {
            var rewards = RewardDisplayService.FromReward("附件", reward).Items;
            for (var i = 0; i < rewards.Count; i++)
            {
                RefreshAttachmentItem(i, rewards[i]);
            }

            for (var i = rewards.Count; i < _attachmentItems.Count; i++)
            {
                _attachmentItems[i].gameObject.SetActive(false);
            }
        }

        private void RefreshAttachmentItem(int index, RewardPopupItemData reward)
        {
            var button = GetOrCreateAttachmentItem(index);
            if (button == null)
            {
                return;
            }

            button.gameObject.SetActive(true);
            var image = button.GetComponent<Image>();
            if (image != null)
            {
                var sprite = RewardDisplayService.GetQualityFrameSprite(reward.Quality);
                if (sprite != null)
                {
                    image.sprite = sprite;
                    image.type = Image.Type.Simple;
                    image.color = Color.white;
                }
            }

            var label = button.GetComponentInChildren<Text>();
            SetText(label, $"{reward.Name}\nx{reward.Count}");
        }

        private Button GetOrCreateAttachmentItem(int index)
        {
            if (_attachmentTemplate == null || _attachmentRoot == null)
            {
                return null;
            }

            while (_attachmentItems.Count <= index)
            {
                var instance = Object.Instantiate(_attachmentTemplate.gameObject, _attachmentRoot, false);
                _attachmentItems.Add(instance.GetComponent<Button>());
            }

            return _attachmentItems[index];
        }

        private void OnClickRead()
        {
            if (_current != null)
            {
                MailController.Instance.ReadAsync(_current.MailId).Coroutine();
            }
        }

        private void OnClickClaim()
        {
            if (_current != null)
            {
                MailController.Instance.ClaimAttachmentAsync(_current.MailId).Coroutine();
            }
        }

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }

        private static void SetActive(Component component, bool active)
        {
            if (component != null)
            {
                component.gameObject.SetActive(active);
            }
        }

        private RectTransform FindListContent(string listPath)
        {
            return FindChild($"{listPath}/Viewport/Content") as RectTransform
                   ?? FindChild(listPath) as RectTransform;
        }

        private T FindListComponent<T>(string listPath, string name) where T : Component
        {
            return FindChildComponent<T>($"{listPath}/Viewport/Content/{name}")
                   ?? FindChildComponent<T>($"{listPath}/{name}");
        }
    }
}
