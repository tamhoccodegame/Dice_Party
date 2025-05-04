using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    private PlayerController playerController;
    private int index;
    
    public void Setup(PlayerController _playerController, int _index)
    {
        playerController = _playerController;
        index = _index;
    }

    private void OnMouseDown()
    {
        playerController.ChooseDirection(index);
    }
}
