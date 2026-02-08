using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    public static UI_Inventory instance;
    public NewBoardGameController currentController;
    private Inventory _inventory;

    public Sprite emptySlotSprite;
    public Image[] itemImg;
    public Image hoverImg;

    private Vector2 selectInput;

    float inputCooldown = 0.25f;
    float inputTimer;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        selectInput = currentController.playerInput.actions["Move"].ReadValue<Vector2>();
        inputTimer -= Time.deltaTime;

        if(inputTimer <= 0)
        {
            if(selectInput.x > 0.5f)
            {
                inputTimer = inputCooldown;
            }
            else if(selectInput.x < -0.5f)
            {
                inputTimer = inputCooldown;
            }
        }
    }

    public void Init(Inventory inventory)
    {
        _inventory = inventory;
    }

}
