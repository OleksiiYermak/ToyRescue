using ToyRescue.Core;
using ToyRescue.Services;
using UnityEngine;
using UnityEngine.UI;

namespace ToyRescue.UI
{
    public sealed class SettingsMenuController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private GameBootstrapper bootstrapper;

        [Header("UI")]
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Button closeButton;
        [SerializeField] private bool hideOnStart = true;

        [Header("State Flow")]
        [SerializeField] private GameState openedState = GameState.SettingsMenu;
        [SerializeField] private GameState closedState = GameState.MainMenu;

        private ISettingsService settingsService;
        private GameStateMachine stateMachine;
        private bool isSynchronizingView;
        private bool isSubscribed;

        private GameObject TargetPanel => panelRoot != null ? panelRoot : gameObject;

        private void Awake()
        {
            ResolveServices();

            if (musicToggle != null)
            {
                musicToggle.onValueChanged.AddListener(OnMusicToggleValueChanged);
            }

            if (sfxToggle != null)
            {
                sfxToggle.onValueChanged.AddListener(OnSfxToggleValueChanged);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Close);
            }
        }

        private void Start()
        {
            if (hideOnStart)
            {
                HideImmediately();
            }
            else
            {
                SynchronizeView();
            }
        }

        private void OnEnable()
        {
            ResolveServices();
            RefreshSettingsSubscription();
            SynchronizeView();
        }

        private void OnDisable()
        {
            UnsubscribeFromSettings();
        }

        private void OnDestroy()
        {
            if (musicToggle != null)
            {
                musicToggle.onValueChanged.RemoveListener(OnMusicToggleValueChanged);
            }

            if (sfxToggle != null)
            {
                sfxToggle.onValueChanged.RemoveListener(OnSfxToggleValueChanged);
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Close);
            }
        }

        public void Open()
        {
            ResolveServices();
            SetPanelVisible(true);
            RefreshSettingsSubscription();
            SynchronizeView();
            stateMachine?.TransitionTo(openedState);
        }

        public void Close()
        {
            ResolveServices();
            SetPanelVisible(false);
            RefreshSettingsSubscription();
            stateMachine?.TransitionTo(closedState);
        }

        public void HideImmediately()
        {
            ResolveServices();
            SetPanelVisible(false);
            RefreshSettingsSubscription();
            SynchronizeView();
        }

        public void SynchronizeView()
        {
            ResolveServices();
            isSynchronizingView = true;

            if (musicToggle != null)
            {
                musicToggle.interactable = settingsService != null;
                musicToggle.isOn = settingsService?.MusicEnabled ?? true;
            }

            if (sfxToggle != null)
            {
                sfxToggle.interactable = settingsService != null;
                sfxToggle.isOn = settingsService?.SfxEnabled ?? true;
            }

            isSynchronizingView = false;
        }

        private void OnMusicToggleValueChanged(bool isEnabled)
        {
            if (isSynchronizingView)
            {
                return;
            }

            ResolveServices();
            settingsService?.SetMusicEnabled(isEnabled);
        }

        private void OnSfxToggleValueChanged(bool isEnabled)
        {
            if (isSynchronizingView)
            {
                return;
            }

            ResolveServices();
            settingsService?.SetSfxEnabled(isEnabled);
        }

        private void OnSettingsChanged(SettingsChangedEvent _)
        {
            SynchronizeView();
        }

        private void SetPanelVisible(bool isVisible)
        {
            GameObject target = TargetPanel;
            if (target.activeSelf == isVisible)
            {
                return;
            }

            target.SetActive(isVisible);
        }

        private void ResolveServices()
        {
            if (settingsService != null && stateMachine != null)
            {
                return;
            }

            ServiceResolver.ResolveBootstrapper(ref bootstrapper)?.EnsureInitialized();
            ServiceResolver.TryResolve(ref bootstrapper, out settingsService, out stateMachine);
        }

        private void RefreshSettingsSubscription()
        {
            if (isActiveAndEnabled && TargetPanel.activeInHierarchy)
            {
                SubscribeToSettings();
                return;
            }

            UnsubscribeFromSettings();
        }

        private void SubscribeToSettings()
        {
            if (isSubscribed || settingsService == null)
            {
                return;
            }

            settingsService.SettingsChanged += OnSettingsChanged;
            isSubscribed = true;
        }

        private void UnsubscribeFromSettings()
        {
            if (!isSubscribed || settingsService == null)
            {
                return;
            }

            settingsService.SettingsChanged -= OnSettingsChanged;
            isSubscribed = false;
        }
    }
}
