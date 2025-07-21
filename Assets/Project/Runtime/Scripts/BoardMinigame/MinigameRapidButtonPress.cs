using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinigameRapidButtonPress : MonoBehaviour, IMinigame
{
    private BoardCar car;
    private PlayerInput input;
    public Transform playerTransform;
    public Vector3 startPosition;
    public Transform targetPosition;

    public float moveSpeed = 2.5f;        // tốc độ tiến
    public float fallbackSpeed = 0.2f;  // tốc độ lùi khi không bấm
    public float inputDecayTime = 0.3f; // sau bao lâu không bấm thì lùi

    private float progress = 0f; // 0: start, 1: target
    private float lastPressTime;
    private bool isRunning = false;

    public event Action OnMinigameFinished;
    public bool IsFinished { get; set; }

    public float bounceAmplitude = 0.5f; // độ cao dao động
    public float bounceFrequency = 5f;   // tần số dao động

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
        CinecameraManager.instance.TriggerCamera(GetComponentInChildren<CinemachineCamera>());
    }

    public void EndMinigame()
    {
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
        Vector3 flatPosition = Vector3.Lerp(startPosition, targetPosition.position, progress);

        // Tính noise sóng sine
        float bounceOffset = Mathf.Sin(Time.time * bounceFrequency) * bounceAmplitude;

        // Thêm vào trục Y
        playerTransform.position = flatPosition + new Vector3(0, bounceOffset, 0);

        // Check xong
        if (progress >= 1f)
        {
            IsFinished = true;
            EndMinigame();
            OnMinigameFinished?.Invoke();
            CinecameraManager.instance.ResetCamera();
        }
    }

}
