using UnityEngine;
using UnityEngine.InputSystem;

public class MNGChayTruongController : PlayerController
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

    public void Awake()
    {
       

        controller = GetComponent<CharacterController>();
        controller.enabled = true;
        animator = GetComponent<Animator>();
        cam = Camera.main;
    }

    private void Start()
    {
    }

    public override void SetInput(PlayerInput playerInput)
    {
        this.playerInput = playerInput;
    }
    void Update()
    {
        if (playerInput == null) return;
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

        WizardMiniGameManager.instance.UpdatePlayerCompletedGame(playerInput);
    }

    public override PlayerInput GetPlayerInput()
    {
        return playerInput;
    }
}
