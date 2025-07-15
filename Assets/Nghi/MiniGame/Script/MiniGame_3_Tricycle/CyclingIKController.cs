using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class CyclingIKController : MonoBehaviour
{
    [Header("Seat Attachment")]
    public Transform seatTarget;

    [Header("Hand Targets")]
    public Transform leftHandleTarget;
    public Transform rightHandleTarget;

    [Header("Foot Targets")]
    public Transform leftPedalTarget;
    public Transform rightPedalTarget;

    [Range(0, 1)] public float handIKWeight = 1.0f;
    [Range(0, 1)] public float footIKWeight = 1.0f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
            animator.applyRootMotion = false;
    }

    void LateUpdate()
    {
        // 🔧 Gắn mông vào ghế mỗi frame sau Rigidbody tính toán
        if (seatTarget != null)
        {
            transform.position = seatTarget.position;
            transform.rotation = seatTarget.rotation;
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        if (leftHandleTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, handIKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, handIKWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandleTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandleTarget.rotation);
        }

        if (rightHandleTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handIKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, handIKWeight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandleTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandleTarget.rotation);
        }

        if (leftPedalTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, footIKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, footIKWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftPedalTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftPedalTarget.rotation);
        }

        if (rightPedalTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, footIKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, footIKWeight);
            animator.SetIKPosition(AvatarIKGoal.RightFoot, rightPedalTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, rightPedalTarget.rotation);
        }
    }
}
