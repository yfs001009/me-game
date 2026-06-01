using Fantasy;
using Fantasy.Entitas;

namespace Fantasy;

/// <summary>
/// Player-selected character state. Unlock ownership is in PlayerUnlockComponent;
/// this aggregate stores the active loadout choices.
/// </summary>
public sealed class PlayerCharacterEntity : Entity
{
    public long PlayerId { get; set; }
    public int SelectedHeroId { get; set; }
    public int SelectedGhostId { get; set; }
}

