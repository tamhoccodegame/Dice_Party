using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ChestGoldNode : BoardNode
{
    private CharacterController controller;
    public Animator chest;
    private AudioSource audioSource;


    //public override void Spawned()
    //{
    //    if (!TurnManager.instance.isFirstTry)
    //    {
    //        chest.Play("FlyDown");
    //    }
    //    audioSource = GetComponent<AudioSource>();
    //}

    //public override void ProcessNode(PlayerRef playerRef, NetworkId playerObject)
    //{
    //    if (HasStateAuthority)
    //    {
    //        RPC_ChestGoldEffect(playerRef, playerObject);
    //    }
    //}

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    //void RPC_ChestGoldEffect(PlayerRef playerRef, NetworkId playerObject)
    //{
    //    StartCoroutine(ProcessCoroutine(playerRef, playerObject));
    //}

    //IEnumerator ProcessCoroutine(PlayerRef playerRef, NetworkId playerObject)
    //{
    //    controller = Runner.FindObject(playerObject).GetComponent<NetworkCharacterController>();
    //    controller.enabled = false;

    //    if (nodeEffect != null) nodeEffect.Play();
    //    yield return new WaitForSecondsRealtime(0.5f);

    //    float elapsedTime = 0;
    //    float duration = 2f;

    //    while (elapsedTime < duration)
    //    {
    //        Vector3 direction = (chest.transform.position - controller.transform.position).normalized;
    //        Quaternion newRotation = Quaternion.LookRotation(direction);
    //        controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, newRotation, 5 * Time.deltaTime);
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    chest.Play("Open");
    //    audioSource.Play(); 
    //    yield return new WaitForSecondsRealtime(4f);
    //    chest.Play("Close");
    //    TurnManager.instance.RequestUpdateCup(playerRef, 1);
    //    yield return new WaitForSecondsRealtime(1.5f);
    //    chest.Play("FlyUp");

    //    yield return new WaitForSecondsRealtime(1f);
    //    controller.enabled = true;
    //    EndTurn(playerRef);
    //}
}
