using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Throne : MonoBehaviour
{
    public PlayerInput playerInput;
    public WinController playerObject;
    public float interactRange;
    public Transform sitPosition;

    public bool isPLayerSitting;

    public LayerMask playerLayer;
    public GameObject effect;

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange, playerLayer);

        if(colliders.Length > 0)
        {
            foreach(Collider collider in colliders)
            {
                if(collider.GetComponent<WinController>().playerInput == WizardPartyData.instance.winner)
                {
                    playerObject = collider.GetComponent<WinController>();
                    playerInput = playerObject?.playerInput;
                }
            }
        }

        if (playerInput == null) return;

        if (playerInput == WizardPartyData.instance.winner)
        {
            if (playerInput.actions["Interact"].triggered)
            {
                if (!isPLayerSitting)
                {
                    isPLayerSitting = true;
                    GetComponent<AudioSource>().Play();
                    playerObject.GetComponent<CharacterController>().enabled = false;
                    playerObject.GetComponent<WinController>().enabled = false;
                    playerObject.transform.position = sitPosition.position;
                    playerObject.transform.rotation = sitPosition.rotation;
                    playerObject.GetComponent<Animator>().Play($"Win{Random.Range(1, 6)}");
                    effect.SetActive(true);
                }
                else
                {
                    isPLayerSitting = false;
                    GetComponent<AudioSource>().Stop();
                    playerObject.GetComponent<CharacterController>().enabled = true;
                    playerObject.GetComponent<WinController>().enabled = true;
                    effect.SetActive(false);
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
