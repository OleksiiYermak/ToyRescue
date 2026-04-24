using System;

namespace ToyRescue.Activities
{
    public sealed class CountdownTimerModel
    {
        public CountdownTimerModel(float durationSeconds = 1f)
        {
            Reset(durationSeconds);
        }

        public float DurationSeconds { get; private set; }
        public float RemainingSeconds { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsComplete => RemainingSeconds <= 0f;

        public void Reset(float durationSeconds)
        {
            DurationSeconds = Math.Max(0.01f, durationSeconds);
            RemainingSeconds = DurationSeconds;
            IsRunning = false;
        }

        public void Start()
        {
            if (!IsComplete)
            {
                IsRunning = true;
            }
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public bool Tick(float deltaTime)
        {
            if (!IsRunning || IsComplete)
            {
                return false;
            }

            float clampedDelta = Math.Max(0f, deltaTime);
            if (clampedDelta <= 0f)
            {
                return false;
            }

            RemainingSeconds = Math.Max(0f, RemainingSeconds - clampedDelta);
            if (IsComplete)
            {
                IsRunning = false;
            }

            return true;
        }
    }
}
