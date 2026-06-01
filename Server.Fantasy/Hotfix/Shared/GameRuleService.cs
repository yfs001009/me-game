using Fantasy;
using GameConfig.battle;
using GameConfig.common;
using Hotfix.Config;

namespace Hotfix.Shared;

public sealed class GameRuleService
{
    public int AccountMinLength => GetInt("AccountMinLength", 3);
    public int AccountMaxLength => GetInt("AccountMaxLength", 12);
    public int PasswordMinLength => GetInt("PasswordMinLength", 6);
    public int PasswordMaxLength => GetInt("PasswordMaxLength", 20);
    public int MatchMinPlayers => GetInt("MatchMinPlayers", 4);
    public int MatchMaxPlayers => GetInt("MatchMaxPlayers", 12);
    public int MatchTargetPlayers => GetInt("MatchTargetPlayers", 8);
    public int MatchEstimatedSeconds => GetInt("MatchEstimatedSeconds", 30);
    public int CustomRoomMinPlayers => GetInt("CustomRoomMinPlayers", 4);
    public int CustomRoomMaxPlayers => GetInt("CustomRoomMaxPlayers", 12);
    public int CustomRoomDefaultPlayers => GetInt("CustomRoomDefaultPlayers", 8);
    public int DefaultMapId => GetInt("DefaultMapId", 1);
    public bool RequireAllReadyToStart => GetBool("RequireAllReadyToStart", true);
    public int WaitingSoloRoomTtlSeconds => GetInt("WaitingSoloRoomTtlSeconds", 120);
    public TimeSpan WaitingSoloRoomTtl => TimeSpan.FromSeconds(WaitingSoloRoomTtlSeconds);
    public int BuildRange => GetInt("BuildRange", 4);

    public MapConfig? GetMapOrDefault(int mapId)
    {
        try
        {
            return ConfigSystem.Instance.Tables.TbMap.GetOrDefault(mapId)
                   ?? ConfigSystem.Instance.Tables.TbMap.GetOrDefault(DefaultMapId)
                   ?? ConfigSystem.Instance.Tables.TbMap.DataList.FirstOrDefault();
        }
        catch (Exception exception)
        {
            Log.Warning($"Read map config failed. MapId={mapId} Error={exception.Message}");
            return null;
        }
    }

    private static GameRule? GetRule(string key)
    {
        try
        {
            return ConfigSystem.Instance.Tables.TbGameRule.GetOrDefault(key);
        }
        catch (Exception exception)
        {
            Log.Warning($"Read game rule failed, using default value. Key={key} Error={exception.Message}");
            return null;
        }
    }

    private static int GetInt(string key, int defaultValue)
    {
        var rule = GetRule(key);
        return rule == null ? defaultValue : rule.IntValue;
    }

    private static bool GetBool(string key, bool defaultValue)
    {
        var rule = GetRule(key);
        return rule == null ? defaultValue : rule.BoolValue;
    }
}
