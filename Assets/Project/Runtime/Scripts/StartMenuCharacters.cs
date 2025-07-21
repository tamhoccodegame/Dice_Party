using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuCharacters : MonoBehaviour
{
    private Animator[] animators;
    private string[] animatorName = { "Hand", "Run", "Dance01", "Dance02" };

    // Start is called before the first frame update
    void Start()
    {
        animators = GetComponentsInChildren<Animator>();
        for (int i = 0; i < animatorName.Length; i++)
        {
            animators[i].Play(animatorName[i]);
        }
    }
}
