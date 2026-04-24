using System;
using UnityEngine;

namespace ToyRescue.Services
{
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
}
