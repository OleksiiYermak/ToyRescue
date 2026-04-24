using System;
using ToyRescue.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ToyRescue.UI
{
    public sealed class GameHudController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private GameBootstrapper bootstrapper;

        [Header("UI")]
        [SerializeField] private GameObject hudRoot;
        [SerializeField] private Text timerText;
        [SerializeField] private Text objectiveText;
        [SerializeField] private Text progressText;

        private EventBus eventBus;
        private GameStateMachine stateMachine;
        private IDisposable activityConfiguredSubscription;
        private IDisposable timerTickSubscription;
        private IDisposable toyCollectedSubscription;
        private IDisposable stateChangedSubscription;

        private void OnEnable()
        {
            ResolveServices();
            Subscribe();
            SynchronizeVisibility();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void OnActivityConfigured(ActivityConfiguredEvent eventData)
        {
            if (objectiveText != null)
            {
                objectiveText.text = eventData.ObjectiveText;
            }

            if (progressText != null)
            {
                progressText.text = FormatProgress(eventData.CollectedCount, eventData.TargetCount);
            }

            SetTimerText(eventData.DurationSeconds);
        }

        private void OnTimerTick(TimerTickEvent eventData)
        {
            SetTimerText(eventData.RemainingSeconds);
        }

        private void OnToyCollected(ToyCollectedEvent eventData)
        {
            if (progressText != null)
            {
                progressText.text = FormatProgress(eventData.CollectedCount, eventData.TargetCount);
            }
        }

        private void OnGameStateChanged(GameStateChangedEvent eventData)
        {
            SynchronizeVisibility(eventData.CurrentState);
        }

        private void Subscribe()
        {
            if (eventBus == null)
            {
                return;
            }

            activityConfiguredSubscription = eventBus.Subscribe<ActivityConfiguredEvent>(OnActivityConfigured);
            timerTickSubscription = eventBus.Subscribe<TimerTickEvent>(OnTimerTick);
            toyCollectedSubscription = eventBus.Subscribe<ToyCollectedEvent>(OnToyCollected);
            stateChangedSubscription = eventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void Unsubscribe()
        {
            activityConfiguredSubscription?.Dispose();
            timerTickSubscription?.Dispose();
            toyCollectedSubscription?.Dispose();
            stateChangedSubscription?.Dispose();

            activityConfiguredSubscription = null;
            timerTickSubscription = null;
            toyCollectedSubscription = null;
            stateChangedSubscription = null;
        }

        private void SynchronizeVisibility()
        {
            if (stateMachine == null)
            {
                return;
            }

            SynchronizeVisibility(stateMachine.CurrentState);
        }

        private void SynchronizeVisibility(GameState currentState)
        {
            if (hudRoot == null)
            {
                return;
            }

            bool isVisible =
                currentState == GameState.PreGameInstruction ||
                currentState == GameState.Playing ||
                currentState == GameState.PausedOrSettings;

            if (hudRoot.activeSelf != isVisible)
            {
                hudRoot.SetActive(isVisible);
            }
        }

        private void SetTimerText(float remainingSeconds)
        {
            if (timerText == null)
            {
                return;
            }

            int totalSeconds = Mathf.Max(0, Mathf.CeilToInt(remainingSeconds));
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            timerText.text = $"{minutes:00}:{seconds:00}";
        }

        private static string FormatProgress(int collectedCount, int targetCount)
        {
            return $"{collectedCount}/{targetCount}";
        }

        private void ResolveServices()
        {
            if (eventBus != null && stateMachine != null)
            {
                return;
            }

            ServiceResolver.ResolveBootstrapper(ref bootstrapper)?.EnsureInitialized();
            ServiceResolver.TryResolve(ref bootstrapper, out eventBus, out stateMachine);
        }
    }
}
