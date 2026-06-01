using Fantasy;
using Hotfix.Shared;

namespace Hotfix.Social.Service;

public sealed class SocialService
{
    private const string FollowingMode = "Following";
    private const string FansMode = "Fans";
    private const string SearchMode = "Search";

    private readonly object gate = new();
    private readonly Dictionary<long, HashSet<long>> followingByPlayerId = new();
    private readonly Dictionary<long, HashSet<long>> followersByPlayerId = new();

    public G2C_SocialListResponse GetList(PlayerProfileInfo profile, string viewMode, string keyword)
    {
        lock (gate)
        {
            viewMode = NormalizeViewMode(viewMode);
            var response = CreateResponse(profile.PlayerId, viewMode, true, "社交列表获取成功。");
            FillPlayers(response.Players, profile.PlayerId, viewMode, keyword);
            return response;
        }
    }

    public G2C_FollowPlayerResponse SetFollow(PlayerProfileInfo profile, long targetPlayerId, bool follow, string viewMode)
    {
        lock (gate)
        {
            viewMode = NormalizeViewMode(viewMode);
            var response = new G2C_FollowPlayerResponse { ViewMode = viewMode };
            if (targetPlayerId <= 0 || targetPlayerId == profile.PlayerId)
            {
                FillCounts(response, profile.PlayerId);
                response.Success = false;
                response.Message = "不能关注该玩家。";
                FillPlayers(response.Players, profile.PlayerId, viewMode, string.Empty);
                return response;
            }

            var targetProfile = SheepServices.Auth.GetProfile(targetPlayerId);
            if (targetProfile == null)
            {
                FillCounts(response, profile.PlayerId);
                response.Success = false;
                response.Message = "玩家不存在。";
                FillPlayers(response.Players, profile.PlayerId, viewMode, string.Empty);
                return response;
            }

            if (follow)
            {
                GetFollowingSet(profile.PlayerId).Add(targetPlayerId);
                GetFollowersSet(targetPlayerId).Add(profile.PlayerId);
            }
            else
            {
                GetFollowingSet(profile.PlayerId).Remove(targetPlayerId);
                GetFollowersSet(targetPlayerId).Remove(profile.PlayerId);
            }

            FillCounts(response, profile.PlayerId);
            response.Success = true;
            response.Message = follow ? "关注成功。" : "已取消关注。";
            FillPlayers(response.Players, profile.PlayerId, viewMode, string.Empty);
            return response;
        }
    }

    private G2C_SocialListResponse CreateResponse(long playerId, string viewMode, bool success, string message)
    {
        var response = new G2C_SocialListResponse
        {
            Success = success,
            Message = message,
            ViewMode = viewMode
        };
        FillCounts(response, playerId);
        return response;
    }

    private void FillCounts(G2C_SocialListResponse response, long playerId)
    {
        response.FollowingCount = GetFollowingSet(playerId).Count;
        response.FollowerCount = GetFollowersSet(playerId).Count;
    }

    private void FillCounts(G2C_FollowPlayerResponse response, long playerId)
    {
        response.FollowingCount = GetFollowingSet(playerId).Count;
        response.FollowerCount = GetFollowersSet(playerId).Count;
    }

    private void FillPlayers(ICollection<SocialPlayerInfo> output, long playerId, string viewMode, string keyword)
    {
        IEnumerable<PlayerProfileInfo> profiles = viewMode switch
        {
            FansMode => GetProfiles(GetFollowersSet(playerId)),
            SearchMode => SheepServices.Auth.SearchProfiles(playerId, keyword),
            _ => GetProfiles(GetFollowingSet(playerId))
        };

        foreach (var profile in profiles.OrderBy(item => item.PlayerId))
        {
            output.Add(ToInfo(playerId, profile!));
        }
    }

    private static IEnumerable<PlayerProfileInfo> GetProfiles(IEnumerable<long> playerIds)
    {
        foreach (var playerId in playerIds)
        {
            var profile = SheepServices.Auth.GetProfile(playerId);
            if (profile != null)
            {
                yield return profile;
            }
        }
    }

    private SocialPlayerInfo ToInfo(long viewerPlayerId, PlayerProfileInfo profile)
    {
        return new SocialPlayerInfo
        {
            Profile = profile,
            IsFollowing = GetFollowingSet(viewerPlayerId).Contains(profile.PlayerId),
            IsFollower = GetFollowersSet(viewerPlayerId).Contains(profile.PlayerId)
        };
    }

    private HashSet<long> GetFollowingSet(long playerId)
    {
        if (!followingByPlayerId.TryGetValue(playerId, out var set))
        {
            set = new HashSet<long>();
            followingByPlayerId.Add(playerId, set);
        }

        return set;
    }

    private HashSet<long> GetFollowersSet(long playerId)
    {
        if (!followersByPlayerId.TryGetValue(playerId, out var set))
        {
            set = new HashSet<long>();
            followersByPlayerId.Add(playerId, set);
        }

        return set;
    }

    private static string NormalizeViewMode(string viewMode)
    {
        if (string.Equals(viewMode, FansMode, StringComparison.OrdinalIgnoreCase))
        {
            return FansMode;
        }

        if (string.Equals(viewMode, SearchMode, StringComparison.OrdinalIgnoreCase))
        {
            return SearchMode;
        }

        return FollowingMode;
    }
}
