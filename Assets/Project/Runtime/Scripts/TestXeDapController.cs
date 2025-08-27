using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TestXeDapController : MNGPlayerController
{
    public float acceleration = 3f;    // tăng tốc mỗi lần nhấn
    public float maxSpeed = 8f;        // tốc độ tối đa
    public float deceleration = 2f;    // giảm tốc khi buông
    public float turnSpeed = 100f;     // tốc độ rẽ

    private float currentSpeed = 0f;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        GetComponentInChildren<Animator>().Play("Sit");
    }

    protected override void Update()
    {
        // Nhấn liên tục phím tiến => tăng tốc
        if (playerInput.actions["Trigger"].triggered)
        {
            currentSpeed += acceleration;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
        }

        // Nếu không nhấn thì giảm dần (trượt)
        if (!playerInput.actions["Trigger"].IsPressed())
        {
            currentSpeed -= deceleration * Time.deltaTime;
            currentSpeed = Mathf.Max(currentSpeed, 0f);
        }

        // Xoay qua trái/phải
        float turn = playerInput.actions["Move"].ReadValue<Vector2>().x;

        transform.Rotate(Vector3.up * turn * turnSpeed * Time.deltaTime);

        // Move theo hướng forward
        Vector3 move = transform.forward * currentSpeed * Time.deltaTime;
        controller.Move(move);

        if(move.magnitude > 0.1f)
        {
            //ChangeAnim("");
        }
        else
        {
            //ChangeAnim("");
        }
    }
}
