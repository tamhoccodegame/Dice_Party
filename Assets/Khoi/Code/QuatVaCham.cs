using System.Collections;
using UnityEngine;

public class QuatVaCham : MonoBehaviour
{
    bool isColliding = false;

    void OnTriggerEnter(Collider hitObject)
    {
        if (isColliding) return;

        if(hitObject.TryGetComponent<MNGVongXoayController>(out var player))
        {
            isColliding = true;
            player.Die();
            StartCoroutine(Reset());
        }
        
    }

    IEnumerator Reset()
    {
        yield return new WaitForSecondsRealtime(1f);
        isColliding = false;
    }
}
