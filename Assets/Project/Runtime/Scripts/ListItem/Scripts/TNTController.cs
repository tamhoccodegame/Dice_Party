using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class TNTController : BoardItem
{
    public float moveSpeed = 10f;
    public float turnSpeed = 100f;

    public string explosionObjectName = "ExplosionFX";
    public float destroyDelay = 0.7f;
    public float triggerDelay = 2f;

    private Rigidbody rb;
    private bool hasExploded = false;
    private Transform explosionFX;
    private MeshRenderer mesh;

    public NewBoardGameController controller;
    public PlayerInput playerInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mesh = GetComponentInChildren<MeshRenderer>();

        explosionFX = transform.Find(explosionObjectName);
        if (explosionFX == null)
        {
            explosionFX = FindDeepChild(transform, explosionObjectName);
        }
        if (explosionFX != null)
        {
            explosionFX.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (hasExploded) return;
        if (playerInput.actions["Trigger"].triggered)
        {
            Explode();
        }

        float moveInput = playerInput.actions["Move"].ReadValue<Vector2>().y;
        float turnInput = playerInput.actions["Move"].ReadValue<Vector2>().x;

        // Lấy hướng camera (camera chính hoặc camera follow hiện tại)
        Transform cam = Camera.main.transform;
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        // Loại bỏ ảnh hưởng trục Y để tránh nghiêng xuống đất
        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        // Tính hướng di chuyển theo input và camera
        Vector3 moveDir = (camForward * moveInput + camRight * turnInput).normalized;

        if (moveDir.magnitude >= 0.1f)
        {
            // Xoay TNT theo hướng di chuyển
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime));

            // Di chuyển TNT
            Vector3 move = moveDir * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
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
            fx.SetParent(null);
            fx.gameObject.SetActive(true);
            Destroy(fx.gameObject, destroyDelay);
        }
        CameraFollow.instance.StartFollowTarget(controller.transform);
        controller.ChangeState(controller.idleState);
        gameObject.SetActive(false);
        Destroy(gameObject, destroyDelay);
    }
    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;

            Transform result = FindDeepChild(child, name);
            if (result != null) return result;
        }
        return null;
    }

    public override void Use(NewBoardGameController controller)
    {
        var tnt = Instantiate(gameObject, controller.transform.position, Quaternion.identity).GetComponent<TNTController>();
        tnt.controller = controller;
        tnt.playerInput = controller.playerInput;
        CameraFollow.instance.StartFollowTarget(tnt.transform);
    }
}
