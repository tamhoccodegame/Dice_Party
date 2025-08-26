using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyGameManager : MonoBehaviour
{
    [Header("Spawn Prefabs (Grouped)")]
    [Tooltip("Các prefab 'đống tiền' (money pile)")]
    public GameObject[] pilePrefabs;

    [Tooltip("Các prefab 'vali tiền' (briefcase/suitcase)")]
    public GameObject[] suitcasePrefabs;

    [Range(0f, 1f)]
    [Tooltip("Tỉ lệ mong muốn của vali tiền trên tổng spawn. 0.5 = 50/50")]
    public float suitcaseRatio = 0.5f;

    [Header("Areas")]
    public MoneyBagArea[] moneyBagAreas;

    [Header("Initial Spawn")]
    [Tooltip("Nếu bật: tự động lấp đầy toàn bộ slots (rows*columns).")]
    public bool fillAllSlots = true;

    [Tooltip("Nếu tắt fillAllSlots: số gift spawn mỗi Area (sẽ clamp <= slots).")]
    public int giftsPerArea = 4;

    [Header("Gameplay")]
    public float gameDuration = 60f;
    private float timer;

    // Đếm toàn cục để giữ cân bằng dài hạn
    private int spawnedPileCount = 0;
    private int spawnedSuitcaseCount = 0;

    void Start()
    {
        SpawnGifts();
        timer = gameDuration;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) EndGame();
    }

    void SpawnGifts()
    {
        if (moneyBagAreas == null) return;

        foreach (var house in moneyBagAreas)
        {
            if (house == null) continue;

            int want = fillAllSlots ? house.SlotCount : Mathf.Min(giftsPerArea, house.SlotCount);

            for (int slot = 0; slot < want; slot++)
            {
                if (!house.CanAddGift()) break;
                SpawnGiftInHouse(house, slot); // chỉ định slot để đảm bảo phủ kín grid
            }
        }
    }

    public void SpawnGiftInHouse(MoneyBagArea house, int slotIndex = -1)
    {
        if (house == null || !house.CanAddGift()) return;

        if (slotIndex == -1)
            slotIndex = house.GetEmptySlotIndex();

        if (slotIndex == -1) return;

        // Ưu tiên type theo cột; nếu NONE thì fallback cân bằng
        SlotType t = house.GetSlotTypeAt(slotIndex);

        GameObject prefab = null;
        if (t == SlotType.Pile && pilePrefabs != null && pilePrefabs.Length > 0)
        {
            prefab = pilePrefabs[Random.Range(0, pilePrefabs.Length)];
            spawnedPileCount++;
        }
        else if (t == SlotType.Suitcase && suitcasePrefabs != null && suitcasePrefabs.Length > 0)
        {
            prefab = suitcasePrefabs[Random.Range(0, suitcasePrefabs.Length)];
            spawnedSuitcaseCount++;
        }
        else
        {
            // Fallback: chọn cân bằng theo tỉ lệ
            prefab = PickBalancedPrefab();
        }

        if (prefab == null) return;

        Vector3 spawnPos = house.GetSlotPosition(slotIndex);

        // Instantiate KHÔNG parent để world scale = local scale gốc
        GameObject giftObj = Instantiate(prefab, spawnPos, Quaternion.identity);

        MoneyBag gift = giftObj.GetComponent<MoneyBag>();
        if (gift == null)
        {
            Debug.LogWarning($"Prefab {prefab.name} không có component MoneyBag!");
            Destroy(giftObj);
            return;
        }

        gift.ownerID = house.ownerID;

        // AddGift sẽ parent + giữ world scale + xoay -90° X
        house.AddGift(gift, slotIndex);
    }

    private GameObject PickBalancedPrefab()
    {
        bool hasPiles = pilePrefabs != null && pilePrefabs.Length > 0;
        bool hasSuitcases = suitcasePrefabs != null && suitcasePrefabs.Length > 0;

        if (!hasPiles && !hasSuitcases) return null;
        if (!hasPiles) return PickFromArray(suitcasePrefabs, isSuitcase: true);
        if (!hasSuitcases) return PickFromArray(pilePrefabs, isSuitcase: false);

        int total = spawnedPileCount + spawnedSuitcaseCount;
        float currentSuitcaseRatio = total > 0 ? (float)spawnedSuitcaseCount / total : 0f;

        bool pickSuitcase;
        if (currentSuitcaseRatio < suitcaseRatio) pickSuitcase = true;
        else if (currentSuitcaseRatio > suitcaseRatio) pickSuitcase = false;
        else pickSuitcase = Random.value < 0.5f;

        return pickSuitcase
            ? PickFromArray(suitcasePrefabs, isSuitcase: true)
            : PickFromArray(pilePrefabs, isSuitcase: false);
    }

    private GameObject PickFromArray(GameObject[] arr, bool isSuitcase)
    {
        if (arr == null || arr.Length == 0) return null;
        GameObject prefab = arr[Random.Range(0, arr.Length)];
        if (isSuitcase) spawnedSuitcaseCount++; else spawnedPileCount++;
        return prefab;
    }

    void EndGame()
    {
        Debug.Log("Game Over!");
        Time.timeScale = 0f;
    }
}
