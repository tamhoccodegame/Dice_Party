using System.Collections;
using UnityEngine;

public class QuatVaCham : MonoBehaviour
{
    bool isColliding = false;

    void OnTriggerEnter(Collider hitObject)
    {
        if (hitObject.TryGetComponent<PlayerBlinking>(out var player))
        {
            player.OnHitByObstacle(hitObject.ClosestPoint(transform.position));    
        }
    }
}
