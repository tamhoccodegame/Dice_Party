using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening; // Nếu dùng DoTween


public class TabManager : MonoBehaviour
{
    [System.Serializable]
    public class TabInfo
    {
        public Button button; // Nút Tab
        public GameObject panel; // Panel tương ứng
        public TextMeshProUGUI buttonText; // Text của nút để tô đậm
    }

    [SerializeField] private TabInfo[] tabs;
    [SerializeField] private float fadeDuration = 0.3f; // Thời gian hiệu ứng fading
    [SerializeField] private Color normalColor = Color.white; // Màu nút bình thường
    [SerializeField] private Color highlightColor = Color.yellow; // Màu tô đậm khi active

    private int currentTabIndex = 0;

    private void Start()
    {
        // Ẩn tất cả panels trừ panel đầu tiên
        for (int i = 0; i < tabs.Length; i++)
        {
            int index = i; // Lưu lại index để tránh lỗi delegate
            tabs[i].button.onClick.AddListener(() => SwitchTab(index));
            tabs[i].panel.SetActive(i == currentTabIndex);
            tabs[i].buttonText.color = (i == currentTabIndex) ? highlightColor : normalColor;
        }
    }

    private void SwitchTab(int tabIndex)
    {
        if (tabIndex == currentTabIndex) return; // Nếu đã ở tab này thì không làm gì cả

        // Ẩn tab cũ bằng hiệu ứng fading
        GameObject oldPanel = tabs[currentTabIndex].panel;
        GameObject newPanel = tabs[tabIndex].panel;

        // Tắt panel cũ
        oldPanel.GetComponent<CanvasGroup>().DOFade(0, fadeDuration).OnComplete(() =>
        {
            oldPanel.SetActive(false);
        });

        // Hiển thị panel mới
        newPanel.SetActive(true);
        newPanel.GetComponent<CanvasGroup>().alpha = 0;
        newPanel.GetComponent<CanvasGroup>().DOFade(1, fadeDuration);

        // Đổi màu highlight cho nút
        tabs[currentTabIndex].buttonText.color = normalColor;
        tabs[tabIndex].buttonText.color = highlightColor;

        // Cập nhật tab hiện tại
        currentTabIndex = tabIndex;
    }
}
