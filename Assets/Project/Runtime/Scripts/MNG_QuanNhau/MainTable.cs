using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTable : MonoBehaviour
{
    public Transform[] sitsPosition;
    public int currentPosition = 0;

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
            other.GetComponent<TestMNGController>().enabled = false;
            other.GetComponent<CharacterController>().enabled = false;
            StartCoroutine(SmoothPositionPlayer(other.transform));
        }
    }

    IEnumerator SmoothPositionPlayer(Transform player)
    {
        Animator animator = player.GetComponent<Animator>();
        animator.CrossFade("Jump", 0.25f);

        Transform target = sitsPosition[currentPosition];

        Vector3 startPos = player.position;
        Vector3 endPos = target.position;

        Quaternion startRot = player.rotation;
        Quaternion endRot = target.rotation;

        float duration = 0.5f;
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
