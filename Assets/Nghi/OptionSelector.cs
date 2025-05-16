using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionSelector : MonoBehaviour
{
    public enum Type { Color, Text }

    [Header("General")]
    public Type selectorType;
    public Button leftButton;
    public Button rightButton;

    [Header("UI References")]
    public Image colorDisplay;         // Dùng cho Color
    public TMP_Text textDisplay;       // Dùng cho Text

    [Header("Values")]
    public List<Color> colorOptions;
    public List<string> textOptions;

    private int currentIndex = 0;

    private void Start()
    {
        leftButton.onClick.AddListener(SelectPrevious);
        rightButton.onClick.AddListener(SelectNext);

        LoadSavedIndex();
        UpdateDisplay();
    }

    private void SelectPrevious()
    {
        currentIndex = (currentIndex - 1 + GetListCount()) % GetListCount();
        UpdateDisplay();
        SaveCurrentIndex();
    }

    private void SelectNext()
    {
        currentIndex = (currentIndex + 1) % GetListCount();
        UpdateDisplay();
        SaveCurrentIndex();
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
