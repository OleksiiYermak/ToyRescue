using System;

namespace ToyRescue.Activities
{
    public enum ActivityOutcome
    {
        InProgress,
        Win,
        Fail
    }

    public sealed class ActivityProgressModel
    {
        public ActivityProgressModel(int targetCount)
        {
            TargetCount = Math.Max(1, targetCount);
        }

        public int TargetCount { get; }
        public int CollectedCount { get; private set; }
        public bool IsComplete => CollectedCount >= TargetCount;

        public bool TryCollectTarget()
        {
            if (IsComplete)
            {
                return false;
            }

            CollectedCount++;
            return true;
        }

        public ActivityOutcome EvaluateOutcome(float remainingSeconds)
        {
            if (IsComplete)
            {
                return ActivityOutcome.Win;
            }

            if (remainingSeconds <= 0f)
            {
                return ActivityOutcome.Fail;
            }

            return ActivityOutcome.InProgress;
        }
    }
}
