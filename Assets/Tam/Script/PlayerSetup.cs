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
        RPC_RequestUpdateCustom(data.hairIndex, data.colorIndex, data.bodyPartIndex);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestUpdateCustom(int hairIndex, int colorIndex, int bodyPartIndex)
    {
        if(Object.HasInputAuthority)
        RPC_UpdateCustom(hairIndex, colorIndex, bodyPartIndex);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UpdateCustom(int hairIndex, int colorIndex, int bodyPartIndex)
    {
        CustomData data = NetworkManager.customData;
        for (int i = 0; i < hair.childCount; i++)
        {
            //Debug.Log(i);
            hair.GetChild(i).gameObject.SetActive(i == hairIndex);
        }
        for (int i = 0; i < color.childCount; i++)
        {
            color.GetChild(i).gameObject.SetActive(i == colorIndex);
        }
        for (int i = 0; i < bodypart.childCount; i++)
        {
            bodypart.GetChild(i).gameObject.SetActive(i == bodyPartIndex);
        }
    }

}
