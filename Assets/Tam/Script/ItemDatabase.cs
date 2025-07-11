using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDictionary
{
    public int itemId;
    public BoardItem item;
}

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase instance;
    public List<ItemDictionary> itemDictionary;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetIdByItem(BoardItem _item)
    {
        foreach(var item in itemDictionary)
        {
            if(item.item == _item) return item.itemId;
        }
        return -1;
    }

    public BoardItem GetItemByItemId(int itemId)
    {
        foreach(var item in itemDictionary)
        {
            if(item.itemId == itemId) return item.item;
        }
        return null;
    }

    public void ReturnItemPosition(BoardItem _item)
    {
        foreach(var item in itemDictionary)
        {
            if(item.item == _item)
            {
                item.item.transform.position = transform.position;
                item.item.transform.SetParent(transform);
                return;
            }
        }
    }

}
