using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    public Animation anima;
    public int animToPlay;
    public bool isRandom = false;

    private AnimationState[] states;

    void Start()
    {
        // Lấy tất cả state trong Animation component
        states = new AnimationState[anima.GetClipCount()];
        Debug.Log(states.Length);

        int i = 0;
        foreach (AnimationState state in anima)
        {
            states[i] = state;
            i++;
        }

        if (!isRandom)
        {
            if (animToPlay >= 0 && animToPlay < states.Length)
            {
                anima.Play(states[animToPlay].name);
            }
        }
        else
        {
            int randomAnim = Random.Range(0, states.Length);
            anima.Play(states[randomAnim].name);
        }
    }
}
