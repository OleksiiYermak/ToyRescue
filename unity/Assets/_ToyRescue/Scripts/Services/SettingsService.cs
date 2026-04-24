using System;
using ToyRescue.Core;
using UnityEngine;

namespace ToyRescue.Services
{
    public sealed class SettingsService : ISettingsService
    {
        private const int TrueValue = 1;
        private const int FalseValue = 0;

        private readonly EventBus eventBus;
        private readonly string musicKey;
        private readonly string sfxKey;

        public SettingsService(EventBus eventBus = null, string keyPrefix = "ToyRescue.Settings")
        {
            this.eventBus = eventBus;
            musicKey = $"{keyPrefix}.MusicEnabled";
            sfxKey = $"{keyPrefix}.SfxEnabled";

            MusicEnabled = PlayerPrefs.GetInt(musicKey, TrueValue) == TrueValue;
            SfxEnabled = PlayerPrefs.GetInt(sfxKey, TrueValue) == TrueValue;
        }

        public event Action<SettingsChangedEvent> SettingsChanged;

        public bool MusicEnabled { get; private set; }

        public bool SfxEnabled { get; private set; }

        public SettingsChangedEvent Snapshot => new SettingsChangedEvent(MusicEnabled, SfxEnabled);

        public void SetMusicEnabled(bool enabled)
        {
            if (MusicEnabled == enabled)
            {
                return;
            }

            MusicEnabled = enabled;
            PlayerPrefs.SetInt(musicKey, enabled ? TrueValue : FalseValue);
            PlayerPrefs.Save();
            PublishChanged();
        }

        public void SetSfxEnabled(bool enabled)
        {
            if (SfxEnabled == enabled)
            {
                return;
            }

            SfxEnabled = enabled;
            PlayerPrefs.SetInt(sfxKey, enabled ? TrueValue : FalseValue);
            PlayerPrefs.Save();
            PublishChanged();
        }

        public void ToggleMusic()
        {
            SetMusicEnabled(!MusicEnabled);
        }

        public void ToggleSfx()
        {
            SetSfxEnabled(!SfxEnabled);
        }

        private void PublishChanged()
        {
            SettingsChangedEvent eventData = Snapshot;
            SettingsChanged?.Invoke(eventData);
            eventBus?.Publish(eventData);
        }
    }
}
