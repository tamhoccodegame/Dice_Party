using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunItemBullet : MonoBehaviour
{
    private int damage;

    public void Init(int _damage)
    {
        damage = _damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        Debug.Log(other.gameObject.name + " " + damage);
        if (other.TryGetComponent<NewBoardGameController>(out var t))
        {
            WizardPartyData.instance.UpdatePlayerHealth(t.playerInput, -damage);
            TurnManager.instance.UpdatePlayerDataUI();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        
    }
}
