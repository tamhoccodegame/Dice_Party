using System.Collections.Specialized;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class WinController : PlayerController
{
    public PlayerInput playerInput;
    public Animator animator;

    public Vector2 movementInput;

    public TextMeshPro awardText;

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

        movementInput = playerInput.actions["Move"].ReadValue<Vector2>();

        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0;

        Vector3 movement = movementInput.x * camRight + movementInput.y * camForward;

        movement.y = -5f;

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
