using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PopUpAnimationController : MonoBehaviour
{
    public PopUpAppearUI appear;
    public PopUpDisappearUI disappear;

    private bool isShowing = false;

    // Coroutine để show panel mà không bị lộ UI
    public IEnumerator ShowRoutine() // Coroutine để show panel
    {
        if (isShowing) yield break; // Nếu đã đang hiện thì không làm gì
        isShowing = true; // Nếu chưa hiện thì đánh dấu là đang hiện

        // Kích hoạt panel
        gameObject.SetActive(true);

        if (appear != null) // Nếu có appear script, sử dụng nó để show
        {
            // Ẩn tất cả UI element trước khi start appear
            appear.HideAllItemsInstant(); 
            yield return null; // đảm bảo hide xong và layout ổn
            yield return appear.PlaySequence(); // Chờ cho animation hoàn thành
        }
        else
        {
            // Nếu không có appear script, show toàn bộ children
            SetChildrenActive(true);
        }
    }

    // Coroutine để hide panel
    public IEnumerator HideRoutine()
    {
        if (!isShowing) yield break; // Nếu không đang hiện thì không làm gì
        isShowing = false; // Đánh dấu là không còn hiện nữa

        if (disappear != null)
            yield return disappear.PlaySequence();

        gameObject.SetActive(false); // Ẩn panel sau khi hoàn thành animation
    }

    // Ẩn ngay lập tức khi Awake
    public void InstantHide()
    {
        isShowing = false;
        gameObject.SetActive(false);
    }

    private void SetChildrenActive(bool active) // Helper method để bật/tắt tất cả children
    {
        foreach (Transform child in transform) // Duyệt qua tất cả children
            child.gameObject.SetActive(active); // Bật/tắt chúng
    }
}
