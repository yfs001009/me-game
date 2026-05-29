using Fantasy;
using Hotfix.Shared;
using Fantasy.Entitas;

namespace Hotfix.Room.Service;

/// <summary>
/// 自定义房间服务。当前为内存房间表，后续迁移为 Room Scene + Redis 房间索引。
/// </summary>
public sealed class CustomRoomService
{
    private const int MaxSelectedBuildingCards = 6;

    private readonly object gate = new();
    private readonly Dictionary<int, RoomEntity> rooms = new();
    private int nextRoomId = 2000;

    public RoomDetailInfo Create(Scene scene, PlayerProfileInfo owner, C2G_CreateRoomRequest request)
    {
        lock (gate)
        {
            RemovePlayerFromRoomsUnsafe(owner.PlayerId);
            var requestedMapId = request.MapId <= 0 ? SheepServices.Rules.DefaultMapId : request.MapId;
            var map = SheepServices.Rules.GetMapOrDefault(requestedMapId);
            var mapId = map?.MapId ?? requestedMapId;
            var minPlayers = map?.MinPlayers ?? SheepServices.Rules.CustomRoomMinPlayers;
            var maxPlayers = map?.MaxPlayers ?? SheepServices.Rules.CustomRoomMaxPlayers;
            var defaultPlayers = map?.RecommendedPlayers ?? SheepServices.Rules.CustomRoomDefaultPlayers;
            var room = Entity.Create<RoomEntity>(scene, id: ++nextRoomId, isPool: false, isRunEvent: true);
            room.RoomId = (int)room.Id;
            room.RoomName = string.IsNullOrWhiteSpace(request.RoomName) ? $"{owner.Nickname}的房间" : request.RoomName.Trim();
            room.Mode = string.IsNullOrWhiteSpace(request.Mode) ? map?.Mode ?? "ClassicInfection" : request.Mode.Trim();
            room.MapId = mapId;
            room.MaxPlayers = Math.Clamp(
                request.MaxPlayers <= 0 ? defaultPlayers : request.MaxPlayers,
                minPlayers,
                maxPlayers);
            room.IsPrivate = request.IsPrivate;
            room.Password = request.Password ?? string.Empty;
            room.OwnerPlayerId = owner.PlayerId;
            room.Players.Add(ToRoomPlayer(owner, true, request.SelectedBuildingCardIds));
            rooms.Add(room.RoomId, room);
            Log.Info($"房间创建成功：房间ID={room.RoomId}，房主ID={owner.PlayerId}，房间名={room.RoomName}，模式={room.Mode}，地图={room.MapId}");
            return ToDetail(room);
        }
    }

    public List<RoomSummaryInfo> ListWaitingRooms()
    {
        lock (gate)
        {
            CleanupExpiredWaitingRooms();
            return rooms.Values.Where(room => room.State == "Waiting").Select(ToSummary).ToList();
        }
    }

    public (bool Success, string Message, RoomDetailInfo? Room) Join(PlayerProfileInfo profile, C2G_JoinRoomRequest request)
    {
        lock (gate)
        {
            if (!rooms.TryGetValue(request.RoomId, out var room) || room.State != "Waiting")
            {
                Log.Warning($"加入房间失败：房间不存在或不可加入。玩家ID={profile.PlayerId}，房间ID={request.RoomId}");
                return (false, "房间不存在或不可加入。", null);
            }

            if (room.IsPrivate && room.Password != (request.Password ?? string.Empty))
            {
                Log.Warning($"加入房间失败：密码错误。玩家ID={profile.PlayerId}，房间ID={request.RoomId}");
                return (false, "房间密码错误。", null);
            }

            if (room.Players.Any(player => player.PlayerId == profile.PlayerId))
            {
                room.UpdatedAtUtc = DateTimeOffset.UtcNow;
                return (true, "已在房间中。", ToDetail(room));
            }

            RemovePlayerFromOtherRoomsUnsafe(profile.PlayerId, room.RoomId);

            if (room.Players.Count >= room.MaxPlayers)
            {
                Log.Warning($"加入房间失败：房间已满。玩家ID={profile.PlayerId}，房间ID={request.RoomId}");
                return (false, "房间已满。", null);
            }

            room.Players.Add(ToRoomPlayer(profile, false, null));
            room.UpdatedAtUtc = DateTimeOffset.UtcNow;
            Log.Info($"玩家加入房间：玩家ID={profile.PlayerId}，房间ID={room.RoomId}，当前人数={room.Players.Count}/{room.MaxPlayers}");
            return (true, "加入房间成功。", ToDetail(room));
        }
    }

    public (bool Success, string Message, RoomDetailInfo? Room) GetDetail(PlayerProfileInfo profile, C2G_RoomDetailRequest request)
    {
        lock (gate)
        {
            if (!rooms.TryGetValue(request.RoomId, out var room))
            {
                return (false, "房间不存在。", null);
            }

            if (room.Players.All(player => player.PlayerId != profile.PlayerId))
            {
                return (false, "玩家不在房间中。", null);
            }

            return (true, "房间详情获取成功。", ToDetail(room));
        }
    }

    public BattleStartInfo? GetBattleByRoom(int roomId)
    {
        lock (gate)
        {
            return rooms.TryGetValue(roomId, out var room) ? room.Battle : null;
        }
    }

    public (bool Success, string Message, RoomDetailInfo? Room) SetReady(PlayerProfileInfo profile, C2G_SetRoomReadyRequest request)
    {
        lock (gate)
        {
            if (!rooms.TryGetValue(request.RoomId, out var room))
            {
                return (false, "房间不存在。", null);
            }

            var player = room.Players.FirstOrDefault(item => item.PlayerId == profile.PlayerId);
            if (player == null)
            {
                return (false, "玩家不在房间中。", ToDetail(room));
            }

            if (player.IsOwner)
            {
                return (false, "房主不需要准备。", ToDetail(room));
            }

            player.IsReady = request.IsReady;
            room.UpdatedAtUtc = DateTimeOffset.UtcNow;
            Log.Info($"玩家准备状态更新：玩家ID={profile.PlayerId}，房间ID={room.RoomId}，IsReady={player.IsReady}");
            return (true, player.IsReady ? "已准备。" : "已取消准备。", ToDetail(room));
        }
    }

    public (bool Success, string Message, RoomDetailInfo? Room, BattleStartInfo? Battle) Start(PlayerProfileInfo profile, C2G_StartRoomRequest request)
    {
        lock (gate)
        {
            if (!rooms.TryGetValue(request.RoomId, out var room))
            {
                return (false, "房间不存在。", null, null);
            }

            if (room.OwnerPlayerId != profile.PlayerId)
            {
                return (false, "只有房主可以开始游戏。", ToDetail(room), null);
            }

            if (room.State != "Waiting")
            {
                return (false, "房间当前状态不能开始游戏。", ToDetail(room), null);
            }

            if (room.Players.Any(player => !player.IsOwner && !player.IsReady))
            {
                return (false, "还有玩家未准备。", ToDetail(room), null);
            }

            room.State = "Playing";
            room.UpdatedAtUtc = DateTimeOffset.UtcNow;
            var map = SheepServices.Rules.GetMapOrDefault(room.MapId);
            var battle = new BattleStartInfo
            {
                BattleId = room.RoomId,
                RoomId = room.RoomId,
                MapId = room.MapId,
                MapAsset = string.IsNullOrWhiteSpace(map?.MapAsset) ? $"battle_map_{room.MapId}" : map.MapAsset,
                Mode = string.IsNullOrWhiteSpace(map?.Mode) ? room.Mode : map.Mode,
                BattleHost = "127.0.0.1",
                BattlePort = 20001,
                BattleProtocol = "KCP"
            };

            room.Battle = battle;
            Log.Info($"房间开始游戏：房间ID={room.RoomId}，地图={battle.MapAsset}，人数={room.Players.Count}");
            return (true, "开始游戏。", ToDetail(room), battle);
        }
    }

    public (bool Success, string Message, RoomDetailInfo? Room) Leave(PlayerProfileInfo profile, C2G_LeaveRoomRequest request)
    {
        lock (gate)
        {
            if (!rooms.TryGetValue(request.RoomId, out var room))
            {
                return (false, "房间不存在。", null);
            }

            var player = room.Players.FirstOrDefault(item => item.PlayerId == profile.PlayerId);
            if (player == null)
            {
                return (false, "玩家不在房间中。", ToDetail(room));
            }

            room.Players.Remove(player);

            if (room.Players.Count == 0)
            {
                rooms.Remove(room.RoomId);
                Log.Info($"房间已清空并移除：房间ID={room.RoomId}");
                return (true, "已离开房间，房间已解散。", null);
            }

            if (player.IsOwner)
            {
                var newOwner = room.Players[0];
                newOwner.IsOwner = true;
                newOwner.IsReady = true;
                room.OwnerPlayerId = newOwner.PlayerId;
            }

            room.UpdatedAtUtc = DateTimeOffset.UtcNow;
            Log.Info($"玩家离开房间：玩家ID={profile.PlayerId}，房间ID={room.RoomId}，当前人数={room.Players.Count}/{room.MaxPlayers}");
            return (true, "已离开房间。", ToDetail(room));
        }
    }

    public int RemoveRoomsByPlayer(long playerId)
    {
        lock (gate)
        {
            return RemovePlayerFromRoomsUnsafe(playerId);
        }
    }

    private int RemovePlayerFromOtherRoomsUnsafe(long playerId, int keepRoomId)
    {
        var removedCount = 0;
        foreach (var room in rooms.Values.Where(room => room.RoomId != keepRoomId).ToList())
        {
            if (RemovePlayerFromRoomUnsafe(room, playerId))
            {
                removedCount++;
            }
        }

        return removedCount;
    }

    private int RemovePlayerFromRoomsUnsafe(long playerId)
    {
        var removedCount = 0;
        foreach (var room in rooms.Values.ToList())
        {
            if (RemovePlayerFromRoomUnsafe(room, playerId))
            {
                removedCount++;
            }
        }

        return removedCount;
    }

    private bool RemovePlayerFromRoomUnsafe(RoomEntity room, long playerId)
    {
        var player = room.Players.FirstOrDefault(item => item.PlayerId == playerId);
        if (player == null)
        {
            return false;
        }

        room.Players.Remove(player);
        if (room.Players.Count == 0)
        {
            rooms.Remove(room.RoomId);
            Log.Info($"移除玩家并清理空房间：玩家ID={playerId}，房间ID={room.RoomId}");
            return true;
        }

        if (player.IsOwner)
        {
            var newOwner = room.Players[0];
            newOwner.IsOwner = true;
            newOwner.IsReady = true;
            room.OwnerPlayerId = newOwner.PlayerId;
        }

        room.UpdatedAtUtc = DateTimeOffset.UtcNow;
        return true;
    }

    private void CleanupExpiredWaitingRooms()
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var room in rooms.Values.ToList())
        {
            if (room.State != "Waiting")
            {
                continue;
            }

            if (room.Players.Count == 0 || (room.Players.Count == 1 && now - room.UpdatedAtUtc > SheepServices.Rules.WaitingSoloRoomTtl))
            {
                rooms.Remove(room.RoomId);
                Log.Info($"清理过期等待房间：房间ID={room.RoomId}，人数={room.Players.Count}");
            }
        }
    }

    private static RoomPlayerInfo ToRoomPlayer(PlayerProfileInfo profile, bool owner, IReadOnlyCollection<int>? selectedBuildingCardIds)
    {
        var player = new RoomPlayerInfo
        {
            PlayerId = profile.PlayerId,
            Nickname = profile.Nickname,
            Level = profile.Level,
            IsOwner = owner,
            IsReady = owner
        };
        player.SelectedBuildingCardIds.AddRange(NormalizeSelectedBuildingCards(selectedBuildingCardIds));
        return player;
    }

    private static RoomDetailInfo ToDetail(RoomEntity room)
    {
        var detail = new RoomDetailInfo { Summary = ToSummary(room) };
        detail.Players.AddRange(room.Players.Select(ClonePlayer));
        return detail;
    }

    private static RoomSummaryInfo ToSummary(RoomEntity room)
    {
        return new RoomSummaryInfo
        {
            RoomId = room.RoomId,
            RoomName = room.RoomName,
            Mode = room.Mode,
            MapId = room.MapId,
            CurrentPlayers = room.Players.Count,
            MaxPlayers = room.MaxPlayers,
            IsPrivate = room.IsPrivate,
            State = room.State
        };
    }

    private static RoomPlayerInfo ClonePlayer(RoomPlayerInfo source)
    {
        var player = new RoomPlayerInfo
        {
            PlayerId = source.PlayerId,
            Nickname = source.Nickname,
            Level = source.Level,
            IsOwner = source.IsOwner,
            IsReady = source.IsReady
        };
        player.SelectedBuildingCardIds.AddRange(source.SelectedBuildingCardIds);
        return player;
    }

    private static List<int> NormalizeSelectedBuildingCards(IReadOnlyCollection<int>? selectedBuildingCardIds)
    {
        var selected = selectedBuildingCardIds?
            .Where(cardId => cardId > 0)
            .Distinct()
            .Take(MaxSelectedBuildingCards)
            .ToList() ?? new List<int>();

        if (selected.Count > 0)
        {
            return selected;
        }

        return Hotfix.Config.ConfigSystem.Instance.Tables.TbBuildingCard.DataList
            .OrderBy(card => card.SortOrder)
            .ThenBy(card => card.CardId)
            .Take(MaxSelectedBuildingCards)
            .Select(card => card.CardId)
            .ToList();
    }
}
