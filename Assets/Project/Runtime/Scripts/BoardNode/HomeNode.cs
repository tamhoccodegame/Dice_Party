using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HomeNode : BoardNode
{
    public GameObject portalVFX;

    //Hàm này tất cả client đều chạy
    public override void ProcessNode(PlayerInput playerInput, Transform playerTransform)
    {
        StartCoroutine(ProcessCoroutine(playerInput, playerTransform));
    }

    IEnumerator ProcessCoroutine(PlayerInput playerInput, Transform playerTransform)
    {
        NewBoardGameController controller = TurnManager.instance.playerControllers[playerInput];
        yield return new WaitForSeconds(0.8f); // Delay nhẹ cho mượt
        nodeEffect.Play();
        //WizardPartyData.instance.UpdaPlayerHealth();
        yield return new WaitForSeconds(1f);
        EndTurn(playerInput);
    }
}
