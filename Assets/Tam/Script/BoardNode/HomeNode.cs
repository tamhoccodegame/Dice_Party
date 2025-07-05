using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeNode : BoardNode
{
    public GameObject portalVFX;

    public override void ProcessNode(PlayerRef playerRef, NetworkId playerObject)
    {
        if (HasStateAuthority)
        {
            RPC_HomeEffect(playerRef, playerObject);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_HomeEffect(PlayerRef playerRef, NetworkId playerObject)
    {
        StartCoroutine(ProcessCoroutine(playerRef, playerObject));
    }

    IEnumerator ProcessCoroutine(PlayerRef playerRef, NetworkId playerObject)
    {
        yield return new WaitForSecondsRealtime(1f);
        CharacterController controller = Runner.FindObject(playerObject)
                                               .GetComponent<CharacterController>();
        if(HasStateAuthority)
        CameraFollow.instance.RPC_StartFollowTarget(transform);
        if (nodeEffect != null) nodeEffect.Play();

        portalVFX.SetActive(true);

        yield return new WaitForSecondsRealtime(0.5f);
        NetworkCharacterController nCtrl = controller.GetComponent<NetworkCharacterController>();
        
        // Lấy index của layer muốn loại bỏ
        int excludeLayer = LayerMask.NameToLayer("Ground");

        // Tạo mask loại bỏ layer đó
        int mask = ~(1 << excludeLayer);

        // Gán vào controller để không tương tác với layer đó
        controller.excludeLayers = mask;

        yield return new WaitForSecondsRealtime(3f);
        portalVFX.SetActive(false);

        controller.excludeLayers = ~0;
        yield return new WaitForSecondsRealtime(0.2f);
        Transform teleportTo = FindFirstObjectByType<PlayerSpawner>().spawnPosition[0];
        BoardGameController bCtrl = controller.GetComponent<BoardGameController>();
        nCtrl.Teleport(teleportTo.position, Quaternion.identity);

        if (HasStateAuthority)
        bCtrl.RPC_SetCurrentNode(teleportTo.GetComponent<NetworkObject>().Id);

        yield return new WaitForSecondsRealtime(1f);
        if (HasStateAuthority)
            CameraFollow.instance.RPC_StartFollowTarget(controller.transform);

        yield return new WaitForSeconds(4f);

        EndTurn(playerRef);
    }
}
