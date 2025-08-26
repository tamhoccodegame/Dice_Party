using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerRespawn : MonoBehaviour
{
    public Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerBlinking>(out var player))
        {
            player.OnHitByObstacle(other.ClosestPoint(transform.position));
            StartCoroutine(RespawnCoroutine(player.GetComponent<CharacterController>()));
        }
    }

    IEnumerator RespawnCoroutine(CharacterController controller)
    {
        controller.enabled = false;
        yield return new WaitForSeconds(3.5f);
        controller.transform.position = respawnPoint.position;
        yield return null;
        controller.enabled = true;
    }
}
