using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyBag : MonoBehaviour
{
    public bool isCarried = false;
    public int ownerID = -1;
    private Transform carryPoint;

    public void PickUp(Transform carryParent)
    {
        isCarried = true;
        carryPoint = carryParent;
        transform.SetParent(carryParent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        // Debug.Log("Gift picked up");
    }

    public void Drop(Vector3 dropPos)
    {
        isCarried = false;
        carryPoint = null;
        transform.SetParent(null);
        transform.position = dropPos;
        transform.rotation = Quaternion.identity;
    }
}
