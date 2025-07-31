using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy__Controller : MonoBehaviour
{
    private Vector3 moveDirection;
    private float moveSpeed;
    private bool active = false;

    public void Init(Vector3 direction, float speed)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        active = true;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!active) return;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active) return;
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Hit!");
            // Gọi hàm chết hoặc knockback của Player
        }
    }

    public void Deactivate()
    {
        active = false;
        gameObject.SetActive(false);
    }
}
