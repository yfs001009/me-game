using GameLogic.SheepBattle.Asset;
using GameLogic.SheepBattle.Reward;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    /// <summary>
    /// 背包物品列表项 Widget
    /// 显示物品品质框、图标、名称、数量
    /// </summary>
    internal sealed class BagItemWidget : UIWidget
    {
        private Image _imgFrame;
        private Image _imgIcon;
        private Text _txtName;
        private Text _txtCount;
        private Button _btnSelf;

        protected override void ScriptGenerator()
        {
            _imgFrame = gameObject.GetComponent<Image>();
            _imgIcon = FindChildComponent<Image>("m_imgIcon");
            _txtName = FindChildComponent<Text>("m_txtName");
            _txtCount = FindChildComponent<Text>("m_txtCount");
            _btnSelf = gameObject.GetComponent<Button>();
        }

        protected override void OnCreate()
        {
            // 初始化时隐藏
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 刷新物品项显示
        /// </summary>
        public void Refresh(BagItemEntryViewModel data, System.Action<BagItemEntryViewModel> onClickCallback)
        {
            if (data == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            gameObject.name = $"m_item_BagItem_{data.ItemId}";

            // 品质框
            if (_imgFrame != null)
            {
                SetQualityFrame(_imgFrame, data.Quality);
            }

            // 物品图标（暂时不显示，等有图标资源）
            if (_imgIcon != null)
            {
                _imgIcon.gameObject.SetActive(false);
            }

            // 物品名称
            if (_txtName != null)
            {
                _txtName.text = data.Name;
                _txtName.color = Color.white;
                _txtName.alignment = TextAnchor.MiddleCenter;
                _txtName.horizontalOverflow = HorizontalWrapMode.Wrap;
                _txtName.verticalOverflow = VerticalWrapMode.Truncate;
                _txtName.resizeTextForBestFit = true;
                _txtName.resizeTextMinSize = 12;
                _txtName.resizeTextMaxSize = 18;
            }

            // 数量
            if (_txtCount != null)
            {
                _txtCount.text = $"x{data.Count}";
                _txtCount.color = Color.white;
            }

            // 点击事件
            if (_btnSelf != null)
            {
                _btnSelf.onClick.RemoveAllListeners();
                _btnSelf.onClick.AddListener(() => onClickCallback?.Invoke(data));
            }
        }

        private static void SetQualityFrame(Image image, int quality)
        {
            var sprite = RewardDisplayService.GetQualityFrameSprite(quality);
            if (sprite != null)
            {
                image.sprite = sprite;
                image.type = Image.Type.Simple;
                image.color = Color.white;
                return;
            }

            // 默认颜色
            image.color = new Color(0.78f, 0.82f, 0.76f, 1f);
        }
    }
}
