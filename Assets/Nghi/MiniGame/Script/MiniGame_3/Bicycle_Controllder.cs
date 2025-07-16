using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Rigidbody))]
public class Bicycle_Controllder : MonoBehaviour
{
    [Header("Movement")]
    public float maxMoveSpeed = 20f;
    public float stopThreshold = 0.05f;
    public float slowdownLerpSpeed = 12f;

    [Header("Mash Tap Settings")]
    private Queue<float> tapTimestamps = new Queue<float>();
    public float tapWindow = 1.2f;
    public float tapPowerMin = 1.5f;
    public float tapPowerMid = 5.5f;
    public float tapPowerMax = 10f;
    public float tapThresholdLow = 2f;
    public float tapThresholdHigh = 5f;
    public float decayWhenNoTap = 4.5f;

    [Header("Wheel Settings")]
    public Transform frontWheelTransform;
    public Transform[] rearWheels;
    public float wheelRadius = 0.3f; // Bán kính bánh xe (m)
    public float wheelSpinMultiplier = 1.0f; // Điều chỉnh tốc độ quay bánh nếu cần

    [Header("References")]
    public Animator playerAnimator;
    public Rigidbody rb;

    [Header("UI TMP References")]
    public TMP_Text powerText;
    public TMP_Text speedText;

    [Header("Steering")]
    public Transform frontAssemblyTransform;
    public Transform handlebarTransform;
    public float maxSteerAngle = 35f;
    public float steerSpeed = 90f;
    public float bodyRotationSpeed = 2f;

    private float tapPower = 0f;
    private float currentSpeed = 0f;
    private bool isCycling = false;
    private float currentSteerAngle = 0f;


    [Header("VFX Settings")]
    public ParticleSystem smokeVFX;
    public Transform smokeSpawnPoint;
    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (playerAnimator == null) Debug.LogWarning("⚠️ Thiếu Animator!");
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        HandleTapInput();
        HandleSteering();
        UpdateAnimator();
        UpdateUI();
        SmokeVFX();
    }

    void FixedUpdate()
    {
        ApplyPhysicalMovement();
        RotateWheels();
        DriftRotateBody();
    }

    void SmokeVFX()
    {
        if (smokeVFX != null)
        {
            var emission = smokeVFX.emission;
            emission.rateOverTime = Mathf.Lerp(5f, 50f, tapPower / tapPowerMax);
        }


        if (smokeVFX == null || smokeSpawnPoint == null) return;

        // Cập nhật vị trí spawn mỗi frame để bám theo xe (nếu xe di chuyển)
        if (smokeSpawnPoint != null)
        {
            smokeVFX.transform.position = smokeSpawnPoint.position;
            smokeVFX.transform.rotation = smokeSpawnPoint.rotation;
        }

        if (isCycling)
        {
            if (!smokeVFX.isPlaying)
                smokeVFX.Play();
        }
        else
        {
            if (smokeVFX.isPlaying)
                smokeVFX.Stop();
        }


    }


    void HandleTapInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float currentTime = Time.time;
            tapTimestamps.Enqueue(currentTime);

            if (playerAnimator != null && !isCycling)
                playerAnimator.Play("Cycling");
        }

        while (tapTimestamps.Count > 0 && Time.time - tapTimestamps.Peek() > tapWindow)
        {
            tapTimestamps.Dequeue();
        }

        float tapRate = tapTimestamps.Count / tapWindow;

        if (tapRate < tapThresholdLow)
            tapPower = Mathf.Lerp(tapPower, tapPowerMin, Time.deltaTime * 3f);
        else if (tapRate < tapThresholdHigh)
            tapPower = Mathf.Lerp(tapPower, tapPowerMid, Time.deltaTime * 3f);
        else
            tapPower = Mathf.Lerp(tapPower, tapPowerMax, Time.deltaTime * 3f);

        if (tapTimestamps.Count == 0)
        {
            tapPower -= decayWhenNoTap * Time.deltaTime;
            tapPower = Mathf.Clamp(tapPower, 0, tapPowerMax);
        }

        isCycling = tapPower > stopThreshold;
    }

    void HandleSteering()
    {
        float steerInput = Input.GetAxis("Horizontal");
        float targetAngle = steerInput * maxSteerAngle;
        currentSteerAngle = Mathf.MoveTowards(currentSteerAngle, targetAngle, steerSpeed * Time.deltaTime);

        if (handlebarTransform != null)
            handlebarTransform.localRotation = Quaternion.Euler(0f, currentSteerAngle, 0f);

        if (frontAssemblyTransform != null)
            frontAssemblyTransform.localRotation = Quaternion.Euler(0f, currentSteerAngle, 0f);

        if (frontWheelTransform != null)
        {
            Vector3 localRotation = frontWheelTransform.localEulerAngles;
            localRotation.y = currentSteerAngle;
            frontWheelTransform.localEulerAngles = localRotation;
        }
    }

    void ApplyPhysicalMovement()
    {
        if (!isCycling) return;

        Vector3 forceDirection = frontAssemblyTransform.forward;
        float speedRatio = tapPower / tapPowerMax;
        Vector3 desiredVelocity = forceDirection * speedRatio * maxMoveSpeed;

        Vector3 force = (desiredVelocity - rb.velocity) * slowdownLerpSpeed;
        rb.AddForce(force, ForceMode.Acceleration);

        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        currentSpeed = flatVelocity.magnitude;
    }

    void DriftRotateBody()
    {
        if (!isCycling) return;

        float steerFactor = currentSteerAngle / maxSteerAngle;
        Quaternion targetRotation = Quaternion.LookRotation(frontAssemblyTransform.forward, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * bodyRotationSpeed * Mathf.Abs(steerFactor));
    }

    void RotateWheels()
    {
        float speed = rb.velocity.magnitude;
        float distanceMoved = speed * Time.fixedDeltaTime;
        float rotationAngle = (distanceMoved / (2 * Mathf.PI * wheelRadius)) * 360f * wheelSpinMultiplier;

        Vector3 rotationAxis = Vector3.right;

        // Lăn bánh trước
        if (frontWheelTransform != null)
            frontWheelTransform.Rotate(rotationAxis * rotationAngle, Space.Self);

        // Lăn bánh sau
        if (rearWheels != null)
        {
            foreach (Transform wheel in rearWheels)
            {
                wheel.Rotate(rotationAxis * rotationAngle, Space.Self);
            }
        }

        // ✅ Lăn và hướng bánh sau về hướng quẹo
        if (rearWheels != null)
        {
            foreach (Transform wheel in rearWheels)
            {
                // 🌀 Xoay thân bánh xe
                wheel.Rotate(rotationAxis * rotationAngle, Space.Self);

                // 🧭 Xoay trục bánh theo hướng đầu xe
                Quaternion targetRotation = Quaternion.LookRotation(frontAssemblyTransform.forward, Vector3.up);
                wheel.rotation = Quaternion.Slerp(wheel.rotation, targetRotation, Time.fixedDeltaTime * 10f); // smooth blend
            }
        }
    }

    void UpdateAnimator()
    {
        if (playerAnimator == null) return;
        playerAnimator.speed = isCycling ? Mathf.Lerp(1f, 3f, tapPower / tapPowerMax) : 0f;
    }

    void UpdateUI()
    {
        if (powerText != null)
            powerText.text = $"Power: {tapPower:F2}";
        if (speedText != null)
            speedText.text = $"Speed: {currentSpeed:F2}";
    }

}
