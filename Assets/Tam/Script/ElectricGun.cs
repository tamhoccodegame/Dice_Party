using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricGun : MonoBehaviour
{
    [ContextMenu("AfterUsed")]
    public void DestroyGun()
    {
        UIInventory.inventory.AddItem(new Item { itemType = Item.ItemType.ElectricGun });
    }
}
