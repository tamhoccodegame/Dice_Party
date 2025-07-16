using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    private NewBoardGameController playerController;
    private int index;
    
    public void Setup(NewBoardGameController _playerController, int _index)
    {
        playerController = _playerController;
        index = _index;
    }

    private void OnMouseDown()
    {
        playerController.ChooseDirection(index);
    }
}
