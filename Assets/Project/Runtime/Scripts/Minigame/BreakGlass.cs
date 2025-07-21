using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakGlass : MonoBehaviour
{
    public bool isBreakable { get; set; } = false;

    public GameObject breakEffect;

    public void SetBreakable(bool isBreakable)
    {
            this.isBreakable = isBreakable;
    }

    public void TryBreak()
    {
        if (!isBreakable) return;
        Break();
    }

    public void Break()
    {
        Instantiate(breakEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
