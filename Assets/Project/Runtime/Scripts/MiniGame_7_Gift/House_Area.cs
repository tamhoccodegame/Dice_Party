using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class House_Area : MonoBehaviour
{
    public int ownerID;
    public int maxGifts = 16;
    public int columns = 4;
    public float spacing = 1.0f;

    // Registry toàn map để Player có thể fallback nếu collider hơi lệch
    public static readonly List<House_Area> All = new List<House_Area>();

    private GiftBox[] slots;          // mỗi slot chứa 1 quà hoặc null
    private Vector3[] slotPositions;  // vị trí cố định của từng slot
    private BoxCollider areaCollider;

    public PlayerInput houseOwner;
    public SpriteRenderer houseOwnerAvatar;

    void Awake()
    {
        All.Add(this);

        areaCollider = GetComponent<BoxCollider>();
        slots = new GiftBox[maxGifts];
        slotPositions = new Vector3[maxGifts];

        // Tính sẵn vị trí slot (grid ngay hàng thẳng lối)
        for (int i = 0; i < maxGifts; i++)
            slotPositions[i] = transform.TransformPoint(IndexToLocalPosition(i));

        // Khuyến nghị để dễ bắt phạm vi: collider là Trigger & bao trùm khu slot
        // areaCollider.isTrigger = true; // bật nếu bạn muốn
    }

    void OnDestroy()
    {
        All.Remove(this);
    }

    public bool CanAddGift() => GetEmptySlotIndex() != -1;

    public void AddGift(GiftBox gift, int slotIndex = -1)
    {
        if (slotIndex == -1) slotIndex = GetEmptySlotIndex();
        if (slotIndex == -1) return;

        slots[slotIndex] = gift;
        gift.isCarried = false;
        gift.transform.SetParent(transform);
        gift.transform.position = slotPositions[slotIndex];
        gift.transform.rotation = Quaternion.identity;

        if(houseOwner != null)
        WizardMiniGameManager.instance.UpdatePlayerScore(houseOwner, 20);
    }

    public void RemoveGift(GiftBox gift)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == gift)
            {
                slots[i] = null;
                if(houseOwner != null)
                WizardMiniGameManager.instance.UpdatePlayerScore(houseOwner, -20);
                return;
            }
        }
    }

    // Tìm quà gần player nhất nằm trong "range" (tha thứ)
    public GiftBox GetNearestGift(Vector3 playerPos, float range)
    {
        GiftBox nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GiftBox gift in slots)
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

    // Slot trống gần player nhất (có thể kèm maxDistance nếu muốn siết)
    public int GetNearestEmptySlot(Vector3 playerPos, float maxDistance = Mathf.Infinity)
    {
        int nearestSlot = -1;
        float minDist = Mathf.Infinity;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null) continue;

            float dist = Vector3.Distance(playerPos, slotPositions[i]);
            if (dist < minDist && dist <= maxDistance)
            {
                minDist = dist;
                nearestSlot = i;
            }
        }
        return nearestSlot;
    }

    public Vector3 GetSlotPosition(int index) => slotPositions[index];

    private int GetEmptySlotIndex()
    {
        for (int i = 0; i < slots.Length; i++)
            if (slots[i] == null) return i;
        return -1;
    }

    private Vector3 IndexToLocalPosition(int index)
    {
        int row = index / columns;
        int col = index % columns;

        float startX = -((columns - 1) * spacing) / 2f;
        float rows = Mathf.CeilToInt((float)maxGifts / columns);
        float startZ = -((rows - 1) * spacing) / 2f;

        float x = startX + col * spacing;
        float z = startZ + row * spacing;

        return new Vector3(x, 0f, z);
    }
}
