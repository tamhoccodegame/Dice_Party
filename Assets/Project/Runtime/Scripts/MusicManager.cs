using System.Collections;
using UnityEngine;


public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    private AudioSource audioSource;
    public AudioClip mainTheme;

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
        PlayMusic(mainTheme);
    }

    public void PlayMusic(AudioClip newClip)
    {
        //if (audioSource.clip == newClip) return;
        StartCoroutine(SwitchTrack(newClip));
    }

    public void PlayMainTheme()
    {
        StartCoroutine(SwitchTrack(mainTheme));
    }


    private IEnumerator SwitchTrack(AudioClip newClip)
    {
        yield return StartCoroutine(FadeOut());

        audioSource.clip = newClip;

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
