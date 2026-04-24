using System;
using UnityEngine;

namespace ToyRescue.Activities
{
    public sealed class GameTimer : MonoBehaviour
    {
        private readonly CountdownTimerModel timerModel = new CountdownTimerModel();
        private int lastPublishedDisplayedSeconds = -1;

        public event Action<float> TimeChanged;
        public event Action Completed;

        public float DurationSeconds => timerModel.DurationSeconds;
        public float RemainingSeconds => timerModel.RemainingSeconds;
        public bool IsRunning => timerModel.IsRunning;

        public void ResetTimer(float durationSeconds)
        {
            timerModel.Reset(durationSeconds);
            PublishTimeChanged(force: true);
        }

        public void StartTimer()
        {
            timerModel.Start();
        }

        public void StartTimer(float durationSeconds)
        {
            timerModel.Reset(durationSeconds);
            timerModel.Start();
            PublishTimeChanged(force: true);
        }

        public void StopTimer()
        {
            timerModel.Stop();
        }

        public void Tick(float deltaTime)
        {
            if (!timerModel.Tick(deltaTime))
            {
                return;
            }

            PublishTimeChanged();
            if (timerModel.IsComplete)
            {
                Completed?.Invoke();
            }
        }

        private void PublishTimeChanged(bool force = false)
        {
            int displayedSeconds = timerModel.DisplayedSeconds;
            if (!force && displayedSeconds == lastPublishedDisplayedSeconds)
            {
                return;
            }

            lastPublishedDisplayedSeconds = displayedSeconds;
            TimeChanged?.Invoke(timerModel.RemainingSeconds);
        }

        private void Update()
        {
            Tick(Time.deltaTime);
        }
    }
}
