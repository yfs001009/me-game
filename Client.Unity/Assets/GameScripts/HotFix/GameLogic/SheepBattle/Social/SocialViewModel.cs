using System.Collections.Generic;
using System.Linq;
using Fantasy;

namespace GameLogic.SheepBattle.Social
{
    public sealed class SocialViewModel
    {
        public const string FollowingMode = "Following";
        public const string FansMode = "Fans";
        public const string SearchMode = "Search";

        public List<SocialPlayerEntryViewModel> Players { get; } = new();
        public string ViewMode { get; private set; } = FollowingMode;
        public int FollowingCount { get; private set; }
        public int FollowerCount { get; private set; }

        public void Apply(G2C_SocialListResponse response)
        {
            Apply(response?.Players, response?.ViewMode, response?.FollowingCount ?? 0, response?.FollowerCount ?? 0);
        }

        public void Apply(G2C_FollowPlayerResponse response)
        {
            Apply(response?.Players, response?.ViewMode, response?.FollowingCount ?? 0, response?.FollowerCount ?? 0);
        }

        private void Apply(IReadOnlyList<SocialPlayerInfo> players, string viewMode, int followingCount, int followerCount)
        {
            Players.Clear();
            ViewMode = string.IsNullOrWhiteSpace(viewMode) ? FollowingMode : viewMode;
            FollowingCount = followingCount;
            FollowerCount = followerCount;
            if (players == null)
            {
                return;
            }

            Players.AddRange(players.Select(item => new SocialPlayerEntryViewModel(item)));
        }
    }

    public sealed class SocialPlayerEntryViewModel
    {
        public SocialPlayerEntryViewModel(SocialPlayerInfo info)
        {
            var profile = info?.Profile;
            PlayerId = profile?.PlayerId ?? 0;
            Nickname = string.IsNullOrWhiteSpace(profile?.Nickname) ? profile?.Account ?? string.Empty : profile.Nickname;
            Account = profile?.Account ?? string.Empty;
            Level = profile?.Level ?? 1;
            RankScore = profile?.RankScore ?? 0;
            IsFollowing = info?.IsFollowing == true;
            IsFollower = info?.IsFollower == true;
        }

        public long PlayerId { get; }
        public string Nickname { get; }
        public string Account { get; }
        public int Level { get; }
        public int RankScore { get; }
        public bool IsFollowing { get; }
        public bool IsFollower { get; }
    }
}
