using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MNGChayTruongController : MonoBehaviour
{
    public Camera cam;
    private CharacterController controller;
    private Animator animator;

    public LayerMask coinMask;

    public string currentAnim;

    public bool isGoal = false;

    private PlayerInput playerInput;
    private Vector2 movementInput;

    public void Awake()
    {
        controller = GetComponent<CharacterController>();
        controller.enabled = true;
        animator = GetComponent<Animator>();
        cam = Camera.main;

        if (VongXoayManager.instance != null)
            //VongXoayManager.instance.RequestUpdateLive(Object.InputAuthority, Object.Id);

            Invoke(nameof(ResetGravity), 2f);
    }

    public void SetInput(PlayerInput playerInput)
    {
        this.playerInput = playerInput;
        playerInput.actions["Move"].started += OnMove;
        playerInput.actions["Move"].canceled += OnMove;
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }

    void ResetGravity()
    {

    }

    void Update()
    {

    }

    public void ChangeAnim(string animName, float blendTime = 0.25f)
    {
        if (animName == currentAnim) return;
        currentAnim = animName;

        animator.CrossFade(animName, blendTime);
    }
    public void Die()
    {
        if (VongXoayManager.instance.isGameOver) return;

        ChangeAnim("Die");
        DisableInput();
    }

    void DisableInput()
    {
        Destroy(this);
    }

    void EnableRagdoll()
    {
        GetComponent<Ragdoll>().EnableRagdoll();
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

        //Coin_Manager.Instance.UpdateGameState();
    }
}
