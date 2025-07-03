using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    private float lifetime = 2f;
    [Tooltip("Optional pickup effect.")]
    public GameObject pickupVFX;

    //[Header("SFX")]
    //public AudioClip pickupSFX;


    [Tooltip("Auto destroy delay after pickup (for VFX to play).")]
    public float destroyDelay = 0.1f;

    private bool isCollected = false;
    private bool canBeCollected = false;

    private void Start()
    {
        if (lifetime > 2)
            Destroy(gameObject, lifetime);

        StartCoroutine(EnablePickupAfterDelay(0.8f));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player") || isCollected || !canBeCollected) return;

        isCollected = true;

        Coin_Manager.Instance.AddCoins(value);

        if (pickupVFX != null)
            Instantiate(pickupVFX, transform.position, Quaternion.identity);

        Audio_Manager.Instance.Play("Coin_PickUp", transform.position);


        // Hide the coin visuals immediately
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, destroyDelay);
    }


    public void SetLifetime(float time)
    {
        lifetime = time;
    }

    private IEnumerator EnablePickupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canBeCollected = true;
    }
}
