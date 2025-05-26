using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    public static Inventory inventory;

    public Transform itemSlotTemplate;
    public Transform itemSlotContainer;

    // Start is called before the first frame update
    void Start()
    {
        inventory = new Inventory();
        inventory.onInventoryChange += OnInventoryChange;
        RefreshUI(inventory.GetAllItems());
    }

    void OnInventoryChange(List<ItemSO> items)
    {
        RefreshUI(items);
    }

    void RefreshUI(List<ItemSO> items)
    {
        foreach(Transform child in itemSlotContainer)
        {
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach(ItemSO item in items)
        {
            RectTransform itemSlot = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();

            itemSlot.Find("Name").GetComponent<TextMeshProUGUI>().text = item.itemName;
            itemSlot.Find("Image").GetComponent<Image>().sprite = item.itemSprite;
            //Y như mọi lần
        }
    }


}
