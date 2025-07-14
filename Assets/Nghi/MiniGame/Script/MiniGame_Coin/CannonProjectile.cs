using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class CannonProjectile : NetworkBehaviour
{
    private Rigidbody _rb;
    private ProjectileData _data;

    public override void Spawned()
    {
        if (!HasStateAuthority) return;
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
        if(!HasStateAuthority) return;
        // Spawn VFX nếu có
        if (_data.impactVFX)
            Runner.Spawn(_data.impactVFX, transform.position, Quaternion.identity);

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
        if (!HasStateAuthority) return;
        // Nếu sau này xài Object Pool thì đổi thành pool.Despawn(this);
        Destroy(gameObject);
    }
}
