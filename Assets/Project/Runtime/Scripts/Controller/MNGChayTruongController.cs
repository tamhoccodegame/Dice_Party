using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class MNGChayTruongController : MonoBehaviour
{
    public Camera cam;
    private CharacterController controller;
    private Animator animator;

    public string currentAnim;

    public bool isGoal = false;
    public float gravityScale;
    public float jumpForce;
    public Vector3 verticalVelocity;

    private PlayerInput playerInput;
    private Vector2 movementInput;

    private int TotalCoins = 0;
    public int coinsToDropOnHit = 3;
    public float coinSpawnHeight;

    public GameObject coinPrefab;
    public float coinLifetime;
    public GameObject pickupVFX;
    public float spawnForce;

    public void Awake()
    {
        controller = GetComponent<CharacterController>();
        controller.enabled = true;
        animator = GetComponent<Animator>();
        cam = Camera.main;
    }

    public void SetInput(PlayerInput playerInput)
    {
        this.playerInput = playerInput;

        Custom customData = PlayerManager.instance.GetComponentInChildren<CustomData>().GetCustom(playerInput);
        GetComponent<PlayerSetup>().UpdateCustom(customData.hairIndex, customData.colorIndex, customData.bodyPartIndex);
    }

    public PlayerInput GetPlayerInput()
    {
        return playerInput;
    }

    void Update()
    {
        // Lấy input
        movementInput = playerInput.actions["Move"].ReadValue<Vector2>();

        // Tạo hướng di chuyển ngang (X-Z)
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0f;
        Vector3 camRight = cam.transform.right;
        camRight.y = 0f;

        Vector3 horizontal = movementInput.x * camRight + movementInput.y * camForward;

        // Xử lý nhảy
        if (controller.isGrounded)
        {
            verticalVelocity.y = -1f; // giữ nhân vật dính xuống đất

            if (playerInput.actions["Trigger"].triggered)
            {
                verticalVelocity.y = jumpForce; // ví dụ jumpForce = 10
            }
        }
        else
        {
            verticalVelocity.y += gravityScale * Time.deltaTime; // áp dụng trọng lực
        }

        // Gộp chuyển động ngang và dọc
        Vector3 movement = (horizontal * 10f) + verticalVelocity;

        // Di chuyển nhân vật
        controller.Move(movement * Time.deltaTime);

        // Xoay theo hướng di chuyển
        if (horizontal.magnitude > 0.1f)
        {
            Quaternion newRotation = Quaternion.LookRotation(horizontal);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 8 * Time.deltaTime);
            ChangeAnim("Run");
        }
        else
        {
            ChangeAnim("Idle");
        }
    }


    public void ChangeAnim(string animName, float blendTime = 0.25f)
    {
        if (animName == currentAnim) return;
        currentAnim = animName;

        animator.CrossFade(animName, blendTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Goal")
        {
            SetGoal();
        }
    }


    void SetGoal()
    {
        if (isGoal) return;
        isGoal = true;

        T_Coin_Manager.Instance.UpdateGoal(playerInput, gameObject);
    }

    //public void AddCoins(int amount)
    //{
    //    TotalCoins += amount;
    //    WizardPartyData.instance.UpdatePlayerCoin(playerInput, TotalCoins);
    //    T_Coin_Manager.Instance.UpdateHUD();
    //}

    //public void RemoveCoins(int amount)
    //{
    //    int actual = Mathf.Min(TotalCoins, amount);
    //    TotalCoins -= actual;
    //    T_Coin_Manager.Instance.UpdateHUD();
    //    WizardPartyData.instance.UpdatePlayerCoin(playerInput, TotalCoins);
    //}


    //public void DropCoins(Vector3 origin)
    //{
    //    int dropCount = Mathf.Min(TotalCoins, coinsToDropOnHit);
    //    if (dropCount <= 0)
    //    {
    //        Debug.Log("[⚠️ DROP] Not enough coins to drop.");
    //        return;
    //    }

    //    RemoveCoins(dropCount);

    //    for (int i = 0; i < dropCount; i++)
    //    {
    //        // 👉 spawn tại player, thêm chút chiều cao để không dính sàn
    //        Vector3 spawnPos = origin + Vector3.up * coinSpawnHeight;
    //        GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);

    //        T_Coins coinScript = coin.GetComponent<T_Coins>();
    //        if (coinScript != null)
    //        {
    //            coinScript.SetLifetime(coinLifetime);
    //            coinScript.value = 1;
    //            coinScript.pickupVFX = pickupVFX;
    //        }

    //        Rigidbody rb = coin.GetComponent<Rigidbody>();
    //        if (rb != null)
    //        {
    //            // 👉 Văng ra các hướng ngẫu nhiên, có hướng lên nhẹ để nảy
    //            Vector3 randomDir = new Vector3(
    //                Random.Range(-1f, 1f),
    //                Random.Range(0.4f, 1.2f),
    //                Random.Range(-1f, 1f)
    //            ).normalized;

    //            rb.AddForce(randomDir * spawnForce, ForceMode.Impulse);

    //            // 👉 Add torque để coin xoay xoay mượt hơn
    //            Vector3 torque = new Vector3(
    //                Random.Range(-200, 200),
    //                Random.Range(-200, 200),
    //                Random.Range(-200, 200)
    //            );
    //            rb.AddTorque(torque);
    //        }
    //    }
    //}

    //private void OnControllerColliderHit(ControllerColliderHit hit)

    //{
    //    if(hit.gameObject.TryGetComponent<T_Coins>(out var coin))
    //    {
    //        coin.TryPickUp(this);
    //    }
    //}

}
