using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HomeNode : BoardNode
{
    public BoardNode home;
    public GameObject portalVFX;

    //Hàm này tất cả client đều chạy
    public override void ProcessNode(GameObject player, Transform playerTransform)
    {
        StartCoroutine(ProcessCoroutine(player, playerTransform));
    }

    IEnumerator ProcessCoroutine(GameObject player, Transform playerTransform)
    {
        NewBoardGameController controller = TurnManager.instance.playerControllers[player];
        yield return new WaitForSeconds(0.8f); // Delay nhẹ cho mượt
        portalVFX.SetActive(true);
        controller.gameObject.GetComponent<CharacterController>().enabled = false;
        controller.gameObject.AddComponent<Rigidbody>();
        CameraFollow.instance.StartFollowTarget(transform);
        yield return new WaitForSeconds(1f);
        portalVFX.SetActive(false);
        yield return new WaitForSeconds(1f);
        Destroy(controller.gameObject.GetComponent<Rigidbody>());
        controller.gameObject.transform.position = home.transform.position;
        controller.SetCurrentNode(home);
        controller.gameObject.transform.GetComponent<CharacterController>().enabled = true;
        CameraFollow.instance.StartFollowTarget(controller.transform);
        yield return new WaitForSeconds(2.5f);

        EndTurn(player);
    }
}
