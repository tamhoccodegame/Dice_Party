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

    public GameObject currentSelectedButton;

    private void Awake()
    {
        OpenMainPanel();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(currentSelectedButton);   
        }
        else
        {
            currentSelectedButton = eventSystem.currentSelectedGameObject;
        }
    }

    public void OpenCustomizePanel()
    {
        mainPanel.HideInstant();
        readyPanel.HideInstant();
        customizePanel.ToggleVisibility(true);

        StartCoroutine(DelaySetSelectedButton(defaultCustomizeButton));
    }

    public void OpenMainPanel()
    {
        mainPanel.ToggleVisibility(true);
        readyPanel.HideInstant();
        customizePanel.HideInstant();

        StartCoroutine(DelaySetSelectedButton(defaultMainButton));
    }

    IEnumerator DelaySetSelectedButton(GameObject selectedObject)
    {
        eventSystem.SetSelectedGameObject(null);
        yield return null;
        eventSystem.SetSelectedGameObject(selectedObject);
        currentSelectedButton = selectedObject;
    }

    public void OpenReadyPanel()
    {
        mainPanel.HideInstant();
        readyPanel.ToggleVisibility(true);
        customizePanel.HideInstant();

        StartCoroutine(DelaySetSelectedButton(defaultReadyButton));

    }
}
