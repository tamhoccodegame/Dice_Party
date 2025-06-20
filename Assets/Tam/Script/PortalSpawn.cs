using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSpawn : NetworkBehaviour
{
    public GameObject portalPrefab;
    public Transform spawnPosition;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            StartCoroutine(SpawnCoroutine());
        }
    }

    IEnumerator SpawnCoroutine()
    {
        yield return new WaitForSecondsRealtime(16f);
        var go = Runner.Spawn(portalPrefab, spawnPosition.position + new Vector3(0, 2, 0), Quaternion.identity);
        yield return new WaitForSecondsRealtime(3f);
        Runner.Despawn(go);
    }
}
