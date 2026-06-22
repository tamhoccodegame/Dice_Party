using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MNGGoal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<MNGPlayerController>(out var player))
        {
            WizardMiniGameManager.instance.UpdatePlayerCompletedGame(player.gameObject);
        }
    }
}
