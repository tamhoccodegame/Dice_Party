using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HomeNode : BoardNode
{
    public BoardNode home;
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
        portalVFX.SetActive(true);
        controller.gameObject.GetComponent<CharacterController>().enabled = false;
        controller.gameObject.AddComponent<Rigidbody>();
        CameraFollow.instance.StartFollowTarget(transform);
        //WizardPartyData.instance.UpdaPlayerHealth();
        yield return new WaitForSeconds(1f);
        portalVFX.SetActive(false);
        yield return new WaitForSeconds(1f);
        Destroy(controller.gameObject.GetComponent<Rigidbody>());
        controller.gameObject.transform.position = home.transform.position;
        controller.SetCurrentNode(home);
        controller.gameObject.transform.GetComponent<CharacterController>().enabled = true;
        CameraFollow.instance.StartFollowTarget(controller.transform);
        yield return new WaitForSeconds(2.5f);

        EndTurn(playerInput);
    }
}
