using TEngine;

namespace GameLogic
{
    [EventInterface(EEventGroup.GroupLogic)]
    public interface IBattleCommand
    {
        void OnSelectBuilding(int buildingId);

        void OnOpenBuildPanel();

        void OnBuildAt(int gridX, int gridY);

        void OnUpgradeBuilding(long instanceId);

        void OnRecycleBuilding(long instanceId);

        void OnExitBuildMode();
    }
}
