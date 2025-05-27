using UnityEngine;

public class QuatVaCham : MonoBehaviour
{
    // Khi va chạm trigger với player
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug để kiểm tra
            Debug.Log("Player bị trúng cánh quạt!");

            // Bạn có thể gọi hàm chết ở player nếu có script Player
            // other.GetComponent<Player>().Die();  // nếu có

            // Hoặc đơn giản là phá hủy player
            Destroy(other.gameObject);
        }
    }
}
