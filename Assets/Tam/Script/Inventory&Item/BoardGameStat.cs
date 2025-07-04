using System.Collections.Generic;
using System.Diagnostics;

public class BoardGameStat
{
    List<BoardItem> items = new();

    private BoardItem selectedItem;

    public int keyQty;
    public int cupQty;
    public int health;


    //Hàm này sẽ được gọi từ UI_Inventory (Khi click chuột vào image item)
    public void SetSelectedItem(BoardItem item)
    {
        selectedItem = item;
    }

    //Hàm này được gọi từ ô rương thường hoặc reward minigame
    public void AddItem(BoardItem item)
    {
        items.Add(item);
        SetSelectedItem(item);
    }

    public BoardItem GetSelectedItem() => selectedItem;

    //Hàm này được gọi từ UI_Inventory để cập nhật UI
    public List<BoardItem> GetItemList()
    {
        return items;
    }
}
