using ToyRescue.Core;
using ToyRescue.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ToyRescue.UI
{
    public sealed class MainMenuController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private GameBootstrapper bootstrapper;
        [SerializeField] private SettingsMenuController settingsMenuController;
        [SerializeField] private AudioClipLibrary audioClipLibrary;

        [Header("UI")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;

        [Header("Scene Flow")]
        [SerializeField] private string gameSceneName = "Game";

        [Header("Audio")]
        [SerializeField] private string menuMusicCueId = "menu";
        [SerializeField] private bool playMenuMusicOnEnable = true;

        private GameStateMachine stateMachine;
        private IAudioService audioService;

        private void Awake()
        {
            ResolveServices();

            if (playButton != null)
            {
                playButton.onClick.AddListener(OnPlayPressed);
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OnSettingsPressed);
            }
        }

        private void Start()
        {
            settingsMenuController?.HideImmediately();
        }

        private void OnEnable()
        {
            ResolveServices();
            stateMachine?.TransitionTo(GameState.MainMenu);

            if (playMenuMusicOnEnable)
            {
                PlayMenuMusic();
            }
        }

        private void OnDestroy()
        {
            if (playButton != null)
            {
                playButton.onClick.RemoveListener(OnPlayPressed);
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.RemoveListener(OnSettingsPressed);
            }
        }

        public void OnPlayPressed()
        {
            ResolveServices();

            if (string.IsNullOrWhiteSpace(gameSceneName))
            {
                Debug.LogWarning($"{nameof(MainMenuController)} is missing a game scene name.");
                return;
            }

            stateMachine?.TransitionTo(GameState.LoadingGame);
            SceneManager.LoadScene(gameSceneName);
        }

        public void OnSettingsPressed()
        {
            ResolveServices();

            if (settingsMenuController != null)
            {
                settingsMenuController.Open();
                return;
            }

            stateMachine?.TransitionTo(GameState.SettingsMenu);
        }

        public void PlayMenuMusic()
        {
            ResolveServices();

            if (audioService == null || audioClipLibrary == null)
            {
                return;
            }

            if (!audioClipLibrary.TryGetMusicCue(menuMusicCueId, out MusicCue cue))
            {
                Debug.LogWarning($"{nameof(MainMenuController)} could not find a music cue with id '{menuMusicCueId}'.");
                return;
            }

            audioService.PlayMusic(cue);
        }

        private void ResolveServices()
        {
            if (stateMachine != null && audioService != null)
            {
                return;
            }

            ServiceResolver.ResolveBootstrapper(ref bootstrapper)?.EnsureInitialized();
            ServiceResolver.TryResolve(ref bootstrapper, out stateMachine, out audioService);
        }
    }
}
