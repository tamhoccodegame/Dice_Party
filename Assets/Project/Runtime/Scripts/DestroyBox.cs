using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBox : MonoBehaviour
{
    public ParticleSystem vfx;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Crate"))
        {
            other.gameObject.SetActive(false);
            ParticleSystem vfxInstance = Instantiate(vfx, other.transform.position, Quaternion.identity);
            vfxInstance.Play();
            Destroy(vfxInstance.gameObject, vfxInstance.main.duration + 0.5f);
            StartCoroutine(RespawnCrate(other.gameObject, 5f));
        }
    }

    IEnumerator RespawnCrate(GameObject crate, float delay)
    {
        yield return new WaitForSeconds(delay);
        crate.SetActive(true);
    }
}
