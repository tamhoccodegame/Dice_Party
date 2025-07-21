using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class PanelSwitcher : MonoBehaviour
{
    public MultiplayerEventSystem eventSystem;

    public SmoothPanelToggle mainPanel;
    public SmoothPanelToggle customizePanel;
    public SmoothPanelToggle readyPanel;


    public GameObject defaultMainButton;
    public GameObject defaultCustomizeButton;
    public GameObject defaultReadyButton;

    private void Awake()
    {
        OpenMainPanel();
    }

    public void OpenCustomizePanel()
    {
        mainPanel.HideInstant();
        readyPanel.HideInstant();
        customizePanel.ToggleVisibility(true);

        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(defaultCustomizeButton);
    }

    public void OpenMainPanel()
    {
        mainPanel.ToggleVisibility(true);
        readyPanel.HideInstant();
        customizePanel.HideInstant();

        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(defaultMainButton);
    }

    public void OpenReadyPanel()
    {
        mainPanel.HideInstant();
        readyPanel.ToggleVisibility(true);
        customizePanel.HideInstant();

        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(defaultReadyButton);
    }
}
