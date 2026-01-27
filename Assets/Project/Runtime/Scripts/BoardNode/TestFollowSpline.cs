using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
public class TestFollowSpline : MonoBehaviour
{
    public SplineContainer splineContainer;
    public float speed = 2f;

    public float t;
    public bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving || splineContainer == null) return;

        t += speed * Time.deltaTime;
        t = Mathf.Clamp01(t);

        UpdatePosition();

        if (t >= 1f)
        {
            isMoving = false;
        }
    }

    void UpdatePosition()
    {
        var spline = splineContainer.Spline;

        Vector3 pos = spline.EvaluatePosition(t);
        Vector3 forward = spline.EvaluateTangent(t);

        transform.position = pos;

        if(forward != Vector3.zero)
        {
            transform.forward = forward.normalized;
        }
    }

    public void Play()
    {
        isMoving = true;
    }

    public void Pause()
    {
        isMoving = false;
    }

    public void SetT(float newT)
    {
        t = Mathf.Clamp01(newT);
        UpdatePosition();
    }
}
