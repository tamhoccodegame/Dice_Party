using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGameStat : MonoBehaviour
{
    List<BoardItem> items = new();

    public int keyQty;
    public int cupQty;
    public int health;

    public void AddItem(BoardItem item)
    {
        items.Add(item);
    }

    public void UseItem(int index, NetworkId user)
    {
        if (index < 0 || index >= items.Count) return;

        items[index].Use(user); // polymorphism gọi đúng logic
        items.RemoveAt(index);
    }

    public List<BoardItem> GetItemList()
    {
        return items;
    }
}
