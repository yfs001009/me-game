using Fantasy.Async;
using GameLogic.SheepBattle.Network;
using TEngine;

namespace GameLogic.SheepBattle.Lobby
{
    public sealed class LobbyController
    {
        public static LobbyController Instance { get; } = new LobbyController();
        public LobbyModel Model { get; } = new LobbyModel();

        private LobbyController()
        {
        }

        public async FTask<LobbyViewModel> RefreshLobbyAsync()
        {
            var response = await SheepNetworkService.Instance.RequestLobbyHomeAsync();
            return Model.UpdateLobby(response);
        }

        public async FTask<RoomViewModel> CreateRoomAsync(string roomName)
        {
            var response = await SheepNetworkService.Instance.CreateRoomAsync(roomName, "ClassicInfection", 1, 4, false, string.Empty);
            return Model.UpdateCurrentRoom(response, roomName);
        }

        public async FTask<RoomViewModel> CreateRoomAsync(string roomName, int mapId, int maxPlayers, bool isPrivate, string password)
        {
            var response = await SheepNetworkService.Instance.CreateRoomAsync(roomName, "ClassicInfection", mapId, maxPlayers, isPrivate, password);
            return Model.UpdateCurrentRoom(response, roomName);
        }

        public async FTask<RoomViewModel> JoinFirstRoomAsync()
        {
            if (Model.LobbyView.JoinableRoomId <= 0)
            {
                return Model.EnterJoinableRoom();
            }

            var response = await SheepNetworkService.Instance.JoinRoomAsync(Model.LobbyView.JoinableRoomId, string.Empty);
            if (!response.Success)
            {
                Model.SetStatus($"状态：{response.Message}");
                return null;
            }

            return Model.UpdateCurrentRoom(response);
        }

        public async FTask<RoomViewModel> JoinRoomAsync(int roomId)
        {
            if (roomId <= 0)
            {
                Model.SetStatus("状态：请选择要加入的房间");
                return null;
            }

            var response = await SheepNetworkService.Instance.JoinRoomAsync(roomId, string.Empty);
            if (!response.Success)
            {
                Model.SetStatus($"状态：{response.Message}");
                return null;
            }

            return Model.UpdateCurrentRoom(response);
        }

        public LobbyViewModel GetCurrentLobbyView()
        {
            return Model.LobbyView;
        }

        public async FTask<bool> LeaveCurrentRoomAsync()
        {
            var roomId = Model.CurrentRoom?.RoomId ?? 0;
            if (roomId > 0)
            {
                var response = await SheepNetworkService.Instance.LeaveRoomAsync(roomId);
                if (!response.Success)
                {
                    Model.SetStatus($"状态：{response.Message}");
                    return false;
                }
            }

            Model.ClearCurrentRoom();
            await RefreshLobbyAsync();
            return true;
        }

        public async FTask TryLeaveCurrentRoomOnShutdownAsync()
        {
            var roomId = Model.CurrentRoom?.RoomId ?? 0;
            if (roomId <= 0)
            {
                return;
            }

            try
            {
                await SheepNetworkService.Instance.LeaveRoomAsync(roomId);
            }
            catch (System.Exception exception)
            {
                Log.Warning($"释放时离开房间失败：{exception.Message}");
            }
            finally
            {
                Model.ClearCurrentRoom();
            }
        }

        public void SetStatus(string status)
        {
            Model.SetStatus(status);
        }
    }
}
