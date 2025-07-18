using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSetup : MonoBehaviour
{
    public GameObject[] hairs;
    public GameObject[] colors;
    public GameObject[] bodyparts;

    public void Awake()
    {
        Custom data = PlayerManager.instance.GetComponentInChildren<CustomData>().GetCustom(GetComponent<PlayerInput>());
        UpdateCustom(data.hairIndex, data.colorIndex, data.bodyPartIndex);
    }


    public void UpdateCustom(int hairIndex, int colorIndex, int bodyPartIndex)
    {
        for (int i = 0; i < hairs.Length; i++)
        {
            hairs[i].SetActive(i == hairIndex);
        }
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i].SetActive(i == colorIndex);
        }
        for (int i = 0; i < bodyparts.Length; i++)
        {
            bodyparts[i].SetActive(i == bodyPartIndex);
        }
    }

}
