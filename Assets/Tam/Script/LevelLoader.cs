using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;

    public Animator animator;

    private void Awake()
    {
        instance = this;
    }

    public void StartLoad()
    {
        animator.Play("StartLoad");
    }

}
