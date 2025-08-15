using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpUIManager : MonoBehaviour
{
    public static PopUpUIManager Instance { get; private set; }

    [Header("Assign all PopUpAnimationControllers here")]
    public List<PopUpAnimationController> panels; // Danh sách các panel có thể hiện

    [Tooltip("Panel mặc định được hiển thị lúc bắt đầu (để trống nếu không muốn auto-show)")]
    public PopUpAnimationController defaultPanel; // Panel mặc định sẽ được hiển thị khi bắt đầu game

    private PopUpAnimationController currentPanel; // Panel hiện tại đang hiển thị
    private IEnumerator currentRoutine; // Coroutine hiện tại đang chạy để tránh xung đột

    void Awake()
    {
        if (Instance != null && Instance != this) // Nếu đã có instance khác, hủy đối tượng này
        {
            Destroy(gameObject);
            return;
        }
        Instance = this; // Gán instance hiện tại

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
    public void SwitchTo(PopUpAnimationController nextPanel) // Hàm để chuyển đổi giữa các panel
    {
        if (nextPanel == null || nextPanel == currentPanel) // Nếu panel NULL hoặc đã đang hiện thì không làm gì
        {
            Debug.Log("❗ Panel NULL hoặc đã đang hiện");
            return;
        }

        if (currentRoutine != null) // Nếu có coroutine đang chạy, dừng nó
            StopCoroutine(currentRoutine); // Dừng coroutine hiện tại để tránh xung đột

        currentRoutine = SwitchRoutine(nextPanel); // Tạo coroutine mới để chuyển đổi panel
        StartCoroutine(currentRoutine); // Bắt đầu coroutine chuyển đổi panel
    }

    private IEnumerator SwitchRoutine(PopUpAnimationController nextPanel) // Coroutine để chuyển đổi giữa các panel
    {
        if (currentPanel != null)// Nếu có panel hiện tại, gọi HideRoutine
            yield return currentPanel.HideRoutine(); 

        currentPanel = nextPanel; // Gán panel mới là panel hiện tại
        yield return currentPanel.ShowRoutine(); // Gọi ShowRoutine để hiện panel mới
    }

    public void HideCurrentPanel()
    {
        if (currentPanel != null)
        {
            StartCoroutine(currentPanel.HideRoutine());
            currentPanel = null;
        }
    }
}
