using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDonNha : MonoBehaviour
{
    public BoxCollider col;

    public int trashCount;
    public LayerMask trashLayer;

    // Update is called once per frame
    void Update()
    {
        Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, Quaternion.identity, trashLayer);

        trashCount = cols.Length;
    }

    private void OnDrawGizmos()
    {
        if (col == null) return;

        Gizmos.color = Color.cyan;

        Bounds b = col.bounds;

        Gizmos.DrawWireCube(b.center, b.size);
    }
}
