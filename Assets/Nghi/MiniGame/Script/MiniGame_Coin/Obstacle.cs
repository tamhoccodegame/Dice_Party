using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Obstacle : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority) return;
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerBlinking>() ??
                         other.GetComponentInParent<PlayerBlinking>();

            if (player != null)
            {
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                player.OnHitByObstacle(hitPoint);
            }
        }
    }
}
