using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakGlass : MonoBehaviour
{
    public bool isBreakable = false;

    public GameObject breakEffect;

    public void SetBreakable(bool isBreakable)
    {
        this.isBreakable = isBreakable;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Instantiate(breakEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
