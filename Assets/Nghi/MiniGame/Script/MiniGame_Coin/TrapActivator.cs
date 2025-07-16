using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapActivator : MonoBehaviour
{
    public float activationDistance = 20f;
    private Transform player;

    public void Awake()
    {
        gameObject.SetActive(false);
    }
    public void Init(Transform playerRef)
    {
        player = playerRef;
    }

    public void CheckActivation()
    {

        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);
        bool shouldBeActive = distance <= activationDistance;

        if (shouldBeActive && !gameObject.activeSelf)
            gameObject.SetActive(true);
        else if (!shouldBeActive && gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
