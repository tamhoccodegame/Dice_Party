using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerCustom : NetworkBehaviour
{
    public GameObject[] hairs;
    public GameObject[] colors;
    public GameObject[]bodyparts;

    [Networked] public int currentHairIndex { get; set; } = 0;
    [Networked] public int currentColorIndex { get; set; } = 0;
    [Networked] public int currentBodypartIndex { get; set; } = 0;

    public override void Spawned()
    {
        if (!Object.HasInputAuthority) return;

        RequestApplyCustom(currentHairIndex, currentColorIndex, currentBodypartIndex);
    }

    public void RequestApplyCustom(int hairIndex, int colorIndex, int bodypartIndex)
    {
        CustomData customData = NetworkManager.customData;
        if (customData != null)
        {
            customData.hairIndex = currentHairIndex;
            customData.colorIndex = currentColorIndex;
            customData.bodyPartIndex = currentBodypartIndex;
        }
        RPC_RequestApplyCustom(hairIndex, colorIndex, bodypartIndex);       
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestApplyCustom(int hairIndex, int colorIndex, int bodypartIndex)
    {
        Debug.Log($"ID {Runner.LocalPlayer.PlayerId}: Tôi gửi yêu cầu Apply lên host rồi!");
        RPC_ApplyCustom(hairIndex, colorIndex, bodypartIndex);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ApplyCustom(int hairIndex, int colorIndex, int bodypartIndex)
    {
        Debug.Log($"Host nhận thông tin: hairIndex:{hairIndex} colorIndex:{colorIndex} bodypartIndex:{bodypartIndex}");
        ApplyHair(hairIndex);
        ApplyColor(colorIndex);
        ApplyBodypart(bodypartIndex);
    }

    public void NextHair()
    {
        if (!Object.HasInputAuthority) return;

        currentHairIndex = (currentHairIndex + 1) % hairs.Length;
        ApplyHair(currentHairIndex);
    }

    public void PrevHair()
    {
        if (!Object.HasInputAuthority) return;

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
        if (!Object.HasInputAuthority) return;

        currentColorIndex = (currentColorIndex + 1) % colors.Length;
        ApplyColor(currentColorIndex);
    }

    public void PrevColor()
    {
        if (!Object.HasInputAuthority) return;

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
        if (!Object.HasInputAuthority) return;

        currentBodypartIndex = (currentBodypartIndex + 1) % bodyparts.Length;
        ApplyBodypart(currentBodypartIndex);
    }

    public void PrevBodypart()
    {
        if (!Object.HasInputAuthority) return;

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
