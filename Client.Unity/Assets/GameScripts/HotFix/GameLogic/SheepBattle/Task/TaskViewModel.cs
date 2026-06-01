using System.Collections.Generic;
using System.Linq;
using Fantasy;

namespace GameLogic.SheepBattle.Task
{
    public sealed class TaskViewModel
    {
        public List<TaskEntryViewModel> Tasks { get; } = new();
        public string TaskType { get; private set; } = string.Empty;
        public string ActivityId { get; private set; } = string.Empty;
        public string FeatureId { get; private set; } = string.Empty;

        public void Apply(IReadOnlyList<OutgameTaskInfo> tasks, string taskType, string activityId, string featureId)
        {
            TaskType = taskType ?? string.Empty;
            ActivityId = activityId ?? string.Empty;
            FeatureId = featureId ?? string.Empty;
            Tasks.Clear();
            if (tasks == null)
            {
                return;
            }

            Tasks.AddRange(tasks.Select(v => new TaskEntryViewModel(v)));
        }
    }

    public sealed class TaskEntryViewModel
    {
        public TaskEntryViewModel(OutgameTaskInfo info)
        {
            TaskId = info?.TaskId ?? 0;
            TaskType = info?.TaskType ?? string.Empty;
            ActivityId = info?.ActivityId ?? string.Empty;
            Title = info?.Title ?? string.Empty;
            Description = info?.Description ?? string.Empty;
            ProgressKey = info?.ProgressKey ?? string.Empty;
            Current = info?.Current ?? 0;
            Target = info?.Target ?? 0;
            State = info?.State ?? string.Empty;
            RefreshGroup = info?.RefreshGroup ?? string.Empty;
            EndsAtUnixSeconds = info?.EndsAtUnixSeconds ?? 0;
            Reward = info?.Reward;
            FeatureId = info?.FeatureId ?? string.Empty;
        }

        public int TaskId { get; }
        public string TaskType { get; }
        public string ActivityId { get; }
        public string Title { get; }
        public string Description { get; }
        public string ProgressKey { get; }
        public long Current { get; }
        public long Target { get; }
        public string State { get; }
        public string RefreshGroup { get; }
        public long EndsAtUnixSeconds { get; }
        public RewardInfo Reward { get; }
        public string FeatureId { get; }
        public bool IsComplete => string.Equals(State, "Complete", System.StringComparison.OrdinalIgnoreCase);
        public bool IsClaimed => string.Equals(State, "Claimed", System.StringComparison.OrdinalIgnoreCase);
        public string ProgressText => Target > 0 ? $"{Current}/{Target}" : Current.ToString();
    }
}
