using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KinhTienDoan : MonoBehaviour
{
    public GameObject panelChonSo;         // Panel chứa các nút 1–9
    public TextMeshPro guongText;          // Text hiển thị số trên mặt gương
    public int soDuocChon = 1;            // Số được chọn
    private bool playerInRange = false;
    private bool daChon = false;

    void Start()
    {
        panelChonSo.SetActive(false);
        guongText.text = "?";
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.T) && !daChon)
        {
            panelChonSo.SetActive(true);
        }
    }

    public void ChonSo(int so)
    {
        soDuocChon = so;
        daChon = true;
        guongText.text = so.ToString();
        panelChonSo.SetActive(false);
        Debug.Log("Đã chọn số tiên đoán: " + so);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
