using Fantasy;
using Hotfix.Shared;

namespace Hotfix.Room.Service;

/// <summary>
/// 自定义房间服务。当前为内存房间表，后续迁移为 Room Scene + Redis 房间索引。
/// </summary>
public sealed class CustomRoomService
{
    private readonly object gate = new();
    private readonly Dictionary<int, CustomRoomRecord> rooms = new();
    private int nextRoomId = 2000;

    public RoomDetailInfo Create(PlayerProfileInfo owner, C2G_CreateRoomRequest request)
    {
        lock (gate)
        {
            var room = new CustomRoomRecord
            {
                RoomId = ++nextRoomId,
                RoomName = string.IsNullOrWhiteSpace(request.RoomName) ? $"{owner.Nickname}的房间" : request.RoomName.Trim(),
                Mode = string.IsNullOrWhiteSpace(request.Mode) ? "ClassicInfection" : request.Mode.Trim(),
                MapId = request.MapId <= 0 ? 1 : request.MapId,
                MaxPlayers = Math.Clamp(request.MaxPlayers, SheepServices.Rules.CustomRoomMinPlayers, SheepServices.Rules.CustomRoomMaxPlayers),
                IsPrivate = request.IsPrivate,
                Password = request.Password ?? string.Empty,
                OwnerPlayerId = owner.PlayerId
            };
            room.Players.Add(ToRoomPlayer(owner, true));
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

            if (room.Players.Count >= room.MaxPlayers)
            {
                Log.Warning($"加入房间失败：房间已满。玩家ID={profile.PlayerId}，房间ID={request.RoomId}");
                return (false, "房间已满。", null);
            }

            room.Players.Add(ToRoomPlayer(profile, false));
            room.UpdatedAtUtc = DateTimeOffset.UtcNow;
            Log.Info($"玩家加入房间：玩家ID={profile.PlayerId}，房间ID={room.RoomId}，当前人数={room.Players.Count}/{room.MaxPlayers}");
            return (true, "加入房间成功。", ToDetail(room));
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
            var removedCount = 0;
            foreach (var room in rooms.Values.ToList())
            {
                var player = room.Players.FirstOrDefault(item => item.PlayerId == playerId);
                if (player == null)
                {
                    continue;
                }

                room.Players.Remove(player);
                if (room.Players.Count == 0)
                {
                    rooms.Remove(room.RoomId);
                    removedCount++;
                    Log.Info($"玩家断线清理房间：玩家ID={playerId}，房间ID={room.RoomId}");
                    continue;
                }

                if (player.IsOwner)
                {
                    var newOwner = room.Players[0];
                    newOwner.IsOwner = true;
                    newOwner.IsReady = true;
                    room.OwnerPlayerId = newOwner.PlayerId;
                }

                room.UpdatedAtUtc = DateTimeOffset.UtcNow;
            }

            return removedCount;
        }
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

    private static RoomPlayerInfo ToRoomPlayer(PlayerProfileInfo profile, bool owner)
    {
        return new RoomPlayerInfo
        {
            PlayerId = profile.PlayerId,
            Nickname = profile.Nickname,
            Level = profile.Level,
            IsOwner = owner,
            IsReady = owner
        };
    }

    private static RoomDetailInfo ToDetail(CustomRoomRecord room)
    {
        var detail = new RoomDetailInfo { Summary = ToSummary(room) };
        detail.Players.AddRange(room.Players.Select(ClonePlayer));
        return detail;
    }

    private static RoomSummaryInfo ToSummary(CustomRoomRecord room)
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
        return new RoomPlayerInfo
        {
            PlayerId = source.PlayerId,
            Nickname = source.Nickname,
            Level = source.Level,
            IsOwner = source.IsOwner,
            IsReady = source.IsReady
        };
    }
}
