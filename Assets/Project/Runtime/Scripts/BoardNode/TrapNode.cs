using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrapNode : BoardNode
{
    public Animator theDeath;

    //Hàm này tất cả client đều chạy
    public override void ProcessNode(PlayerInput playerInput, Transform playerTransform)
    {
        StartCoroutine(ProcessCoroutine(playerInput, playerTransform));
    }

    IEnumerator ProcessCoroutine(PlayerInput playerInput, Transform playerTransform)
    {
        theDeath.Play("Attack");
        NewBoardGameController controller = TurnManager.instance.playerControllers[playerInput];
        yield return new WaitForSeconds(0.8f); // Delay nhẹ cho mượt
        nodeEffect.gameObject.SetActive(true);
        controller.EnableRagdoll();
        yield return new WaitForSeconds(0.3f); // Delay nhẹ cho mượt
        EndTurn(playerInput);
    }
}

