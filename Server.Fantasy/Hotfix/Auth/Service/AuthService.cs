using System.Security.Cryptography;
using System.Text;
using Fantasy;
using Hotfix.Shared;

namespace Hotfix.Auth.Service;

/// <summary>
/// 账号与会话服务。当前为内存实现，遵守 Fantasy Hotfix 层调用方式；后续替换为 Redis + DB。
/// </summary>
public sealed class AuthService
{
    private readonly object gate = new();
    private readonly Dictionary<string, AccountRecord> accountsByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, long> tokenToPlayerId = new();
    private readonly Dictionary<long, AccountRecord> accountsById = new();
    private long nextPlayerId = 10000;

    public (bool Success, string Message) Register(string account, string password, string nickname)
    {
        lock (gate)
        {
            account = Normalize(account);
            if (account.Length < 4 || password.Length < 6)
            {
                Log.Warning($"注册失败：账号或密码长度不符合要求。账号={account}");
                return (false, "账号至少4位，密码至少6位。 ");
            }

            if (accountsByName.ContainsKey(account))
            {
                Log.Warning($"注册失败：账号已存在。账号={account}");
                return (false, "账号已存在。 ");
            }

            var record = new AccountRecord
            {
                PlayerId = ++nextPlayerId,
                Account = account,
                PasswordHash = Hash(password),
                Nickname = string.IsNullOrWhiteSpace(nickname) ? string.Empty : nickname.Trim()
            };
            accountsByName.Add(account, record);
            accountsById.Add(record.PlayerId, record);
            Log.Info($"玩家注册成功：玩家ID={record.PlayerId}，账号={record.Account}，昵称={record.Nickname}");
            return (true, "注册成功。 ");
        }
    }

    public (string Token, PlayerProfileInfo Profile) Login(string account, string password)
    {
        lock (gate)
        {
            account = Normalize(account);
            if (!accountsByName.TryGetValue(account, out var record))
            {
                Log.Info($"账号不存在，自动注册新玩家：账号={account}");
                Register(account, password, string.Empty);
                record = accountsByName[account];
            }

            if (record.PasswordHash != Hash(password))
            {
                Log.Warning($"登录失败：密码错误。账号={account}");
                throw new UnauthorizedAccessException("账号或密码错误。 ");
            }

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            tokenToPlayerId[token] = record.PlayerId;
            Log.Info($"玩家登录成功：玩家ID={record.PlayerId}，账号={record.Account}，昵称={record.Nickname}");
            return (token, ToProfile(record));
        }
    }

    public PlayerProfileInfo RequireProfile(string token)
    {
        lock (gate)
        {
            if (string.IsNullOrWhiteSpace(token) || !tokenToPlayerId.TryGetValue(token, out var playerId) || !accountsById.TryGetValue(playerId, out var record))
            {
                Log.Warning("会话校验失败：Token无效或已过期。");
                throw new UnauthorizedAccessException("登录状态已失效。 ");
            }

            return ToProfile(record);
        }
    }

    public (bool Success, string Message, PlayerProfileInfo Profile) SetNickname(string token, string nickname)
    {
        lock (gate)
        {
            if (string.IsNullOrWhiteSpace(token) || !tokenToPlayerId.TryGetValue(token, out var playerId) || !accountsById.TryGetValue(playerId, out var record))
            {
                Log.Warning("设置昵称失败：Token无效或已过期。");
                throw new UnauthorizedAccessException("登录状态已失效。 ");
            }

            nickname = Normalize(nickname);
            if (nickname.Length < 2 || nickname.Length > 12)
            {
                return (false, "昵称需要 2-12 个字符。", ToProfile(record));
            }

            record.Nickname = nickname;
            Log.Info($"玩家设置昵称成功：玩家ID={record.PlayerId}，昵称={record.Nickname}");
            return (true, "昵称设置成功。", ToProfile(record));
        }
    }

    private static string Normalize(string value) => (value ?? string.Empty).Trim();

    private static string Hash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value ?? string.Empty));
        return Convert.ToHexString(bytes);
    }

    private static PlayerProfileInfo ToProfile(AccountRecord record)
    {
        return new PlayerProfileInfo
        {
            PlayerId = record.PlayerId,
            Account = record.Account,
            Nickname = record.Nickname,
            Level = record.Level,
            Exp = record.Exp,
            AvatarId = record.AvatarId,
            RankScore = record.RankScore
        };
    }
}
