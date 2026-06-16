using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HealNode : BoardNode
{
    //Hàm này tất cả client đều chạy
    public override void ProcessNode(GameObject player, Transform playerTransform)
    {
        StartCoroutine(ProcessCoroutine(player, playerTransform));
    }

    IEnumerator ProcessCoroutine(GameObject player, Transform playerTransform)
    {
        NewBoardGameController controller = TurnManager.instance.playerControllers[player];
        yield return new WaitForSeconds(0.8f); // Delay nhẹ cho mượt
        if(nodeEffect != null)
        nodeEffect.Play();
        WizardPartyData.instance.UpdatePlayerHealth(player, 10);
        TurnManager.instance.UpdatePlayerDataUI();
        yield return new WaitForSeconds(1f);
        EndTurn(player);
    }
}
