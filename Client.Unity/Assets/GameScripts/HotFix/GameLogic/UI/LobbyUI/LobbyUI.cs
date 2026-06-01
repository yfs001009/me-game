using GameLogic.SheepBattle.Asset;
using GameLogic.SheepBattle.Character;
using GameLogic.SheepBattle.Chat;
using GameLogic.SheepBattle.Common;
using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Lobby;
using GameLogic.SheepBattle.Mail;
using GameLogic.SheepBattle.Shop;
using GameLogic.SheepBattle.Social;
using GameLogic.SheepBattle.Task;
using System.Linq;
using TEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [Window(UILayer.UI, location: "LobbyUI")]
    internal sealed class LobbyUI : UIWindow
    {
        private Text _txtPlayer;
        private Text _txtLevel;
        private Text _txtGold;
        private Text _txtGem;
        private Text _txtCrystal;
        private Button _btnBattle;
        private Button _btnDungeon;
        private Button _btnCustom;
        private Button _btnBag;
        private Button _btnCard;
        private Button _btnHero;
        private Button _btnFriend;
        private Button _btnMail;
        private Button _btnShop;
        private Button _btnChat;
        private Text _txtLatestChat;

        protected override void ScriptGenerator()
        {
            _txtPlayer = FindChildComponent<Text>("m_topBar/m_playerPanel/m_txtPlayerName");
            _txtLevel = FindChildComponent<Text>("m_topBar/m_playerPanel/m_imgLevel/m_txtLevel");
            _txtGold = FindChildComponent<Text>("m_topBar/m_currencyGold/m_txtValue");
            _txtGem = FindChildComponent<Text>("m_topBar/m_currencyGem/m_txtValue");
            _txtCrystal = FindChildComponent<Text>("m_topBar/m_currencyCrystal/m_txtValue");
            _btnBattle = FindChildComponent<Button>("m_mainPanel/m_btnBattle")
                         ?? FindChildComponent<Button>("m_mainPanel/m_btnBag (3)");
            _btnDungeon = FindChildComponent<Button>("m_mainPanel/m_btnDungeon");
            _btnCustom = FindChildComponent<Button>("m_mainPanel/m_btnCustom");
            _btnBag = FindChildComponent<Button>("m_bottomBar/m_btnBag");
            _btnCard = FindChildComponent<Button>("m_bottomBar/m_btnCard");
            _btnHero = FindChildComponent<Button>("m_bottomBar/m_btnHero");
            _btnFriend = FindChildComponent<Button>("m_topBar/m_btnFriend");
            _btnMail = FindChildComponent<Button>("m_topBar/m_btnMail");
            _btnShop = FindChildComponent<Button>("m_sideMenu/m_btnShop");
            _btnChat = FindChildComponent<Button>("m_bottomBar/m_btnChat");
            _txtLatestChat = FindChildComponent<Text>("m_bottomBar/m_btnChat/m_txtLatestChat");
        }

        protected override void RegisterEvent()
        {
            _btnBattle?.onClick.AddListener(OnClickBattle);
            _btnDungeon?.onClick.AddListener(OnClickRoomList);
            _btnCustom?.onClick.AddListener(OnClickCreateRoom);
            _btnBag?.onClick.AddListener(OnClickBag);
            _btnCard?.onClick.AddListener(OnClickCard);
            _btnHero?.onClick.AddListener(OnClickHero);
            _btnFriend?.onClick.AddListener(OnClickFriend);
            _btnMail?.onClick.AddListener(OnClickMail);
            _btnShop?.onClick.AddListener(OnClickShop);
            _btnChat?.onClick.AddListener(OnClickChat);
            AddUIEvent<LobbyViewChangedEvent>(OnLobbyViewChanged);
            AddUIEvent<LobbyStatusChangedEvent>(OnLobbyStatusChanged);
            AddUIEvent<AssetViewChangedEvent>(OnAssetViewChanged);
            AddUIEvent<ChatViewChangedEvent>(OnChatViewChanged);
        }

        protected override void OnCreate()
        {
            ApplyView(LobbyController.Instance.GetCurrentLobbyView());
            AssetController.Instance.RefreshAsync().Coroutine();
            ChatController.Instance.RefreshCompositeAsync().Coroutine();
        }

        protected override void OnRefresh()
        {
            ApplyView(UserData as LobbyViewModel ?? LobbyController.Instance.GetCurrentLobbyView());
        }

        private void ApplyView(LobbyViewModel viewModel)
        {
            var playerName = string.IsNullOrWhiteSpace(viewModel?.PlayerName) ? "未登录" : viewModel.PlayerName;
            var level = viewModel?.Level > 0 ? viewModel.Level : 1;

            SetText(_txtPlayer, playerName);
            SetText(_txtLevel, level.ToString());
            ApplyAssetView(AssetController.Instance.Model);
        }

        private void ApplyAssetView(AssetViewModel viewModel)
        {
            // Currency is a pure numeric asset, so the lobby top bar reads it from AssetSnapshot instead of the bag.
            SetText(_txtGold, FormatAmount(viewModel.GetCurrencyAmount("Gold")));
            SetText(_txtGem, FormatAmount(viewModel.GetCurrencyAmount("Diamond")));
            SetText(_txtCrystal, FormatAmount(viewModel.GetCurrencyAmount("EventToken")));
        }

        private void OnLobbyViewChanged(LobbyViewChangedEvent eventData)
        {
            ApplyView(eventData.ViewModel);
        }

        private void OnLobbyStatusChanged(LobbyStatusChangedEvent eventData)
        {
            Log.Info(eventData.Status);
        }

        private void OnAssetViewChanged(AssetViewChangedEvent eventData)
        {
            ApplyAssetView(eventData.ViewModel);
        }

        private void OnChatViewChanged(ChatViewChangedEvent eventData)
        {
            RefreshLatestChat(eventData.ViewModel);
        }

        private void OnClickBattle()
        {
            GameEvent.Get<ILobbyCommand>()?.OnStartMatch();
        }

        private void OnClickRoomList()
        {
            GameEvent.Get<ILobbyCommand>()?.OnOpenRoomList();
        }

        private void OnClickCreateRoom()
        {
            GameModule.UI.ShowUIAsync<CreateRoomUI>();
        }

        private void OnClickBag()
        {
            OpenBagAsync().Coroutine();
        }

        private async Fantasy.Async.FTask OpenBagAsync()
        {
            var view = await AssetController.Instance.RefreshAsync();
            GameModule.UI.ShowUIAsync<BagUI>(view);
        }

        private void OnClickCard()
        {
            CommonNoticeService.Show("当前版本使用默认前 6 张建筑卡入局，LoadoutUI 后续接入这里。", "卡组");
        }

        private void OnClickHero()
        {
            OpenCharacterAsync().Coroutine();
        }

        private async Fantasy.Async.FTask OpenCharacterAsync()
        {
            var view = await CharacterController.Instance.RefreshAsync();
            GameModule.UI.ShowUIAsync<CharacterUI>(view);
        }

        private void OnClickFriend()
        {
            OpenSocialAsync().Coroutine();
        }

        private async Fantasy.Async.FTask OpenSocialAsync()
        {
            var view = await SocialController.Instance.RefreshAsync(SocialViewModel.FollowingMode);
            GameModule.UI.ShowUIAsync<SocialUI>(view);
        }

        private void OnClickMail()
        {
            OpenMailAsync().Coroutine();
        }

        private async Fantasy.Async.FTask OpenMailAsync()
        {
            var view = await MailController.Instance.RefreshAsync();
            GameModule.UI.ShowUIAsync<MailUI>(view);
        }

        private void OnClickShop()
        {
            OpenShopAsync().Coroutine();
        }

        private async Fantasy.Async.FTask OpenShopAsync()
        {
            var view = await ShopController.Instance.RefreshAsync();
            await TaskController.Instance.RefreshAsync();
            GameModule.UI.ShowUIAsync<ShopUI>(view);
        }

        private void OnClickChat()
        {
            OpenChatAsync().Coroutine();
        }

        private async Fantasy.Async.FTask OpenChatAsync()
        {
            var view = await ChatController.Instance.RefreshCompositeAsync();
            GameModule.UI.ShowUIAsync<ChatUI>(view);
        }

        private void RefreshLatestChat(ChatViewModel viewModel)
        {
            var latest = viewModel?.CompositeMessages?.LastOrDefault();
            if (latest == null)
            {
                SetText(_txtLatestChat, string.Empty);
                return;
            }

            var sender = string.IsNullOrWhiteSpace(latest.UserName) ? latest.UnitId.ToString() : latest.UserName;
            var content = latest.Nodes == null ? string.Empty : string.Concat(latest.Nodes.Select(node => node?.Content ?? string.Empty));
            SetText(_txtLatestChat, $"{sender}：{content}");
        }

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }

        private static string FormatAmount(long amount)
        {
            return amount.ToString("N0");
        }
    }
}
