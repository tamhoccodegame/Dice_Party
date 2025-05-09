using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerCustom : NetworkBehaviour
{
    public Transform hair;
    public Transform color;
    public Transform bodypart;

    int currentHairIndex = 0;
    int currentColorIndex = 0;
    int currentBodypartIndex = 0;

    public override void Spawned()
    {
        if (!Object.HasInputAuthority) return;

        for(int i = 0; i < hair.childCount; i++)
        {
            if (hair.GetChild(i).gameObject.activeSelf)
            {
                currentHairIndex = i;
                break;
            }
        }
        for(int i = 0; i <  color.childCount; i++)
        {
            if (color.GetChild(i).gameObject.activeSelf)
            {
                currentColorIndex = i;
                break;
            }
        }
        for(int i = 0; i < bodypart.childCount; i++)
        {
            if (bodypart.GetChild(i).gameObject.activeSelf)
            {
                currentBodypartIndex = i;
                break;
            }
        }

        ApplyHair(currentColorIndex);
        ApplyColor(currentColorIndex);
        ApplyBodypart(currentBodypartIndex);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestApplyCustom()
    {
        RPC_ApplyCustom();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ApplyCustom()
    {
        CustomData customData = NetworkManager.customData;
        if (customData != null)
        {
            customData.hairIndex = currentHairIndex;
            customData.colorIndex = currentColorIndex;
            customData.bodyPartIndex = currentBodypartIndex;
        }
    }

    [ContextMenu("NextHair")]
    public void NextHair()
    {
        if (!Object.HasInputAuthority) return;

        currentHairIndex = (currentHairIndex + 1) % hair.childCount;
        ApplyHair(currentHairIndex);
    }

    [ContextMenu("PrevHair")]
    public void PrevHair()
    {
        if (!Object.HasInputAuthority) return;

        currentHairIndex--;
        if (currentHairIndex < 0) currentHairIndex = hair.childCount - 1;
        ApplyHair(currentHairIndex);
    }

    public void ApplyHair(int index)
    {
        if (!Object.HasInputAuthority) return;

        currentHairIndex = Mathf.Clamp(index, 0, hair.childCount - 1);

        for (int i = 0; i < hair.childCount; i++)
        {
            hair.GetChild(i).gameObject.SetActive(i == currentHairIndex);
        }
    }

    [ContextMenu("NextColor")]
    public void NextColor()
    {
        if (!Object.HasInputAuthority) return;

        currentColorIndex = (currentColorIndex + 1) % color.childCount;
        ApplyColor(currentColorIndex);
    }

    [ContextMenu("PrevColor")]
    public void PrevColor()
    {
        if (!Object.HasInputAuthority) return;

        currentColorIndex--;
        if (currentColorIndex < 0) currentColorIndex = color.childCount - 1;
        ApplyColor(currentColorIndex);
    }

    public void ApplyColor(int index)
    {
        if (!Object.HasInputAuthority) return;

        currentColorIndex = Mathf.Clamp(index, 0, color.childCount - 1);

        for (int i = 0; i < color.childCount; i++)
        {
            color.GetChild(i).gameObject.SetActive(i == currentColorIndex);
        }
    }

    [ContextMenu("NextBodypart")]
    public void NextBodypart()
    {
        if (!Object.HasInputAuthority) return;

        currentBodypartIndex = (currentBodypartIndex + 1) % bodypart.childCount;
        ApplyBodypart(currentBodypartIndex);
    }

    [ContextMenu("PrevBodypart")]
    public void PrevBodypart()
    {
        if (!Object.HasInputAuthority) return;

        currentBodypartIndex--;
        if (currentBodypartIndex < 0) currentBodypartIndex = bodypart.childCount - 1;
        ApplyBodypart(currentBodypartIndex);
    }

    public void ApplyBodypart(int index)
    {
        if (!Object.HasInputAuthority) return;

        currentBodypartIndex = Mathf.Clamp(index, 0, bodypart.childCount - 1);

        for (int i = 0; i < bodypart.childCount; i++)
        {
            bodypart.GetChild(i).gameObject.SetActive(i == currentBodypartIndex);
        }
    }
}
