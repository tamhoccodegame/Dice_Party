using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Speed at which the coin rotates around the Y-axis.")]
    public float rotationSpeed = 90f;

    void Update()
    {
        // Rotate the coin around its Y axis
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }


    public int value = 1;
    private float lifetime = 0f;
    [Tooltip("Optional pickup effect.")]
    public GameObject pickupVFX;
    

    [Tooltip("Auto destroy delay after pickup (for VFX to play).")]
    public float destroyDelay = 0.1f;

    private bool isCollected = false;


 

    private void Start()
    {
        if (lifetime > 0)
            Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")||isCollected) return;

        isCollected = true;

        Coin_Manager.Instance.AddCoins(value);

        if (pickupVFX != null)
            Instantiate(pickupVFX, transform.position, Quaternion.identity);

        // Hide the coin visuals immediately
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // Destroy after a short delay
        Destroy(gameObject, destroyDelay);
    }

    public void SetLifetime(float time)
    {
        lifetime = time;
    }

}
