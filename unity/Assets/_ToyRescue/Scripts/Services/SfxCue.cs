using System;
using UnityEngine;

namespace ToyRescue.Services
{
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
