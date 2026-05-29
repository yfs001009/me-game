using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Lobby;
using TEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.Tips, location: "MatchQueueUI")]
    internal sealed class MatchQueueUI : UIWindow
    {
        private Text _txtStatus;
        private Button _btnRefresh;
        private Button _btnClose;
        private LobbyViewModel _viewModel;

        protected override void ScriptGenerator()
        {
            _txtStatus = FindChildComponent<Text>("m_imgPanel/m_txtStatus");
            _btnRefresh = FindChildComponent<Button>("m_imgPanel/m_btnRefresh");
            _btnClose = FindChildComponent<Button>("m_imgPanel/m_btnClose");
        }

        protected override void RegisterEvent()
        {
            AddUIEvent<LobbyViewChangedEvent>(OnLobbyViewChanged);
        }

        protected override void OnCreate()
        {
            _btnRefresh.onClick.AddListener(OnClickRefresh);
            _btnClose.onClick.AddListener(() => GameModule.UI.CloseUI<MatchQueueUI>());
        }

        protected override void OnRefresh()
        {
            _viewModel = UserData as LobbyViewModel ?? LobbyController.Instance.GetCurrentLobbyView();
            ApplyView();
        }

        private void ApplyView()
        {
            if (_viewModel == null || !_viewModel.IsMatching)
            {
                _txtStatus.text = "尚未进入匹配。";
                return;
            }

            _txtStatus.text = _viewModel.MatchRoomId > 0
                ? $"已分配房间：#{_viewModel.MatchRoomId}"
                : $"匹配中...\n预计等待 {_viewModel.MatchEstimatedSeconds} 秒";
        }

        private void OnLobbyViewChanged(LobbyViewChangedEvent eventData)
        {
            _viewModel = eventData.ViewModel;
            ApplyView();
        }

        private void OnClickRefresh()
        {
            GameEvent.Get<ILobbyCommand>()?.OnRefreshLobby();
        }
    }
}
