using Fantasy;
using Fantasy.Entitas;

namespace Fantasy;

/// <summary>
/// Gate Scene owned player account data. This is still memory-backed for MVP,
/// but it is now modeled as a Fantasy Entity so persistence/roaming can replace
/// the current process-local dictionaries without changing protocol handlers.
/// </summary>
public sealed class PlayerAccountEntity : Entity
{
    public long PlayerId { get; set; }
    public string Account { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public int Exp { get; set; }
    public int AvatarId { get; set; } = 1;
    public int RankScore { get; set; } = 1000;
}

public sealed class PlayerSessionComponent : Entity
{
    public string Token { get; set; } = string.Empty;
    public long PlayerId { get; set; }
    public DateTimeOffset LoginAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
