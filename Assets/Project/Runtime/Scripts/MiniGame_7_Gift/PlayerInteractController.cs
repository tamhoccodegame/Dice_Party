using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class PlayerInteractController : MonoBehaviour
{
    public enum CarryMode { IK, Animation }
    [Header("Carry Mode")]
    public CarryMode carryMode = CarryMode.IK;

    [Header("Interact Config")]
    public Transform carryPoint;
    public int playerID;

    [Tooltip("Bán kính tìm House_Area và gift (thoáng tay).")]
    public float interactRange = 2.5f;

    [Tooltip("Khoảng 'coyote time' cho phím E (bấm lệch frame vẫn ăn).")]
    public float inputBufferSeconds = 0.12f;

    [Tooltip("Nếu không bắt được Area, fallback quét gift trực tiếp trong bán kính này.")]
    public float directGiftFallbackRange = 2.5f;

    [Tooltip("Kích thước mảng cache cho OverlapSphereNonAlloc.")]
    public int areaHitCapacity = 12;
    public int giftHitCapacity = 24;

    [Header("Layers")]
    public LayerMask dropAreaLayer; // Layer của House_Area colliders
    public LayerMask giftLayer;     // Layer của gift colliders

    [Header("IK Settings")]
    public Transform leftHandIKTarget;
    public Transform rightHandIKTarget;
    [Range(0, 1)] public float handIKWeight = 1.0f;

    [HideInInspector] public int score = 0;

    private GiftBox carriedGift;
    private Animator animator;
    private bool isHoldingGift = false;

    // Input buffer
    private float interactBufferTimer = 0f;

    // NonAlloc caches (tránh GC => mượt)
    private Collider[] areaHits;
    private Collider[] giftHits;

    void Start()
    {
        animator = GetComponent<Animator>();
        areaHits = new Collider[Mathf.Max(4, areaHitCapacity)];
        giftHits = new Collider[Mathf.Max(8, giftHitCapacity)];

        if (carryMode == CarryMode.Animation)
        {
            animator.SetLayerWeight(1, 0f);
        }
    }

    void Update()
    {
        // Bấm E => nạp buffer
        if (Input.GetKeyDown(KeyCode.E))
            interactBufferTimer = inputBufferSeconds;

        if (interactBufferTimer > 0f)
        {
            bool acted = (carriedGift == null) ? TryPickupBuffered() : TryDropBuffered();
            if (acted) interactBufferTimer = 0f; // tiêu buffer khi đã hành động
        }

        if (interactBufferTimer > 0f)
            interactBufferTimer -= Time.deltaTime;
    }

    // ===== PICK =====
    private bool TryPickupBuffered()
    {
        Vector3 p = transform.position;
        GiftBox bestGift = null;
        float bestDist = Mathf.Infinity;

        // 1) Ưu tiên: trong phạm vi các House_Area (NonAlloc)
        int count = Physics.OverlapSphereNonAlloc(p, interactRange, areaHits, dropAreaLayer);
        for (int i = 0; i < count; i++)
        {
            House_Area area = areaHits[i].GetComponent<House_Area>();
            if (area == null) continue;

            GiftBox g = area.GetNearestGift(p, interactRange);
            if (g == null) continue;

            float d = Vector3.Distance(p, g.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                bestGift = g;
            }
        }

        // 2) Fallback: nếu chưa tìm thấy qua Area, quét gift trực tiếp (NonAlloc)
        if (bestGift == null)
        {
            int gCount = Physics.OverlapSphereNonAlloc(p, directGiftFallbackRange, giftHits, giftLayer);
            for (int i = 0; i < gCount; i++)
            {
                GiftBox g = giftHits[i].GetComponent<GiftBox>();
                if (g == null || g.isCarried) continue;

                float d = Vector3.Distance(p, g.transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    bestGift = g;
                }
            }
        }

        if (bestGift != null)
        {
            House_Area parent = bestGift.GetComponentInParent<House_Area>();
            if (parent != null) parent.RemoveGift(bestGift);

            carriedGift = bestGift;
            bestGift.PickUp(carryPoint);

            //leftHandIKTarget = bestGift.transform.Find("LeftHandTarget");
            //rightHandIKTarget = bestGift.transform.Find("RightHandTarget");
            //handIKWeight = 1f;
            isHoldingGift = true;

            //!!!!!!!!!!!!!!!!!
            if (carryMode == CarryMode.IK)
            {
                leftHandIKTarget = bestGift.transform.Find("LeftHandTarget");
                rightHandIKTarget = bestGift.transform.Find("RightHandTarget");
                handIKWeight = 1f;
            }
            else if (carryMode == CarryMode.Animation)
            {
                animator.SetLayerWeight(1, 1f); // bật layer Carry
            }

            return true;
        }

        return false; // chưa hành động, giữ buffer tới frame sau
    }

    // ===== DROP =====
    private bool TryDropBuffered()
    {
        Vector3 p = transform.position;

        // 1) Ưu tiên: tìm House_Area mình sở hữu trong phạm vi
        House_Area nearestArea = null;
        float bestAreaDist = Mathf.Infinity;

        int count = Physics.OverlapSphereNonAlloc(p, interactRange, areaHits, dropAreaLayer);
        for (int i = 0; i < count; i++)
        {
            House_Area area = areaHits[i].GetComponent<House_Area>();
            if (area == null || area.ownerID != playerID) continue;

            float d = Vector3.Distance(p, area.transform.position);
            if (d < bestAreaDist)
            {
                bestAreaDist = d;
                nearestArea = area;
            }
        }

        // 2) Fallback: nếu không bắt được collider Area (ví dụ collider hơi nhỏ), lấy từ registry
        if (nearestArea == null)
        {
            foreach (var area in House_Area.All)
            {
                if (area == null || area.ownerID != playerID) continue;
                float d = Vector3.Distance(p, area.transform.position);
                if (d <= interactRange * 1.25f && d < bestAreaDist) // nới nhẹ biên
                {
                    bestAreaDist = d;
                    nearestArea = area;
                }
            }
        }

        if (nearestArea != null && nearestArea.CanAddGift())
        {
            // Luôn chọn slot trống gần nhất với Player (siêu nhạy, không cần đứng đúng ô)
            int slotIndex = nearestArea.GetNearestEmptySlot(p, Mathf.Infinity);
            if (slotIndex != -1)
            {
                Vector3 dropPos = nearestArea.GetSlotPosition(slotIndex);
                carriedGift.Drop(dropPos);
                nearestArea.AddGift(carriedGift, slotIndex);

                leftHandIKTarget = null;
                rightHandIKTarget = null;
                handIKWeight = 0f;
                isHoldingGift = false;
                carriedGift = null;

                if (carryMode == CarryMode.Animation)
                    animator.SetLayerWeight(1, 0f);

                score++;
                return true;
            }
        }

        return false; // chưa hành động, giữ buffer tới frame sau
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
