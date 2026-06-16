using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChestNode : BoardNode
{
    public override void ProcessNode(GameObject player, Transform playerTransform)
    {
        StartCoroutine(ProcessCoroutine(player, playerTransform));
    }

    IEnumerator ProcessCoroutine(GameObject player, Transform playerTransform)
    {
        yield return new WaitForSeconds(0.5f);
        base.EndTurn(player);
    }
}
