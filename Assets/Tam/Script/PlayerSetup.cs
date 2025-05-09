using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{
    public Transform hair;
    public Transform color;
    public Transform bodypart;

    public override void Spawned()
    {
        CustomData data = NetworkManager.customData;
        for (int i = 0; i < hair.childCount; i++)
        {
            Debug.Log(i);
            hair.GetChild(i).gameObject.SetActive(i == data.hairIndex);
        }
        for (int i = 0; i < color.childCount; i++)
        {
            color.GetChild(i).gameObject.SetActive(i == data.colorIndex);
        }
        for (int i = 0; i < bodypart.childCount; i++)
        {
            bodypart.GetChild(i).gameObject.SetActive(i == data.bodyPartIndex);
        }
    }

}
