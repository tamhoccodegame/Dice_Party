using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public Action<List<ItemSO>> onInventoryChange;

    private List<ItemSO> items = new List<ItemSO>();

    public void AddItem(ItemSO newItem)
    {
        items.Add(newItem);
        onInventoryChange?.Invoke(items);
    }

    public void RemoveItem(ItemSO item)
    {
        items.Remove(item);
        onInventoryChange?.Invoke(items);
    }

    public List<ItemSO> GetAllItems()
    {
        return items;
    }
    

}
