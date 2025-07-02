using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookController : MonoBehaviour
{
    public Transform player;
    public float lookSmooth = 5f;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookSmooth);
    }
}
