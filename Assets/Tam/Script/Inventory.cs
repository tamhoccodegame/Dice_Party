using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public Action<List<Item>> onInventoryChange;
    public Action<GameObject> onItemUsed;

    private List<Item> items = new List<Item>();

    public void UseItem(Item item)
    {
        RemoveItem(item);
        onItemUsed?.Invoke(ItemAssets.instance.GetPrefab(item.itemType));
    }

    public void AddItem(Item newItem)
    {
        items.Add(newItem);
        onInventoryChange?.Invoke(items);
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
        onInventoryChange?.Invoke(items);
    }

    public List<Item> GetAllItems()
    {
        return items;
    }
}
