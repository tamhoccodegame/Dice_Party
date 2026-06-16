using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBlinking : MonoBehaviour
{
    [Header("Animation & Invincibility")]
    public Animator animator;
    public float invincibleDuration = 2f; // Thời gian bất tử sau khi bị va chạm
    public float blinkInterval = 0.15f; // Thời gian giữa các lần nhấp nháy

    private bool isInvincible { get; set; } // Biến để kiểm tra trạng thái bất tử
    private bool isBlinking { get; set; } // Biến để kiểm tra trạng thái nhấp nháy

    private List<SkinnedMeshRenderer> skinnedMeshes = new List<SkinnedMeshRenderer>(); // Danh sách các SkinnedMeshRenderer để điều khiển hiển thị

    private void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>(); // Lấy Animator từ con nếu không có trên chính đối tượng

        skinnedMeshes.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>()); // Lấy tất cả SkinnedMeshRenderer từ con để điều khiển hiển thị
    }

    public void OnHitByObstacle(Vector3 hitPoint) // Hàm này sẽ được gọi khi va chạm với vật cản
    {
        if (isInvincible) // Nếu đang bất tử thì không làm gì
        {
            Debug.Log("[⚡ IMMUNE] Player is invincible");
            return;
        }

        PlayerInput playerInput = GetComponent<PlayerController>().GetPlayerInput();
        WizardMiniGameManager.instance.UpdatePlayerScore(gameObject, -20);


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

    private void SetAllMeshesVisible(bool visible) // Hàm này sẽ bật/tắt hiển thị của tất cả các SkinnedMeshRenderer
    {
        foreach (var mesh in skinnedMeshes) // Duyệt qua tất cả các SkinnedMeshRenderer
        {
            if (mesh != null)
                mesh.enabled = visible; // Bật/tắt hiển thị của từng SkinnedMeshRenderer
        }
    }

    public bool IsInvincible() => isInvincible;

    void PlayHurtAnim()
    {
        animator.SetTrigger("isHurt");
    }

    void SetVisible(bool visible) // Hàm này sẽ được gọi để bật/tắt hiển thị của tất cả các SkinnedMeshRenderer
    {
        SetAllMeshesVisible(visible);
    }
}
