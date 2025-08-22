using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class WinController : PlayerController
{
    public PlayerInput playerInput;
    public Animator animator;

    public Vector2 movementInput;

    public TextMeshPro awardText;

    public bool isDancing = false;

    public override PlayerInput GetPlayerInput()
    {
        return playerInput;
    }

    public override void SetInput(PlayerInput input)
    {
        playerInput = input;
    }

    public string currentAnim;

    private CharacterController _cc;
    public float moveSpeed;

    private void Start()
    {
        _cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main == null) return;

        if (playerInput.actions["Interact"].triggered && playerInput != WizardPartyData.instance.winner)
        {
            if (!isDancing)
            {
                ChangeAnimation($"Win{Random.Range(1, 7)}");
            }
            else
            {
                isDancing = false;
            }
        }

        movementInput = playerInput.actions["Move"].ReadValue<Vector2>();

        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0;

        Vector3 movement = movementInput.x * camRight + movementInput.y * camForward;

        movement.y = -5f;

        if (isDancing) return;

        _cc.Move(movement * moveSpeed * Time.deltaTime);

        if (movementInput.magnitude > 0)
        {
            Vector3 lookDir = movement;
            lookDir.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), 20 * Time.deltaTime);
            ChangeAnimation("Run");
        }
        else
        {
            ChangeAnimation("Idle");
        }
    }

    public void ChangeAnimation(string animation)
    {
        if (animation == currentAnim) return;
        currentAnim = animation;
        animator.CrossFade(currentAnim, 0.25f);
    }

    public void SetAwardText(MatchAwardSystem.MatchTitle matchTitle)
    {
        awardText.text = matchTitle.ToString();
    }
}
