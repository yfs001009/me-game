using Fantasy;
using Fantasy.Entitas;
using GameConfig.task;
using Hotfix.Asset.Model;
using Hotfix.Config;
using Hotfix.Mail.Service;
using Hotfix.Shared;

namespace Hotfix.Task.Service;

public sealed class OutgameTaskService
{
    private const string StateDoing = "Doing";
    private const string StateClaimed = "Claimed";

    private readonly object gate = new();
    private readonly Dictionary<long, PlayerOutgameTaskEntity> states = new();

    public G2C_OutgameTaskListResponse GetList(Scene scene, long playerId, string taskType, string activityId, string featureId)
    {
        lock (gate)
        {
            var response = new G2C_OutgameTaskListResponse
            {
                Success = true,
                Message = "任务列表获取成功。"
            };

            foreach (var template in FilterTemplates(taskType, activityId, featureId))
            {
                response.Tasks.Add(ToInfo(scene, playerId, template));
            }

            return response;
        }
    }

    public G2C_ClaimOutgameTaskRewardResponse Claim(Scene scene, long playerId, int taskId)
    {
        lock (gate)
        {
            var response = new G2C_ClaimOutgameTaskRewardResponse();
            var template = ConfigSystem.Instance.Tables.TbTask.GetOrDefault(taskId);
            if (template == null)
            {
                response.Success = false;
                response.Message = "任务不存在。";
                response.Snapshot = SheepServices.Assets.CreateSnapshot(playerId);
                return response;
            }

            if (!SheepServices.Features.IsOpen(template.FeatureId))
            {
                response.Success = false;
                response.Message = "任务暂未开放。";
                response.Snapshot = SheepServices.Assets.CreateSnapshot(playerId);
                return response;
            }

            var record = GetTaskRecord(GetOrCreateState(scene, playerId), template);
            var progress = GetProgress(playerId, template.ProgressKey);
            if (record.State == StateClaimed)
            {
                response.Success = false;
                response.Message = "任务奖励已领取。";
                FillTasks(scene, playerId, response.Tasks, template.TaskType, template.ActivityId, template.FeatureId);
                response.Snapshot = SheepServices.Assets.CreateSnapshot(playerId);
                return response;
            }

            if (progress < template.Target)
            {
                response.Success = false;
                response.Message = "任务尚未完成。";
                FillTasks(scene, playerId, response.Tasks, template.TaskType, template.ActivityId, template.FeatureId);
                response.Snapshot = SheepServices.Assets.CreateSnapshot(playerId);
                return response;
            }

            if (!SheepServices.Assets.TryTransferReward(
                    playerId,
                    CreateReward(template),
                    new AssetTransferContext("OutgameTask", template.TaskId.ToString(), "OutgameTaskClaim"),
                    out var snapshot,
                    out var message))
            {
                response.Success = false;
                response.Message = message;
                FillTasks(scene, playerId, response.Tasks, template.TaskType, template.ActivityId, template.FeatureId);
                response.Snapshot = snapshot;
                return response;
            }

            record.State = StateClaimed;
            record.UpdatedAtUtc = DateTimeOffset.UtcNow;
            response.Success = true;
            response.Message = "任务奖励领取成功。";
            FillTasks(scene, playerId, response.Tasks, template.TaskType, template.ActivityId, template.FeatureId);
            response.Snapshot = snapshot;
            Log.Info($"局外任务领奖：玩家ID={playerId}，任务ID={taskId}");
            return response;
        }
    }

    public long AddProgress(long playerId, string progressKey, long delta)
    {
        if (string.IsNullOrWhiteSpace(progressKey) || delta <= 0)
        {
            return 0;
        }

        return SheepServices.Assets.AddProgressValue(playerId, progressKey.Trim(), delta);
    }

    public void SetProgress(long playerId, string progressKey, long value)
    {
        if (string.IsNullOrWhiteSpace(progressKey))
        {
            return;
        }

        SheepServices.Assets.SetProgressValue(playerId, progressKey.Trim(), value);
    }

    private void FillTasks(Scene scene, long playerId, ICollection<OutgameTaskInfo> output, string taskType, string activityId, string featureId)
    {
        foreach (var template in FilterTemplates(taskType, activityId, featureId))
        {
            output.Add(ToInfo(scene, playerId, template));
        }
    }

    private IEnumerable<TaskConfig> FilterTemplates(string taskType, string activityId, string featureId)
    {
        var normalizedType = NormalizeFilter(taskType);
        var normalizedActivity = NormalizeFilter(activityId);
        var normalizedFeature = NormalizeFilter(featureId);
        foreach (var template in ConfigSystem.Instance.Tables.TbTask.DataList.OrderBy(v => v.TaskId))
        {
            if (!SheepServices.Features.IsOpen(template.FeatureId))
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(normalizedType) &&
                !string.Equals(template.TaskType, normalizedType, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(normalizedActivity) &&
                !string.Equals(template.ActivityId, normalizedActivity, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(normalizedFeature) &&
                !string.Equals(template.FeatureId, normalizedFeature, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            yield return template;
        }
    }

    private OutgameTaskInfo ToInfo(Scene scene, long playerId, TaskConfig template)
    {
        var record = GetTaskRecord(GetOrCreateState(scene, playerId), template);
        var current = GetProgress(playerId, template.ProgressKey);
        return new OutgameTaskInfo
        {
            TaskId = template.TaskId,
            TaskType = template.TaskType,
            ActivityId = template.ActivityId,
            Title = template.Title,
            Description = template.Description,
            ProgressKey = template.ProgressKey,
            Current = Math.Min(current, template.Target),
            Target = template.Target,
            State = record.State == StateClaimed ? StateClaimed : current >= template.Target ? "Complete" : StateDoing,
            RefreshGroup = template.RefreshGroup,
            EndsAtUnixSeconds = 0,
            Reward = MailService.ToRewardInfo(CreateReward(template)),
            FeatureId = template.FeatureId
        };
    }

    private PlayerOutgameTaskEntity GetOrCreateState(Scene scene, long playerId)
    {
        if (states.TryGetValue(playerId, out var state))
        {
            return state;
        }

        state = Entity.Create<PlayerOutgameTaskEntity>(scene, isPool: false, isRunEvent: true);
        state.PlayerId = playerId;
        states.Add(playerId, state);
        return state;
    }

    private static OutgameTaskRecord GetTaskRecord(PlayerOutgameTaskEntity state, TaskConfig template)
    {
        if (state.TasksById.TryGetValue(template.TaskId, out var record))
        {
            return record;
        }

        record = new OutgameTaskRecord
        {
            TaskId = template.TaskId,
            RefreshGroup = template.RefreshGroup,
            State = StateDoing
        };
        state.TasksById.Add(template.TaskId, record);
        return record;
    }

    private static long GetProgress(long playerId, string progressKey)
    {
        return SheepServices.Assets.GetProgressValue(playerId, progressKey);
    }

    private static AssetReward CreateReward(TaskConfig template)
    {
        var reward = new AssetReward();
        if (template.RewardCurrencyId > 0 && template.RewardCurrencyAmount > 0)
        {
            reward.Currencies.Add(new CurrencyAmount(template.RewardCurrencyId, template.RewardCurrencyAmount));
        }

        if (template.RewardItemId > 0 && template.RewardItemCount > 0)
        {
            reward.Items.Add(new ItemAmount(template.RewardItemId, template.RewardItemCount));
        }

        return reward;
    }

    private static string NormalizeFilter(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
    }
}
