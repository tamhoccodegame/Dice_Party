using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyNode : BoardNode
{
    public GameObject keyPrefab;

    //Hàm này tất cả client đều chạy
    public override void ProcessNode(PlayerRef playerRef, NetworkId playerObject)
    {
        if (Object.HasStateAuthority)
        {
            int keyQty = Random.Range(1, 4); // 1 -> 3 chìa

            RPC_SpawnKeyEffect(playerRef, playerObject, keyQty);
        }
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_SpawnKeyEffect(PlayerRef playerRef, NetworkId playerObject, int keyQty)
    {
        StartCoroutine(ProcessCoroutine(playerRef, playerObject, keyQty));
    }

    IEnumerator ProcessCoroutine(PlayerRef playerRef, NetworkId playerObject, int keyQty)
    {
        Transform playerTransform = Runner.FindObject(playerObject).transform;

        for (int i = 0; i < keyQty; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * 0.3f;
            randomOffset.y = Mathf.Abs(randomOffset.y);

            GameObject key = Instantiate(
                keyPrefab,
                transform.position + Vector3.up * 0.5f + randomOffset,
                Quaternion.identity
            );

            // 💥 Add force để văng ra như explosion
            Rigidbody rb = key.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomDir = (Random.insideUnitSphere + Vector3.up * 1.5f).normalized;
                float explosionForce = 20f;
                rb.AddForce(randomDir * explosionForce, ForceMode.Impulse);
            }

            // Bắt đầu bay về player
            KeyPickupMover mover = key.GetComponent<KeyPickupMover>();

            bool collected = false;
            mover.Init(playerTransform, () =>
            {
                TurnManager.instance.RequestUpdateKey(playerRef, 1);
                collected = true;
            });

            yield return new WaitUntil(() => collected);
            yield return new WaitForSeconds(0.15f);
        }


        yield return new WaitForSeconds(0.3f); // Delay nhẹ cho mượt
        EndTurn(playerRef);
    }
}
