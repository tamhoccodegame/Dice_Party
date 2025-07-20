using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Crate"))
        {
            other.gameObject.SetActive(false);
            StartCoroutine(RespawnCrate(other.gameObject, 5f));
        }
    }

    IEnumerator RespawnCrate(GameObject crate, float delay)
    {
        yield return new WaitForSeconds(delay);
        crate.SetActive(true);
    }
}
