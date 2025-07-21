using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_N : MonoBehaviour
{
    [Header("Animation & Invincibility")]
    public Animator animator;
    public float invincibleDuration = 2f;
    public float blinkInterval = 0.15f;

    private bool isInvincible = false;
    private bool isBlinking = false;

    private List<SkinnedMeshRenderer> skinnedMeshes = new List<SkinnedMeshRenderer>();

    private void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // Lấy toàn bộ SkinnedMeshRenderer trong con cháu
        skinnedMeshes.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>());
    }

    public void OnHitByObstacle(Vector3 hitPoint)
    {
        if (isInvincible)
        {
            Debug.Log("[⚡ IMMUNE] Player is invincible");
            return;
        }

        animator.SetTrigger("isHurt");
        Debug.Log("[😵 HIT] Player took damage at " + hitPoint);
        Audio_Manager.Instance.Play2D("Hurt");

        Coin_Manager.Instance.DropCoins(hitPoint);

        StartCoroutine(InvincibilityRoutine());
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        isBlinking = true;

        float elapsed = 0f;
        bool visible = true;

        while (elapsed < invincibleDuration)
        {
            visible = !visible;
            SetAllMeshesVisible(visible);

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        SetAllMeshesVisible(true);
        isInvincible = false;
        isBlinking = false;

        Debug.Log("[🛡️ DONE] Invincibility ended.");
    }

    private void SetAllMeshesVisible(bool visible)
    {
        foreach (var mesh in skinnedMeshes)
        {
            if (mesh != null)
                mesh.enabled = visible;
        }
    }

    public bool IsInvincible() => isInvincible;


}
