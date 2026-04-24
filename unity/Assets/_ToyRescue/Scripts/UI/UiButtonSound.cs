using ToyRescue.Core;
using ToyRescue.Services;
using UnityEngine;
using UnityEngine.UI;

namespace ToyRescue.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class UiButtonSound : MonoBehaviour
    {
        [SerializeField] private GameBootstrapper bootstrapper;
        [SerializeField] private AudioClipLibrary audioClipLibrary;
        [SerializeField] private Button targetButton;
        [SerializeField] private string sfxCueId = "button_click";

        private IAudioService audioService;

        private void Awake()
        {
            targetButton ??= GetComponent<Button>();
            ResolveServices();

            if (targetButton != null)
            {
                targetButton.onClick.AddListener(Play);
            }
        }

        private void OnDestroy()
        {
            if (targetButton != null)
            {
                targetButton.onClick.RemoveListener(Play);
            }
        }

        public void Play()
        {
            ResolveServices();

            if (audioService == null || audioClipLibrary == null)
            {
                return;
            }

            if (!audioClipLibrary.TryGetSfxCue(sfxCueId, out SfxCue cue))
            {
                Debug.LogWarning($"{nameof(UiButtonSound)} on {name} could not find an SFX cue with id '{sfxCueId}'.");
                return;
            }

            audioService.PlaySfx(cue);
        }

        private void ResolveServices()
        {
            if (audioService != null)
            {
                return;
            }

            ServiceResolver.ResolveBootstrapper(ref bootstrapper)?.EnsureInitialized();
            ServiceResolver.TryResolve(ref bootstrapper, out audioService);
        }
    }
}
