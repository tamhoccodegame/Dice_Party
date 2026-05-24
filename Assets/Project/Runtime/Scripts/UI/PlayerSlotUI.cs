using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PlayerSlotUI : MonoBehaviour
{
    public GameObject afterJoinPanel;
    public GameObject customizePanel;
    public GameObject unreadyPanel;
    public GameObject unreadyButton;
    public GameObject readyPanel;
    public Button applyButton;


    public OptionSelector colorSelector;
    public OptionSelector hairSelector;
    public OptionSelector bodyPartSelector;

    private bool isReady = false;

    private PlayerCustom playerCustom;
    public PlayerInput playerInput;

    public InputSystemUIInputModule inputSystemUIInputModule;

    private void Awake()
    {
        inputSystemUIInputModule = GetComponentInChildren<InputSystemUIInputModule>();
    }

 

    public void InitSelector(PlayerCustom playerCustom)
    {
        Debug.Log("Init");
        this.playerCustom = playerCustom;
        List<string> hairNames = new List<string>();
        List<string> bodyPartNames = new List<string>();

        int count = 0;

        foreach(var hair in playerCustom.hairs)
        {
            count++;
            hairNames.Add($"Hair {count}");
        }

        count = 0;

        foreach(var bodyPart in playerCustom.bodyparts)
        {
            count++;
            bodyPartNames.Add($"Body {count}");
        }

        colorSelector.Init(playerCustom);
        hairSelector.Init(playerCustom, hairNames);
        bodyPartSelector.Init(playerCustom, bodyPartNames);
    }

    public void SetReady(bool isReady)
    {
        this.isReady = isReady;
        Lobby.instance.SetReady(playerInput, isReady);

        if(isReady)
        playerCustom.GetComponent<Animator>().CrossFade("Ready", 0.25f);
        else 
        playerCustom.GetComponent<Animator>().CrossFade("Idle", 0.25f);
    }

    public void ApplyCustom()
    {
        playerCustom.ApplyCustoms();
    }

}
