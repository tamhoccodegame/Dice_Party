using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    private BoardGameController playerController;
    private int index;
    
    public void Setup(BoardGameController _playerController, int _index)
    {
        playerController = _playerController;
        index = _index;
    }

    private void OnMouseDown()
    {
        playerController.ChooseDirection(index);
    }
}
