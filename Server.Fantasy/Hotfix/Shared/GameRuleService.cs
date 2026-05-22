namespace Hotfix.Shared;

public sealed class GameRuleService
{
    public int MatchTargetPlayers => 4;
    public int CustomRoomMinPlayers => 2;
    public int CustomRoomMaxPlayers => 10;
    public int CustomRoomDefaultPlayers => 4;
    public int DefaultMapId => 1;
    public bool RequireAllReadyToStart => true;
    public TimeSpan WaitingSoloRoomTtl => TimeSpan.FromSeconds(120);
}
