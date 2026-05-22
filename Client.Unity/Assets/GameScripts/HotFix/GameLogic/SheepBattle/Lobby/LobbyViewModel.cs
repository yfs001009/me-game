namespace GameLogic.SheepBattle.Lobby
{
    using System.Collections.Generic;

    public sealed class LobbyViewModel
    {
        public string PlayerName { get; set; } = string.Empty;
        public long PlayerId { get; set; }
        public int Level { get; set; }
        public int RoomCount { get; set; }
        public bool IsMatching { get; set; }
        public int MatchRoomId { get; set; }
        public int JoinableRoomId { get; set; }
        public string JoinableRoomName { get; set; } = string.Empty;
        public int JoinableRoomCurrentPlayers { get; set; }
        public int JoinableRoomMaxPlayers { get; set; }
        public string RoomListText { get; set; } = string.Empty;
        public List<RoomSummaryViewModel> Rooms { get; } = new();
    }

    public sealed class RoomSummaryViewModel
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public int MapId { get; set; }
        public int CurrentPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public bool IsPrivate { get; set; }
        public string State { get; set; } = string.Empty;
    }
}
