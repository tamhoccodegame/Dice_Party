using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class ChestGoldNode : BoardNode
{
    private CharacterController controller;
    public Animator chest;
    private AudioSource audioSource;


    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override void ProcessNode(GameObject player, Transform playerTransform)
    {
        //if (TurnManager.instance.chestGolds[WizardPartyData.instance.currentChestIndex] != transform)
        //{
        //    base.ProcessNode(playerInput, playerTransform);
        //    return;
        //}

        if(processCoroutine == null)
        processCoroutine = StartCoroutine(ProcessCoroutine(player, playerTransform));
    }

    IEnumerator ProcessCoroutine(GameObject player, Transform playerTransform)
    {
        if (nodeEffect != null) nodeEffect.Play();
        NewBoardGameController controller = TurnManager.instance.playerControllers[player];
        this.controller = controller.GetComponent<CharacterController>(); 
        yield return new WaitForSecondsRealtime(0.5f);

        float elapsedTime = 0;
        float duration = 2f;

        while (elapsedTime < duration)
        {
            Vector3 direction = (chest.transform.position - this.controller.transform.position).normalized;
            Quaternion newRotation = Quaternion.LookRotation(direction);
            playerTransform.rotation = Quaternion.Slerp(this.controller.transform.rotation, newRotation, 15 * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        chest.Play("Open");
        audioSource.Play();
        yield return new WaitForSecondsRealtime(4f);
        chest.Play("Close");

        yield return new WaitForSecondsRealtime(1.5f);
        chest.Play("FlyUp");
        yield return new WaitForSecondsRealtime(1f);
        WizardPartyData.instance.UpdatePlayerCup(player, 1);
        TurnManager.instance.UpdatePlayerDataUI();

        yield return new WaitForSecondsRealtime(3f);
        this.controller.enabled = true;
        WizardPartyData.instance.isGoldChestOpened = true;

        if(controller.movingState.stepLeft > 0)
        {
            controller.SetCurrentNode(this);
            controller.ChangeState(controller.movingState);
        }
        else
        {
            EndTurn(player);
        }
        processCoroutine = null;
    }
}
