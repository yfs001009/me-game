using Fantasy;
using Fantasy.Async;
using GameLogic.SheepBattle.Battle;
using GameLogic.SheepBattle.Common;
using GameLogic.SheepBattle.Network;
using TEngine;
using Log = TEngine.Log;

namespace GameLogic.SheepBattle.Lobby
{
    public sealed class LobbyController : ILobbyCommand
    {
        public static LobbyController Instance { get; } = new LobbyController();
        public LobbyModel Model { get; } = new LobbyModel();
        public BattleStartInfo PendingBattle { get; private set; }

        private LobbyController()
        {
        }

        public void OnRefreshLobby()
        {
            RefreshLobbyByCommandAsync().Coroutine();
        }

        public void OnStartMatch()
        {
            StartMatchByCommandAsync().Coroutine();
        }

        public void OnOpenRoomList()
        {
            OpenRoomListByCommandAsync().Coroutine();
        }

        public void OnCreateRoom(string roomName, int mapId, int maxPlayers, bool isPrivate, string password)
        {
            CreateRoomByCommandAsync(roomName, mapId, maxPlayers, isPrivate, password).Coroutine();
        }

        public void OnJoinRoom(int roomId, string password)
        {
            JoinRoomByCommandAsync(roomId, password).Coroutine();
        }

        public void OnRoomPrimaryAction()
        {
            RoomPrimaryActionByCommandAsync().Coroutine();
        }

        public void OnLeaveRoom()
        {
            LeaveRoomByCommandAsync().Coroutine();
        }

        public void OnRefreshCurrentRoom()
        {
            RefreshCurrentRoomByCommandAsync().Coroutine();
        }

        public void OnTryEnterPendingBattle()
        {
            TryEnterPendingBattle();
        }

        private async FTask RefreshLobbyByCommandAsync()
        {
            SetStatus("状态：刷新大厅中...");
            await RefreshLobbyAsync();
            SetStatus("状态：大厅刷新完成");
        }

        private async FTask StartMatchByCommandAsync()
        {
            SetStatus("状态：正在进入匹配...");
            var lobby = await StartMatchAsync();
            GameModule.UI.ShowUIAsync<MatchQueueUI>(lobby);
        }

        private async FTask OpenRoomListByCommandAsync()
        {
            SetStatus("状态：刷新房间列表中...");
            var lobby = await RefreshLobbyAsync();
            GameModule.UI.ShowUIAsync<RoomListUI>(lobby);
        }

        private async FTask CreateRoomByCommandAsync(string roomName, int mapId, int maxPlayers, bool isPrivate, string password)
        {
            var room = await CreateRoomAsync(roomName, mapId, maxPlayers, isPrivate, password);
            GameModule.UI.CloseUI<CreateRoomUI>();
            GameModule.UI.CloseUI<LobbyUI>();
            GameModule.UI.ShowUIAsync<RoomUI>(room);
        }

        private async FTask JoinRoomByCommandAsync(int roomId, string password)
        {
            var room = await JoinRoomAsync(roomId, password);
            if (room == null || room.RoomId <= 0)
            {
                CommonNoticeService.Show("加入房间失败，请刷新后重试。");
                return;
            }

            GameModule.UI.CloseUI<RoomPasswordUI>();
            GameModule.UI.CloseUI<RoomListUI>();
            GameModule.UI.CloseUI<LobbyUI>();
            GameModule.UI.ShowUIAsync<RoomUI>(room);
        }

        private async FTask RoomPrimaryActionByCommandAsync()
        {
            if (IsLocalOwner(Model.CurrentRoom))
            {
                var response = await StartCurrentRoomAsync();
                if (response == null || !response.Success)
                {
                    return;
                }

                GameModule.UI.CloseUI<RoomUI>();
                EnterPendingBattle(response.Battle ?? ConsumePendingBattle());
                return;
            }

            var localPlayer = GetLocalPlayer(Model.CurrentRoom);
            await SetReadyAsync(!(localPlayer?.IsReady ?? false));
        }

        private async FTask LeaveRoomByCommandAsync()
        {
            try
            {
                if (!await LeaveCurrentRoomAsync())
                {
                    return;
                }

                GameModule.UI.CloseUI<RoomUI>();
                GameModule.UI.ShowUIAsync<LobbyUI>(GetCurrentLobbyView());
            }
            catch (System.Exception exception)
            {
                SetStatus($"状态：离开房间失败：{exception.Message}");
                Log.Error($"离开房间失败：{exception}");
            }
        }

        private async FTask RefreshCurrentRoomByCommandAsync()
        {
            await RefreshCurrentRoomAsync();
            TryEnterPendingBattle();
        }

        private void TryEnterPendingBattle()
        {
            var battle = PendingBattle;
            if (battle == null || battle.BattleId <= 0)
            {
                return;
            }

            GameModule.UI.CloseUI<RoomUI>();
            EnterPendingBattle(ConsumePendingBattle());
        }

        private static void EnterPendingBattle(BattleStartInfo battle)
        {
            if (battle == null || battle.BattleId <= 0)
            {
                return;
            }

            BattleController.Instance.EnterBattle(battle);
        }

        public async FTask<LobbyViewModel> RefreshLobbyAsync()
        {
            var response = await SheepNetworkService.Instance.RequestLobbyHomeAsync();
            return Model.UpdateLobby(response);
        }

        public async FTask<LobbyViewModel> StartMatchAsync()
        {
            var response = await SheepNetworkService.Instance.StartMatchAsync("ClassicInfection");
            Model.UpdateMatchStatus(response);
            return await RefreshLobbyAsync();
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
            return await JoinRoomAsync(roomId, string.Empty);
        }

        public async FTask<RoomViewModel> JoinRoomAsync(int roomId, string password)
        {
            if (roomId <= 0)
            {
                Model.SetStatus("状态：请选择要加入的房间");
                return null;
            }

            var response = await SheepNetworkService.Instance.JoinRoomAsync(roomId, password ?? string.Empty);
            if (!response.Success)
            {
                Model.SetStatus($"状态：{response.Message}");
                return null;
            }

            return Model.UpdateCurrentRoom(response);
        }

        public async FTask<RoomViewModel> RefreshCurrentRoomAsync()
        {
            var roomId = Model.CurrentRoom?.RoomId ?? 0;
            if (roomId <= 0)
            {
                return null;
            }

            var response = await SheepNetworkService.Instance.RequestRoomDetailAsync(roomId);
            if (!response.Success)
            {
                Model.SetStatus($"状态：{response.Message}");
                return null;
            }

            PendingBattle = response.Battle;
            return Model.UpdateCurrentRoom(response);
        }

        public async FTask<RoomViewModel> SetReadyAsync(bool isReady)
        {
            var roomId = Model.CurrentRoom?.RoomId ?? 0;
            if (roomId <= 0)
            {
                Model.SetStatus("状态：尚未进入房间");
                return null;
            }

            var response = await SheepNetworkService.Instance.SetRoomReadyAsync(roomId, isReady);
            if (!response.Success)
            {
                Model.SetStatus($"状态：{response.Message}");
                return null;
            }

            return Model.UpdateCurrentRoom(response);
        }

        public async FTask<G2C_StartRoomResponse> StartCurrentRoomAsync()
        {
            var roomId = Model.CurrentRoom?.RoomId ?? 0;
            if (roomId <= 0)
            {
                Model.SetStatus("状态：尚未进入房间");
                return null;
            }

            var response = await SheepNetworkService.Instance.StartRoomAsync(roomId);
            if (!response.Success)
            {
                Model.SetStatus($"状态：{response.Message}");
                Model.UpdateCurrentRoom(response);
                return response;
            }

            PendingBattle = response.Battle;
            Model.UpdateCurrentRoom(response);
            return response;
        }

        public BattleStartInfo ConsumePendingBattle()
        {
            var battle = PendingBattle;
            PendingBattle = null;
            return battle;
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

        private static bool IsLocalOwner(RoomViewModel viewModel)
        {
            return GetLocalPlayer(viewModel)?.IsOwner ?? false;
        }

        private static RoomPlayerViewModel GetLocalPlayer(RoomViewModel viewModel)
        {
            var playerId = SheepNetworkService.Instance.Profile?.PlayerId ?? 0;
            if (playerId <= 0 || viewModel?.Players == null)
            {
                return null;
            }

            for (var i = 0; i < viewModel.Players.Count; i++)
            {
                var player = viewModel.Players[i];
                if (player != null && player.PlayerId == playerId)
                {
                    return player;
                }
            }

            return null;
        }
    }
}
