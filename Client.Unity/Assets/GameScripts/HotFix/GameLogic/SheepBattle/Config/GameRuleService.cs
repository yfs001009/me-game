using System;
using GameConfig.common;
using TEngine;

namespace GameLogic.SheepBattle.Config
{
    public sealed class GameRuleService
    {
        public static GameRuleService Instance { get; } = new GameRuleService();

        public int SplashDurationSeconds => GetInt("SplashDurationSeconds", 2);
        public int LoadingMinSeconds => GetInt("LoadingMinSeconds", 1);
        public string LoadingTipsGroup => GetString("LoadingTipsGroup", "MvpDefault");
        public bool VersionCheckEnabled => GetBool("VersionCheckEnabled", true);
        public int AccountMinLength => GetInt("AccountMinLength", 3);
        public int AccountMaxLength => GetInt("AccountMaxLength", 12);
        public int PasswordMinLength => GetInt("PasswordMinLength", 6);
        public int PasswordMaxLength => GetInt("PasswordMaxLength", 20);
        public int DefaultGold => GetInt("DefaultGold", 1000);
        public int DefaultWood => GetInt("DefaultWood", 500);
        public int MatchMinPlayers => GetInt("MatchMinPlayers", 4);
        public int MatchMaxPlayers => GetInt("MatchMaxPlayers", 12);
        public int MatchTargetPlayers => GetInt("MatchTargetPlayers", 8);
        public int MatchEstimatedSeconds => GetInt("MatchEstimatedSeconds", 30);
        public int CustomRoomMinPlayers => GetInt("CustomRoomMinPlayers", 4);
        public int CustomRoomMaxPlayers => GetInt("CustomRoomMaxPlayers", 12);
        public int CustomRoomDefaultPlayers => GetInt("CustomRoomDefaultPlayers", 8);
        public int DefaultMapId => GetInt("DefaultMapId", 1);
        public string DefaultMapName => GetString("DefaultMapId", "迷雾森林");
        public bool RequireAllReadyToStart => GetBool("RequireAllReadyToStart", true);
        public int WaitingSoloRoomTtlSeconds => GetInt("WaitingSoloRoomTtlSeconds", 120);
        public int BattleLoadTimeoutSeconds => GetInt("BattleLoadTimeoutSeconds", 30);
        public int PreparePhaseSeconds => GetInt("PreparePhaseSeconds", 30);
        public int InitialTrollMinPlayers => GetInt("InitialTrollMinPlayers", 1);
        public int InitialTrollMaxPlayers => GetInt("InitialTrollMaxPlayers", 2);
        public int BattleDurationSeconds => GetInt("BattleDurationSeconds", 900);
        public int BuildRange => GetInt("BuildRange", 4);
        public int CorePackupSeconds => GetInt("CorePackupSeconds", 8);
        public int WallRecyclePercent => GetInt("WallRecyclePercent", 25);
        public int TowerRecyclePercent => GetInt("TowerRecyclePercent", 50);
        public int AdvancedBuildingRecyclePercent => GetInt("AdvancedBuildingRecyclePercent", 60);
        public int ReturnLobbyDelaySeconds => GetInt("ReturnLobbyDelaySeconds", 5);

        public string GetLoadingTip()
        {
            return LoadingTipsGroup switch
            {
                "MvpDefault" => "提示：守住核心，等待反击时机。",
                _ => "提示：寻找据点，建立防线。"
            };
        }

        private GameRuleService()
        {
        }

        private static GameRule GetRule(string key)
        {
            try
            {
                return ConfigSystem.Instance.Tables.TbGameRule.GetOrDefault(key);
            }
            catch (Exception exception)
            {
                Log.Warning($"读取规则表失败，使用默认值。Key={key} Error={exception.Message}");
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

        private static string GetString(string key, string defaultValue)
        {
            var rule = GetRule(key);
            return rule == null || string.IsNullOrWhiteSpace(rule.StringValue) ? defaultValue : rule.StringValue;
        }
    }
}
