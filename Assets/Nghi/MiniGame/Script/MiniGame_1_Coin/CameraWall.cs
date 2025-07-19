using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("⚠️ Player bị tường camera đuổi kịp!");
            // Xử lý GameOver hoặc bất kỳ logic nào
        }
    }
}
