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

    public override void ProcessNode(PlayerInput playerInput, Transform playerTransform)
    {
        if(processCoroutine == null)
        processCoroutine = StartCoroutine(ProcessCoroutine(playerInput, playerTransform));
    }

    IEnumerator ProcessCoroutine(PlayerInput playerInput, Transform playerTransform)
    {
        if (nodeEffect != null) nodeEffect.Play();
        NewBoardGameController controller = TurnManager.instance.playerControllers[playerInput];
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
        WizardPartyData.instance.UpdatePlayerCup(playerInput, 1);
        TurnManager.instance.UpdatePlayerDataUI();

        yield return new WaitForSecondsRealtime(1.5f);
        chest.Play("FlyUp");

        yield return new WaitForSecondsRealtime(1f);
        this.controller.enabled = true;
        WizardPartyData.instance.isGoldChestOpened = true;

        if(controller.StepsLeft > 0)
        {
            controller.ChangeState(controller.movingState);
        }
        else
        {
            EndTurn(playerInput);
        }
        processCoroutine = null;
    }
}
