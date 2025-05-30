using UnityEngine;

public class QuatVaCham : MonoBehaviour
{
    // Khi va chạm trigger với player
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<MNGVongXoayController>(out MNGVongXoayController player))
        {
            player.Die();
        }
    }
}
