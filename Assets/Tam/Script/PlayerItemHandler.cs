using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemHandler : MonoBehaviour
{
    private Inventory inventory;
    public GameObject currentItem;

    private Animator animator;
    private PlayerController controller;
    public bool isUsingItem = false;

    public Transform handTransform;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        inventory = UIInventory.inventory;
        inventory.onItemUsed += OnItemUsed;
    }

    void OnItemUsed(GameObject itemPrefab)
    {
        controller.enabled = false;
        Instantiate(itemPrefab, handTransform);

        if(itemPrefab.GetComponent<ElectricGun>())
        {
            //animator.Play("Aim");
        }

    }

}
