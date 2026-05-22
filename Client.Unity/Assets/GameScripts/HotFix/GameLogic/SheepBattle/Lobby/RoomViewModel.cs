namespace GameLogic.SheepBattle.Lobby
{
    using System.Collections.Generic;

    public sealed class RoomViewModel
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public int MapId { get; set; }
        public int CurrentPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public string State { get; set; } = string.Empty;
        public List<RoomPlayerViewModel> Players { get; } = new();
    }

    public sealed class RoomPlayerViewModel
    {
        public long PlayerId { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public int Level { get; set; }
        public bool IsOwner { get; set; }
        public bool IsReady { get; set; }
    }
}
