using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class PlayerInteractController : MonoBehaviour
{
    [Header("Pickup Settings")]
    public Transform carryPoint;
    public float pickupRange = 1.2f;
    public LayerMask giftLayer;
    public LayerMask dropAreaLayer;
    public int playerID;

    [Header("IK Settings")]
    public Transform leftHandIKTarget;
    public Transform rightHandIKTarget;
    [Range(0, 1)] public float handIKWeight = 1.0f;

    [HideInInspector] public int score = 0;

    private GiftBox carriedGift;
    private Animator animator;
    private bool isHoldingGift = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (carriedGift == null)
                TryPickupGift();
            else
                TryDropGift();
        }
    }

    void TryPickupGift()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange, giftLayer);
        foreach (Collider hit in hits)
        {
            GiftBox gift = hit.GetComponent<GiftBox>();
            if (gift != null && !gift.isCarried)
            {
                House_Area area = gift.GetComponentInParent<House_Area>();
                if (area != null) area.RemoveGift(gift);

                carriedGift = gift;
                gift.PickUp(carryPoint);

                leftHandIKTarget = gift.transform.Find("LeftHandTarget");
                rightHandIKTarget = gift.transform.Find("RightHandTarget");
                handIKWeight = 1f;
                isHoldingGift = true;

                break;
            }
        }
    }

    void TryDropGift()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange, dropAreaLayer);
        foreach (Collider hit in hits)
        {
            House_Area area = hit.GetComponent<House_Area>();
            if (area != null && area.ownerID == playerID && area.CanAddGift())
            {
                Vector3 dropPos = area.GetNextDropPosition();
                carriedGift.Drop(dropPos);
                area.AddGift(carriedGift);

                leftHandIKTarget = null;
                rightHandIKTarget = null;
                handIKWeight = 0f;
                isHoldingGift = false;

                carriedGift = null;
                score++;
                Debug.Log($"Player {playerID + 1} Score: {score}");
                break;
            }
        }
    }



    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        if (isHoldingGift)
        {
            if (leftHandIKTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, handIKWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, handIKWeight);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIKTarget.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIKTarget.rotation);
            }

            if (rightHandIKTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handIKWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, handIKWeight);
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIKTarget.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandIKTarget.rotation);
            }
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
        }
    }
}
