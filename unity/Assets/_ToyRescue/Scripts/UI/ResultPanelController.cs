using System;
using ToyRescue.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ToyRescue.UI
{
    public sealed class ResultPanelController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private GameBootstrapper bootstrapper;

        [Header("UI")]
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Text titleText;
        [SerializeField] private Text bodyText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        [Header("Scene Flow")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        [Header("Copy")]
        [SerializeField] private string winTitle = "Great job!";
        [SerializeField] private string failTitle = "Let's try again!";
        [SerializeField, TextArea] private string winMessage = "You cleaned up the target toys before the timer ran out.";
        [SerializeField, TextArea] private string failMessage = "Time ran out before all the target toys reached the basket.";

        private EventBus eventBus;
        private GameStateMachine stateMachine;
        private IDisposable activityWonSubscription;
        private IDisposable activityFailedSubscription;
        private IDisposable stateChangedSubscription;

        private void Awake()
        {
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartScene);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(ReturnToMainMenu);
            }
        }

        private void OnEnable()
        {
            ResolveServices();
            Subscribe();
            HideImmediately();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void OnDestroy()
        {
            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(RestartScene);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.RemoveListener(ReturnToMainMenu);
            }
        }

        public void RestartScene()
        {
            stateMachine?.TransitionTo(GameState.LoadingGame);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void ReturnToMainMenu()
        {
            if (string.IsNullOrWhiteSpace(mainMenuSceneName))
            {
                Debug.LogWarning($"{nameof(ResultPanelController)} is missing a main menu scene name.");
                return;
            }

            stateMachine?.TransitionTo(GameState.ReturningToMenu);
            SceneManager.LoadScene(mainMenuSceneName);
        }

        public void HideImmediately()
        {
            SetPanelVisible(false);
        }

        private void OnActivityWon(ActivityWonEvent _)
        {
            if (titleText != null)
            {
                titleText.text = winTitle;
            }

            if (bodyText != null)
            {
                bodyText.text = winMessage;
            }

            SetPanelVisible(true);
        }

        private void OnActivityFailed(ActivityFailedEvent _)
        {
            if (titleText != null)
            {
                titleText.text = failTitle;
            }

            if (bodyText != null)
            {
                bodyText.text = failMessage;
            }

            SetPanelVisible(true);
        }

        private void OnGameStateChanged(GameStateChangedEvent eventData)
        {
            if (eventData.CurrentState == GameState.PreGameInstruction ||
                eventData.CurrentState == GameState.Playing ||
                eventData.CurrentState == GameState.PausedOrSettings)
            {
                SetPanelVisible(false);
            }
        }

        private void Subscribe()
        {
            if (eventBus == null)
            {
                return;
            }

            activityWonSubscription = eventBus.Subscribe<ActivityWonEvent>(OnActivityWon);
            activityFailedSubscription = eventBus.Subscribe<ActivityFailedEvent>(OnActivityFailed);
            stateChangedSubscription = eventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void Unsubscribe()
        {
            activityWonSubscription?.Dispose();
            activityFailedSubscription?.Dispose();
            stateChangedSubscription?.Dispose();

            activityWonSubscription = null;
            activityFailedSubscription = null;
            stateChangedSubscription = null;
        }

        private void SetPanelVisible(bool isVisible)
        {
            if (panelRoot == null || panelRoot.activeSelf == isVisible)
            {
                return;
            }

            panelRoot.SetActive(isVisible);
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
