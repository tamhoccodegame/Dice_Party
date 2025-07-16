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
    public NetworkObject pickupVFX;

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
            Runner.Spawn(pickupVFX, transform.position, Quaternion.identity);

        Audio_Manager.Instance.Play("Coin_PickUp", transform.position);


        // Hide the coin visuals immediately
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        RPC_Destroy();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_Destroy()
    {
        Destroy(gameObject, destroyDelay);
    }

    public void SetLifetime(float time)
    {
        if (HasStateAuthority) return;
        lifetime = time;
        canBeCollected = false;
        StartCoroutine(EnablePickupAfterDelay(0.9f));
    }

    private IEnumerator EnablePickupAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        canBeCollected = true;
    }
}
