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
    private float interactCooldown = 0.15f; // chống spam bấm

    private Transform playerTransform; // tham chiếu Player để so sánh khoảng cách

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
        playerTransform = transform;
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
        Collider[] areaHits = Physics.OverlapSphere(transform.position, pickupRange, dropAreaLayer);

        GiftBox nearestGift = null;
        float nearestDist = Mathf.Infinity;

        foreach (Collider areaCol in areaHits)
        {
            House_Area area = areaCol.GetComponent<House_Area>();
            if (area == null) continue;

            GiftBox gift = area.GetNearestGift(transform.position);
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
            House_Area parentArea = nearestGift.GetComponentInParent<House_Area>();
            if (parentArea != null)
            {
                parentArea.RemoveGift(nearestGift);
            }

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
        Collider[] hits = Physics.OverlapSphere(playerTransform.position, pickupRange, dropAreaLayer);

        House_Area nearestArea = null;
        float nearestDist = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            House_Area area = hit.GetComponent<House_Area>();
            if (area != null && area.ownerID == playerID && area.CanAddGift())
            {
                float dist = Vector3.Distance(playerTransform.position, area.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestArea = area;
                }
            }
        }

        if (nearestArea != null)
        {
            // Lấy đúng vị trí drop gần Player nhất trong area
            Vector3 dropPos = nearestArea.GetNearestDropPosition(playerTransform.position);

            carriedGift.Drop(dropPos);
            nearestArea.AddGift(carriedGift);

            // Reset IK
            leftHandIKTarget = null;
            rightHandIKTarget = null;
            handIKWeight = 0f;
            isHoldingGift = false;

            carriedGift = null;
            score++;
            Debug.Log($"Player {playerID + 1} Score: {score}");
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
