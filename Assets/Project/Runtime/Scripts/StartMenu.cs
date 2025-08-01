using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class StartMenu : MonoBehaviour
{
    public InputSystemUIInputModule uiInputModule;
    public GameObject firstSelectedObject;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        PlayerManager.instance.AddPlayer(playerInput);
        playerInput.uiInputModule = uiInputModule;
        playerInput.SwitchCurrentActionMap("UI");
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
