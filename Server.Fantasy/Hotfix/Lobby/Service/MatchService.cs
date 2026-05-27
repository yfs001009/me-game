using Fantasy;
using Hotfix.Shared;

namespace Hotfix.Lobby.Service;

/// <summary>
/// 匹配服务。当前为单进程内存队列，后续迁移为 Redis 队列或 Fantasy Match Scene。
/// </summary>
public sealed class MatchService
{
    private readonly object gate = new();
    private readonly Dictionary<long, MatchTicket> tickets = new();

    public MatchStatusInfo Start(PlayerProfileInfo profile, string mode)
    {
        lock (gate)
        {
            mode = string.IsNullOrWhiteSpace(mode) ? "ClassicInfection" : mode.Trim();
            tickets[profile.PlayerId] = new MatchTicket
            {
                PlayerId = profile.PlayerId,
                Mode = mode,
                StartTimeUtc = DateTimeOffset.UtcNow
            };
            Log.Info($"Match started. PlayerId={profile.PlayerId} Mode={mode}");
            return GetStatusUnsafe(profile.PlayerId);
        }
    }

    public MatchStatusInfo GetStatus(long playerId)
    {
        lock (gate)
        {
            return GetStatusUnsafe(playerId);
        }
    }

    private MatchStatusInfo GetStatusUnsafe(long playerId)
    {
        if (!tickets.TryGetValue(playerId, out var ticket))
        {
            return new MatchStatusInfo { IsMatching = false };
        }

        return new MatchStatusInfo
        {
            IsMatching = true,
            Mode = ticket.Mode,
            EstimatedSeconds = Math.Max(1, SheepServices.Rules.MatchEstimatedSeconds - (int)(DateTimeOffset.UtcNow - ticket.StartTimeUtc).TotalSeconds),
            AllocatedRoomId = 0
        };
    }
}
