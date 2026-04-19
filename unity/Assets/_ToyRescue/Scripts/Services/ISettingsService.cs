using System;
using ToyRescue.Core;

namespace ToyRescue.Services
{
    public interface ISettingsService
    {
        event Action<SettingsChangedEvent> SettingsChanged;

        bool MusicEnabled { get; }
        bool SfxEnabled { get; }

        SettingsChangedEvent Snapshot { get; }

        void SetMusicEnabled(bool enabled);
        void SetSfxEnabled(bool enabled);
        void ToggleMusic();
        void ToggleSfx();
    }
}
