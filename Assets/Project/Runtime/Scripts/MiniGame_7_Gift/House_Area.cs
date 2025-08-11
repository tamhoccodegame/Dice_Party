using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House_Area : MonoBehaviour
{
    public int ownerID;
    public int maxGifts = 16;
    public int columns = 4;
    public float spacing = 1.0f;

    private List<GiftBox> giftsInArea = new List<GiftBox>();
    private BoxCollider areaCollider;

    void Awake()
    {
        areaCollider = GetComponent<BoxCollider>();
        if (!areaCollider) Debug.LogError("House_Area cần BoxCollider để tính vị trí!");
    }

    public bool CanAddGift() => giftsInArea.Count < maxGifts;

    public void AddGift(GiftBox gift)
    {
        if (!CanAddGift()) return;

        giftsInArea.Add(gift);
        gift.isCarried = false;
        gift.transform.SetParent(transform);
        ArrangeGifts();
    }

    public void RemoveGift(GiftBox gift)
    {
        if (giftsInArea.Contains(gift))
        {
            giftsInArea.Remove(gift);
            ArrangeGifts();
        }
    }

    public GiftBox GetNearestGift(Vector3 playerPos)
    {
        GiftBox nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GiftBox gift in giftsInArea)
        {
            if (gift == null) continue;

            float dist = Vector3.Distance(playerPos, gift.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = gift;
            }
        }

        return nearest;
    }



    public Vector3 GetNextDropPosition()
    {
        int index = giftsInArea.Count;
        Vector3 localPos = IndexToLocalPosition(index);
        return transform.TransformPoint(localPos);
    }

    void ArrangeGifts()
    {
        for (int i = 0; i < giftsInArea.Count; i++)
        {
            Vector3 targetPos = transform.TransformPoint(IndexToLocalPosition(i));
            giftsInArea[i].transform.position = targetPos;
            giftsInArea[i].transform.rotation = Quaternion.identity;
        }
    }

    Vector3 IndexToLocalPosition(int index)
    {
        int row = index / columns;
        int col = index % columns;

        Vector3 size = areaCollider.size;
        float startX = -((columns - 1) * spacing) / 2f;
        float startZ = -((Mathf.CeilToInt((float)maxGifts / columns) - 1) * spacing) / 2f;

        float x = startX + col * spacing;
        float z = startZ + row * spacing;

        return new Vector3(x, 0f, z);
    }

    public Vector3 GetNearestDropPosition(Vector3 playerPos)
    {
        // Nếu đã full quà thì không thể drop
        if (!CanAddGift())
            return Vector3.zero;

        // Lấy tất cả vị trí khả dụng (từ index 0 -> maxGifts-1)
        List<int> emptySlots = new List<int>();
        for (int i = 0; i < maxGifts; i++)
        {
            if (i >= giftsInArea.Count) // slot trống
                emptySlots.Add(i);
        }

        // Tìm slot trống gần player nhất
        float minDist = Mathf.Infinity;
        Vector3 nearestPos = Vector3.zero;

        foreach (int slotIndex in emptySlots)
        {
            Vector3 worldPos = transform.TransformPoint(IndexToLocalPosition(slotIndex));
            float dist = Vector3.Distance(playerPos, worldPos);
            if (dist < minDist)
            {
                minDist = dist;
                nearestPos = worldPos;
            }
        }

        return nearestPos;
    }

}
