using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class OptionSelector : MonoBehaviour
{
    public enum Type { Color, Text }

    public enum CustomType { Color, Hair, Bodypart }

    [Header("General")]
    public Type selectorType;
    public CustomType customType;
    public Button leftButton;
    public Button rightButton;

    [Header("UI References")]
    public Image colorDisplay;         // Dùng cho Color
    public TMP_Text textDisplay;       // Dùng cho Text

    [Header("Values")]
    public List<Color> colorOptions;
    public List<string> textOptions;

    private int currentIndex = 0;

    public bool isActive;
    bool cacheActiveState = false;
    public MultiplayerEventSystem eventSystem;

    private Vector2 navigateValue;

    public PlayerCustom playerCustom;

    private float inputCooldown = 0.25f; // 250ms delay giữa mỗi lần chọn
    private float inputTimer = 0f;


    private void Start()
    {
        leftButton.onClick.AddListener(SelectPrevious);
        rightButton.onClick.AddListener(SelectNext);

        UpdateDisplay();
    }

    public void Init(PlayerCustom playerCustom, List<string> _textOptions = null)
    {
        textOptions.Clear();

        textOptions = _textOptions;

        this.playerCustom = playerCustom;
    }

    private void Update()
    {
        isActive = eventSystem.currentSelectedGameObject == gameObject;

        InputSystemUIInputModule inputModule = eventSystem.currentInputModule as InputSystemUIInputModule;
        if (inputModule != null)
        {
            InputActionReference navigateAction = inputModule.move;
            // Nếu muốn đọc raw value:
            navigateValue = navigateAction.action.ReadValue<Vector2>();
        }

        if (!isActive) return;

        inputTimer -= Time.unscaledDeltaTime;

        if (inputTimer <= 0f)
        {
            if (navigateValue.x < -0.5f)
            {
                SelectPrevious();
                inputTimer = inputCooldown;
            }
            else if (navigateValue.x > 0.5f)
            {
                SelectNext();
                inputTimer = inputCooldown;
            }
        }
    }

    private void SelectPrevious()
    {
        currentIndex = (currentIndex - 1 + GetListCount()) % GetListCount();
        UpdateDisplay();
        SaveCurrentIndex();

        switch(customType)
        {
            case CustomType.Color:
                playerCustom.PrevColor();
                break;
            case CustomType.Hair:
                playerCustom.PrevHair();
                break;
            case CustomType.Bodypart:
                playerCustom.PrevBodypart();
                break;
        }
    }

    private void SelectNext()
    {
        currentIndex = (currentIndex + 1) % GetListCount();
        UpdateDisplay();
        SaveCurrentIndex();

        switch (customType)
        {
            case CustomType.Color:
                playerCustom.NextColor();
                break;
            case CustomType.Hair:
                playerCustom.NextHair();
                break;
            case CustomType.Bodypart:
                playerCustom.NextBodypart();
                break;
        }
    }

    private int GetListCount()
    {
        return selectorType == Type.Color ? colorOptions.Count : textOptions.Count;
    }

    private void UpdateDisplay()
    {
        if (selectorType == Type.Color && colorDisplay != null)
        {
            colorDisplay.color = colorOptions[currentIndex];
        }
        else if (selectorType == Type.Text && textDisplay != null)
        {
            textDisplay.text = textOptions[currentIndex];
        }
    }

    private void SaveCurrentIndex()
    {
        string key = gameObject.name + "_SelectedIndex";
        PlayerPrefs.SetInt(key, currentIndex);
    }

    private void LoadSavedIndex()
    {
        string key = gameObject.name + "_SelectedIndex";
        currentIndex = PlayerPrefs.GetInt(key, 0);
    }

    public int GetSelectedIndex() => currentIndex;

    public Color GetSelectedColor() => colorOptions[currentIndex];

    public string GetSelectedText() => textOptions[currentIndex];
}
