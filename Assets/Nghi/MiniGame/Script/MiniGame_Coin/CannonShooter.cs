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

    public void Update()
    {
        if (Time.time < _nextFireTime) return;

        Shoot();
        _nextFireTime = Time.time + fireInterval;
    }

    private void Shoot()
    {
        if (!firePoint || !projectilePrefab || !projectileData)
        {
            Debug.LogError("[CannonShooter] Missing reference!");
            return;
        }

        // 1. VFX tại nòng đại bác khi bắn
        if (projectileData.shootVFX != null)
        {
            GameObject flash = Instantiate(projectileData.shootVFX, firePoint.position, firePoint.rotation);
        }

        // Spawn đạn tại firePoint
        GameObject projGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Khởi tạo đạn
        if (projGO.TryGetComponent(out CannonProjectile proj))
            proj.Init(projectileData, firePoint.forward);
        else
            Debug.LogError("[CannonShooter] Projectile prefab lacks CannonProjectile!");
    }
}
