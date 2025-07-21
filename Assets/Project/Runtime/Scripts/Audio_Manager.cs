using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Manager : MonoBehaviour
{
    public static Audio_Manager Instance;

    [Header("🎵 All Audio Clips")]
    public List<SoundData> sounds;

    private Dictionary<string, SoundData> soundDict;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        soundDict = new Dictionary<string, SoundData>();

        foreach (var s in sounds)
        {
            if (!soundDict.ContainsKey(s.name))
                soundDict.Add(s.name, s);

            if (s.playOnAwake)
                Play(s.name, transform.position);
        }
    }

    public void Play(string soundName, Vector3 position)
    {
        if (!soundDict.ContainsKey(soundName))
        {
            Debug.LogWarning($"[AudioManager] ❌ Sound '{soundName}' not found!");
            return;
        }

        SoundData s = soundDict[soundName];

        GameObject go = new GameObject($"[SFX]_{s.name}");
        go.transform.position = position;

        AudioSource src = go.AddComponent<AudioSource>();
        src.clip = s.clip;
        src.volume = s.volume;
        src.pitch = s.pitch;
        src.loop = s.loop;
        src.outputAudioMixerGroup = s.outputGroup;
        src.spatialBlend = s.is3D ? 1f : 0f;

        src.Play();

        if (!s.loop)
            Destroy(go, s.clip.length + 0.1f);
    }

    public void Play2D(string soundName)
    {
        Play(soundName, Camera.main.transform.position);
    }

    public void Stop(string soundName)
    {
        var go = GameObject.Find($"[SFX]_{soundName}");
        if (go) Destroy(go);
    }

    public void StopAll()
    {
        foreach (var obj in GameObject.FindObjectsOfType<AudioSource>())
        {
            if (obj.gameObject.name.StartsWith("[SFX]_"))
                Destroy(obj.gameObject);
        }
    }
}
