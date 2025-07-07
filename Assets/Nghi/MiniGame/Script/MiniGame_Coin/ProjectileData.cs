using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Gameplay/Projectile Data")]
public class ProjectileData : ScriptableObject
{
    [Header("Ballistics")]
    public float speed = 40f;         // m/s
    public float lifeTime = 4f;       // s

    [Header("FX")]
    public GameObject impactVFX;
    public GameObject shootVFX;
}
