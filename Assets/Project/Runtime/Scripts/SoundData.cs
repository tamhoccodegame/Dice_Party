using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundData
{
    public string name;

    public AudioClip clip;
    public AudioMixerGroup outputGroup;

    public bool loop = false;
    public bool playOnAwake = false;
    public bool is3D = false;

    [Range(0f, 1f)] public float volume = 1f;
    [Range(-3f, 3f)] public float pitch = 1f;
}
