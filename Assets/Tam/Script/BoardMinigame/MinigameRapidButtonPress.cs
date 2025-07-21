using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinigameRapidButtonPress : MonoBehaviour, IMinigame
{
    private BoardCar car;
    private PlayerInput input;
    public Transform playerTransform;
    public Vector3 startPosition;
    public Transform targetPosition;

    public float moveSpeed = 1.2f;        // tốc độ tiến
    public float fallbackSpeed = 0.2f;  // tốc độ lùi khi không bấm
    public float inputDecayTime = 0.3f; // sau bao lâu không bấm thì lùi

    private float progress = 0f; // 0: start, 1: target
    private float lastPressTime;
    private bool isRunning = false;

    public event Action OnMinigameFinished;
    public bool IsFinished { get; set; }

    public void Init(BoardCar player)
    {
        playerTransform = player.transform;
        input = player.GetInput();
        car = player;
        car.SetCurrentNode(GetComponentInParent<BoardNode>());
        car.StopAllCoroutines();
        startPosition = playerTransform.position;
        progress = 0f;
        lastPressTime = Time.time;
        IsFinished = false;
    }

    public void StartMinigame()
    {
        isRunning = true;
    }

    public void EndMinigame()
    {
        isRunning = false;
        // Ẩn UI, gọi animation thành công, v.v.
        car.SetCurrentNode(targetPosition.GetComponent<BoardNode>());
        car.TryMove();
    }

    private void Update()
    {
        if (!isRunning || IsFinished) return;

        // Check input
        if (input.actions["Trigger"].triggered)
        {
            progress += Time.deltaTime * moveSpeed;
            lastPressTime = Time.time;
        }
        else
        {
            if (Time.time - lastPressTime > inputDecayTime)
            {
                progress -= Time.deltaTime * fallbackSpeed;
            }
        }

        // Clamp
        progress = Mathf.Clamp01(progress);

        // Move player theo progress
        playerTransform.position = Vector3.Lerp(startPosition, targetPosition.position, progress);

        // Check xong
        if (progress >= 1f)
        {
            IsFinished = true;
            OnMinigameFinished?.Invoke();
            EndMinigame();
        }
    }
}
