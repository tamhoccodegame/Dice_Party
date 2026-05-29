using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCharacterAnimation : MonoBehaviour
{
    public Animator animator;
    public Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        rb.useGravity = false;
        animator.Play("Falling");
    }

    void FixedUpdate()
    {
        rb.AddForce(Vector3.down * 25f, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Ground")
        {
            StartCoroutine(LandingAnimation());
        }
    }

    IEnumerator LandingAnimation()
    {
        animator.CrossFade("Land", 0.25f);
        yield return new WaitForSeconds(2f);
        animator.CrossFade("StandUp", 0.25f);
        yield return new WaitForSeconds(2f);
        animator.CrossFade("Idle", 0.25f);
    }
}
