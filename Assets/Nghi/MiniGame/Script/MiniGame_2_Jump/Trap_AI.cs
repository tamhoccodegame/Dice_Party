using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_AI : MonoBehaviour
{
    public SplineFollower follower;
    private List<Trap_AI> allTraps;
    private float startDelay;
    private bool isActive = false;
    private string trapName;
    private bool isWaiting = false;

    public float raycastDistance = 1.5f; // Khoảng cách để check trap phía trước

    public void Setup(SplineFollower f, List<Trap_AI> all, float delay, string name)
    {
        follower = f;
        allTraps = all;
        startDelay = delay;
        trapName = name;
        follower.follow = false;
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(startDelay);
        isActive = true;
        follower.follow = true;
    }

    void Update()
    {
        if (!isActive || follower == null || isWaiting) return;

        Vector3 dir = follower.direction == Spline.Direction.Forward ? follower.result.forward : -follower.result.forward;
        Ray ray = new Ray(transform.position, dir);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            Trap_AI other = hit.collider.GetComponent<Trap_AI>();
            if (other != null && other != this)
            {
                StartCoroutine(HandleTrapAvoidance());
            }
        }
    }

    IEnumerator HandleTrapAvoidance()
    {
        isWaiting = true;
        follower.follow = false;
        yield return new WaitForSeconds(1f);
        ReverseDirection();
        follower.follow = true;
        isWaiting = false;
    }

    void ReverseDirection()
    {
        follower.direction = (follower.direction == Spline.Direction.Forward) ? Spline.Direction.Backward : Spline.Direction.Forward;
    }


    //public SplineFollower follower;
    //private List<Trap_AI> allTraps;
    //private float startDelay;
    //private bool isActive = false;
    //private string trapName;

    //public void Setup(SplineFollower f, List<Trap_AI> all, float delay, string name)
    //{
    //    follower = f;
    //    allTraps = all;
    //    startDelay = delay;
    //    trapName = name;
    //    follower.follow = false;
    //    StartCoroutine(DelayedStart());
    //}

    //IEnumerator DelayedStart()
    //{
    //    yield return new WaitForSeconds(startDelay);
    //    isActive = true;
    //    follower.follow = true;
    //}

    //void Update()
    //{
    //    if (!isActive || follower == null) return;

    //    foreach (var other in allTraps)
    //    {
    //        if (other == this || !other.isActive) continue;

    //        double percentA = follower.GetPercent();
    //        double percentB = other.follower.GetPercent();
    //        float dist = Mathf.Abs((float)(percentA - percentB));

    //        if (dist < 0.05f) // Gần nhau quá
    //        {
    //            // Tạm dừng và đổi hướng
    //            follower.follow = false;
    //            Invoke(nameof(ResumeFollow), 0.5f);
    //            follower.direction = (follower.direction == Spline.Direction.Forward) ? Spline.Direction.Backward : Spline.Direction.Forward;
    //            break;
    //        }
    //    }
    //}

    //void ResumeFollow()
    //{
    //    follower.follow = true;
    //}
}
