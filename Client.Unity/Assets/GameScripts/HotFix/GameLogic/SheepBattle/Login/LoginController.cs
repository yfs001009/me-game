using Fantasy.Async;
using GameLogic.SheepBattle.Lobby;
using GameLogic.SheepBattle.Common;
using GameLogic.SheepBattle.Network;
using TEngine;

namespace GameLogic.SheepBattle.Login
{
    public sealed class LoginController : ILoginCommand
    {
        public static LoginController Instance { get; } = new LoginController();
        public LoginModel Model { get; } = new LoginModel();

        private LoginController()
        {
        }

        public void OnLogin(string account, string password)
        {
            LoginAsync(account, password).Coroutine();
        }

        public void OnRegister(string account, string password, string nickname)
        {
            RegisterAsync(account, password, nickname).Coroutine();
        }

        public void OnSubmitNickname(string nickname)
        {
            SubmitNickname(nickname);
        }

        public FTask Login(string account, string password)
        {
            return LoginAsync(account, password);
        }

        public FTask Register(string account, string password, string nickname)
        {
            return RegisterAsync(account, password, nickname);
        }

        private async FTask LoginAsync(string account, string password)
        {
            try
            {
                if (!ValidateCredentials(account, password))
                {
                    return;
                }

                Model.BeginLogin(account);
                Log.Info($"开始登录：账号={account}");
                var loginResponse = await SheepNetworkService.Instance.LoginAsync(account, password);
                if (!loginResponse.Success)
                {
                    Model.SetIdle("登录失败");
                    CommonNoticeService.Show(loginResponse.Message, "登录失败");
                    return;
                }

                Model.SetIdle("登录成功");

                GameModule.UI.CloseUI<LoginUI>();
                if (string.IsNullOrWhiteSpace(loginResponse.Profile?.Nickname))
                {
                    Log.Info("登录成功但昵称为空，打开取名面板。");
                    GameModule.UI.ShowUIAsync<NicknameUI>();
                    return;
                }

                await EnterLobbyAsync();
            }
            catch (System.Exception exception)
            {
                Model.SetIdle("登录失败");
                Log.Error($"登录失败：{exception}");
                CommonNoticeService.Show(exception.Message, "登录失败");
            }
        }

        private async FTask RegisterAsync(string account, string password, string nickname)
        {
            try
            {
                if (!ValidateCredentials(account, password))
                {
                    return;
                }

                Model.BeginRegister(account);
                Log.Info($"开始注册：账号={account}，昵称={nickname}");
                var response = await SheepNetworkService.Instance.RegisterAsync(account, password, string.Empty);
                Model.SetIdle(response.Success ? "注册完成" : "注册失败");
                CommonNoticeService.Show(response.Message);
            }
            catch (System.Exception exception)
            {
                Model.SetIdle("注册失败");
                Log.Error($"注册失败：{exception}");
                CommonNoticeService.Show(exception.Message, "注册失败");
            }
        }

        public async FTask<bool> CompleteNicknameAsync(string nickname)
        {
            try
            {
                var response = await SheepNetworkService.Instance.SetNicknameAsync(nickname);
                if (!response.Success)
                {
                    CommonNoticeService.Show(response.Message);
                    return false;
                }

                await EnterLobbyAsync();
                GameModule.UI.CloseUI<NicknameUI>();
                return true;
            }
            catch (System.Exception exception)
            {
                Log.Error($"设置昵称失败：{exception}");
                CommonNoticeService.Show(exception.Message, "设置昵称失败");
                return false;
            }
        }

        public void SubmitNickname(string nickname)
        {
            CompleteNicknameAsync(nickname).Coroutine();
        }

        private static async FTask EnterLobbyAsync()
        {
            var lobbyView = await LobbyController.Instance.RefreshLobbyAsync();
            Log.Info($"进入大厅：玩家={lobbyView.PlayerName}，大厅房间数={lobbyView.RoomCount}");
            GameModule.UI.ShowUIAsync<LobbyUI>(lobbyView);
        }

        private static bool ValidateCredentials(string account, string password)
        {
            if (string.IsNullOrWhiteSpace(account) || account.Trim().Length < 4)
            {
                Log.Warning("账号长度不足：账号至少需要4位。");
                return false;
            }

            if (string.IsNullOrEmpty(password) || password.Length < 6)
            {
                Log.Warning("密码长度不足：密码至少需要6位。");
                return false;
            }

            return true;
        }
    }
}
