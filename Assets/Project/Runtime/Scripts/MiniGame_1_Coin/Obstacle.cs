using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController_N>() ??
                         other.GetComponentInParent<PlayerController_N>();

            if (player != null)
            {
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                player.OnHitByObstacle(hitPoint);
            }
        }
    }
}
