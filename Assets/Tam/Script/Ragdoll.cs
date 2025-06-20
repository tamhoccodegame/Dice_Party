using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private Rigidbody[] rigibodies;

    // Start is called before the first frame update
    void Start()
    {
        rigibodies = GetComponentsInChildren<Rigidbody>();    

        DisableRagdoll();
    }

    public void EnableRagdoll()
    {
        foreach(var r in rigibodies)
        {
            r.isKinematic = false;
            GetComponent<Animator>().enabled = false;
            GetComponent<MNGVongXoayController>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
        }
    }

    public void DisableRagdoll()
    {
        foreach( var r in rigibodies)
        {
            r.isKinematic = true;
            GetComponent<Animator>().enabled = true;
            GetComponent<MNGVongXoayController>().enabled = true;
            GetComponent<CharacterController>().enabled = true;
        }
    }
}
