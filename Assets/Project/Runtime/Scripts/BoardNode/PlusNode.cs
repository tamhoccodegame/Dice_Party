using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlusNode : BoardNode
{
    //Hàm này tất cả client đều chạy
    public override void ProcessNode(GameObject player, Transform playerTransform)
    {
        StartCoroutine(ProcessCoroutine(player, playerTransform));
    }

    IEnumerator ProcessCoroutine(GameObject player, Transform playerTransform)
    {
        NewBoardGameController controller = TurnManager.instance.playerControllers[player];
        if(nodeEffect != null) 
        nodeEffect.Play();
        yield return new WaitForSeconds(0.2f);
        controller.SetStepLeft(3);
        controller.ChangeState(controller.movingState);
        yield return null;
    }
}
