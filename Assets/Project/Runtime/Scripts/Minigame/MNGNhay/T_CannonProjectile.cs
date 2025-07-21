using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class T_CannonProjectile : MonoBehaviour
{
    private Rigidbody _rb;
    private ProjectileData _data;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    public void Init(ProjectileData data, Vector3 direction)
    {
        _data = data;

        // Nạp lực – dùng VelocityChange để bỏ qua khối lượng
        _rb.AddForce(direction.normalized * _data.speed, ForceMode.VelocityChange);

        // Tự hủy sau _data.lifeTime giây
        Invoke(nameof(Despawn), _data.lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Spawn VFX nếu có
        if (_data.impactVFX)
            Instantiate(_data.impactVFX, transform.position, Quaternion.identity);

        if (collision.collider.CompareTag("Player"))
        {
            PlayerBlinking player = collision.collider.GetComponent<PlayerBlinking>();
            if (player == null)
                player = collision.collider.GetComponentInParent<PlayerBlinking>();

            if (player != null)
            {
                Vector3 hitPoint = collision.contacts[0].point;
                player.OnHitByObstacle(hitPoint);
            }
        }


        Despawn();
    }



    private void Despawn()
    {
        // Nếu sau này xài Object Pool thì đổi thành pool.Despawn(this);
        Destroy(gameObject);
    }
}
