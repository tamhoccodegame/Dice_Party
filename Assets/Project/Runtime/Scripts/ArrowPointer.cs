using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    private BoardCar playerController;
    private int index;

    public Color hoverColor;
    public Color normalColor;

    private float inputCooldown = 0.25f; // 250ms delay giữa mỗi lần chọn
    private float inputTimer = 0f;

    public void Setup(BoardCar _playerController, int _index)
    {
        
    }

    public void Hover()
    {
        foreach(var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.material.color = hoverColor;
        }
    }

    public void UnHover()
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.material.color = normalColor;
        }
    }
}
