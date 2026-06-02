using System.Collections.Generic;
using System.Linq;
using GameLogic.SheepBattle.Asset;
using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Reward;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "BagUI")]
    internal sealed class BagUI : UIWindow
    {
        private Button _btnClose;
        private Button _btnRefresh;
        private RectTransform _listContainer;
        private RectTransform _listRoot;
        private Button _itemTemplate;
        private Text _txtEmpty;
        private RectTransform _detailContainer;
        private Text _txtDetailName;
        private Text _txtDetailDesc;
        private Button _btnUse;

        private readonly List<BagItemWidget> _items = new();
        private AssetViewModel _viewModel;
        private BagItemEntryViewModel _current;
        private const string BagItemAsset = "BagItemWidget";

        protected override void ScriptGenerator()
        {
            _btnClose = FindChildComponent<Button>("m_imgPanel/m_btnClose");
            _btnRefresh = FindChildComponent<Button>("m_imgPanel/m_btnRefresh");
            _listContainer = FindChild("m_imgPanel/m_listItems") as RectTransform;
            _listRoot = FindListContent("m_imgPanel/m_listItems");
            _itemTemplate = FindListComponent<Button>("m_imgPanel/m_listItems", "m_item_BagItemTemplate")
                            ?? FindListComponent<Button>("m_imgPanel/m_listItems", "m_btnItemTemplate");
            _txtEmpty = FindChildComponent<Text>("m_imgPanel/m_txtEmpty")
                        ?? FindChildComponent<Text>("m_imgPanel/m_listItems/m_txtEmpty");
            _detailContainer = FindChild("m_imgPanel/m_detailPanel") as RectTransform;
            _txtDetailName = FindChildComponent<Text>("m_imgPanel/m_detailPanel/m_txtName");
            _txtDetailDesc = FindChildComponent<Text>("m_imgPanel/m_detailPanel/m_txtDescription");
            _btnUse = FindChildComponent<Button>("m_imgPanel/m_detailPanel/m_btnUse");
        }

        protected override void RegisterEvent()
        {
            AddUIEvent<AssetViewChangedEvent>(OnAssetViewChanged);
        }

        protected override void OnCreate()
        {
            if (_itemTemplate != null)
            {
                _itemTemplate.gameObject.SetActive(false);
            }

            _btnClose?.onClick.AddListener(() => GameModule.UI.CloseUI<BagUI>());
            _btnRefresh?.onClick.AddListener(() => AssetController.Instance.RefreshAsync().Coroutine());
            _btnUse?.onClick.AddListener(OnClickUse);
        }

        protected override void OnRefresh()
        {
            _viewModel = UserData as AssetViewModel ?? AssetController.Instance.Model;
            RefreshList();
        }

        private void OnAssetViewChanged(AssetViewChangedEvent eventData)
        {
            _viewModel = eventData.ViewModel;
            RefreshList();
        }

        private void RefreshList()
        {
            var entries = _viewModel?.BagItems ?? new List<BagItemEntryViewModel>();
            var hasItems = entries.Count > 0;
            SetActive(_txtEmpty, !hasItems);
            SetActive(_listContainer, hasItems);
            SetActive(_detailContainer, hasItems);

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

        private void RefreshItem(int index, BagItemEntryViewModel entry)
        {
            var widget = GetOrCreateItem(index);
            if (widget == null)
            {
                return;
            }

            widget.Refresh(entry, OnBagItemClicked);
        }

        private void OnBagItemClicked(BagItemEntryViewModel entry)
        {
            _current = entry;
            RefreshDetail();
        }

        private BagItemWidget GetOrCreateItem(int index)
        {
            if (_itemTemplate == null || _listRoot == null)
            {
                return null;
            }

            while (_items.Count <= index)
            {
                var widget = CreateWidgetByPath<BagItemWidget>(_listRoot, BagItemAsset, false)
                             ?? CreateWidgetByPrefab<BagItemWidget>(_itemTemplate.gameObject, _listRoot, false);
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
            SetText(_txtDetailName, _current?.Name ?? string.Empty);
            if (_txtDetailName != null)
            {
                _txtDetailName.color = Color.black;
            }

            SetText(_txtDetailDesc, BuildDetailText());
            if (_btnUse != null)
            {
                _btnUse.interactable = _current?.CanUse == true;
            }
        }

        private string BuildDetailText()
        {
            var description = _current?.Description ?? string.Empty;
            if (_current != null)
            {
                var quality = $"品质：{RewardDisplayService.GetQualityName(_current.Quality)}";
                var identity = $"ID：{_current.ItemId}  类型：{_current.ItemType}  数量：{_current.Count}";
                description = string.IsNullOrWhiteSpace(description)
                    ? $"{identity}\n{quality}"
                    : $"{identity}\n{quality}\n{description}";
            }

            var buffs = _viewModel?.Buffs;
            if (buffs == null || buffs.Count == 0)
            {
                return description;
            }

            var buffText = string.Join("\n", buffs.Select(v => $"{v.BuffKey} 到期 {FormatUnixTime(v.ExpiresAtUnixSeconds)}"));
            return string.IsNullOrWhiteSpace(description) ? buffText : $"{description}\n\n当前增益：\n{buffText}";
        }

        private void OnClickUse()
        {
            if (_current == null || !_current.CanUse)
            {
                return;
            }

            AssetController.Instance.UseItemAsync(_current.ItemId).Coroutine();
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

        private static string FormatUnixTime(long unixSeconds)
        {
            if (unixSeconds <= 0)
            {
                return "-";
            }

            return System.DateTimeOffset.FromUnixTimeSeconds(unixSeconds).ToLocalTime().ToString("MM-dd HH:mm");
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
