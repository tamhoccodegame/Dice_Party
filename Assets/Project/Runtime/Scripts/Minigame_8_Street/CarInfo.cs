using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInfo : MonoBehaviour
{
    [Header("Behaviour (tweak these)")]
    public float baseSpeed = 10f;     // v0
    public float acceleration = 2.5f;    // a (comfortable accel)
    public float deceleration = 6f;      // b (comfortable decel)
    public float minGap = 2f;            // s0 (minimum gap when stopped) (center-to-center offset minus length)
    public float timeHeadway = 1.1f;     // T (time headway)
    public float delta = 4f;             // exponent in IDM (usually 4)

    [Header("Vehicle geometry")]
    public float vehicleLength = 4f;     // dùng để tính center-to-center gap

    [Header("Runtime (debug)")]
    public float currentSpeed = 0f;
    public CarInfo carAhead = null; // ref tới xe phía trước trong cùng lane

    [Header("Emergency")]
    public float emergencyClampMargin = 0.05f; // bớt chút để chắc chắn không xuyên

    private void Awake()
    {
        // khởi tạo một xíu randomness cho feeling tự nhiên
        currentSpeed = Mathf.Clamp(baseSpeed * Random.Range(0.85f, 1.05f), 0f, baseSpeed * 1.1f);
    }

    /// <summary>
    /// Called by TrafficManager each frame (in leader->follower order).
    /// dir: movement direction (normalized).
    /// dt: deltaTime (use Time.deltaTime).
    /// Returns true if movement performed.
    /// </summary>
    public void Step(Vector3 dir, float dt)
    {
        // Lead info
        float leadSpeed = 0f;
        float centerDist = float.PositiveInfinity;

        if (carAhead != null)
        {
            leadSpeed = carAhead.currentSpeed;
            // center-to-center distance projected along lane direction
            centerDist = Vector3.Dot(dir, carAhead.transform.position - transform.position);
        }

        // actual gap available (center-to-center minus vehicleLength)
        float sActual = (float.IsInfinity(centerDist) ? 1e6f : Mathf.Max(centerDist - vehicleLength, 0.001f));

        // relative speed
        float deltaV = currentSpeed - leadSpeed;

        // desired dynamic spacing s*
        float sqrtTerm = 2f * Mathf.Sqrt(acceleration * deceleration);
        float sStar = minGap + currentSpeed * timeHeadway + (currentSpeed * deltaV) / Mathf.Max(sqrtTerm, 0.01f);
        sStar = Mathf.Max(minGap, sStar);

        // IDM acceleration
        float accelCmd = acceleration * (1f - Mathf.Pow(currentSpeed / Mathf.Max(baseSpeed, 0.001f), delta) - Mathf.Pow(sStar / Mathf.Max(sActual, 0.001f), 2f));

        if (float.IsNaN(accelCmd) || float.IsInfinity(accelCmd))
            accelCmd = -deceleration; // fail-safe

        // clamp comfortable braking
        accelCmd = Mathf.Clamp(accelCmd, -deceleration, acceleration);

        // integrate speed
        currentSpeed += accelCmd * dt;
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, baseSpeed * 1.2f);

        // predict new position
        Vector3 proposedPos = transform.position + dir * (currentSpeed * dt);

        // if there is a leader, ensure we don't cross min allowed center distance
        if (carAhead != null)
        {
            float newCenterDist = Vector3.Dot(dir, carAhead.transform.position - proposedPos);
            float minCenterDist = minGap + vehicleLength;

            if (newCenterDist < minCenterDist + emergencyClampMargin)
            {
                // clamp position to keep min gap
                proposedPos = carAhead.transform.position - dir * (minCenterDist + emergencyClampMargin);

                // match/slow down to leader to avoid pushing
                currentSpeed = Mathf.Min(currentSpeed, carAhead.currentSpeed);
                // safety extra braking if still too close
                currentSpeed = Mathf.Max(0f, currentSpeed - deceleration * dt * 1.5f);
            }
        }

        // commit
        transform.position = proposedPos;

        // smooth rotation to direction
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), Mathf.Clamp01(dt * 8f));
    }

    // Helper to quickly set parameters (called by manager at spawn)
    public void Configure(float desiredSpd, float minGap_, float headway, float accel_, float decel_, float length)
    {
        baseSpeed = desiredSpd;
        minGap = minGap_;
        timeHeadway = headway;
        acceleration = accel_;
        deceleration = decel_;
        vehicleLength = length;
        // initialize currentSpeed near desired so cars don't teleport
        currentSpeed = Mathf.Clamp(baseSpeed * Random.Range(0.9f, 1.05f), 0f, baseSpeed * 1.1f);
    }

    // Debug gizmos
    private void OnDrawGizmosSelected()
    {
        // draw a forward line representing dynamic safe distance at current speed
        Gizmos.color = Color.yellow;
        Vector3 fwd = transform.forward;
        float dynSafe = minGap + currentSpeed * timeHeadway + 0.1f;
        Gizmos.DrawLine(transform.position, transform.position + fwd * (dynSafe + vehicleLength));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerInteractMoneyController>(out var player))
        {
            if (!player.isFalling)
            {
                player.LoseOneBag();
            }
        }
    }
}
