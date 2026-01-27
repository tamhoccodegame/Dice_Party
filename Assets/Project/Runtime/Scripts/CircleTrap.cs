using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FramerateMilestone
{
    public float time;
    public float speed;
}

public class CircleTrap : MonoBehaviour
{
    public List<FramerateMilestone> framerateMilestones;

    public int currentMilestoneIndex = 0;

    public Animator wizardAnimator;

    public AudioSource engineSound;
    public AudioSource bladeSound;

    public void PlaySound(string soundName)
    {
        if (soundName == "Engine") engineSound.Play();
        else if (soundName == "Blade") bladeSound.Play();
    }

    public float time { get; set; } = 0;

    public enum State
    {
        Spin,
        Chop,
        Chop2,
        Null,
    }

    public State cachedState;
    public State state { get; set; }

    public Animator animator;

    public void Awake()
    {
    }

    private void Start()
    {
        StartCoroutine(WaitToStart());
    }

    IEnumerator WaitToStart()
    {
        while(!VongXoayManager.instance.isGameStarted || VongXoayManager.instance.isGameOver) yield return null;

        yield return new WaitForSecondsRealtime(1f);

        TryChangeState();
        InvokeRepeating(nameof(CountDown), 1f, 1f);
    }

    void CountDown()
    {
        if (!VongXoayManager.instance.isGameStarted || VongXoayManager.instance.isGameOver) return;
        time += 1;
        if(time >= framerateMilestones[currentMilestoneIndex].time && currentMilestoneIndex < framerateMilestones.Count - 1)
        {
            currentMilestoneIndex++;
            animator.speed = framerateMilestones[currentMilestoneIndex].speed;
        }
    }
    private void Update()
    {
        //if (VongXoayManager.instance == null) return; 
        //if (!VongXoayManager.instance.isGameStarted || VongXoayManager.instance.isGameOver) return;
    }

    public void TryChangeState()
    {
        StartCoroutine(TryChangeStateCoroutine());
    }

    IEnumerator TryChangeStateCoroutine()
    {
        state = (State)Random.Range(0, 3);
        if (state != cachedState)
        {
            cachedState = state;
            //wizardAnimator.CrossFade("Cast", 0.25f);
            yield return new WaitForSeconds(0f);
            animator.CrossFade("PhaseTransition", 0.1f);
            wizardAnimator.CrossFade("Idle", 0.25f);
            StartCoroutine(ChangeStateCoroutine());
        }
        else
        {
            animator.CrossFade("PhaseTransition", 0.1f);
            StartCoroutine(ChangeStateCoroutine());
        }
    }

    IEnumerator ChangeStateCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.8f);
        ChangeAnimation();
    
    }

    void ChangeAnimation()
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
