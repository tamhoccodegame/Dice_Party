using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockWayNode : BoardNode
{
    public Animator gate;
    public BoardNode nodeToLock;

    public override void ProcessNode(BoardCar player)
    {
        StartCoroutine(ProcessCoroutine(player));
    }

    IEnumerator ProcessCoroutine(BoardCar player)
    {
        CinecameraManager.instance.ResetCamera();
        yield return new WaitForSecondsRealtime(1.5f);
        gate.Play("Close");
        foreach(var node in nextNodes)
        {
            if(node == nodeToLock)
            {
                nextNodes.Remove(node);
                break;
            }
        }
        yield return new WaitForSeconds(1f);
        player.SetCanMove(true);
    }

}
