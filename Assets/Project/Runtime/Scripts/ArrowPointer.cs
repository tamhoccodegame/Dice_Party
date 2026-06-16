using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    public Color hoverColor;
    public Color normalColor;

    //public void Setup(BoardCar _playerController, int _index)
    //{

    //}

    public void Hover()
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
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
