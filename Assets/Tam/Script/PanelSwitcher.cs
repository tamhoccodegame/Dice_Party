using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelSwitcher : MonoBehaviour
{
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

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(defaultCustomizeButton);
    }

    public void OpenMainPanel()
    {
        mainPanel.ToggleVisibility(true);
        readyPanel.HideInstant();
        customizePanel.HideInstant();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(defaultMainButton);
    }

    public void OpenReadyPanel()
    {
        mainPanel.HideInstant();
        readyPanel.ToggleVisibility(true);
        customizePanel.HideInstant();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(defaultReadyButton);
    }
}
