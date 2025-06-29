using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardSlotRect : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Transform cups;
    public TextMeshProUGUI keyText;
    public TextMeshProUGUI healthText;
    //public Slider healthSlider;

    public void UpdateName(string name)
    {
        nameText.text = name;
    }

    public void UpdateCup(int cup)
    {
        foreach(Transform child in cups)
        {
            child.gameObject.SetActive(false);
        }

        for(int i = 0; i < cup; i++)
        {
            cups.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void UpdateKey(int key)
    {
        keyText.text = key.ToString();
    }

    public void UpdateHealth(int health)
    {
        //healthSlider.value = health;
        healthText.text = health.ToString();    
    }
}
