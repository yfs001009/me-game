using Fantasy;
using Fantasy.Entitas;

namespace Fantasy;

/// <summary>
/// Room Scene target model. During the MVP it is hosted by the Gate Scene and
/// indexed by CustomRoomService; later it can move behind Address/Roaming calls.
/// </summary>
public sealed class RoomEntity : Entity
{
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public int MapId { get; set; }
    public int MaxPlayers { get; set; }
    public bool IsPrivate { get; set; }
    public string Password { get; set; } = string.Empty;
    public string State { get; set; } = "Waiting";
    public long OwnerPlayerId { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public List<RoomPlayerInfo> Players { get; } = new();
    public BattleStartInfo? Battle { get; set; }
}
