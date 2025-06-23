using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class MNGCauKinhController : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float gravity = -20f;

    [Header("References")]
    public CharacterController controller;
    public Transform camTransform;

    [Networked] private Vector3 velocity { get; set; }
    [Networked] private bool isGrounded { get; set; }

    private Vector2 moveInput;
    private bool jumpInput;

    public override void FixedUpdateNetwork()
    {
        if (!HasInputAuthority) return;

        moveInput.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //if (controller.isGrounded)
        //{
        //    velocity.y = -1f;
        //}
        //else
        //{
        //    velocity += gravity * Runner.DeltaTime;
        //}
    }


}
