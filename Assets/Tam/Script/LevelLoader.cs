using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : NetworkBehaviour
{
    public static LevelLoader instance;

    public Animator animator;

    private void Awake()
    {
        instance = this;
    }

    public void PlayStartLoad()
    {
        animator.Play("StartLoad");
    }

    public void LoadScene(string sceneName)
    {
        animator.Play("StartLoad");
        if(HasStateAuthority) StartCoroutine(WaitToLoad(sceneName));
    }

    IEnumerator WaitToLoad(string sceneName)
    {
        yield return new WaitForSeconds(3f);
        Runner.LoadScene(sceneName);
    }


}
