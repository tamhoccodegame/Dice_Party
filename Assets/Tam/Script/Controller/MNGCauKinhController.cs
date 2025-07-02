using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(CharacterController))]
public class MNGCauKinhController : NetworkBehaviour
{
    private CinemachineCamera cam;
    [Networked] private Vector3 clientCamForward { get; set; }

    private CharacterController controller;
    private Animator animator;

    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float verticalVelocity;

    [Networked] private Vector2 moveInput { get; set; }
    [Networked] private bool jumpRequest { get; set; }
    [Networked] private string NetworkAnim { get; set; } // Animation sync

    private float lastInputSendTime = 0f;
    private float inputSendInterval = 1f / 30f; // Gửi tối đa 30 lần/giây (tick rate tương đương 30Hz)


    public string currentAnim;

    public LayerMask glassLayer;
    public Transform feet;

    GlassBreakManager manager;

    public override void Spawned()
    {
        controller = GetComponent<CharacterController>();
        controller.enabled = true;
        animator = GetComponent<Animator>();
        manager = GlassBreakManager.instance;

        cam = FindFirstObjectByType<CinemachineCamera>();
        cam.Follow = transform;
        cam.LookAt = transform;
    }

    void Update()
    {
        if (!Object.HasInputAuthority) return;
        
        if (manager != null && manager.Object.IsValid && manager.isGameStarted)
        {
            // Collect input on client
            //Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            bool jump = Input.GetKeyDown(KeyCode.Space);

            // Send input to host
            RPC_SendInput(jump, cam.transform.forward);
        }

        if (Physics.Raycast(feet.position, Vector3.down, out RaycastHit hit, 0.1f, glassLayer))
        {
            hit.collider.gameObject.GetComponent<BreakGlass>().TryBreak();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SendInput(/*Vector2 input, */bool jump, Vector3 camForward)
    {
        //moveInput = input;
        clientCamForward = camForward; 
        if (jump) jumpRequest = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
        {
            // Client đọc networked animation state
            if (NetworkAnim != currentAnim)
            {
                animator.CrossFade(NetworkAnim, 0.25f);
                currentAnim = NetworkAnim;
            }
            return;
        }

        // Gravity
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Runner.DeltaTime;
        }

        // Jump
        if (jumpRequest && controller.isGrounded)
        {
            ChangeAnim("Jump");
            verticalVelocity = jumpForce;
        }

        jumpRequest = false; // reset jump request

        Vector3 moveDir = Vector3.zero;

        if (GetInput(out NetworkInputData data))
        {
            Debug.Log(data.direction);

            data.direction.Normalize();
            // Movement
            Vector3 camForward = Vector3.ProjectOnPlane(clientCamForward, Vector3.up).normalized;
            Vector3 camRight = Vector3.Cross(Vector3.up, camForward).normalized;

            camForward.y = 0;
            camRight.y = 0;

            moveDir = camRight * data.direction.x + camForward * data.direction.z;
            Vector3 movement = moveDir * moveSpeed;
            movement.y = verticalVelocity;

            controller.Move(movement * Runner.DeltaTime);
        }
        else
        {
            Debug.Log("No input data");
        }


            // Rotation
            Vector3 moveDirection = moveDir;
        moveDirection.y = 0;
        if (moveDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Runner.DeltaTime);
        }

        // Animation
        if (controller.isGrounded)
        {
            if (moveDirection.magnitude > 0)
                ChangeAnim("Run");
            else
                ChangeAnim("Idle");
        }
    }

    public void ChangeAnim(string animName, float blendTime = 0.25f)
    {
        if (animName == currentAnim) return;
        currentAnim = animName;

        if (Object.HasStateAuthority)
            NetworkAnim = animName;

        animator.CrossFade(animName, blendTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Goal")
        {
            manager.RequestAddRank(Runner.LocalPlayer);
        }
        else if(other.name == "Deadzone")
        {
            controller.enabled = false;
            verticalVelocity = 0;
            transform.position = manager.spawnPosition.position + new Vector3(0, 3, 0);
            controller.enabled = true;
        }
    }

}
