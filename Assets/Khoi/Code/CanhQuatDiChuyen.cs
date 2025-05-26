using UnityEngine;

public class CanhQuatDiChuyen : MonoBehaviour
{
    public float speedMin = 50f;     // Tốc độ quay nhỏ nhất
    public float speedMax = 150f;    // Tốc độ quay lớn nhất

    private float currentSpeed;      // Tốc độ hiện tại của cánh quạt
    private float direction = 1f;    // Hướng quay (1 = lên, -1 = xuống)
    private float changeTime;        // Thời gian chờ để đổi hướng
    private float timer = 0f;        // Bộ đếm thời gian

    void Start()
    {
        // Gán tốc độ ngẫu nhiên trong khoảng cho mỗi cánh
        currentSpeed = Random.Range(speedMin, speedMax);

        // Gán thời gian đổi hướng ngẫu nhiên từ 1-3 giây
        changeTime = Random.Range(1f, 3f);
    }

    void Update()
    {
        // Quay cánh quạt theo trục Z (có thể đổi sang X nếu bạn muốn xoay kiểu khác)
        transform.Rotate(Vector3.right * currentSpeed * direction * Time.deltaTime);

        // Đếm thời gian để đảo hướng
        timer += Time.deltaTime;
        if (timer >= changeTime)
        {
            direction *= -1f; // Đảo hướng quay
            timer = 0f;

            // Tạo lại tốc độ và thời gian đổi hướng ngẫu nhiên mới
            currentSpeed = Random.Range(speedMin, speedMax);
            changeTime = Random.Range(1f, 3f);
        }
    }
}
