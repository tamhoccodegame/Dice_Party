using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HealNode : BoardNode
{
    //Hàm này tất cả client đều chạy
    public override void ProcessNode(PlayerInput playerInput, Transform playerTransform)
    {
        StartCoroutine(ProcessCoroutine(playerInput, playerTransform));
    }

    IEnumerator ProcessCoroutine(PlayerInput playerInput, Transform playerTransform)
    {
        NewBoardGameController controller = TurnManager.instance.playerControllers[playerInput];
        yield return new WaitForSeconds(0.8f); // Delay nhẹ cho mượt
        if(nodeEffect != null)
        nodeEffect.Play();
        WizardPartyData.instance.UpdatePlayerHealth(playerInput, 10);
        TurnManager.instance.UpdatePlayerDataUI();
        yield return new WaitForSeconds(1f);
        EndTurn(playerInput);
    }
}
