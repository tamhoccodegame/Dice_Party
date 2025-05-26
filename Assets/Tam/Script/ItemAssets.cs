using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets instance;

    private void Awake()
    {
        instance = this;
    }

    [Header("Sprite")]
    public Sprite electricGunSprite;

    [Header("Prefab")]
    public GameObject electricGunPrefab;

    public Sprite GetSprite(Item.ItemType itemType)
    {
        switch (itemType)
        {
            case Item.ItemType.ElectricGun:
                return electricGunSprite;
            default: return null;
        }
    }

    public GameObject GetPrefab(Item.ItemType itemType)
    {
        switch (itemType)
        {
            case Item.ItemType.ElectricGun:
                return electricGunPrefab;
            default: return null;
        }
    }
}
