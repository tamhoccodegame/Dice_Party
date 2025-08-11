using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House_Area : MonoBehaviour
{
    public int ownerID;
    public int maxGifts = 16;
    public int columns = 4; // số cột khi xếp
    public float spacing = 1.0f; // khoảng cách giữa các gift

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

        // Lấy size và center từ BoxCollider
        Vector3 size = areaCollider.size;
        Vector3 center = areaCollider.center;

        // Số hàng tối đa
        int totalRows = Mathf.CeilToInt((float)maxGifts / columns);

        // Canh giữa dựa theo spacing
        float totalWidth = (columns - 1) * spacing;
        float totalHeight = (totalRows - 1) * spacing;

        float startX = -totalWidth / 2f;
        float startZ = -totalHeight / 2f;

        // Tính local position
        float x = startX + col * spacing;
        float z = startZ + row * spacing;

        // Cộng thêm center để đúng vị trí BoxCollider
        return new Vector3(x + center.x, 0f, z + center.z);
    }

}
