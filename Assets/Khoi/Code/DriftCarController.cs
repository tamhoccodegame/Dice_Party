using UnityEngine;

public class DriftCarController : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider;

    [Header("Wheel Meshes")]
    public Transform frontLeftMesh;
    public Transform frontRightMesh;
    public Transform rearLeftMesh;
    public Transform rearRightMesh;

    [Header("Car Settings")]
    public float maxMotorTorque = 1500f;
    public float maxSteerAngle = 30f;
    public float brakeTorque = 3000f;
    public float driftStiffness = 0.5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Giảm trọng tâm để đỡ lật
    }

    void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteerAngle * Input.GetAxis("Horizontal");
        bool isDrifting = Input.GetKey(KeyCode.Space);

        // Điều khiển động cơ và lái
        frontLeftCollider.steerAngle = steering;
        frontRightCollider.steerAngle = steering;

        rearLeftCollider.motorTorque = motor;
        rearRightCollider.motorTorque = motor;

        // Phanh hoặc Drift
        if (isDrifting)
        {
            rearLeftCollider.brakeTorque = brakeTorque;
            rearRightCollider.brakeTorque = brakeTorque;
            SetStiffness(rearLeftCollider, driftStiffness);
            SetStiffness(rearRightCollider, driftStiffness);
        }
        else
        {
            rearLeftCollider.brakeTorque = 0f;
            rearRightCollider.brakeTorque = 0f;
            SetStiffness(rearLeftCollider, 1f);
            SetStiffness(rearRightCollider, 1f);
        }

        UpdateWheelVisual(frontLeftCollider, frontLeftMesh);
        UpdateWheelVisual(frontRightCollider, frontRightMesh);
        UpdateWheelVisual(rearLeftCollider, rearLeftMesh);
        UpdateWheelVisual(rearRightCollider, rearRightMesh);
    }

    void SetStiffness(WheelCollider col, float stiffness)
    {
        WheelFrictionCurve friction = col.sidewaysFriction;
        friction.extremumValue = 1f * stiffness;
        friction.asymptoteValue = 0.5f * stiffness;
        friction.stiffness = stiffness;
        col.sidewaysFriction = friction;
    }

    void UpdateWheelVisual(WheelCollider collider, Transform mesh)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        mesh.position = pos;
        mesh.rotation = rot;
    }
}
