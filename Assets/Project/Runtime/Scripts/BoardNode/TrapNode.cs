using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrapNode : BoardNode
{
    public Animator theDeath;

    //Hàm này tất cả client đều chạy
    public override void ProcessNode(GameObject player, Transform playerTransform)
    {
        StartCoroutine(ProcessCoroutine(player, playerTransform));
    }

    IEnumerator ProcessCoroutine(GameObject player, Transform playerTransform)
    {
        theDeath.Play("Attack");
        NewBoardGameController controller = TurnManager.instance.playerControllers[player];
        yield return new WaitForSeconds(0.8f); // Delay nhẹ cho mượt
        WizardPartyData.instance.UpdatePlayerHealth(player, -10);
        TurnManager.instance.UpdatePlayerDataUI();
        nodeEffect.gameObject.SetActive(true);
        controller.EnableRagdoll();
        yield return new WaitForSeconds(5f);
        TurnManager.instance.NextTurn();
    }
}

