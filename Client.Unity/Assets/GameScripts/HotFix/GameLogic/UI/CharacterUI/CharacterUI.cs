using System.Collections.Generic;
using GameLogic.SheepBattle.Character;
using GameLogic.SheepBattle.Event;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "CharacterUI")]
    internal sealed class CharacterUI : UIWindow
    {
        private const string HeroCategory = "Hero";
        private const string GhostCategory = "Ghost";

        private Button _btnHeroTab;
        private Button _btnGhostTab;
        private Button _btnClose;
        private RectTransform _listRoot;
        private Button _itemTemplate;
        private Text _txtName;
        private Text _txtCategory;
        private Text _txtAbility;
        private Text _txtDescription;
        private Button _btnSelect;

        private readonly List<Button> _items = new();
        private CharacterViewModel _viewModel;
        private CharacterEntryViewModel _current;
        private string _category = HeroCategory;

        protected override void ScriptGenerator()
        {
            _btnHeroTab = FindChildComponent<Button>("m_imgPanel/m_tabs/m_btnHeroTab");
            _btnGhostTab = FindChildComponent<Button>("m_imgPanel/m_tabs/m_btnGhostTab");
            _btnClose = FindChildComponent<Button>("m_imgPanel/m_btnClose");
            _listRoot = FindListContent("m_imgPanel/m_listCharacters");
            _itemTemplate = FindListComponent<Button>("m_imgPanel/m_listCharacters", "m_btnCharacterTemplate");
            _txtName = FindChildComponent<Text>("m_imgPanel/m_detailPanel/m_txtName");
            _txtCategory = FindChildComponent<Text>("m_imgPanel/m_detailPanel/m_txtCategory");
            _txtAbility = FindChildComponent<Text>("m_imgPanel/m_detailPanel/m_txtAbility");
            _txtDescription = FindChildComponent<Text>("m_imgPanel/m_detailPanel/m_txtDescription");
            _btnSelect = FindChildComponent<Button>("m_imgPanel/m_detailPanel/m_btnSelect");
        }

        protected override void RegisterEvent()
        {
            AddUIEvent<CharacterViewChangedEvent>(OnCharacterViewChanged);
        }

        protected override void OnCreate()
        {
            _itemTemplate.gameObject.SetActive(false);
            _btnHeroTab.onClick.AddListener(() => SwitchCategory(HeroCategory));
            _btnGhostTab.onClick.AddListener(() => SwitchCategory(GhostCategory));
            _btnClose.onClick.AddListener(() => GameModule.UI.CloseUI<CharacterUI>());
            _btnSelect.onClick.AddListener(OnClickSelect);
        }

        protected override void OnRefresh()
        {
            _viewModel = UserData as CharacterViewModel ?? GameLogic.SheepBattle.Character.CharacterController.Instance.GetCurrentView();
            RefreshList();
        }

        private void OnCharacterViewChanged(CharacterViewChangedEvent eventData)
        {
            _viewModel = eventData.ViewModel;
            RefreshList();
        }

        private void SwitchCategory(string category)
        {
            _category = category;
            RefreshList();
        }

        private void RefreshList()
        {
            var entries = _viewModel?.GetByCategory(_category) ?? new List<CharacterEntryViewModel>();
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

        private void RefreshItem(int index, CharacterEntryViewModel entry)
        {
            var button = GetOrCreateItem(index);
            button.gameObject.SetActive(true);
            button.name = $"m_btnCharacter_{entry.CharacterId}";
            var label = button.GetComponentInChildren<Text>();
            var state = entry.IsSelected ? "已选择" : entry.IsUnlocked ? "可选择" : "未解锁";
            label.text = $"{entry.Name}  {entry.Race}  {state}";
            label.color = entry.IsUnlocked ? Color.black : new Color(0.42f, 0.42f, 0.42f, 1f);
            button.interactable = entry.IsUnlocked;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                _current = entry;
                RefreshDetail();
            });
        }

        private Button GetOrCreateItem(int index)
        {
            while (_items.Count <= index)
            {
                var instance = Object.Instantiate(_itemTemplate.gameObject, _listRoot, false);
                _items.Add(instance.GetComponent<Button>());
            }

            return _items[index];
        }

        private void RefreshDetail()
        {
            SetText(_txtName, _current?.Name ?? "-");
            SetText(_txtCategory, _current == null ? string.Empty : $"{CategoryLabel(_current.Category)} / {_current.Race}");
            SetText(_txtAbility, _current == null ? string.Empty : $"{_current.AbilityName}\n{_current.AbilityDesc}");
            SetText(_txtDescription, _current?.Description ?? string.Empty);

            _btnSelect.interactable = _current?.IsUnlocked == true && _current.IsSelected == false;
            var label = _btnSelect.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.text = _current?.IsSelected == true ? "已选择" : _current?.IsUnlocked == true ? "选择" : "未解锁";
            }
        }

        private void OnClickSelect()
        {
            if (_current == null || !_current.IsUnlocked)
            {
                return;
            }

            GameLogic.SheepBattle.Character.CharacterController.Instance.SelectAsync(_current.CharacterId).Coroutine();
        }

        private static string CategoryLabel(string category)
        {
            return string.Equals(category, GhostCategory, System.StringComparison.OrdinalIgnoreCase) ? "幽灵" : "英雄";
        }

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
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
