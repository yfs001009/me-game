using Fantasy;
using Fantasy.Entitas;

namespace Fantasy;

/// <summary>
/// Battle Scene target model. It is still advanced by RPC requests in this MVP,
/// but the state now has an Entity boundary for the later fixed TimerComponent tick.
/// </summary>
public sealed class BattleEntity : Entity
{
    public int BattleId { get; set; }
    public int RoomId { get; set; }
    public int MapId { get; set; }
    public string MapAsset { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public string State { get; set; } = "Loading";
    public long Tick { get; set; }
    public DateTimeOffset StartedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastTickAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset RunningStartedAtUtc { get; set; }
    public bool TrollSelected { get; set; }
    public List<BattlePlayerEntity> Players { get; } = new();
    public List<BattleBuildingEntity> Buildings { get; } = new();
    public List<BattleAttackEventRecord> AttackEvents { get; } = new();
    public List<PendingTowerHitRecord> PendingTowerHits { get; } = new();
}

public sealed class BattlePlayerEntity : Entity
{
    public long PlayerId { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string Camp { get; set; } = "Elf";
    public bool SceneLoaded { get; set; }
    public int Gold { get; set; } = 300;
    public int Wood { get; set; } = 180;
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float MoveSpeed { get; set; } = 4f;
    public int Hp { get; set; } = 100;
    public int MaxHp { get; set; } = 100;
    public int Attack { get; set; } = 20;
    public float AttackRange { get; set; } = 1.4f;
    public int AttackIntervalMs { get; set; } = 900;
    public long LastAttackTick { get; set; }
    public List<int> SelectedBuildingCardIds { get; } = new();
    public List<BattleEquipmentSlotEntity> EquipmentSlots { get; } = new();
}

public sealed class BattleEquipmentSlotEntity
{
    public int SlotIndex { get; init; }
    public int ItemId { get; set; }
    public int GoodsId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string EffectDesc { get; set; } = string.Empty;
}

public sealed class BattleBuildingEntity : Entity
{
    public long InstanceId { get; set; }
    public long OwnerPlayerId { get; set; }
    public int BuildingId { get; set; }
    public int Level { get; set; } = 1;
    public int GridX { get; set; }
    public int GridY { get; set; }
    public int Width { get; set; } = 1;
    public int Height { get; set; } = 1;
    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public long LastEffectTick { get; set; }
    public string State { get; set; } = "Built";
}

public sealed class BattleAttackEventRecord
{
    public long EventId { get; init; }
    public long SourceBuildingInstanceId { get; init; }
    public long SourcePlayerId { get; init; }
    public long TargetPlayerId { get; init; }
    public long TargetBuildingInstanceId { get; init; }
    public float FromX { get; init; }
    public float FromY { get; init; }
    public float ToX { get; init; }
    public float ToY { get; init; }
    public int Damage { get; init; }
}

public sealed class PendingTowerHitRecord
{
    public long EventId { get; init; }
    public long SourceBuildingInstanceId { get; init; }
    public long TargetPlayerId { get; init; }
    public long ResolveTick { get; init; }
    public int Damage { get; init; }
}
