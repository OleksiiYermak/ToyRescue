using System;
using UnityEngine;

namespace ToyRescue.Services
{
    public interface IAudioService: IDisposable
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

    [Serializable]
    public sealed class MusicCue
    {
        [SerializeField] private string id;
        [SerializeField] private AudioClip clip;
        [SerializeField] private bool loop = true;
        [SerializeField, Range(0f, 1f)] private float volume = 1f;

        public MusicCue()
        {
        }

        public MusicCue(string id, AudioClip clip, bool loop = true, float volume = 1f)
        {
            this.id = id;
            this.clip = clip;
            this.loop = loop;
            this.volume = Mathf.Clamp01(volume);
        }

        public string Id => id;
        public AudioClip Clip => clip;
        public bool Loop => loop;
        public float Volume => volume;
    }

    [Serializable]
    public sealed class SfxCue
    {
        [SerializeField] private string id;
        [SerializeField] private AudioClip clip;
        [SerializeField, Range(0f, 1f)] private float volume = 1f;

        public SfxCue()
        {
        }

        public SfxCue(string id, AudioClip clip, float volume = 1f)
        {
            this.id = id;
            this.clip = clip;
            this.volume = Mathf.Clamp01(volume);
        }

        public string Id => id;
        public AudioClip Clip => clip;
        public float Volume => volume;
    }
}
