using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInfo
{
    public string itemName;
    public GameObject itemPrefab;
}

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase instance;

    public List<ItemInfo> itemInfos;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public GameObject GetItemPrefab(string itemName)
    {
        ItemInfo itemInfo = null;
        foreach(ItemInfo item in itemInfos)
        {
            if (item.itemName == itemName)
            {
                itemInfo = item;
                break;
            }
        }

        if (itemInfo != null)
            return itemInfo.itemPrefab;
        return null;
    }
}
