using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    public Animator animator;
    public string animToPlay;

    public string[] anims;

    // Start is called before the first frame update
    void Start()
    {
        if(!string.IsNullOrEmpty(animToPlay))
            animator.Play(animToPlay);
        else
        {
            int randomAnim = Random.Range(0, anims.Length);
            animator.Play(anims[randomAnim]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
