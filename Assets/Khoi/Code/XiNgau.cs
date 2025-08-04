using UnityEngine;

public class XiNgau : MonoBehaviour
{
    public int currentNumber = 1;
    public bool isSpinning = true;

    private KinhTienDoan kinhTienDoan;

    void Start()
    {
        GameObject miror = GameObject.Find("Miror");
        if (miror != null)
        {
            kinhTienDoan = miror.GetComponent<KinhTienDoan>();
        }
    }

    void Update()
    {
        if (isSpinning)
        {
            currentNumber = Random.Range(1, 10);
            Debug.Log("Đang xoay: " + currentNumber);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isSpinning)
        {
            isSpinning = false;

            if (kinhTienDoan != null && kinhTienDoan.soDuocChon >= 1)
            {
                currentNumber = kinhTienDoan.soDuocChon;
                Debug.Log("Dừng đúng số đã chọn: " + currentNumber);
            }
            else
            {
                currentNumber = Random.Range(1, 10);
                Debug.Log("Không có số chọn, dừng ngẫu nhiên: " + currentNumber);
            }
        }
    }
}
