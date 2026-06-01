using Fantasy.Async;
using GameLogic.SheepBattle.Asset;
using GameLogic.SheepBattle.Common;
using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Network;
using GameLogic.SheepBattle.Reward;
using TEngine;

namespace GameLogic.SheepBattle.Task
{
    public sealed class TaskController
    {
        public static TaskController Instance { get; } = new();
        public TaskViewModel Model { get; } = new();

        private TaskController()
        {
        }

        public async FTask<TaskViewModel> RefreshAsync(string taskType = "", string activityId = "", string featureId = "")
        {
            var response = await SheepNetworkService.Instance.RequestOutgameTaskListAsync(taskType, activityId, featureId);
            if (!response.Success)
            {
                CommonNoticeService.Show(response.Message);
                return Model;
            }

            Model.Apply(response.Tasks, taskType, activityId, featureId);
            GameEvent.Send(new TaskViewChangedEvent(Model));
            return Model;
        }

        public async FTask ClaimAsync(int taskId)
        {
            var response = await SheepNetworkService.Instance.ClaimOutgameTaskRewardAsync(taskId);
            if (!response.Success)
            {
                CommonNoticeService.Show(response.Message);
                return;
            }

            Model.Apply(response.Tasks, Model.TaskType, Model.ActivityId, Model.FeatureId);
            if (response.Snapshot != null)
            {
                AssetController.Instance.ApplySnapshot(response.Snapshot);
            }
            else
            {
                await AssetController.Instance.RefreshAsync();
            }

            GameEvent.Send(new TaskViewChangedEvent(Model));
            var task = Model.Tasks.Find(v => v.TaskId == taskId);
            RewardDisplayService.Show(RewardDisplayService.FromReward("任务奖励", task?.Reward));
        }
    }
}
