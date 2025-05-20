using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerCustom : NetworkBehaviour
{
    public Transform hair;
    public Transform color;
    public Transform bodypart;

    public int currentHairIndex = 0;
    public int currentColorIndex = 0;
    public int currentBodypartIndex = 0;

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

        currentHairIndex = (currentHairIndex + 1) % hair.childCount;
        ApplyHair(currentHairIndex);
    }

    public void PrevHair()
    {
        if (!Object.HasInputAuthority) return;

        currentHairIndex--;
        if (currentHairIndex < 0) currentHairIndex = hair.childCount - 1;
        ApplyHair(currentHairIndex);
    }

    public void ApplyHair(int index)
    {
        currentHairIndex = Mathf.Clamp(index, 0, hair.childCount - 1);

        for (int i = 0; i < hair.childCount; i++)
        {
            hair.GetChild(i).gameObject.SetActive(i == currentHairIndex);
        }
    }

    public void NextColor()
    {
        if (!Object.HasInputAuthority) return;

        currentColorIndex = (currentColorIndex + 1) % color.childCount;
        ApplyColor(currentColorIndex);
    }

    public void PrevColor()
    {
        if (!Object.HasInputAuthority) return;

        currentColorIndex--;
        if (currentColorIndex < 0) currentColorIndex = color.childCount - 1;
        ApplyColor(currentColorIndex);
    }

    public void ApplyColor(int index)
    {
        currentColorIndex = Mathf.Clamp(index, 0, color.childCount - 1);

        for (int i = 0; i < color.childCount; i++)
        {
            color.GetChild(i).gameObject.SetActive(i == currentColorIndex);
        }
    }

    public void NextBodypart()
    {
        if (!Object.HasInputAuthority) return;

        currentBodypartIndex = (currentBodypartIndex + 1) % bodypart.childCount;
        ApplyBodypart(currentBodypartIndex);
    }

    public void PrevBodypart()
    {
        if (!Object.HasInputAuthority) return;

        currentBodypartIndex--;
        if (currentBodypartIndex < 0) currentBodypartIndex = bodypart.childCount - 1;
        ApplyBodypart(currentBodypartIndex);
    }

    public void ApplyBodypart(int index)
    {
        currentBodypartIndex = Mathf.Clamp(index, 0, bodypart.childCount - 1);

        for (int i = 0; i < bodypart.childCount; i++)
        {
            bodypart.GetChild(i).gameObject.SetActive(i == currentBodypartIndex);
        }
    }
}
