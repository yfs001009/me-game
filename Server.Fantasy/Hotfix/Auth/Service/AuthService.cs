using System.Security.Cryptography;
using System.Text;
using Fantasy;
using Hotfix.Shared;
using Fantasy.Entitas;

namespace Hotfix.Auth.Service;

/// <summary>
/// 账号与会话服务。当前为单进程内存实现，后续迁移到 Data Scene 后再替换为 Redis + DB。
/// 业务失败通过返回值交给 Handler 写入响应，避免用异常承载可预期的登录/Token 校验失败。
/// </summary>
public sealed class AuthService
{
    private readonly object gate = new();
    private readonly Dictionary<string, PlayerAccountEntity> accountsByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, long> tokenToPlayerId = new();
    private readonly Dictionary<long, PlayerAccountEntity> accountsById = new();
    private long nextPlayerId = 10000;

    public (bool Success, string Message) Register(Scene scene, string account, string password, string nickname)
    {
        lock (gate)
        {
            account = Normalize(account);
            var rules = SheepServices.Rules;
            if (account.Length < rules.AccountMinLength || account.Length > rules.AccountMaxLength ||
                password.Length < rules.PasswordMinLength || password.Length > rules.PasswordMaxLength)
            {
                Log.Warning($"注册失败：账号或密码长度不符合要求。账号={account}");
                return (false, $"账号需{rules.AccountMinLength}-{rules.AccountMaxLength}位，密码需{rules.PasswordMinLength}-{rules.PasswordMaxLength}位。");
            }

            if (accountsByName.ContainsKey(account))
            {
                Log.Warning($"注册失败：账号已存在。账号={account}");
                return (false, "账号已存在。 ");
            }

            var record = Entity.Create<PlayerAccountEntity>(scene, id: ++nextPlayerId, isPool: false, isRunEvent: true);
            record.PlayerId = record.Id;
            record.Account = account;
            record.PasswordHash = Hash(password);
            record.Nickname = string.IsNullOrWhiteSpace(nickname) ? string.Empty : nickname.Trim();
            accountsByName.Add(account, record);
            accountsById.Add(record.PlayerId, record);
            Log.Info($"玩家注册成功：玩家ID={record.PlayerId}，账号={record.Account}，昵称={record.Nickname}");
            return (true, "注册成功。 ");
        }
    }

    public (bool Success, string Message, string Token, PlayerProfileInfo Profile) Login(Scene scene, string account, string password)
    {
        lock (gate)
        {
            account = Normalize(account);
            if (!accountsByName.TryGetValue(account, out var record))
            {
                Log.Info($"账号不存在，自动注册新玩家：账号={account}");
                var registerResult = Register(scene, account, password, string.Empty);
                if (!registerResult.Success)
                {
                    return (false, registerResult.Message, string.Empty, new PlayerProfileInfo());
                }

                record = accountsByName[account];
            }

            if (record.PasswordHash != Hash(password))
            {
                Log.Warning($"登录失败：密码错误。账号={account}");
                return (false, "账号或密码错误。", string.Empty, new PlayerProfileInfo());
            }

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            tokenToPlayerId[token] = record.PlayerId;
            var session = record.GetOrAddComponent<PlayerSessionComponent>();
            session.Token = token;
            session.PlayerId = record.PlayerId;
            session.LoginAtUtc = DateTimeOffset.UtcNow;
            Log.Info($"玩家登录成功：玩家ID={record.PlayerId}，账号={record.Account}，昵称={record.Nickname}");
            return (true, "登录成功。", token, ToProfile(record));
        }
    }

    public bool TryRequireProfile(string token, out PlayerProfileInfo profile, out string message)
    {
        lock (gate)
        {
            profile = new PlayerProfileInfo();
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(token) || !tokenToPlayerId.TryGetValue(token, out var playerId) || !accountsById.TryGetValue(playerId, out var record))
            {
                Log.Warning("会话校验失败：Token无效或已过期。");
                message = "登录状态已失效。";
                return false;
            }

            profile = ToProfile(record);
            return true;
        }
    }

    public (bool Success, string Message, PlayerProfileInfo Profile) SetNickname(string token, string nickname)
    {
        lock (gate)
        {
            if (string.IsNullOrWhiteSpace(token) || !tokenToPlayerId.TryGetValue(token, out var playerId) || !accountsById.TryGetValue(playerId, out var record))
            {
                Log.Warning("设置昵称失败：Token无效或已过期。");
                return (false, "登录状态已失效。", new PlayerProfileInfo());
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

    private static PlayerProfileInfo ToProfile(PlayerAccountEntity record)
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
