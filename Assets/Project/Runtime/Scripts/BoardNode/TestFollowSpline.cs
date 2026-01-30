using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Collections.Generic;
public class TestFollowSpline : MonoBehaviour
{
    public SplineAnimate splineAnimate;
    float saveT;
    public List<float> knotTs = new();

    public bool isMoving = false;


    private void Awake()
    {
        CacheKnotTs();
    }
    private void Start()
    {
        saveT = splineAnimate.NormalizedTime;
        splineAnimate.Pause();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(isMoving)
            {
                splineAnimate.Pause();
            }
            else
            {
                splineAnimate.Play();
            }

                isMoving = !isMoving;
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            Getasjoaif();
        }
    }

    void CacheKnotTs()
    {
        knotTs.Clear();

        Spline spline = splineAnimate.Container.Spline;
        float totalLength = spline.GetLength();

        float accumulatedLength = 0f;
        knotTs.Add(0f); // knot đầu tiên luôn là t = 0

        for (int i = 1; i < spline.Count; i++)
        {
            // sample đoạn spline từ knot i-1 -> i
            float segmentLength = spline.GetCurveLength(i - 1);
            accumulatedLength += segmentLength;

            float t = accumulatedLength / totalLength;
            knotTs.Add(t);
        }
    }

    void Getasjoaif()
    {
        splineAnimate.Pause();
        isMoving = false;
        splineAnimate.NormalizedTime = knotTs[5];
    }

}
