using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Audio;

namespace UnityUtils
{
    [Serializable]
    public class AudioData
    {
        [ReadOnly] public int id;
        [ReadOnly] public string name;
        public AudioClip clip;
        public AudioMixerGroup group;
        [Range(0, 1)] public float volume = 1f;
        public bool loop;
        public bool randomPitch;
        [ShowIf(nameof(randomPitch)), MinMaxSlider(-3, 3, true)] public Vector2 pitchRange = new(0.8f, 1.2f);
    }
}