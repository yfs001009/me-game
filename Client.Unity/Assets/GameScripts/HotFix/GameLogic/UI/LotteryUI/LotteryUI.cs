using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Asset;
using GameLogic.SheepBattle.Lottery;
using TEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "LotteryUI")]
    internal sealed class LotteryUI : UIWindow
    {
        private Button _btnClose;
        private Button _btnNormalOnce;
        private Button _btnNormalTen;
        private Button _btnPremiumOnce;
        private Button _btnPremiumTen;
        private Text _txtResult;

        protected override void ScriptGenerator()
        {
            _btnClose = FindChildComponent<Button>("m_imgPanel/m_btnClose");
            _btnNormalOnce = FindChildComponent<Button>("m_imgPanel/m_btnNormalOnce");
            _btnNormalTen = FindChildComponent<Button>("m_imgPanel/m_btnNormalTen");
            _btnPremiumOnce = FindChildComponent<Button>("m_imgPanel/m_btnPremiumOnce");
            _btnPremiumTen = FindChildComponent<Button>("m_imgPanel/m_btnPremiumTen");
            _txtResult = FindChildComponent<Text>("m_imgPanel/m_txtResult");
        }

        protected override void RegisterEvent()
        {
            AddUIEvent<LotteryDrawCompletedEvent>(OnLotteryDrawCompleted);
        }

        protected override void OnCreate()
        {
            _btnClose?.onClick.AddListener(() => GameModule.UI.CloseUI<LotteryUI>());
            _btnNormalOnce?.onClick.AddListener(() => LotteryController.Instance.DrawAsync("Normal", 1).Coroutine());
            _btnNormalTen?.onClick.AddListener(() => LotteryController.Instance.DrawAsync("Normal", 10).Coroutine());
            _btnPremiumOnce?.onClick.AddListener(() => LotteryController.Instance.DrawAsync("Premium", 1).Coroutine());
            _btnPremiumTen?.onClick.AddListener(() => LotteryController.Instance.DrawAsync("Premium", 10).Coroutine());
        }

        private void OnLotteryDrawCompleted(LotteryDrawCompletedEvent eventData)
        {
            if (_txtResult == null || eventData.Response == null)
            {
                return;
            }

            var pool = eventData.Response.Results.Count > 0 ? eventData.Response.Results[0].Pool : "Normal";
            var drawCount = AssetController.Instance.Model.GetProgressValue($"Lottery.{pool}.DrawCount");
            var pity = AssetController.Instance.Model.GetProgressValue($"Lottery.{pool}.Pity");
            _txtResult.text = $"抽奖完成：{eventData.Response.Results.Count} 次\n累计：{drawCount}\n保底计数：{pity}";
        }
    }
}
