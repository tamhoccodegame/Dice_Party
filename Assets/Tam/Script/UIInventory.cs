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

    private void Awake()
    {
        inventory = new Inventory();
    }

    // Start is called before the first frame update
    void Start()
    {
        inventory.onInventoryChange += OnInventoryChange;
        RefreshUI(inventory.GetAllItems());
        inventory.AddItem(new Item { itemType = Item.ItemType.ElectricGun });
    }

    void OnInventoryChange(List<Item> items)
    {
        RefreshUI(items);
    }

    void RefreshUI(List<Item> items)
    {
        foreach(Transform child in itemSlotContainer)
        {
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach(Item item in items)
        {
            RectTransform itemSlot = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlot.gameObject.SetActive(true);
            //Y như mọi lần
            itemSlot.GetComponent<Image>().sprite = ItemAssets.instance.GetSprite(item.itemType);
            itemSlot.GetComponent<Button>().onClick.RemoveAllListeners();
            itemSlot.GetComponent<Button>().onClick.AddListener(() =>
            {
                inventory.UseItem(item);
            });
        }
    }


}
