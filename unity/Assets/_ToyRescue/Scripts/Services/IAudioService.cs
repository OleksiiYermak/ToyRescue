using System;

namespace ToyRescue.Services
{
    public interface IAudioService : IDisposable
    {
        bool MusicEnabled { get; }
        bool SfxEnabled { get; }

        void ApplySettings(AudioSettingsSnapshot settings);
        void PlayMusic(MusicCue cue);
        void StopMusic();
        void PlaySfx(SfxCue cue);
    }

    public readonly struct AudioSettingsSnapshot
    {
        public AudioSettingsSnapshot(bool musicEnabled, bool sfxEnabled)
        {
            MusicEnabled = musicEnabled;
            SfxEnabled = sfxEnabled;
        }

        public bool MusicEnabled { get; }
        public bool SfxEnabled { get; }
    }
}
