using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kraken : MonoBehaviour
{
    public float impactRadius = 100f;
    public LayerMask impactLayer;
    public ParticleSystem particle;
    public void TriggerImpactEffect()
    {
        particle.Play();
        Collider[] hits = Physics.OverlapSphere(transform.position, impactRadius, impactLayer);

        foreach (var hit in hits)
        {
            Debug.Log("Hit: " + hit.name);
            if (hit.TryGetComponent<ImpactDice>(out var reaction))
            {
                reaction.TriggerImpact();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}
