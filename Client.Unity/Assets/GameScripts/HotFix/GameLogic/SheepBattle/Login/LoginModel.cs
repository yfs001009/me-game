using GameLogic.SheepBattle.Event;
using TEngine;

namespace GameLogic.SheepBattle.Login
{
    public sealed class LoginModel
    {
        public string LastAccount { get; private set; } = string.Empty;
        public bool IsBusy { get; private set; }
        public string Status { get; private set; } = string.Empty;

        public void BeginLogin(string account)
        {
            LastAccount = account?.Trim() ?? string.Empty;
            IsBusy = true;
            Status = "正在登录";
            GameEvent.Send(new LoginStatusChangedEvent(Status, IsBusy));
        }

        public void BeginRegister(string account)
        {
            LastAccount = account?.Trim() ?? string.Empty;
            IsBusy = true;
            Status = "正在注册";
            GameEvent.Send(new LoginStatusChangedEvent(Status, IsBusy));
        }

        public void SetIdle(string status)
        {
            IsBusy = false;
            Status = status ?? string.Empty;
            GameEvent.Send(new LoginStatusChangedEvent(Status, IsBusy));
        }
    }
}
