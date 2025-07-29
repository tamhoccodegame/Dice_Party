using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBlinking : MonoBehaviour
{
    [Header("Animation & Invincibility")]
    public Animator animator;
    public float invincibleDuration = 2f;
    public float blinkInterval = 0.15f;

    private bool isInvincible { get; set; }
    private bool isBlinking { get; set; }

    private List<SkinnedMeshRenderer> skinnedMeshes = new List<SkinnedMeshRenderer>();

    private void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        skinnedMeshes.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>());
    }

    public void OnHitByObstacle(Vector3 hitPoint)
    {
        if (isInvincible)
        {
            Debug.Log("[⚡ IMMUNE] Player is invincible");
            return;
        }

        PlayerInput playerInput = GetComponent<PlayerController>().GetPlayerInput();
        int currentLives = WizardPartyData.instance.playerLives[playerInput];
        WizardPartyData.instance.UpdatePlayerLive(playerInput, currentLives - 1);
        if(currentLives - 1 <= 0)
        {
            WizardPartyData.instance.UpdatePlayerLive(playerInput, 0);
            WizardMiniGameManager.instance.playersCompleteGame.Add(playerInput);
            GetComponent<PlayerController>().enabled = false;
            GetComponent<Animator>().Play("Die");
        }
        WizardMiniGameManager.instance.UpdateHUD();


        PlayHurtAnim();
        Debug.Log("[😵 HIT] Player took damage at " + hitPoint);

        if (Audio_Manager.Instance != null)
        Audio_Manager.Instance.Play2D("Hurt");

        //GetComponent<MNGChayTruongController>().DropCoins(hitPoint);

        StartInvisibly();
    }

    void StartInvisibly()
    {
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
            SetVisible(visible); // Gửi cho toàn bộ client

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        SetVisible(true); // Đảm bảo hiện lại
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

    void PlayHurtAnim()
    {
        animator.SetTrigger("isHurt");
    }

    void SetVisible(bool visible)
    {
        SetAllMeshesVisible(visible);
    }
}
