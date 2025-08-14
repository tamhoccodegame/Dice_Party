using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Throne : MonoBehaviour
{
    public PlayerInput playerInput;
    public float interactRange;
    public Transform sitPosition;

    public bool isPLayerSitting;


    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);

        if(colliders.Length > 0)
        {
            PlayerInput player = colliders[0].GetComponent<WinController>().playerInput;
            if (player == WizardPartyData.instance.winner)
            {
                if (player.actions["Interact"].triggered)
                {
                    if (!isPLayerSitting)
                    {
                        isPLayerSitting = true;
                        player.GetComponent<CharacterController>().enabled = false;
                        player.GetComponent<WinController>().enabled = false;
                        player.transform.position = sitPosition.position;
                        player.transform.rotation = sitPosition.rotation;
                        player.GetComponent<Animator>().Play($"Win{Random.Range(1, 6)}");
                    }
                    else
                    {
                        isPLayerSitting = false;
                        player.GetComponent<CharacterController>().enabled = true;
                        player.GetComponent<WinController>().enabled = true;
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
