using ToyRescue.Core;
using UnityEngine;

namespace ToyRescue.Services
{
    public sealed class AudioService : IAudioService
    {
        private readonly AudioSource musicSource;
        private readonly AudioSource sfxSource;
        private readonly ISettingsService settingsService;
        
        public AudioService(AudioSource musicSource, AudioSource sfxSource, ISettingsService settingsService = null)
        {
            this.musicSource = musicSource;
            this.sfxSource = sfxSource;
            this.settingsService = settingsService;
            
            if (settingsService != null)
            {
                ApplySettings(new AudioSettingsSnapshot(settingsService.MusicEnabled, settingsService.SfxEnabled));
                settingsService.SettingsChanged += OnSettingsChanged;
            }
            else
            {
                ApplySettings(new AudioSettingsSnapshot(true, true));
            }
        }

        public bool MusicEnabled { get; private set; } = true;

        public bool SfxEnabled { get; private set; } = true;

        public void ApplySettings(AudioSettingsSnapshot settings)
        {
            MusicEnabled = settings.MusicEnabled;
            SfxEnabled = settings.SfxEnabled;

            if (musicSource != null)
            {
                musicSource.mute = !MusicEnabled;
            }

            if (sfxSource != null)
            {
                sfxSource.mute = !SfxEnabled;
            }
        }

        public void PlayMusic(MusicCue cue)
        {
            if (cue == null || cue.Clip == null)
            {
                Debug.LogWarning("AudioService.PlayMusic skipped because the cue or clip is missing.");
                return;
            }

            if (musicSource == null)
            {
                Debug.LogWarning($"AudioService.PlayMusic skipped because no music AudioSource is assigned. Cue: {cue.Id}");
                return;
            }

            if (musicSource.clip == cue.Clip && musicSource.isPlaying)
            {
                return;
            }

            musicSource.clip = cue.Clip;
            musicSource.loop = cue.Loop;
            musicSource.volume = cue.Volume;
            musicSource.mute = !MusicEnabled;
            musicSource.Play();
        }

        public void StopMusic()
        {
            if (musicSource == null)
            {
                return;
            }

            musicSource.Stop();
            musicSource.clip = null;
        }

        public void PlaySfx(SfxCue cue)
        {
            if (!SfxEnabled)
            {
                return;
            }

            if (cue == null || cue.Clip == null)
            {
                Debug.LogWarning("AudioService.PlaySfx skipped because the cue or clip is missing.");
                return;
            }

            if (sfxSource == null)
            {
                Debug.LogWarning($"AudioService.PlaySfx skipped because no SFX AudioSource is assigned. Cue: {cue.Id}");
                return;
            }

            sfxSource.PlayOneShot(cue.Clip, cue.Volume);
        }
        
        public void Dispose()
        {
            if (settingsService != null)
            {
                settingsService.SettingsChanged -= OnSettingsChanged;
            }
        }

        private void OnSettingsChanged(SettingsChangedEvent eventData)
        {
            ApplySettings(new AudioSettingsSnapshot(eventData.MusicEnabled, eventData.SfxEnabled));
        }
    }
}
