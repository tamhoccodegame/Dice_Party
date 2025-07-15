using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tire_Movement_Spline : MonoBehaviour
{
    public enum TireMode
    {
        Classic,
        RollingBar
    }

    [Header("Spline Settings")]
    public SplineFollower follower;
    public bool loopBack = false; // Tick để bật chế độ qua lại

    [Header("Tire Settings")]
    public TireMode mode = TireMode.RollingBar;
    public float wheelRadius = 0.35f;
    public Transform wheelMesh;

    [Header("Despawn")]
    public bool destroyOnFinish = false;

    private Vector3 lastPos;
    private bool goingBack = false;

    //void Start()
    //{
    //    if (wheelMesh == null || follower == null)
    //    {
    //        Debug.LogError("Missing wheel mesh or spline follower.");
    //        enabled = false;
    //        return;
    //    }

    //    lastPos = transform.position;

    //    // Event khi tới cuối
    //    follower.onEndReached += OnEndReached;
    //}

    void Start()
    {
        if (wheelMesh == null || follower == null)
        {
            Debug.LogError("Missing wheel mesh or spline follower.");
            enabled = false;
            return;
        }

        lastPos = transform.position;

        if (loopBack)
        {
            follower.onEndReached += (_) => ToggleDirection();
            follower.onBeginningReached += (_) => ToggleDirection();
        }
        else if (destroyOnFinish)
        {
            follower.onEndReached += (_) => Destroy(gameObject);
        }
    }

    void ToggleDirection()
    {
        StartCoroutine(SmoothReverse());
    }

    IEnumerator SmoothReverse()
    {
        yield return new WaitForSeconds(0.1f);
        goingBack = !goingBack;
        follower.direction = goingBack ? Spline.Direction.Backward : Spline.Direction.Forward;
    }



    void Update()
    {
        RotateMesh();
    }

    void RotateMesh()
    {
        Vector3 delta = transform.position - lastPos;
        float dist = delta.magnitude;
        if (dist < 0.001f) return;

        float angle = (dist / (2 * Mathf.PI * wheelRadius)) * 360f;
        Vector3 rotAxis = (mode == TireMode.RollingBar) ? transform.right :
                          Vector3.Cross(delta.normalized, Vector3.up).normalized;

        wheelMesh.Rotate(rotAxis, angle, Space.World);
        lastPos = transform.position;
    }

    void OnEndReached(double _) // Parameter unused
    {
        if (loopBack)
        {
            goingBack = !goingBack;
            follower.direction = goingBack ? Spline.Direction.Backward : Spline.Direction.Forward;
        }
        else if (destroyOnFinish)
        {
            Destroy(gameObject);
            // Optionally spawn new trap here
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            var player = collision.collider.GetComponent<PlayerController_N>() ??
                         collision.collider.GetComponentInParent<PlayerController_N>();

            if (player != null)
            {
                Vector3 hitPoint = collision.contacts[0].point;
                player.OnHitByObstacle(hitPoint);
            }
        }
    }
}
