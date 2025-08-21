using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;                // Gán đầu nòng
    [SerializeField] private ProjectileData projectileData;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Fire Control")]
    [SerializeField, Tooltip("Seconds per shot")]
    private float fireInterval = .5f;                            // x giây/viên

    private float _nextFireTime;

    private void Update()
    {
        if (!T_Coin_Manager.Instance.isGameStarted || T_Coin_Manager.Instance.isGameOver) return; // chỉ bắn khi game đang chạy
        if (Time.time < _nextFireTime) return; // chưa đến thời điểm bắn

        Shoot();
        _nextFireTime = Time.time + fireInterval; // cập nhật thời gian bắn tiếp theo
    }

    private void Shoot()
    {
        if (!firePoint || !projectilePrefab || !projectileData) // Kiểm tra các tham chiếu cần thiết
        {
            Debug.LogError("[CannonShooter] Missing reference!");
            return;
        }

        // 1. VFX tại nòng đại bác khi bắn
        if (projectileData.shootVFX != null && !projectileData.shootVFX.Equals(null)) // Kiểm tra prefab VFX có hợp lệ không
        {
            GameObject flash = Instantiate(projectileData.shootVFX, firePoint.position, firePoint.rotation); // spawn VFX tại nòng
            Destroy(flash, 2f); // auto cleanup
        }
        else
        {
            Debug.LogWarning("[CannonShooter] shootVFX prefab missing or destroyed.");
        }

        // Spawn đạn tại firePoint
        GameObject projGO = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation);  // hướng theo nòng

        // Khởi tạo đạn
        if (projGO.TryGetComponent(out T_CannonProjectile proj)) // Kiểm tra xem prefab có chứa T_CannonProjectile không
            proj.Init(projectileData, firePoint.forward); //Init đạn với dữ liệu và hướng
        //Init là hàm khởi tạo trong T_CannonProjectile
        else
            Debug.LogError("[CannonShooter] Projectile prefab lacks CannonProjectile!");
    }
}
