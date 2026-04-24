using ToyRescue.Services;
using UnityEngine;

namespace ToyRescue.Core
{
    public sealed class GameBootstrapper : MonoBehaviour
    {
        private static GameBootstrapper instance;

        [Header("Lifecycle")]
        [SerializeField] private bool initializeOnAwake = true;
        [SerializeField] private bool dontDestroyOnLoad = true;
        [SerializeField] private bool transitionToMainMenuOnStart = true;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        public ServiceRegistry Services { get; private set; }
        public EventBus EventBus { get; private set; }
        public GameStateMachine StateMachine { get; private set; }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            if (!initializeOnAwake)
            {
                return;
            }

            Initialize();
        }
        
        private void OnDestroy()
        {
            if (Services != null && Services.TryGet<IAudioService>(out var audio)) 
            {
                audio?.Dispose();
            }
            
            if (instance == this)
            {
                instance = null;
            }
        }

        public void Initialize()
        {
            if (Services != null)
            {
                return;
            }

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            EnsureAudioSources();

            EventBus = new EventBus();
            Services = new ServiceRegistry();
            ServiceRegistry.SetCurrent(Services);

            var settingsService = new SettingsService(EventBus);
            var audioService = new AudioService(musicSource, sfxSource, settingsService);
            var analyticsService = new ConsoleAnalyticsService();
            var assetLoaderService = new AddressablesAssetLoaderService();

            StateMachine = new GameStateMachine(EventBus);

            Services.Register(EventBus);
            Services.Register(StateMachine);
            Services.Register<ISettingsService>(settingsService);
            Services.Register<IAudioService>(audioService);
            Services.Register<IAnalyticsService>(analyticsService);
            Services.Register<IAssetLoaderService>(assetLoaderService);

            if (transitionToMainMenuOnStart)
            {
                StateMachine.TransitionTo(GameState.MainMenu);
            }
        }

        private void EnsureAudioSources()
        {
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.playOnAwake = false;
                musicSource.loop = true;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
                sfxSource.loop = false;
            }
        }
    }
}