using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Di chuyển theo spline riêng (Dùng Dreamteck.Splines).
//Kiểm tra có va chạm với Enemy khác không (sắp giao nhau).
//Điều chỉnh tốc độ nếu cần.
public class Wave_AI : MonoBehaviour
{
    public SplineFollower follower;
    public float speed = 5f;
    public LayerMask enemyLayer;
    public float rayLength = 1.5f;

    private bool isActive = false;

    private void Start()
    {
        follower.follow = false;
    }

    private void Update()
    {
        if (!isActive) return;

        bool obstacleInFront = DetectObstacle();
        follower.follow = !obstacleInFront;
        follower.followSpeed = follower.follow ? speed : 0;

        Debug.Log($"[{name}] Active={isActive}, Obstacle={obstacleInFront}, Pos={transform.position}");
    }

    public void SetActiveState(bool state)
    {
        isActive = state;

        if (state)
        {
            Debug.Log($"[{name}] --> Activated");
            follower.SetPercent(0f);
            follower.follow = true;
        }
        else
        {
            Debug.Log($"[{name}] --> Deactivated");
            follower.follow = false;
            follower.SetPercent(0f); // Reset lại vị trí đầu spline
        }
    }

    private bool DetectObstacle()
    {
        Vector3[] directions = new Vector3[]
        {
            transform.forward, -transform.forward,
            transform.right, -transform.right
        };

        foreach (var dir in directions)
        {
            if (Physics.Raycast(transform.position, dir, rayLength, enemyLayer))
            {
                Debug.DrawRay(transform.position, dir * rayLength, Color.red);
                return true;
            }
        }

        return false;
    }
}
