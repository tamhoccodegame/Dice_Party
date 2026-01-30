using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;

[System.Serializable]
public class BoardItemAndSpawnPosition
{
    public GameObject item;
    public Transform spawnPosition;
}

public class ItemController : MonoBehaviour
{
    public NewBoardGameController controller;
    public BoardItem usingItem;

    public BoardItemAndSpawnPosition[] itemAndSpawnPosition;

    private void Update()
    {
        if (usingItem == null) return;
        usingItem.Use();
    }

    public void UseItem(int itemIndex)
    {
        var _item = itemAndSpawnPosition[itemIndex];

        if (_item != null)
        {
            //Spawn Item
            var spawnItem = Instantiate(_item.item, _item.spawnPosition.position, _item.spawnPosition.rotation)
                            .GetComponent<BoardItem>();
            spawnItem.transform.SetParent(transform);
            spawnItem.Init(controller);
            spawnItem.itemStartUse += ItemStartUse;
            spawnItem.itemEndUse += ItemEndUse;
            usingItem = spawnItem.GetComponent<BoardItem>();
        }
        else
        {
            Debug.LogError("Không tìm thấy item");
        }
    }

    void ItemStartUse()
    {

    }
    
    void ItemEndUse()
    {
        controller.ChangeState(controller.idleState);
        Destroy(usingItem.gameObject);
        usingItem = null;
    }
}
