using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BreakGlass>(out BreakGlass br))
        {
            br.TryBreak();
        }
    }
}
