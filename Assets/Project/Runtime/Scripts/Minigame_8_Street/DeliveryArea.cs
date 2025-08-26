using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeliveryArea : MonoBehaviour
{
    [Header("Delivery Config")]
    public float deliveryRadius = 2.5f;
    public LayerMask playerLayer;
    public GameObject deliveryVFXPrefab;

    private void Update()
    {
        // Quét xem có player nào trong phạm vi không
        Collider[] hits = Physics.OverlapSphere(transform.position, deliveryRadius, playerLayer);
        foreach (var hit in hits)
        {
            PlayerInteractMoneyController player = hit.GetComponent<PlayerInteractMoneyController>();
            if (player != null)
            {
                TryDeliver(player);
            }
        }
    }

    private void TryDeliver(PlayerInteractMoneyController player)
    {
        if (player == null) return;
        if (!playerHasGift(player)) return;

        int deliveredCount = player.carriedBags.Count; // lấy số bag player đang cầm
        if (deliveredCount <= 0) return;

        player.GetComponent<MNGPlayerController>().speedFactor = 1f;

        // --- Spawn VFX tại vị trí tay ---
        if (deliveryVFXPrefab != null && player.carryPoint != null)
        {
            GameObject vfx = Instantiate(deliveryVFXPrefab, player.carryPoint.position, Quaternion.identity);
            Destroy(vfx, 2f);
        }

        // --- Xoá túi hiển thị ---
        if (player.carriedBagInstance != null)
            GameObject.Destroy(player.carriedBagInstance);

        PlayerInput playerInput = player.GetComponent<MNGPlayerController>().GetPlayerInput();
        WizardMiniGameManager.instance.UpdatePlayerScore(playerInput, player.carriedBags.Count);

        // --- Xoá danh sách bag logic ---
        if (player.carriedBags != null)
            player.carriedBags.Clear();

        // Reset state trong player
        player.carriedBagInstance = null;
        player.isHoldingBag = false;
        player.leftHandIKTarget = null;
        player.rightHandIKTarget = null;
        player.handIKWeight = 0f;

        if (player.carryMode == PlayerInteractMoneyController.CarryMode.Animation)
            player.GetComponent<Animator>().SetLayerWeight(1, 0f);


    }


    private bool playerHasGift(PlayerInteractMoneyController player)
    {
        return player != null && player.isHoldingBag && (player.carriedBagInstance != null || player.carriedBags != null);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, deliveryRadius);
    }
}
