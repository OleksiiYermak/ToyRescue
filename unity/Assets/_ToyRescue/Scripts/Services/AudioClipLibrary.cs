using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToyRescue.Services
{
    public sealed class AudioClipLibrary : MonoBehaviour
    {
        [Header("Music")]
        [SerializeField] private MusicCue[] musicCues = Array.Empty<MusicCue>();

        [Header("SFX")]
        [SerializeField] private SfxCue[] sfxCues = Array.Empty<SfxCue>();

        private Dictionary<string, MusicCue> musicCuesById;
        private Dictionary<string, SfxCue> sfxCuesById;

        private void Awake()
        {
            RebuildLookups();
        }

        private void OnValidate()
        {
            musicCuesById = null;
            sfxCuesById = null;
        }

        public bool TryGetMusicCue(string cueId, out MusicCue cue)
        {
            EnsureLookups();
            return musicCuesById.TryGetValue(cueId ?? string.Empty, out cue);
        }

        public bool TryGetSfxCue(string cueId, out SfxCue cue)
        {
            EnsureLookups();
            return sfxCuesById.TryGetValue(cueId ?? string.Empty, out cue);
        }

        private void EnsureLookups()
        {
            if (musicCuesById == null || sfxCuesById == null)
            {
                RebuildLookups();
            }
        }

        private void RebuildLookups()
        {
            musicCuesById = BuildLookup(musicCues, cue => cue?.Id, "music");
            sfxCuesById = BuildLookup(sfxCues, cue => cue?.Id, "sfx");
        }

        private Dictionary<string, TCue> BuildLookup<TCue>(TCue[] cues, Func<TCue, string> getId, string cueType)
            where TCue : class
        {
            var lookup = new Dictionary<string, TCue>(StringComparer.OrdinalIgnoreCase);

            if (cues == null)
            {
                return lookup;
            }

            for (int i = 0; i < cues.Length; i++)
            {
                TCue cue = cues[i];
                if (cue == null)
                {
                    continue;
                }

                string cueId = getId(cue);
                if (string.IsNullOrWhiteSpace(cueId))
                {
                    Debug.LogWarning($"{nameof(AudioClipLibrary)} on {name} has a {cueType} cue with a missing id at index {i}.");
                    continue;
                }

                if (lookup.ContainsKey(cueId))
                {
                    Debug.LogWarning($"{nameof(AudioClipLibrary)} on {name} has a duplicate {cueType} cue id '{cueId}'.");
                    continue;
                }

                lookup.Add(cueId, cue);
            }

            return lookup;
        }
    }
}
