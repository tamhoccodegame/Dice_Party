using System.Collections.Generic;
using UnityEngine;

public class CongDichChuyen : MonoBehaviour
{
    public string playerTag = "Player";

    private GameObject currentPlayer; // Người vừa đi vào cổng
    private bool isChoosing = false; // Đang chờ chọn mục tiêu

    private void Update()
    {
        // Nếu đang chờ chọn và nhấn chuột trái
        if (isChoosing && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject selectedPlayer = hit.collider.gameObject;

                // Chỉ cho chọn người chơi khác
                if (selectedPlayer.CompareTag(playerTag) && selectedPlayer != currentPlayer)
                {
                    // Hoán đổi vị trí
                    Vector3 temp = currentPlayer.transform.position;
                    currentPlayer.transform.position = selectedPlayer.transform.position;
                    selectedPlayer.transform.position = temp;

                    Debug.Log("Đã hoán đổi vị trí với " + selectedPlayer.name);

                    isChoosing = false; // Tắt chế độ chọn
                    currentPlayer = null;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            currentPlayer = other.gameObject;
            isChoosing = true;

            Debug.Log("Đi vào cổng - hãy click chuột vào người chơi khác để hoán đổi vị trí.");
        }
    }
}
