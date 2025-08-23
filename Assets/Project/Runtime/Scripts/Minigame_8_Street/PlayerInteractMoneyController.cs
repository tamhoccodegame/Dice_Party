using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteractMoneyController : MonoBehaviour
{
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

    private int playerLayer;


    [Header("IK Settings")]
    public Transform leftHandIKTarget;
    public Transform rightHandIKTarget;
    [Range(0, 1)] public float handIKWeight = 1.0f;

    [HideInInspector] public int score = 0;
    [Header("UI")]
    public TMP_Text scoreText; // drag text UI vào Inspector
    public TMP_Text countText; // drag text UI vào Inspector

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

        // cache layer index để bật/tắt collision nhanh
        playerLayer = gameObject.layer;
        carLayer = LayerMask.NameToLayer("Vehicle");

        UpdateScoreUI(); // init
        UpdateCountUI(); // init
    }

    public void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    public void UpdateCountUI()
    {
        if (countText != null)
        {
            countText.text = $"Bags: {carriedBags.Count}";
        }
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
        UpdateCountUI();
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

        // Tạm thời bỏ collision Player <-> Vehicle
        Physics.IgnoreLayerCollision(playerLayer, carLayer, true);

        Debug.Log($"[Player {playerID}] Hit by car! Bags left: {carriedBags.Count}");

        // Crossfade sang Fall
        animator.CrossFade("Fall", 0.05f);

        // Fall chỉ 1s rồi tự recover
        StartCoroutine(RecoverFromFall());
    }

    private IEnumerator RecoverFromFall()
    {
        yield return new WaitForSeconds(2f); // Fall cố định 

        // Bật lại collision Player <-> Vehicle
        Physics.IgnoreLayerCollision(playerLayer, carLayer, false);

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
            UpdateCountUI();
            Debug.Log($"[Player {playerID}] Picked bag #{carriedBags.Count} / {maxCarryCount}. Total now: {carriedBags.Count}");

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


