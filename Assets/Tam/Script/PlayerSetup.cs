using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{
    public GameObject[] hairs;
    public GameObject[] colors;
    public GameObject[] bodyparts;


    private void Start()
    {
        if (!Object.HasInputAuthority) return;

        CustomData data = NetworkManager.customData;
        if (data != null)
            RPC_RequestUpdateCustom(data.hairIndex, data.colorIndex, data.bodyPartIndex);
    }

    public override void Spawned()
    {
        if (!Object.HasInputAuthority) return;

        CustomData data = NetworkManager.customData;
        if(data != null) 
        RPC_RequestUpdateCustom(data.hairIndex, data.colorIndex, data.bodyPartIndex);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestUpdateCustom(int hairIndex, int colorIndex, int bodyPartIndex)
    {
        RPC_UpdateCustom(hairIndex, colorIndex, bodyPartIndex);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UpdateCustom(int hairIndex, int colorIndex, int bodyPartIndex)
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
