using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpUIManager : MonoBehaviour
{
    public static PopUpUIManager Instance { get; private set; }

    [Header("Assign all PopUpAnimationControllers here")]
    public List<PopUpAnimationController> panels;

    [Tooltip("Panel mặc định được hiển thị lúc bắt đầu (để trống nếu không muốn auto-show)")]
    public PopUpAnimationController defaultPanel;

    private PopUpAnimationController currentPanel;
    private IEnumerator currentRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Ẩn toàn bộ panel ngay khi load
        foreach (var panel in panels)
            panel.InstantHide();
    }

    void Start()
    {
        // Nếu có defaultPanel, show nó (có animation)
        if (defaultPanel != null)
        {
            SwitchTo(defaultPanel);
        }
    }

    // Gọi từ Button Next/Back: kéo đúng PopUpAnimationController vào
    public void SwitchTo(PopUpAnimationController nextPanel)
    {
        if (nextPanel == null || nextPanel == currentPanel)
        {
            Debug.Log("❗ Panel NULL hoặc đã đang hiện");
            return;
        }

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = SwitchRoutine(nextPanel);
        StartCoroutine(currentRoutine);
    }

    private IEnumerator SwitchRoutine(PopUpAnimationController nextPanel)
    {
        if (currentPanel != null)
            yield return currentPanel.HideRoutine();

        currentPanel = nextPanel;
        yield return currentPanel.ShowRoutine();
    }

    public void HideCurrentPanel()
    {
        if (currentPanel != null)
        {
            StartCoroutine(currentPanel.HideRoutine());
            currentPanel = null;
        }
    }

    //public static PopUpUIManager Instance { get; private set; }

    //[Header("Assign all PopUpAnimationControllers here")]
    //public List<PopUpAnimationController> panels;

    //[Tooltip("Panel mặc định được hiển thị lúc bắt đầu (để trống nếu không muốn auto-show)")]
    //public PopUpAnimationController defaultPanel;

    //private PopUpAnimationController currentPanel;
    //private IEnumerator currentRoutine;

    //void Awake()
    //{
    //    if (Instance != null && Instance != this)
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }
    //    Instance = this;

    //    // Ẩn toàn bộ panel ngay khi load
    //    foreach (var panel in panels)
    //        panel.InstantHide();
    //}

    //void Start()
    //{
    //    // Nếu có defaultPanel, show nó (có animation)
    //    if (defaultPanel != null)
    //    {
    //        SwitchTo(defaultPanel);
    //    }
    //}

    //// Gọi từ Button Next/Back: kéo đúng PopUpAnimationController vào
    //public void SwitchTo(PopUpAnimationController nextPanel)
    //{
    //    if (nextPanel == null || nextPanel == currentPanel)
    //    {
    //        Debug.Log("❗ Panel NULL hoặc đã đang hiện");
    //        return;
    //    }

    //    if (currentRoutine != null)
    //        StopCoroutine(currentRoutine);

    //    currentRoutine = SwitchRoutine(nextPanel);
    //    StartCoroutine(currentRoutine);
    //}

    //private IEnumerator SwitchRoutine(PopUpAnimationController nextPanel)
    //{
    //    if (currentPanel != null)
    //        yield return currentPanel.HideRoutine();

    //    currentPanel = nextPanel;
    //    yield return currentPanel.ShowRoutine();
    //}

    //public void HideCurrentPanel()
    //{
    //    if (currentPanel != null)
    //    {
    //        StartCoroutine(currentPanel.HideRoutine());
    //        currentPanel = null;
    //    }
    //}
}
