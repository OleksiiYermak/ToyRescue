using ToyRescue.Core;
using ToyRescue.Data;
using UnityEngine;

namespace ToyRescue.Activities
{
    [DisallowMultipleComponent]
    public sealed class ActivityController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private GameBootstrapper bootstrapper;
        [SerializeField] private ActivityDefinition activityDefinition;
        [SerializeField] private ToyRegistry toyRegistry;
        [SerializeField] private BasketCollector basketCollector;
        [SerializeField] private GameTimer gameTimer;

        [Header("Flow")]
        [SerializeField] private bool beginActivityOnStart;
        [SerializeField] private bool transitionToPreGameInstructionOnStart = true;

        private readonly ITargetToyMatcher targetToyMatcher = new TargetToyMatcher();

        private EventBus eventBus;
        private GameStateMachine stateMachine;
        private ActivityProgressModel progressModel;
        private bool hasStarted;
        private bool hasEnded;

        private void Awake()
        {
            ResolveServices();

            if (toyRegistry == null)
            {
                toyRegistry = GetComponentInChildren<ToyRegistry>(true);
            }

            if (basketCollector == null)
            {
                basketCollector = FindFirstObjectByType<BasketCollector>();
            }

            if (gameTimer == null)
            {
                gameTimer = GetComponentInChildren<GameTimer>(true);
            }
        }

        private void OnEnable()
        {
            toyRegistry?.EnsureToysRegistered();
            SubscribeToToys();
            SubscribeToTimer();
        }

        private void Start()
        {
            ConfigureActivity();

            if (beginActivityOnStart)
            {
                BeginActivity();
            }
        }

        private void OnDisable()
        {
            UnsubscribeFromToys();
            UnsubscribeFromTimer();
        }

        public void BeginActivity()
        {
            ResolveServices();

            if (progressModel == null)
            {
                ConfigureActivity();
            }

            if (activityDefinition == null || hasStarted || hasEnded)
            {
                return;
            }

            hasStarted = true;
            stateMachine?.TransitionTo(GameState.Playing);
            gameTimer?.StartTimer();
            eventBus?.Publish(new ActivityStartedEvent(activityDefinition.ActivityId));
        }

        public void ConfigureActivity()
        {
            ResolveServices();

            if (activityDefinition == null)
            {
                Debug.LogWarning($"{nameof(ActivityController)} on {name} is missing an {nameof(ActivityDefinition)}.");
                return;
            }

            hasStarted = false;
            hasEnded = false;
            progressModel = new ActivityProgressModel(activityDefinition.TargetCount);

            toyRegistry?.EnsureToysRegistered();
            toyRegistry?.ResetToys();
            basketCollector?.ResetCollector();

            eventBus?.Publish(new ActivityConfiguredEvent(
                activityDefinition.ActivityId,
                activityDefinition.TutorialText,
                progressModel.TargetCount,
                activityDefinition.DurationSeconds,
                progressModel.CollectedCount));

            if (gameTimer != null)
            {
                gameTimer.ResetTimer(activityDefinition.DurationSeconds);
            }
            else
            {
                eventBus?.Publish(new TimerTickEvent(activityDefinition.DurationSeconds));
            }

            if (ShouldTransitionToPreGameInstructionOnConfigure())
            {
                stateMachine?.TransitionTo(GameState.PreGameInstruction);
            }
        }

        private bool ShouldTransitionToPreGameInstructionOnConfigure()
        {
            return transitionToPreGameInstructionOnStart && !beginActivityOnStart;
        }

        private void HandleToySelected(ToyInteractable toy)
        {
            if (toy == null || !CanAcceptInput())
            {
                return;
            }

            eventBus?.Publish(new ToySelectedEvent(toy.ToyId, toy.Category));

            bool isTargetToy = targetToyMatcher.IsTargetToy(
                activityDefinition.ValidToyCategories,
                toy.Category,
                toy.CountsAsTarget);

            if (!isTargetToy)
            {
                eventBus?.Publish(new ToyRejectedEvent(toy.ToyId));
                return;
            }

            if (!progressModel.TryCollectTarget())
            {
                return;
            }

            toy.MarkCollected();
            basketCollector?.CollectToy(toy);

            eventBus?.Publish(new ToyAcceptedEvent(toy.ToyId));
            eventBus?.Publish(new ToyCollectedEvent(toy.ToyId, progressModel.CollectedCount, progressModel.TargetCount));

            if (progressModel.EvaluateOutcome(GetRemainingSeconds()) == ActivityOutcome.Win)
            {
                CompleteWithWin();
            }
        }

        private void HandleTimerChanged(float remainingSeconds)
        {
            eventBus?.Publish(new TimerTickEvent(remainingSeconds));
        }

        private void HandleTimerCompleted()
        {
            if (progressModel == null || hasEnded)
            {
                return;
            }

            if (progressModel.EvaluateOutcome(0f) == ActivityOutcome.Fail)
            {
                CompleteWithFail();
            }
        }

        private bool CanAcceptInput()
        {
            if (progressModel == null || !hasStarted || hasEnded)
            {
                return false;
            }

            return stateMachine == null || stateMachine.CurrentState == GameState.Playing;
        }

        private float GetRemainingSeconds()
        {
            return gameTimer != null ? gameTimer.RemainingSeconds : 0f;
        }

        private void CompleteWithWin()
        {
            if (hasEnded || activityDefinition == null)
            {
                return;
            }

            hasEnded = true;
            gameTimer?.StopTimer();
            stateMachine?.TransitionTo(GameState.Win);
            eventBus?.Publish(new ActivityWonEvent(activityDefinition.ActivityId, GetRemainingSeconds()));
        }

        private void CompleteWithFail()
        {
            if (hasEnded || activityDefinition == null)
            {
                return;
            }

            hasEnded = true;
            gameTimer?.StopTimer();
            stateMachine?.TransitionTo(GameState.Fail);
            eventBus?.Publish(new ActivityFailedEvent(activityDefinition.ActivityId));
        }

        private void SubscribeToToys()
        {
            if (toyRegistry == null)
            {
                return;
            }

            var toys = toyRegistry.Toys;
            for (int i = 0; i < toys.Count; i++)
            {
                if (toys[i] == null)
                {
                    continue;
                }

                toys[i].Selected -= HandleToySelected;
                toys[i].Selected += HandleToySelected;
            }
        }

        private void UnsubscribeFromToys()
        {
            if (toyRegistry == null)
            {
                return;
            }

            var toys = toyRegistry.Toys;
            for (int i = 0; i < toys.Count; i++)
            {
                if (toys[i] != null)
                {
                    toys[i].Selected -= HandleToySelected;
                }
            }
        }

        private void SubscribeToTimer()
        {
            if (gameTimer == null)
            {
                return;
            }

            gameTimer.TimeChanged -= HandleTimerChanged;
            gameTimer.TimeChanged += HandleTimerChanged;
            gameTimer.Completed -= HandleTimerCompleted;
            gameTimer.Completed += HandleTimerCompleted;
        }

        private void UnsubscribeFromTimer()
        {
            if (gameTimer == null)
            {
                return;
            }

            gameTimer.TimeChanged -= HandleTimerChanged;
            gameTimer.Completed -= HandleTimerCompleted;
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
