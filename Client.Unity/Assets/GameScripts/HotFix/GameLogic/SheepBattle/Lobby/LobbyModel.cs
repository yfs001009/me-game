using Fantasy;
using GameLogic.SheepBattle.Event;
using TEngine;

namespace GameLogic.SheepBattle.Lobby
{
    public sealed class LobbyModel
    {
        public LobbyViewModel LobbyView { get; private set; } = new LobbyViewModel();
        public RoomViewModel CurrentRoom { get; private set; }

        public LobbyViewModel UpdateLobby(G2C_LobbyHomeResponse response)
        {
            var firstRoom = response?.Rooms != null && response.Rooms.Count > 0 ? response.Rooms[0] : null;
            LobbyView = new LobbyViewModel
            {
                PlayerName = response?.Profile?.Nickname ?? string.Empty,
                PlayerId = response?.Profile?.PlayerId ?? 0,
                Level = response?.Profile?.Level ?? 0,
                RoomCount = response?.Rooms?.Count ?? 0,
                IsMatching = response?.MatchStatus?.IsMatching ?? false,
                MatchRoomId = response?.MatchStatus?.AllocatedRoomId ?? 0,
                MatchEstimatedSeconds = response?.MatchStatus?.EstimatedSeconds ?? 0,
                JoinableRoomId = firstRoom?.RoomId ?? 0,
                JoinableRoomName = firstRoom?.RoomName ?? string.Empty,
                JoinableRoomCurrentPlayers = firstRoom?.CurrentPlayers ?? 0,
                JoinableRoomMaxPlayers = firstRoom?.MaxPlayers ?? 0,
                RoomListText = BuildRoomListText(response)
            };

            if (response?.Rooms != null)
            {
                foreach (var room in response.Rooms)
                {
                    LobbyView.Rooms.Add(ToRoomSummaryView(room));
                }
            }

            GameEvent.Send(new LobbyViewChangedEvent(LobbyView));
            return LobbyView;
        }

        public RoomViewModel EnterJoinableRoom()
        {
            if (LobbyView.JoinableRoomId <= 0)
            {
                CurrentRoom = null;
                GameEvent.Send(new RoomViewChangedEvent(CurrentRoom));
                return CurrentRoom;
            }

            CurrentRoom = new RoomViewModel
            {
                RoomId = LobbyView.JoinableRoomId,
                RoomName = LobbyView.JoinableRoomName,
                Mode = "ClassicInfection",
                MapId = 1,
                CurrentPlayers = LobbyView.JoinableRoomCurrentPlayers,
                MaxPlayers = LobbyView.JoinableRoomMaxPlayers,
                State = "Waiting"
            };

            GameEvent.Send(new RoomViewChangedEvent(CurrentRoom));
            return CurrentRoom;
        }

        public RoomViewModel UpdateCurrentRoom(G2C_CreateRoomResponse response, string fallbackRoomName)
        {
            return UpdateCurrentRoom(response?.Room, fallbackRoomName, "Creating");
        }

        public void UpdateMatchStatus(G2C_StartMatchResponse response)
        {
            if (response?.Status == null)
            {
                SetStatus("状态：匹配请求失败");
                return;
            }

            LobbyView.IsMatching = response.Status.IsMatching;
            LobbyView.MatchRoomId = response.Status.AllocatedRoomId;
            LobbyView.MatchEstimatedSeconds = response.Status.EstimatedSeconds;
            SetStatus($"状态：匹配中，预计 {response.Status.EstimatedSeconds} 秒");
            GameEvent.Send(new LobbyViewChangedEvent(LobbyView));
        }

        public RoomViewModel UpdateCurrentRoom(G2C_JoinRoomResponse response)
        {
            return UpdateCurrentRoom(response?.Room, LobbyView.JoinableRoomName, "Waiting");
        }

        public RoomViewModel UpdateCurrentRoom(G2C_LeaveRoomResponse response)
        {
            return UpdateCurrentRoom(response?.Room, string.Empty, "Waiting");
        }

        public RoomViewModel UpdateCurrentRoom(G2C_RoomDetailResponse response)
        {
            return UpdateCurrentRoom(response?.Room, CurrentRoom?.RoomName ?? string.Empty, CurrentRoom?.State ?? "Waiting");
        }

        public RoomViewModel UpdateCurrentRoom(G2C_SetRoomReadyResponse response)
        {
            return UpdateCurrentRoom(response?.Room, CurrentRoom?.RoomName ?? string.Empty, CurrentRoom?.State ?? "Waiting");
        }

        public RoomViewModel UpdateCurrentRoom(G2C_StartRoomResponse response)
        {
            return UpdateCurrentRoom(response?.Room, CurrentRoom?.RoomName ?? string.Empty, "Playing");
        }

        private RoomViewModel UpdateCurrentRoom(RoomDetailInfo room, string fallbackRoomName, string fallbackState)
        {
            CurrentRoom = ToRoomView(room, fallbackRoomName, fallbackState);

            GameEvent.Send(new RoomViewChangedEvent(CurrentRoom));
            return CurrentRoom;
        }

        public void ClearCurrentRoom()
        {
            CurrentRoom = null;
            GameEvent.Send(new RoomViewChangedEvent(CurrentRoom));
        }

        public void SetStatus(string status)
        {
            GameEvent.Send(new LobbyStatusChangedEvent(status));
        }

        private static string BuildRoomListText(G2C_LobbyHomeResponse response)
        {
            if (response?.Rooms == null || response.Rooms.Count == 0)
            {
                return "可加入房间：暂无";
            }

            var room = response.Rooms[0];
            return $"可加入房间：{room.RoomName}  #{room.RoomId}  人数：{room.CurrentPlayers}/{room.MaxPlayers}";
        }

        private static RoomSummaryViewModel ToRoomSummaryView(RoomSummaryInfo room)
        {
            return new RoomSummaryViewModel
            {
                RoomId = room?.RoomId ?? 0,
                RoomName = room?.RoomName ?? string.Empty,
                Mode = room?.Mode ?? string.Empty,
                MapId = room?.MapId ?? 0,
                CurrentPlayers = room?.CurrentPlayers ?? 0,
                MaxPlayers = room?.MaxPlayers ?? 0,
                IsPrivate = room?.IsPrivate ?? false,
                State = room?.State ?? string.Empty
            };
        }

        private static RoomViewModel ToRoomView(RoomDetailInfo room, string fallbackRoomName, string fallbackState)
        {
            var view = new RoomViewModel
            {
                RoomId = room?.Summary?.RoomId ?? 0,
                RoomName = room?.Summary?.RoomName ?? fallbackRoomName,
                Mode = room?.Summary?.Mode ?? "ClassicInfection",
                MapId = room?.Summary?.MapId ?? 1,
                CurrentPlayers = room?.Summary?.CurrentPlayers ?? 0,
                MaxPlayers = room?.Summary?.MaxPlayers ?? 4,
                State = room?.Summary?.State ?? fallbackState
            };

            if (room?.Players != null)
            {
                foreach (var player in room.Players)
                {
                    view.Players.Add(new RoomPlayerViewModel
                    {
                        PlayerId = player.PlayerId,
                        Nickname = player.Nickname,
                        Level = player.Level,
                        IsOwner = player.IsOwner,
                        IsReady = player.IsReady
                    });
                }
            }

            return view;
        }
    }
}
