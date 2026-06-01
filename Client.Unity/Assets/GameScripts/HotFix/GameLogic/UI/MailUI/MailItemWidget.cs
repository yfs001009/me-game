using GameLogic.SheepBattle.Mail;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    /// <summary>
    /// 邮件列表项 Widget
    /// 显示邮件标题、已读状态、附件图标
    /// </summary>
    internal sealed class MailItemWidget : UIWidget
    {
        private Text _txtTitle;
        private Text _txtState;
        private Image _imgAttachment;
        private Button _btnSelf;

        protected override void ScriptGenerator()
        {
            _txtTitle = FindChildComponent<Text>("m_txtTitle");
            _txtState = FindChildComponent<Text>("m_txtState");
            _imgAttachment = FindChildComponent<Image>("m_imgAttachment");
            _btnSelf = gameObject.GetComponent<Button>();
        }

        protected override void OnCreate()
        {
            // 初始化时隐藏
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 刷新邮件项显示
        /// </summary>
        public void Refresh(MailEntryViewModel data, System.Action<MailEntryViewModel> onClickCallback)
        {
            if (data == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            gameObject.name = $"m_item_Mail_{data.MailId}";

            // 标题
            if (_txtTitle != null)
            {
                _txtTitle.text = data.Title;
            }

            // 已读状态
            if (_txtState != null)
            {
                _txtState.text = data.IsRead ? "已读" : "未读";
                _txtState.color = data.IsRead
                    ? new Color(0.5f, 0.5f, 0.5f, 1f)  // 灰色
                    : new Color(0.2f, 0.6f, 0.3f, 1f); // 绿色
            }

            // 附件图标
            if (_imgAttachment != null)
            {
                _imgAttachment.gameObject.SetActive(data.HasAttachment);
            }

            // 点击事件
            if (_btnSelf != null)
            {
                _btnSelf.onClick.RemoveAllListeners();
                _btnSelf.onClick.AddListener(() => onClickCallback?.Invoke(data));
            }
        }
    }
}
