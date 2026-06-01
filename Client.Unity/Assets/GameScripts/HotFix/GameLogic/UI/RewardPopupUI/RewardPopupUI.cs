using System.Collections.Generic;
using GameLogic.SheepBattle.Reward;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "RewardPopupUI")]
    internal sealed class RewardPopupUI : UIWindow
    {
        private Button _btnClose;
        private CanvasGroup _canvasGroup;
        private RectTransform _panel;
        private RectTransform _listRoot;
        private Button _itemTemplate;
        private Text _txtTitle;

        private readonly List<Button> _items = new();
        private RewardPopupData _data;
        private float _elapsed;

        protected override void ScriptGenerator()
        {
            _canvasGroup = FindChildComponent<CanvasGroup>("m_imgPanel");
            _panel = FindChild("m_imgPanel") as RectTransform;
            _btnClose = FindChildComponent<Button>("m_imgPanel/m_btnClose");
            _txtTitle = FindChildComponent<Text>("m_imgPanel/m_txtTitle");
            _listRoot = FindListContent("m_imgPanel/m_listRewards");
            _itemTemplate = FindListComponent<Button>("m_imgPanel/m_listRewards", "m_btnRewardTemplate");
        }

        protected override void OnCreate()
        {
            _btnClose?.onClick.AddListener(() => GameModule.UI.CloseUI<RewardPopupUI>());
            if (_itemTemplate != null)
            {
                _itemTemplate.gameObject.SetActive(false);
            }
        }

        protected override void OnRefresh()
        {
            _data = UserData as RewardPopupData;
            SetText(_txtTitle, _data?.Title ?? "获得奖励");
            RefreshList();
            _elapsed = 0f;
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
            }

            if (_panel != null)
            {
                _panel.localScale = Vector3.one * 0.92f;
            }
        }

        protected override void OnUpdate()
        {
            if (_canvasGroup == null && _panel == null)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            var t = Mathf.Clamp01(_elapsed / 0.18f);
            var eased = 1f - (1f - t) * (1f - t);
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = eased;
            }

            if (_panel != null)
            {
                _panel.localScale = Vector3.one * Mathf.Lerp(0.92f, 1f, eased);
            }
        }

        private void RefreshList()
        {
            var rewards = _data?.Items ?? new List<RewardPopupItemData>();
            for (var i = 0; i < rewards.Count; i++)
            {
                RefreshItem(i, rewards[i]);
            }

            for (var i = rewards.Count; i < _items.Count; i++)
            {
                _items[i].gameObject.SetActive(false);
            }
        }

        private void RefreshItem(int index, RewardPopupItemData reward)
        {
            var button = GetOrCreateItem(index);
            if (button == null)
            {
                return;
            }

            button.gameObject.SetActive(true);

            var frame = button.GetComponent<Image>();
            SetFrame(frame, reward.Quality);

            var label = button.transform.Find("m_txtLabel")?.GetComponent<Text>();
            SetText(label, $"{reward.Name}\nx{reward.Count}");

            var quality = button.transform.Find("m_txtQuality")?.GetComponent<Text>();
            SetText(quality, RewardDisplayService.GetQualityName(reward.Quality));
        }

        private Button GetOrCreateItem(int index)
        {
            if (_itemTemplate == null || _listRoot == null)
            {
                return null;
            }

            while (_items.Count <= index)
            {
                var instance = Object.Instantiate(_itemTemplate.gameObject, _listRoot, false);
                _items.Add(instance.GetComponent<Button>());
            }

            return _items[index];
        }

        private static void SetFrame(Image image, int quality)
        {
            if (image == null)
            {
                return;
            }

            var sprite = RewardDisplayService.GetQualityFrameSprite(quality);
            if (sprite != null)
            {
                image.sprite = sprite;
                image.type = Image.Type.Simple;
            }
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
