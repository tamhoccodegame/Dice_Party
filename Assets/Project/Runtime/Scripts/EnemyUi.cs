using Codice.Client.BaseCommands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUi : MonoBehaviour
{
    public float moveSpeed;

    public Rigidbody body;

    private void Start()
    {
    }

    private void Update()
    {
        Vector3 vel = transform.forward * moveSpeed;
        vel.y = body.velocity.y; // giữ lại vận tốc Y hiện tại (do gravity)
        body.velocity = vel;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("TrafficStop"))
        {
            StartCoroutine(DelayMove());
        }
    }

    IEnumerator DelayMove()
    {
        float speed = moveSpeed;
        moveSpeed = 0;
        yield return new WaitForSeconds(1.5f);
        moveSpeed = speed;
    }
}
