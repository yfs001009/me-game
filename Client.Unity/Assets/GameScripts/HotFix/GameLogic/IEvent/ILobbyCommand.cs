using TEngine;

namespace GameLogic
{
    [EventInterface(EEventGroup.GroupLogic)]
    public interface ILobbyCommand
    {
        void OnRefreshLobby();

        void OnStartMatch();

        void OnOpenRoomList();

        void OnCreateRoom(string roomName, int mapId, int maxPlayers, bool isPrivate, string password);

        void OnJoinRoom(int roomId, string password);

        void OnRoomPrimaryAction();

        void OnLeaveRoom();

        void OnRefreshCurrentRoom();

        void OnTryEnterPendingBattle();
    }
}
