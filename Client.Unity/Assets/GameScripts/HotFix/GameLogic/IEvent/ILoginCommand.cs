using TEngine;

namespace GameLogic
{
    [EventInterface(EEventGroup.GroupLogic)]
    public interface ILoginCommand
    {
        void OnLogin(string account, string password);

        void OnRegister(string account, string password, string nickname);

        void OnSubmitNickname(string nickname);
    }
}
