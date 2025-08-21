using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlotType
{
    None,
    Pile,       // đống tiền
    Suitcase    // vali tiền
}


public class MoneyBagArea : MonoBehaviour
{
    [Header("Grid Settings")]
    public int ownerID;

    [Tooltip("Số hàng (trên trục Z)")]
    [Min(1)] public int rows = 2;

    [Tooltip("Số cột (trên trục X)")]
    [Min(1)] public int columns = 3;

    [Tooltip("Spacing được auto tính toán, không chỉnh tay")]
    [SerializeField] private float spacingX;
    [SerializeField] private float spacingZ;

    public static readonly List<MoneyBagArea> All = new List<MoneyBagArea>();

    private MoneyBag[] slots;
    private Vector3[] slotPositions;
    private BoxCollider areaCollider;

    public int SlotCount => rows * columns;

    void Awake()
    {
        All.Add(this);

        areaCollider = GetComponent<BoxCollider>();
        if (areaCollider == null)
        {
            Debug.LogError($"{name} thiếu BoxCollider để tính Area!");
            return;
        }

        InitGridAndSlots();
    }

    void OnDestroy()
    {
        All.Remove(this);
    }

    // Gọi khi thay đổi rows/columns trong Editor (play mode hoặc không)
#if UNITY_EDITOR
    void OnValidate()
    {
        if (rows < 1) rows = 1;
        if (columns < 1) columns = 1;

        areaCollider = GetComponent<BoxCollider>();
        if (areaCollider != null && Application.isPlaying == false)
        {
            InitGridAndSlots();
        }
    }
#endif

    private void InitGridAndSlots()
    {
        int totalSlots = rows * columns;

        // giữ lại nội dung cũ nếu có thể (đang play)
        var oldSlots = slots;
        slots = new MoneyBag[totalSlots];

        if (oldSlots != null)
        {
            for (int i = 0; i < Mathf.Min(oldSlots.Length, slots.Length); i++)
                slots[i] = oldSlots[i];
        }

        slotPositions = new Vector3[totalSlots];

        CalculateGrid();

        for (int i = 0; i < totalSlots; i++)
            slotPositions[i] = IndexToWorldPosition(i);
    }

    // ================= GRID ===================
    private void CalculateGrid()
    {
        // Dùng local size/center rồi map ra world bằng TransformPoint.
        // Cách này an toàn với non-uniform scale & rotation.
        Vector3 size = areaCollider.size;

        spacingX = size.x / Mathf.Max(1, columns);
        spacingZ = size.z / Mathf.Max(1, rows);
    }

    private Vector3 IndexToWorldPosition(int index)
    {
        int row = index / columns;   // row theo Z
        int col = index % columns;   // col theo X

        Vector3 localCenter = areaCollider.center;

        float startX = localCenter.x - areaCollider.size.x * 0.5f + spacingX * 0.5f;
        float startZ = localCenter.z - areaCollider.size.z * 0.5f + spacingZ * 0.5f;

        float x = startX + col * spacingX;
        float z = startZ + row * spacingZ;

        // Y: mặt trên collider
        float y = localCenter.y + areaCollider.size.y * 0.5f;

        Vector3 localPos = new Vector3(x, y, z);
        return transform.TransformPoint(localPos);
    }

    // ================= SLOT ===================
    public bool CanAddGift() => GetEmptySlotIndex() != -1;

    public void AddGift(MoneyBag gift, int slotIndex = -1)
    {
        if (slotIndex < 0 || slotIndex >= SlotCount)
            slotIndex = GetEmptySlotIndex();
        if (slotIndex == -1) return;

        slots[slotIndex] = gift;
        gift.isCarried = false;

        // Giữ world scale khi parent vào Area để KHÔNG bị bè theo scale của Plane
        Vector3 worldScale = gift.transform.lossyScale;

        gift.transform.SetParent(transform, true); // giữ world pos/rot tạm thời
        gift.transform.position = slotPositions[slotIndex];
        gift.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);

        SetWorldScale(gift.transform, worldScale);
    }

    private static void SetWorldScale(Transform tr, Vector3 targetWorldScale)
    {
        Vector3 parentScale = tr.parent ? tr.parent.lossyScale : Vector3.one;

        // Tránh chia 0 nếu ai đó scale parent = 0
        float sx = Mathf.Approximately(parentScale.x, 0f) ? 1f : parentScale.x;
        float sy = Mathf.Approximately(parentScale.y, 0f) ? 1f : parentScale.y;
        float sz = Mathf.Approximately(parentScale.z, 0f) ? 1f : parentScale.z;

        tr.localScale = new Vector3(
            targetWorldScale.x / sx,
            targetWorldScale.y / sy,
            targetWorldScale.z / sz
        );
    }

    public void RemoveGift(MoneyBag gift)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == gift)
            {
                slots[i] = null;
                StartCoroutine(RespawnGiftAfterDelay(i));
                return;
            }
        }
    }

    private IEnumerator RespawnGiftAfterDelay(int slotIndex)
    {
        yield return new WaitForSeconds(2f);
        if (slots[slotIndex] == null)
        {
            MoneyGameManager gm = FindObjectOfType<MoneyGameManager>();
            if (gm != null) gm.SpawnGiftInHouse(this, slotIndex);
        }
    }

    public Vector3 GetSlotPosition(int index) => slotPositions[index];

    public int GetEmptySlotIndex()
    {
        for (int i = 0; i < slots.Length; i++)
            if (slots[i] == null) return i;
        return -1;
    }

    public int GetNearestEmptySlot(Vector3 fromPos, float maxDistance = Mathf.Infinity)
    {
        int nearestSlot = -1;
        float minDist = Mathf.Infinity;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null) continue;

            float dist = Vector3.Distance(fromPos, slotPositions[i]);
            if (dist < minDist && dist <= maxDistance)
            {
                minDist = dist;
                nearestSlot = i;
            }
        }
        return nearestSlot;
    }

    public MoneyBag GetNearestGift(Vector3 playerPos, float range)
    {
        MoneyBag nearest = null;
        float minDist = Mathf.Infinity;

        foreach (MoneyBag gift in slots)
        {
            if (gift == null || gift.isCarried) continue;
            float dist = Vector3.Distance(playerPos, gift.transform.position);
            if (dist < minDist && dist <= range)
            {
                minDist = dist;
                nearest = gift;
            }
        }
        return nearest;
    }

    // Chia theo CỘT: luân phiên Suitcase/Pile cho tất cả cột (không còn None)
    public SlotType GetSlotTypeAt(int index)
    {
        int col = index % columns;
        return (col % 2 == 0) ? SlotType.Suitcase : SlotType.Pile;
    }
}
