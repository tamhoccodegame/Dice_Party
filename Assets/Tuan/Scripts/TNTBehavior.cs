using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class TransformExtensions
{
    public static Transform FindDeepChild(this Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = child.FindDeepChild(name);
            if (result != null)
                return result;
        }
        return null;
    }
}
public class TNTBehavior : MonoBehaviour
{
    public string explosionObjectName = "ExplosionFX";
    public float destroyDelay = 0.7f;
    public float triggerDelay = 2f;
    private bool hasExploded = false;

    void Start()
    {
       
        Transform fx = transform.Find(explosionObjectName);
        if (fx == null)
        {
            fx = transform.FindDeepChild(explosionObjectName);
        }
        if (fx != null)
        {
            fx.gameObject.SetActive(false);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Ground"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.3f, rb.velocity.z);
            }
        }
    }

    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Transform fx = transform.Find(explosionObjectName);
        if (fx != null)
        {
            fx.gameObject.SetActive(true);
        }

        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if (mesh != null)
            mesh.enabled = false;

        Destroy(gameObject, destroyDelay);
    }
}
