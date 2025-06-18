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
        yield return new WaitForSecondsRealtime(18f);
        var go = Runner.Spawn(portalPrefab, spawnPosition.position, Quaternion.identity);
        yield return new WaitForSecondsRealtime(5f);
        Runner.Despawn(go);
    }
}
