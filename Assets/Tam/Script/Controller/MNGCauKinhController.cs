using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(NetworkCharacterController))]
public class MNGCauKinhController : NetworkBehaviour
{
    private CinemachineCamera cam;
    private Vector3 clientCamForward;

    private NetworkCharacterController controller;
    private Animator animator;

    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float verticalVelocity;

    public bool isGoal = false;

    [Networked] private string NetworkAnim { get; set; } // Animation sync

    public string currentAnim;

    public LayerMask glassLayer;
    public Transform feet;

    GlassBreakManager manager;

    public override void Spawned()
    {
        controller = GetComponent<NetworkCharacterController>();
        controller.enabled = true;
        animator = GetComponent<Animator>();
        manager = GlassBreakManager.instance;


        if (!HasInputAuthority) return;
        cam = FindFirstObjectByType<CinemachineCamera>();
        cam.Follow = transform;
        cam.LookAt = transform;
    }

    void Update()
    {
        if (!Object.HasInputAuthority) return;
        
        if (manager != null && manager.Object.IsValid && manager.isGameStarted)
        {
            // Send input to host
            RPC_SendInput(cam.transform.forward);
        }

        if (Physics.Raycast(feet.position, Vector3.down, out RaycastHit hit, 0.1f, glassLayer))
        {
            hit.collider.gameObject.GetComponent<BreakGlass>().TryBreak();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SendInput(Vector3 camForward)
    {
        //moveInput = input;
        clientCamForward = camForward; 
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

        Vector3 moveDir = Vector3.zero;

        if (GetInput(out NetworkInputData data))
        {
            if(data.buttons.IsSet(NetworkInputData.JUMPBUTTON))
            {
                controller.Jump();
                ChangeAnim("Jump");
            }
            
            // Movement
            data.direction.Normalize();
            Vector3 camForward = Vector3.ProjectOnPlane(clientCamForward, Vector3.up).normalized;
            Vector3 camRight = Vector3.Cross(Vector3.up, camForward).normalized;

            camForward.y = 0;
            camRight.y = 0;

            moveDir = camRight * data.direction.x + camForward * data.direction.z;

            controller.Move(moveDir);


            // Animation
            if (controller.Grounded)
            {
                if (moveDir.magnitude > 0)
                    ChangeAnim("Run");
                else
                    ChangeAnim("Idle");
            }
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
            manager.RequestAddRank(Object.Id);
            SetGoal();
        }
        else if(other.name == "Deadzone")
        {
            controller.Teleport(manager.spawnPosition.position + new Vector3(0, 3, 0));
        }
    }

    void SetGoal()
    {

    }

}
