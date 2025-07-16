using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tire_Movement : MonoBehaviour
{
    public enum Axis
    {
        X, Y, Z,
        NegX, NegY, NegZ
    }

    public enum TireMode
    {
        Classic,      // Bánh xe cua lăn ngang
        RollingBar    // Thanh kimbap gai lăn tới-lùi
    }

    [Header("General Settings")]
    public TireMode mode = TireMode.Classic;

    [Header("Ping Pong Settings")]
    public float moveSpeed = 5f;
    public float wheelRadius = 0.35f;
    public float detectionDistance = 0.5f;
    public LayerMask wallLayer;

    [Header("Wheel Mesh")]
    public Transform wheelMesh;

    [Header("Direction Settings")]
    public Axis moveAxis = Axis.X;
    public Axis faceAxis = Axis.Z;
    public bool lockFacingDirection = true;

    [Header("Upright")]
    public float uprightSmoothing = 10f;

    private Vector3 moveDirection;
    private Vector3 lastPos;
    private Quaternion initialRotation; private Rigidbody _rb;

    public void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (wheelMesh == null)
        {
            Debug.LogError("❌ Wheel mesh not assigned.");
            enabled = false;
            return;
        }

        moveDirection = GetAxisVector(moveAxis);
        lastPos = transform.position;

        if (lockFacingDirection)
        {
            initialRotation = Quaternion.LookRotation(GetAxisVector(faceAxis), Vector3.up);
            transform.rotation = initialRotation;
        }
        else
        {
            UpdateFacing();
        }
    }

    public void Update()
    {
        CheckWallAndReflect();
        Move();
        RotateMesh();
        StayUpright();

        if (!lockFacingDirection)
            UpdateFacing();
    }

    void CheckWallAndReflect()
    {
        Ray ray = new Ray(transform.position, moveDirection);
        if (Physics.Raycast(ray, out RaycastHit hit, detectionDistance, wallLayer))
        {
            moveDirection *= -1f;

            if (!lockFacingDirection)
                UpdateFacing();
        }

        Debug.DrawRay(transform.position, moveDirection * detectionDistance, Color.red);
    }

    void Move()
    {
        _rb.velocity = moveDirection * moveSpeed;
    }

    void RotateMesh()
    {
        Vector3 delta = transform.position - lastPos;
        float dist = delta.magnitude;

        if (dist < 0.001f) return;

        float angle = (dist / (2 * Mathf.PI * wheelRadius)) * 360f;
        Vector3 rotAxis;

        if (mode == TireMode.Classic)
        {
            // Xoay như bánh xe thật
            rotAxis = Vector3.Cross(moveDirection.normalized, Vector3.up).normalized;
        }
        else if (mode == TireMode.RollingBar)
        {
            // Thanh gai lăn trên sàn
            rotAxis = transform.right; // Lăn như cây trục lăn nằm ngang
        }
        else
        {
            rotAxis = Vector3.up;
        }

        wheelMesh.Rotate(rotAxis, angle, Space.World);
        lastPos = transform.position;
    }

    void StayUpright()
    {
        Quaternion targetRot = Quaternion.LookRotation(transform.forward, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, uprightSmoothing * Time.deltaTime);
    }

    void UpdateFacing()
    {
        Vector3 dir = GetAxisVector(faceAxis);
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }

    Vector3 GetAxisVector(Axis axis)
    {
        return axis switch
        {
            Axis.X => transform.right,
            Axis.Y => transform.up,
            Axis.Z => transform.forward,
            Axis.NegX => -transform.right,
            Axis.NegY => -transform.up,
            Axis.NegZ => -transform.forward,
            _ => transform.forward
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerBlinking>() ??
                         other.GetComponentInParent<PlayerBlinking>();

            if (player != null)
            {
                player.OnHitByObstacle(other.transform.position);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }


    //public enum Axis
    //{
    //    X, Y, Z,
    //    NegX, NegY, NegZ
    //}

    //[Header("Ping Pong Settings")]
    //public float moveSpeed = 5f;
    //public float wheelRadius = 0.35f;
    //public float detectionDistance = 0.5f;
    //public LayerMask wallLayer;

    //[Header("Wheel Mesh")]
    //public Transform wheelMesh;

    //[Header("Direction Settings")]
    //public Axis moveAxis = Axis.X;
    //public Axis faceAxis = Axis.Z;
    //public bool lockFacingDirection = true;

    //[Header("Upright")]
    //public float uprightSmoothing = 10f;

    //private Vector3 worldMoveDir;
    //private Vector3 lastPos;
    //private Quaternion initialFacingRotation;

    //void Start()
    //{
    //    if (wheelMesh == null)
    //    {
    //        Debug.LogError("❌ Wheel mesh not assigned.");
    //        enabled = false;
    //        return;
    //    }

    //    worldMoveDir = GetAxisVector(moveAxis);
    //    lastPos = transform.position;

    //    if (lockFacingDirection)
    //    {
    //        initialFacingRotation = Quaternion.LookRotation(GetAxisVector(faceAxis), Vector3.up);
    //        transform.rotation = initialFacingRotation;
    //    }
    //    else
    //    {
    //        LookTowardFacingAxis();
    //    }
    //}

    //void Update()
    //{
    //    CheckWallAndReflect();
    //    Move();
    //    RotateWheelMesh();
    //    StayUpright();

    //    if (!lockFacingDirection)
    //    {
    //        LookTowardFacingAxis();
    //    }
    //}

    //void CheckWallAndReflect()
    //{
    //    Ray ray = new Ray(transform.position, worldMoveDir);
    //    if (Physics.Raycast(ray, out RaycastHit hit, detectionDistance, wallLayer))
    //    {
    //        worldMoveDir *= -1f;

    //        if (!lockFacingDirection)
    //            LookTowardFacingAxis();
    //    }

    //    Debug.DrawRay(transform.position, worldMoveDir * detectionDistance, Color.red);
    //}

    //void Move()
    //{
    //    transform.position += worldMoveDir * moveSpeed * Time.deltaTime;
    //}

    //void RotateWheelMesh()
    //{
    //    Vector3 delta = transform.position - lastPos;
    //    float distance = delta.magnitude;
    //    if (distance < 0.0001f) return;

    //    float angle = (distance / (2 * Mathf.PI * wheelRadius)) * 360f;
    //    Vector3 rotationAxis = Vector3.Cross(worldMoveDir.normalized, Vector3.up).normalized;
    //    wheelMesh.Rotate(rotationAxis, angle, Space.World);

    //    lastPos = transform.position;
    //}

    //void StayUpright()
    //{
    //    Quaternion targetRot = Quaternion.LookRotation(transform.forward, Vector3.up);
    //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, uprightSmoothing * Time.deltaTime);
    //}

    //void LookTowardFacingAxis()
    //{
    //    Vector3 faceDir = GetAxisVector(faceAxis);
    //    transform.rotation = Quaternion.LookRotation(faceDir, Vector3.up);
    //}

    //Vector3 GetAxisVector(Axis axis)
    //{
    //    return axis switch
    //    {
    //        Axis.X => transform.right,
    //        Axis.Y => transform.up,
    //        Axis.Z => transform.forward,
    //        Axis.NegX => -transform.right,
    //        Axis.NegY => -transform.up,
    //        Axis.NegZ => -transform.forward,
    //        _ => transform.forward
    //    };
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.CompareTag("Player"))
    //    {
    //        var player = collision.collider.GetComponent<PlayerController_N>() ??
    //                     collision.collider.GetComponentInParent<PlayerController_N>();

    //        if (player != null)
    //        {
    //            Vector3 hitPoint = collision.contacts[0].point;
    //            player.OnHitByObstacle(hitPoint);
    //        }
    //    }
    //}
}
