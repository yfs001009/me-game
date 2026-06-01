using System.Collections.Generic;
using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Social;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "SocialUI")]
    internal sealed class SocialUI : UIWindow
    {
        private Button _btnClose;
        private Button _btnFollowing;
        private Button _btnFans;
        private Button _btnSearch;
        private InputField _inputSearch;
        private RectTransform _listRoot;
        private Button _playerTemplate;
        private Text _txtEmpty;
        private Text _txtTitle;

        private readonly List<Button> _items = new();
        private SocialViewModel _viewModel;

        protected override void ScriptGenerator()
        {
            _btnClose = FindChildComponent<Button>("m_imgPanel/m_btnClose");
            _btnFollowing = FindChildComponent<Button>("m_imgPanel/m_tabs/m_btnFollowing");
            _btnFans = FindChildComponent<Button>("m_imgPanel/m_tabs/m_btnFans");
            _btnSearch = FindChildComponent<Button>("m_imgPanel/m_search/m_btnSearch");
            _inputSearch = FindChildComponent<InputField>("m_imgPanel/m_search/m_inputSearch");
            _listRoot = FindListContent("m_imgPanel/m_listPlayers");
            _playerTemplate = FindListComponent<Button>("m_imgPanel/m_listPlayers", "m_btnPlayerTemplate");
            _txtEmpty = FindChildComponent<Text>("m_imgPanel/m_listPlayers/m_txtEmpty");
            _txtTitle = FindChildComponent<Text>("m_imgPanel/m_txtTitle");
        }

        protected override void RegisterEvent()
        {
            AddUIEvent<SocialViewChangedEvent>(OnSocialViewChanged);
        }

        protected override void OnCreate()
        {
            _playerTemplate?.gameObject.SetActive(false);
            _btnClose?.onClick.AddListener(() => GameModule.UI.CloseUI<SocialUI>());
            _btnFollowing?.onClick.AddListener(() => SocialController.Instance.RefreshAsync(SocialViewModel.FollowingMode).Coroutine());
            _btnFans?.onClick.AddListener(() => SocialController.Instance.RefreshAsync(SocialViewModel.FansMode).Coroutine());
            _btnSearch?.onClick.AddListener(OnClickSearch);
        }

        protected override void OnRefresh()
        {
            _viewModel = UserData as SocialViewModel ?? SocialController.Instance.Model;
            RefreshList();
        }

        private void OnSocialViewChanged(SocialViewChangedEvent eventData)
        {
            _viewModel = eventData.ViewModel;
            RefreshList();
        }

        private void RefreshList()
        {
            var entries = _viewModel?.Players ?? new List<SocialPlayerEntryViewModel>();
            SetText(_txtTitle, $"关注 {_viewModel?.FollowingCount ?? 0}  粉丝 {_viewModel?.FollowerCount ?? 0}");
            SetActive(_txtEmpty, entries.Count == 0);
            for (var i = 0; i < entries.Count; i++)
            {
                RefreshItem(i, entries[i]);
            }

            for (var i = entries.Count; i < _items.Count; i++)
            {
                _items[i].gameObject.SetActive(false);
            }
        }

        private void RefreshItem(int index, SocialPlayerEntryViewModel entry)
        {
            var button = GetOrCreateItem(index);
            if (button == null)
            {
                return;
            }

            button.gameObject.SetActive(true);
            button.name = $"m_btnPlayer_{entry.PlayerId}";
            var label = button.GetComponentInChildren<Text>();
            if (label != null)
            {
                var relation = entry.IsFollowing ? "已关注" : entry.IsFollower ? "粉丝" : "未关注";
                label.text = $"{entry.Nickname}  Lv.{entry.Level}  {relation}";
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                SocialController.Instance.SetFollowAsync(
                    entry.PlayerId,
                    !entry.IsFollowing,
                    _viewModel?.ViewMode ?? SocialViewModel.FollowingMode,
                    _inputSearch?.text ?? string.Empty).Coroutine();
            });
        }

        private Button GetOrCreateItem(int index)
        {
            if (_playerTemplate == null || _listRoot == null)
            {
                return null;
            }

            while (_items.Count <= index)
            {
                var instance = Object.Instantiate(_playerTemplate.gameObject, _listRoot, false);
                _items.Add(instance.GetComponent<Button>());
            }

            return _items[index];
        }

        private void OnClickSearch()
        {
            SocialController.Instance.RefreshAsync(SocialViewModel.SearchMode, _inputSearch?.text ?? string.Empty).Coroutine();
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
