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
    public IEnumerator ShowRoutine()
    {
        if (isShowing) yield break;
        isShowing = true;

        // Kích hoạt panel
        gameObject.SetActive(true);

        if (appear != null)
        {
            // Ẩn tất cả UI element trước khi start appear
            appear.HideAllItemsInstant();
            yield return null; // đảm bảo hide xong và layout ổn
            yield return appear.PlaySequence();
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
        if (!isShowing) yield break;
        isShowing = false;

        if (disappear != null)
            yield return disappear.PlaySequence();

        gameObject.SetActive(false);
    }

    // Ẩn ngay lập tức khi Awake
    public void InstantHide()
    {
        isShowing = false;
        gameObject.SetActive(false);
    }

    private void SetChildrenActive(bool active)
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(active);
    }

    //public PopUpAppearUI appear;
    //public PopUpDisappearUI disappear;

    //private bool isShowing = false;

    //// Coroutine để show panel
    //public IEnumerator ShowRoutine()
    //{
    //    if (isShowing) yield break;
    //    isShowing = true;

    //    // Ẩn toàn bộ children trước 1 frame để tránh lộ UI
    //    SetChildrenActive(false);
    //    gameObject.SetActive(true);
    //    yield return null;
    //    SetChildrenActive(true);

    //    if (appear != null)
    //        yield return appear.PlaySequence();
    //}

    //// Coroutine để hide panel
    //public IEnumerator HideRoutine()
    //{
    //    if (!isShowing) yield break;
    //    isShowing = false;

    //    if (disappear != null)
    //        yield return disappear.PlaySequence();

    //    gameObject.SetActive(false);
    //}

    //// Ẩn ngay lập tức khi Awake
    //public void InstantHide()
    //{
    //    isShowing = false;
    //    gameObject.SetActive(false);
    //}

    //private void SetChildrenActive(bool active)
    //{
    //    foreach (Transform child in transform)
    //        child.gameObject.SetActive(active);
    //}
}
