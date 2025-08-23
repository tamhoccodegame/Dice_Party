using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlusNode : BoardNode
{
    //Hàm này tất cả client đều chạy
    public override void ProcessNode(PlayerInput playerInput, Transform playerTransform)
    {
        StartCoroutine(ProcessCoroutine(playerInput, playerTransform));
    }

    IEnumerator ProcessCoroutine(PlayerInput playerInput, Transform playerTransform)
    {
        NewBoardGameController controller = TurnManager.instance.playerControllers[playerInput];
        if(nodeEffect != null) 
        nodeEffect.Play();
        yield return new WaitForSeconds(0.2f);
        controller.SetStepLeft(3);
        controller.ChangeState(controller.movingState);
        yield return null;
    }
}
