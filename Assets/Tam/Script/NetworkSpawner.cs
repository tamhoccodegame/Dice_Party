using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkSpawner : NetworkBehaviour
{
    public GameObject obj;

    public override void Spawned()
    {
        Runner.Spawn(obj, Vector3.zero, Quaternion.identity, Runner.LocalPlayer);
    }
}
