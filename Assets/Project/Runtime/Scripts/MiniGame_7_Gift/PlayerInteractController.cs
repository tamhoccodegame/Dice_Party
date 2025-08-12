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
    private float lastInteractTime = 0f;
    private float interactCooldown = 0.12f; // giảm nhẹ delay

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && Time.time - lastInteractTime > interactCooldown)
        {
            lastInteractTime = Time.time;

            if (carriedGift == null)
                TryPickupGift();
            else
                TryDropGift();
        }
    }

    void TryPickupGift()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange, giftLayer);
        GiftBox nearestGift = null;
        float nearestDist = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            GiftBox gift = hit.GetComponent<GiftBox>();
            if (gift != null && !gift.isCarried)
            {
                float dist = Vector3.Distance(transform.position, gift.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestGift = gift;
                }
            }
        }

        if (nearestGift != null)
        {
            House_Area area = nearestGift.GetComponentInParent<House_Area>();
            if (area != null)
                area.RemoveGift(nearestGift);

            carriedGift = nearestGift;
            nearestGift.PickUp(carryPoint);

            leftHandIKTarget = nearestGift.transform.Find("LeftHandTarget");
            rightHandIKTarget = nearestGift.transform.Find("RightHandTarget");
            handIKWeight = 1f;
            isHoldingGift = true;
        }
    }

    void TryDropGift()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange, dropAreaLayer);
        House_Area nearestArea = null;
        float nearestDist = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            House_Area area = hit.GetComponent<House_Area>();
            if (area != null && area.ownerID == playerID && area.CanAddGift())
            {
                float dist = Vector3.Distance(transform.position, area.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestArea = area;
                }
            }
        }

        if (nearestArea != null)
        {
            Vector3 dropPos = nearestArea.GetNearestDropPosition(transform.position);
            carriedGift.Drop(dropPos);
            nearestArea.AddGift(carriedGift);

            leftHandIKTarget = null;
            rightHandIKTarget = null;
            handIKWeight = 0f;
            isHoldingGift = false;
            carriedGift = null;
            score++;
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (!animator) return;

        if (isHoldingGift)
        {
            if (leftHandIKTarget)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, handIKWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, handIKWeight);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIKTarget.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIKTarget.rotation);
            }
            if (rightHandIKTarget)
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
