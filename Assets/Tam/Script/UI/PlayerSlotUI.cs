using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlotUI : MonoBehaviour
{
    public GameObject afterJoinPanel;
    public GameObject customizePanel;
    public GameObject adjustAppearancePanel;
    public GameObject unreadyPanel;
    public GameObject unreadyButton;
    public GameObject readyPanel;

    public OptionSelector hairSelector;
    public OptionSelector bodyPartSelector;

    private void Awake()
    {
        //hairSelector.textOptions.Clear();
        //bodyPartSelector.textOptions.Clear();
    }

    public void AddHairName(string hairName)
    {
        hairSelector.textOptions.Add(hairName);
    }

    public void AddBodypartName(string bodypartName)
    {
        bodyPartSelector.textOptions.Add(bodypartName);
    }
}
