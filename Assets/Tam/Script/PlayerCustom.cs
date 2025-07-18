using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCustom : MonoBehaviour
{
    public GameObject[] hairs;
    public GameObject[] colors;
    public GameObject[] bodyparts;

    public int currentHairIndex = 0;
    public int currentColorIndex = 0;
    public int currentBodypartIndex = 0;

    private PlayerInput playerInput;

    public void Awake()
    {
    }

    public void Init(PlayerInput playerInput)
    {
        this.playerInput = playerInput;
        ApplyCustoms();
    }

    public void ApplyCustoms()
    {
        CustomData customData = PlayerManager.instance.GetComponentInChildren<CustomData>();
        if (customData != null)
        {
            Custom custom = new Custom { hairIndex = currentHairIndex, colorIndex = currentColorIndex, bodyPartIndex = currentBodypartIndex };
            customData.SaveCustom(playerInput, custom);
        }
        ApplyHair(currentHairIndex);
        ApplyColor(currentColorIndex);
        ApplyBodypart(currentBodypartIndex);
    }

    public void NextHair()
    {
        currentHairIndex = (currentHairIndex + 1) % hairs.Length;
        ApplyHair(currentHairIndex);
    }

    public void PrevHair()
    {
        currentHairIndex--;
        if (currentHairIndex < 0) currentHairIndex = hairs.Length - 1;
        ApplyHair(currentHairIndex);
    }

    public void ApplyHair(int index)
    {
        for (int i = 0; i < hairs.Length; i++)
        {
            hairs[i].SetActive(i == index);
        }
    }

    public void NextColor()
    {
        currentColorIndex = (currentColorIndex + 1) % colors.Length;
        ApplyColor(currentColorIndex);
    }

    public void PrevColor()
    {
        currentColorIndex--;
        if (currentColorIndex < 0) currentColorIndex = colors.Length - 1;
        ApplyColor(currentColorIndex);
    }

    public void ApplyColor(int index)
    {
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i].SetActive(i == index);
        }
    }

    public void NextBodypart()
    {
        currentBodypartIndex = (currentBodypartIndex + 1) % bodyparts.Length;
        ApplyBodypart(currentBodypartIndex);
    }

    public void PrevBodypart()
    {
        currentBodypartIndex--;
        if (currentBodypartIndex < 0) currentBodypartIndex = bodyparts.Length - 1;
        ApplyBodypart(currentBodypartIndex);
    }

    public void ApplyBodypart(int index)
    {
        for (int i = 0; i < bodyparts.Length; i++)
        {
            bodyparts[i].SetActive(i == index);
        }
    }
}
