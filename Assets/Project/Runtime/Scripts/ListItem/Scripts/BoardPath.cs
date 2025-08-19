using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoardPath : MonoBehaviour
{

    public Transform[] slots;

    public Transform GetSlot(int index)
    {
        if (slots == null || slots.Length == 0) return null;
        return slots[index % slots.Length];
    }

    public int totalSlots => slots != null ? slots.Length : 0;
    public int GetNearestSlotIndex(Vector3 pos)
    {
        int nearestIndex = 0;
        float minDist = float.MaxValue;
        for (int i = 0; i < slots.Length; i++)
        {
            float dist = Vector3.Distance(pos, slots[i].position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestIndex = i;
            }
        }
        return nearestIndex;
    }
}

