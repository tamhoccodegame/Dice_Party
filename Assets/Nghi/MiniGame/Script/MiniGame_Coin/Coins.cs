using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Fusion;

public class Coins : NetworkBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Speed at which the coin rotates around the Y-axis.")]
    public float rotationSpeed = 90f;

    public override void FixedUpdateNetwork()
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

    [Networked, UnitySerializeField] private bool isCollected { get; set; } = false;
    [Networked, UnitySerializeField] private bool canBeCollected { get; set; } = true;

    public override void Spawned()
    {
        if (!HasStateAuthority) return;

        if (lifetime > 2)
            Destroy(gameObject, lifetime);
    }  

    public void EatCoin(NetworkId eater)
    {
        if(isCollected || !canBeCollected) return;  

        isCollected = true;

        Coin_Manager.Instance.RequestUpdateCoin(Runner.FindObject(eater).GetComponent<NetworkObject>().InputAuthority, value);

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
        yield return new WaitForSecondsRealtime(delay);
        canBeCollected = true;
    }
}
