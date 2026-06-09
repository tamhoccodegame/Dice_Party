using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairGoal : MonoBehaviour
{
    public Transform sitPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            MNGPlayerController p = other.GetComponent<MNGPlayerController>();
            WizardMiniGameManager.instance.UpdatePlayerCompletedGame(p.GetPlayerInput());
            p.enabled = false;
            other.GetComponent<CharacterController>().enabled = false;
            StartCoroutine(SmoothPositionPlayer(other.transform));
        }
    }

    IEnumerator SmoothPositionPlayer(Transform player)
    {
        Animator animator = player.GetComponent<Animator>();
        animator.CrossFade("Jump", 0.25f);


        Vector3 startPos = player.position;
        Vector3 endPos = sitPosition.position;

        Quaternion startRot = player.rotation;
        Quaternion endRot = sitPosition.rotation;

        float duration = 0.3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            player.position = Vector3.Lerp(startPos, endPos, t);
            player.rotation = Quaternion.Slerp(startRot, endRot, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.position = endPos;
        player.rotation = endRot;

        animator.CrossFade("Sitting", 0.25f);
    }
}
