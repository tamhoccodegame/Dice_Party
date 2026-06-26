using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;

    public Animator animator;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayStartLoad()
    {
        animator.Play("StartLoad");
    }

    public void LoadScene(string sceneName)
    {
        animator.Play("StartLoad");
        StartCoroutine(WaitToLoad(sceneName));
    }

    IEnumerator WaitToLoad(string sceneName)
    {
        yield return new WaitForSeconds(3f);

        yield return SceneManager.LoadSceneAsync(sceneName);

        yield return Resources.UnloadUnusedAssets();

        System.GC.Collect();

        animator.Play("EndLoad");
    }


}
