using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractMoneyController : MonoBehaviour
{
    //public enum CarryMode { IK, Animation }
    //[Header("Carry Mode")]
    //public CarryMode carryMode = CarryMode.IK;

    //[Header("Interact Config")]
    //public Transform carryPoint;
    //public int playerID;

    //[Tooltip("Bán kính tìm House_Area và gift (thoáng tay).")]
    //public float interactRange = 2.5f;

    //[Tooltip("Khoảng 'coyote time' cho phím E (bấm lệch frame vẫn ăn).")]
    //public float inputBufferSeconds = 0.12f;

    //[Tooltip("Nếu không bắt được Area, fallback quét gift trực tiếp trong bán kính này.")]
    //public float directBagFallbackRange = 2.5f;

    //[Tooltip("Kích thước mảng cache cho OverlapSphereNonAlloc.")]
    //public int areaHitCapacity = 12;
    //public int giftHitCapacity = 24;

    //[Header("Layers")]
    //public LayerMask dropAreaLayer; // Layer của House_Area colliders
    //public LayerMask bagLayer;     // Layer của gift colliders

    //[Header("IK Settings")]
    //public Transform leftHandIKTarget;
    //public Transform rightHandIKTarget;
    //[Range(0, 1)] public float handIKWeight = 1.0f;

    //[HideInInspector] public int score = 0;


    //[Header("Carry")]
    //public GameObject bagPrefab; // Bag để cầm
    //[HideInInspector] public List<MoneyBag> carriedBags = new List<MoneyBag>();
    //[HideInInspector] public GameObject carriedBagInstance; // instance của bag
    //public int maxCarryCount = 3;

    //private Animator animator;
    //[HideInInspector] public bool isHoldingBag = false;

    //// Input buffer
    //private float interactBufferTimer = 0f;

    //// NonAlloc caches (tránh GC => mượt)
    //private Collider[] areaHits;
    //private Collider[] giftHits;



    //// thêm biến ở class
    //private Vector3 baseBagScale = Vector3.one;

    //[Header("Carry Scale Config")]
    //[Tooltip("Mức tăng scale mỗi lần nhặt thêm 1 bag.")]
    //public float bagScaleStep = 0.25f;


    //void Start()
    //{
    //    animator = GetComponent<Animator>();
    //    areaHits = new Collider[Mathf.Max(4, areaHitCapacity)];
    //    giftHits = new Collider[Mathf.Max(8, giftHitCapacity)];

    //    if (carryMode == CarryMode.Animation)
    //    {
    //        animator.SetLayerWeight(1, 0f);
    //    }
    //}

    //void Update()
    //{
    //    // Bấm E => nạp buffer
    //    if (Input.GetKeyDown(KeyCode.E))
    //        interactBufferTimer = inputBufferSeconds;

    //    if (interactBufferTimer > 0f)
    //    {
    //        bool acted = false;

    //        // Chỉ thử pick, không drop
    //        if (carriedBags.Count < maxCarryCount)
    //            acted = TryPickupBuffered();

    //        if (acted)
    //            interactBufferTimer = 0f; // tiêu buffer khi đã hành động
    //    }

    //    if (interactBufferTimer > 0f)
    //        interactBufferTimer -= Time.deltaTime;
    //}




    //// ===== PICK =====
    //private bool TryPickupBuffered()
    //{
    //    Vector3 p = transform.position;
    //    MoneyBag bestGift = null;
    //    float bestDist = Mathf.Infinity;

    //    // 1) Ưu tiên: trong phạm vi các House_Area (NonAlloc)
    //    int count = Physics.OverlapSphereNonAlloc(p, interactRange, areaHits, dropAreaLayer);
    //    for (int i = 0; i < count; i++)
    //    {
    //        MoneyBagArea area = areaHits[i].GetComponent<MoneyBagArea>();
    //        if (area == null) continue;

    //        MoneyBag g = area.GetNearestGift(p, interactRange);
    //        if (g == null) continue;

    //        float d = Vector3.Distance(p, g.transform.position);
    //        if (d < bestDist)
    //        {
    //            bestDist = d;
    //            bestGift = g;
    //        }
    //    }

    //    // 2) Fallback: nếu chưa tìm thấy qua Area, quét gift trực tiếp (NonAlloc)
    //    if (bestGift == null)
    //    {
    //        int gCount = Physics.OverlapSphereNonAlloc(p, directBagFallbackRange, giftHits, bagLayer);
    //        for (int i = 0; i < gCount; i++)
    //        {
    //            MoneyBag g = giftHits[i].GetComponent<MoneyBag>();
    //            if (g == null || g.isCarried) continue;

    //            float d = Vector3.Distance(p, g.transform.position);
    //            if (d < bestDist)
    //            {
    //                bestDist = d;
    //                bestGift = g;
    //            }
    //        }
    //    }

    //    if (bestGift != null && carriedBags.Count < maxCarryCount)
    //    {
    //        MoneyBagArea parent = bestGift.GetComponentInParent<MoneyBagArea>();
    //        if (parent != null) parent.RemoveGift(bestGift);

    //        carriedBags.Add(bestGift);

    //        // Nếu đã spawn bagInstance trước đó thì chỉ cần scale nó lên
    //        if (carriedBagInstance != null)
    //        {
    //            float scaleFactor = 1f + (bagScaleStep * (carriedBags.Count - 1));
    //            carriedBagInstance.transform.localScale = baseBagScale * scaleFactor;
    //        }

    //        bestGift.gameObject.SetActive(false); // ẩn gift đi

    //        if (carriedBags.Count == 1)
    //        {
    //            // Spawn bag ở tay
    //            carriedBagInstance = Instantiate(bagPrefab, carryPoint);
    //            carriedBagInstance.transform.SetParent(carryPoint);
    //            carriedBagInstance.transform.localPosition = Vector3.zero;
    //            carriedBagInstance.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
    //            //carriedBagInstance.transform.localRotation = Quaternion.identity;

    //            // Lưu lại scale gốc của prefab để sau này nhân lên
    //            baseBagScale = carriedBagInstance.transform.localScale;

    //            leftHandIKTarget = carriedBagInstance.transform.Find("LeftHandTarget");
    //            rightHandIKTarget = carriedBagInstance.transform.Find("RightHandTarget");
    //            handIKWeight = 1f;
    //            isHoldingBag = true;

    //            //!!!!!!!!!!!!!!!!!
    //            if (carryMode == CarryMode.IK)
    //            {
    //                handIKWeight = 1f;
    //            }
    //            else if (carryMode == CarryMode.Animation)
    //            {
    //                animator.SetLayerWeight(1, 1f); // bật layer Carry
    //            }
    //        }

    //        Debug.Log($"[Player {playerID}] Picked bag #{carriedBags.Count} / {maxCarryCount}. Total now: {carriedBags.Count}");

    //        return true;
    //    }
    //    return false;
    //}


    public enum CarryMode { IK, Animation }
    [Header("Carry Mode")]
    public CarryMode carryMode = CarryMode.IK;

    [Header("Interact Config")]
    public Transform carryPoint;
    public int playerID;
    public float interactRange = 2.5f;
    public float inputBufferSeconds = 0.12f;
    public float directBagFallbackRange = 2.5f;
    public int areaHitCapacity = 12;
    public int giftHitCapacity = 24;

    [Header("Layers")]
    public LayerMask dropAreaLayer;
    public LayerMask bagLayer;
    public LayerMask carLayer; // <<-- thêm để check xe
    public LayerMask carHitboxLayer; // layer chỉ dành cho trigger hitbox (khác với collider vật lý của xe)


    [Header("IK Settings")]
    public Transform leftHandIKTarget;
    public Transform rightHandIKTarget;
    [Range(0, 1)] public float handIKWeight = 1.0f;

    [HideInInspector] public int score = 0;

    [Header("Carry")]
    public GameObject bagPrefab;
    [HideInInspector] public List<MoneyBag> carriedBags = new List<MoneyBag>();
    [HideInInspector] public GameObject carriedBagInstance;
    public int maxCarryCount = 3;

    private Animator animator;
    [HideInInspector] public bool isHoldingBag = false;

    private float interactBufferTimer = 0f;
    private Collider[] areaHits;
    private Collider[] giftHits;

    private Vector3 baseBagScale = Vector3.one;
    [Header("Carry Scale Config")]
    public float bagScaleStep = 0.25f;

    // ==== mới thêm ====
    [HideInInspector] public bool isFalling = false;
    private Vector3 cachedPosition;
    void Start()
    {
        animator = GetComponent<Animator>();
        areaHits = new Collider[Mathf.Max(4, areaHitCapacity)];
        giftHits = new Collider[Mathf.Max(8, giftHitCapacity)];

        if (carryMode == CarryMode.Animation)
            animator.SetLayerWeight(1, 0f);
    }

    void Update()
    {
        if (isFalling) return; // đang té thì bỏ qua input

        // buffer input
        if (Input.GetKeyDown(KeyCode.E))
            interactBufferTimer = inputBufferSeconds;

        if (interactBufferTimer > 0f)
        {
            bool acted = false;
            if (carriedBags.Count < maxCarryCount)
                acted = TryPickupBuffered();

            if (acted)
                interactBufferTimer = 0f;
        }

        if (interactBufferTimer > 0f)
            interactBufferTimer -= Time.deltaTime;
    }

    // ===== VA CHẠM XE =====
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (((1 << collision.gameObject.layer) & carLayer) != 0)
    //    {
    //        if (carriedBags.Count > 0 && !isFalling)
    //        {
    //            LoseOneBag();
    //        }
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & carHitboxLayer) != 0)
        {
            Debug.Log($"[Player {playerID}] Hit by car hitbox: {other.gameObject.name}");

            if (carriedBags.Count > 0 && !isFalling)
            {
                LoseOneBag();
            }
        }
    }

    // ===== MẤT BAG =====
    private void LoseOneBag()
    {
        carriedBags.RemoveAt(carriedBags.Count - 1);

        // update scale hoặc disable nếu hết
        if (carriedBagInstance != null)
        {
            if (carriedBags.Count > 0)
            {
                float scaleFactor = 1f + (bagScaleStep * (carriedBags.Count - 1));
                carriedBagInstance.transform.localScale = baseBagScale * scaleFactor;
            }
            else
            {
                Destroy(carriedBagInstance);
                carriedBagInstance = null;
            }
        }

        isFalling = true;
        cachedPosition = transform.position;
        transform.position = cachedPosition + Vector3.up * 0.2f; // nhích lên chút để ko lún

        Debug.Log($"[Player {playerID}] Hit by car! Bags left: {carriedBags.Count}");

        // Crossfade sang Fall
        animator.CrossFade("Fall", 0.05f);

        // Fall chỉ 1s rồi tự recover
        StartCoroutine(RecoverFromFall());
    }

    private IEnumerator RecoverFromFall()
    {
        yield return new WaitForSeconds(1f); // Fall cố định 1s

        // recover
        isFalling = false;

        if (carriedBags.Count > 0)
        {
            if (carriedBagInstance != null)
            {
                carriedBagInstance.SetActive(true);
                float scaleFactor = 1f + (bagScaleStep * (carriedBags.Count - 1));
                carriedBagInstance.transform.localScale = baseBagScale * scaleFactor;
            }
            else
            {
                carriedBagInstance = Instantiate(bagPrefab, carryPoint);
                carriedBagInstance.transform.localPosition = Vector3.zero;
                carriedBagInstance.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                baseBagScale = carriedBagInstance.transform.localScale;
            }

            if (carryMode == CarryMode.IK)
            {
                handIKWeight = 1f;
            }
            else if (carryMode == CarryMode.Animation)
            {
                animator.SetLayerWeight(1, 1f);
                animator.CrossFade("CarryIdle", 0.1f); // <<-- ép trở về state Carry
            }

            isHoldingBag = true;
        }
        else
        {
            if (carryMode == CarryMode.Animation)
                animator.SetLayerWeight(1, 0f);

            animator.CrossFade("Idle", 0.1f); // <<-- ép trở về Idle
            isHoldingBag = false;
        }
    }



    //// ===== PICK =====
    private bool TryPickupBuffered()
    {
        Vector3 p = transform.position;
        MoneyBag bestGift = null;
        float bestDist = Mathf.Infinity;

        // 1) Ưu tiên: trong phạm vi các House_Area (NonAlloc)
        int count = Physics.OverlapSphereNonAlloc(p, interactRange, areaHits, dropAreaLayer);
        for (int i = 0; i < count; i++)
        {
            MoneyBagArea area = areaHits[i].GetComponent<MoneyBagArea>();
            if (area == null) continue;

            MoneyBag g = area.GetNearestGift(p, interactRange);
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
            int gCount = Physics.OverlapSphereNonAlloc(p, directBagFallbackRange, giftHits, bagLayer);
            for (int i = 0; i < gCount; i++)
            {
                MoneyBag g = giftHits[i].GetComponent<MoneyBag>();
                if (g == null || g.isCarried) continue;

                float d = Vector3.Distance(p, g.transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    bestGift = g;
                }
            }
        }

        if (bestGift != null && carriedBags.Count < maxCarryCount)
        {
            MoneyBagArea parent = bestGift.GetComponentInParent<MoneyBagArea>();
            if (parent != null) parent.RemoveGift(bestGift);

            carriedBags.Add(bestGift);

            // Nếu đã spawn bagInstance trước đó thì chỉ cần scale nó lên
            if (carriedBagInstance != null)
            {
                float scaleFactor = 1f + (bagScaleStep * (carriedBags.Count - 1));
                carriedBagInstance.transform.localScale = baseBagScale * scaleFactor;
            }

            bestGift.gameObject.SetActive(false); // ẩn gift đi

            if (carriedBags.Count == 1)
            {
                // Spawn bag ở tay
                carriedBagInstance = Instantiate(bagPrefab, carryPoint);
                carriedBagInstance.transform.SetParent(carryPoint);
                carriedBagInstance.transform.localPosition = Vector3.zero;
                carriedBagInstance.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                //carriedBagInstance.transform.localRotation = Quaternion.identity;

                // Lưu lại scale gốc của prefab để sau này nhân lên
                baseBagScale = carriedBagInstance.transform.localScale;

                leftHandIKTarget = carriedBagInstance.transform.Find("LeftHandTarget");
                rightHandIKTarget = carriedBagInstance.transform.Find("RightHandTarget");
                handIKWeight = 1f;
                isHoldingBag = true;

                //!!!!!!!!!!!!!!!!!
                if (carryMode == CarryMode.IK)
                {
                    handIKWeight = 1f;
                }
                else if (carryMode == CarryMode.Animation)
                {
                    animator.SetLayerWeight(1, 1f); // bật layer Carry
                }
            }

            Debug.Log($"[Player {playerID}] Picked bag #{carriedBags.Count} / {maxCarryCount}. Total now: {carriedBags.Count}");

            return true;
        }
        return false;
    }



    // ===== DROP =====
    private bool TryDropBuffered()
    {
        Vector3 p = transform.position;

        // 1) Ưu tiên: tìm House_Area mình sở hữu trong phạm vi
        MoneyBagArea nearestArea = null;
        float bestAreaDist = Mathf.Infinity;

        int count = Physics.OverlapSphereNonAlloc(p, interactRange, areaHits, dropAreaLayer);
        for (int i = 0; i < count; i++)
        {
            MoneyBagArea area = areaHits[i].GetComponent<MoneyBagArea>();
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
            foreach (var area in MoneyBagArea.All)
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

        if (nearestArea != null && nearestArea.CanAddGift() && carriedBags.Count > 0)
        {
            MoneyBag dropGift = carriedBags[carriedBags.Count - 1]; // drop cái cuối cùng
            carriedBags.RemoveAt(carriedBags.Count - 1);

            dropGift.gameObject.SetActive(true);
            int slotIndex = nearestArea.GetNearestEmptySlot(p, Mathf.Infinity);
            nearestArea.AddGift(dropGift, slotIndex);

            // update scale hoặc destroy nếu hết
            if (carriedBags.Count == 0)
            {
                Destroy(carriedBagInstance);
                carriedBagInstance = null;
                leftHandIKTarget = null;
                rightHandIKTarget = null;
                handIKWeight = 0f;
                isHoldingBag = false;

                if (carryMode == CarryMode.Animation)
                    animator.SetLayerWeight(1, 0f);
            }
            else
            {
                float scaleFactor = 1f + (0.3f * (carriedBags.Count - 1));
                carriedBagInstance.transform.localScale = Vector3.one * scaleFactor;
            }

            score++;
            return true;
        }

        return false;
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (!animator) return;

        if (isHoldingBag)
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


