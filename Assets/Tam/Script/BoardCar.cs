using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class BoardCar : MonoBehaviour
{
    private CharacterController controller;

    public BoardNode currentNode;
    public BoardNode toMoveNode;

    public int currentPlayerInputIndex = 0;
    private PlayerInput currentPlayerInput;
    public List<PlayerInput> inputs;

    public int stepLeft = 0;

    [Header("Players Animator")]
    public Animator[] animators;

    [Header("Car Animator")]
    public Animator carAnim;

    // Start is called before the first frame update
    void Start()
    {
        inputs = PlayerManager.instance.players;
        UpdatePlayerInput();
    }

    void UpdatePlayerInput()
    {
        currentPlayerInput = inputs[currentPlayerInputIndex];
        currentPlayerInput.actions["Trigger"].started += OnTrigger;
    }

    private void OnTrigger(InputAction.CallbackContext obj)
    {
        stepLeft = 10;
        Debug.Log(stepLeft);
        currentPlayerInputIndex++;
        UpdatePlayerInput();
    }
}
