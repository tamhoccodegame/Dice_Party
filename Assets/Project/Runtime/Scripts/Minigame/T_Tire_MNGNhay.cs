using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class T_Tire_MNGNhay : MonoBehaviour
{
    public float rollSpeed;
    private Rigidbody rb;
    private int direction;
    private Action onDestroy;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Init(int direction, Action OnDestroy)
    {
        this.direction = direction;
        this.onDestroy = OnDestroy;
        Invoke(nameof(DestroySelf), 6f);
    }

    void DestroySelf()
    {
        onDestroy?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, -(rollSpeed * direction) * Time.deltaTime);
        rb.velocity = Vector3.right * direction * rollSpeed / 10;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerController>(out var controller))
        {
            PlayerInput playerInput = controller.GetPlayerInput();
            int currentLives = WizardPartyData.instance.playerLives[playerInput];
            WizardPartyData.instance.UpdatePlayerLive(playerInput, Mathf.Max(0, currentLives - 1));
            NhayLopManager.instance.UpdateHUD();
        }
    }
}
