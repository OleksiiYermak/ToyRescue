using System;
using UnityEngine;

namespace ToyRescue.Activities
{
    public sealed class GameTimer : MonoBehaviour
    {
        private readonly CountdownTimerModel timerModel = new CountdownTimerModel();

        public event Action<float> TimeChanged;
        public event Action Completed;

        public float DurationSeconds => timerModel.DurationSeconds;
        public float RemainingSeconds => timerModel.RemainingSeconds;
        public bool IsRunning => timerModel.IsRunning;

        public void ResetTimer(float durationSeconds)
        {
            timerModel.Reset(durationSeconds);
            TimeChanged?.Invoke(timerModel.RemainingSeconds);
        }

        public void StartTimer()
        {
            timerModel.Start();
        }

        public void StartTimer(float durationSeconds)
        {
            timerModel.Reset(durationSeconds);
            timerModel.Start();
            TimeChanged?.Invoke(timerModel.RemainingSeconds);
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

            TimeChanged?.Invoke(timerModel.RemainingSeconds);
            if (timerModel.IsComplete)
            {
                Completed?.Invoke();
            }
        }

        private void Update()
        {
            Tick(Time.deltaTime);
        }
    }
}
