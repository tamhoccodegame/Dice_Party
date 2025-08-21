using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarryMovementSpeed : MonoBehaviour
{
    [Header("Base speed (dùng bởi movement của bạn)")]
    public float baseMoveSpeed = 5f;

    [Header("External multiplier (set bởi PlayerMoneyController)")]
    [Range(0.1f, 1f)] public float externalSpeedMultiplier = 1f;

    public void SetExternalSpeedMultiplier(float m)
    {
        externalSpeedMultiplier = Mathf.Clamp(m, 0.1f, 1f);
    }

    public float CurrentSpeed()
    {
        return baseMoveSpeed * externalSpeedMultiplier;
    }
}
