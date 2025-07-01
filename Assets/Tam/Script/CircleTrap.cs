using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FramerateMilestone
{
    public float time;
    public float speed;
}

public class CircleTrap : NetworkBehaviour
{
    public List<FramerateMilestone> framerateMilestones;

    public int currentMilestoneIndex = 0;

    [Networked] public float time { get; set; } = 0;

    public enum State
    {
        Spin,
        Chop,
        Chop2,
        Null,
    }

    public State cachedState;
    [Networked] public State state { get; set; }

    public Animator animator;

    public override void Spawned()
    {
        state = State.Null;
        InvokeRepeating(nameof(CountDown), 1f, 1f);
    }

    void CountDown()
    {
        if(!VongXoayManager.instance.isGameStarted || VongXoayManager.instance.isGameOver) return;
        time += 1;
        if(time >= framerateMilestones[currentMilestoneIndex].time && currentMilestoneIndex < framerateMilestones.Count)
        {
            currentMilestoneIndex++;
            animator.speed = framerateMilestones[currentMilestoneIndex].speed;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!VongXoayManager.instance.isGameStarted || VongXoayManager.instance.isGameOver) return;

        if (state == State.Null) TryChangeState();

        if (cachedState != state)
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
