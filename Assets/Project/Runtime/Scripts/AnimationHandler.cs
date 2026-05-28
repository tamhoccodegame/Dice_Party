using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    public Animator animator;
    public string animToPlay;

    // Start is called before the first frame update
    void Start()
    {
        if(animToPlay != null)
            animator.Play(animToPlay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
