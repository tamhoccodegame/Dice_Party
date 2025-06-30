using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleTrap : NetworkBehaviour
{
    public float changeFateCooldown = 8f;

    public enum State
    {
        Spin,
        Chop,
        Chop2
    }

    public State cachedState;
    [Networked] public State state { get; set; }

    public Animator animator;

    public override void FixedUpdateNetwork()
    {
        if(cachedState != state)
        {
            cachedState = state;
            switch (state)
            {
                case State.Spin:
                    animator.CrossFade("SpinBase", 0.1f);
                    break;
                case State.Chop:
                    animator.CrossFade("Chop", 0.1f);
                    break;
                case State.Chop2:
                    animator.CrossFade("Chop2", 0.1f);
                    break;
            }
        }
    }

    public void TryChangeState()
    {
        if (HasStateAuthority)
        {
            StartCoroutine(ChangeStateCoroutine());
        }
    }

    IEnumerator ChangeStateCoroutine()
    {
        animator.CrossFade("PhaseTransition", 0.1f);
        yield return new WaitForSeconds(0.8f);

        state = (State)Random.Range(0, 3);
        if(state == cachedState)
        {
            switch (state)
            {
                case State.Spin:
                    animator.CrossFade("SpinBase", 0.1f);
                    break;
                case State.Chop:
                    animator.CrossFade("Chop", 0.1f);
                    break;
                case State.Chop2:
                    animator.CrossFade("Chop2", 0.1f);
                    break;
            }
        }
    }
}
