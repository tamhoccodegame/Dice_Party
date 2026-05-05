using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class CheckerNormalized : MonoBehaviour
{
    public SplineAnimate splineAnimate;

    private void Start()
    {
        Destroy(gameObject, splineAnimate.Duration + 0.2f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<BoardNode>(out var boardNode))
        {
            boardNode.normalizeTime = splineAnimate.NormalizedTime;
        }
    }
}
