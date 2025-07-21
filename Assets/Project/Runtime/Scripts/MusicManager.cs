using System.Collections;
using UnityEngine;


public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AudioClip menuMusic;
    public AudioClip boardMusic;
    public AudioClip[] mngMusics;

    private AudioSource audioSource;

    public enum MusicType
    {
        Menu,
        Board,
        MNG,
    }

    public MusicType type;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);

        audioSource = GetComponent<AudioSource>();
        type = MusicType.Menu;
        PlayMusic(type); // Ban đầu phát nhạc Menu
    }

    public void PlayMusic(MusicType musicType, int mngIndex = 0)
    {
        if (musicType == type) return;
        StartCoroutine(SwitchTrack(musicType, mngIndex));
    }

    private IEnumerator SwitchTrack(MusicType newType, int mngIndex)
    {
        yield return StartCoroutine(FadeOut());

        switch (newType)
        {
            case MusicType.Menu:
                audioSource.clip = menuMusic;
                break;
            case MusicType.Board:
                audioSource.clip = boardMusic;
                break;
            case MusicType.MNG:
                if (mngIndex >= 0 && mngIndex < mngMusics.Length)
                    audioSource.clip = mngMusics[mngIndex];
                break;
        }

        type = newType;

        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0f)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    private IEnumerator FadeIn()
    {
        audioSource.volume = 0f;
        audioSource.Play();

        while (audioSource.volume < 1f)
        {
            audioSource.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.volume = 1f;
    }
}
