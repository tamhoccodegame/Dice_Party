using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static FallingItem;

public class ItemCollector : MonoBehaviour
{
    [Header("Setup")]
    public Transform itemContainer; // Trung tâm đáy ly (local)
    public GameObject vfxPrefab;
    public TextMeshProUGUI itemCountTMP;

    [Header("Arrangement Settings")]
    public float radius = 0.25f;             // Bán kính sắp xếp item
    public float heightStep = 0.08f;         // Khoảng cách giữa các lớp theo chiều cao
    public float itemSpacing = 0.2f;         // Khoảng cách giữa các item trên cùng một lớp
    public float randomRotationAngle = 15f;  // Độ lệch ngẫu nhiên của từng item

    private List<GameObject> collectedItems = new List<GameObject>();

    public PlayerInput playerInput;

    //public void AddItem(GameObject item, Vector3 hitPoint)
    //{
    //    // 1. VFX tại vị trí va chạm
    //    if (vfxPrefab != null)
    //        Instantiate(vfxPrefab, hitPoint, Quaternion.identity);

    //    // 2. Tạo bản sao mini của item (có thể khác prefab ngoài đời)
    //    GameObject miniItem = Instantiate(item.GetComponent<FallingItem>().visualInCupPrefab, itemContainer);
    //    collectedItems.Add(miniItem);

    //    // 3. Arrange lại toàn bộ
    //    ArrangeItems();

    //    // 4. Update UI
    //    UpdateCountUI();
    //}

    private void Start()
    {
        //itemCountTMP.text = "Collected items: 0";
    }

    [Header("Bomb Settings")]
    public int penaltyCount = 3;             // Số lượng item bị trừ khi trúng bomb
    public GameObject bombVFX;
    public void AddItem(GameObject item, Vector3 hitPoint, bool isGroundHit = false)
    {
        

        var fallingItem = item.GetComponent<FallingItem>();
        Debug.Log($"[AddItem] Type: {fallingItem.itemType}, GroundHit: {isGroundHit}");
        if (fallingItem.itemType == ItemType.Bomb)
        {
            Debug.Log("💣 Bomb detected!");
            if (bombVFX != null)
            {
                Instantiate(bombVFX, hitPoint, Quaternion.identity);
                GameObject vfx = Instantiate(bombVFX, hitPoint, Quaternion.identity);
                Destroy(vfx, 2f);
            }
                

            if (!isGroundHit) // Chỉ trừ điểm nếu bomb trúng ly
            {
                WizardMiniGameManager.instance.UpdatePlayerScore(playerInput, -3);
                Debug.Log("⚠️ Bomb hit CUP! Trừ điểm!");
                for (int i = 0; i < penaltyCount && collectedItems.Count > 0; i++)
                {
                    var lastItem = collectedItems[^1];
                    collectedItems.RemoveAt(collectedItems.Count - 1);
                    Destroy(lastItem);
                }
                UpdateCountUI();
            }
            

            Destroy(item);
            return;
        }

        WizardMiniGameManager.instance.UpdatePlayerScore(playerInput, 1);

        // Item thường
        if (vfxPrefab != null)
        {
            //Instantiate(vfxPrefab, hitPoint, Quaternion.identity);
            GameObject vfx_Normal = Instantiate(vfxPrefab, hitPoint, Quaternion.identity);
            Destroy(vfx_Normal, 2f);
        }
            

        GameObject miniItem = Instantiate(fallingItem.visualInCupPrefab, itemContainer);
        collectedItems.Add(miniItem);

        ArrangeItems();
        UpdateCountUI();
        Destroy(item);
    }

    void ArrangeItems()
    {
        int itemsPerLayer = Mathf.Max(1, Mathf.FloorToInt((2 * Mathf.PI * radius) / itemSpacing));

        for (int i = 0; i < collectedItems.Count; i++)
        {
            int layer = i / itemsPerLayer;
            int indexInLayer = i % itemsPerLayer;

            float angle = (360f / itemsPerLayer) * indexInLayer;
            float y = layer * heightStep;

            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            float z = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

            Vector3 localPos = new Vector3(x, y, z);
            Quaternion localRot = Quaternion.Euler(
                Random.Range(-randomRotationAngle, randomRotationAngle),
                Random.Range(0, 360f),
                Random.Range(-randomRotationAngle, randomRotationAngle)
            );

            collectedItems[i].transform.localPosition = localPos;
            collectedItems[i].transform.localRotation = localRot;
        }
    }

    void UpdateCountUI()
    {
        if (itemCountTMP != null)
            itemCountTMP.text = $"Collected items: {collectedItems.Count}";
    }

    void OnDrawGizmosSelected()
    {
        if (itemContainer == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(itemContainer.position, radius);

        // Vẽ các lớp để debug chiều cao
        int maxLayer = Mathf.CeilToInt(collectedItems.Count / Mathf.Max(1f, (2 * Mathf.PI * radius) / itemSpacing));
        for (int i = 0; i < maxLayer; i++)
        {
            Vector3 layerPos = itemContainer.position + Vector3.up * i * heightStep;
            Gizmos.DrawWireSphere(layerPos, radius);
        }
    }
}
