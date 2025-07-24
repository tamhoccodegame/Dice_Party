using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepBackNode : BoardNode
{
    public GameObject horse;
    public BoardNode nodeToStepBack;

    public override void ProcessNode(BoardCar player)
    {
        StartCoroutine(ProcessCoroutine(player));
    }

    IEnumerator ProcessCoroutine(BoardCar player)
    {
        CinecameraManager.instance.ResetCamera();
        yield return new WaitForSeconds(2f);
        horse.SetActive(true);
        horse.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(0.5f);
        player.enabled = false;
        
        while(Vector3.Distance(player.transform.position, nodeToStepBack.transform.position) > 0.4f)
        {
            player.transform.position = Vector3.Lerp(player.transform.position, nodeToStepBack.transform.position, 8 * Time.deltaTime);
            yield return null;
        }
        horse.SetActive(false);
        player.SetCurrentNode(nodeToStepBack);
        player.SetCanMove(true);
        yield return null;
    }
}
