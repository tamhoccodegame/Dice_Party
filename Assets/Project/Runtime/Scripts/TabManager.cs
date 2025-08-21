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
        //public Button button; // Nút Tab
        //public GameObject panel; // Panel tương ứng
        //public TextMeshProUGUI buttonText; // Text của nút để tô đậm

        public Button button; // Nút Tab
        public GameObject panel; // Panel tương ứng
        public TextMeshProUGUI buttonText; // Text của nút để tô đậm
        public Color normalColor = Color.white; // Màu bình thường riêng
        public Color highlightColor = Color.yellow; // Màu khi được chọn
    }

    [SerializeField] private TabInfo[] tabs; // Mảng chứa thông tin các tab
    [SerializeField] private float fadeDuration = 0.3f; // Thời gian hiệu ứng fading

    private int currentTabIndex = 0;

    private void Start()
    {

        for (int i = 0; i < tabs.Length; i++) // Duyệt qua tất cả các tab
        {
            int index = i; // Lưu index 
            tabs[i].button.onClick.AddListener(() => SwitchTab(index)); // Gán sự kiện click cho nút tab
            tabs[i].panel.SetActive(i == currentTabIndex); // Chỉ kích hoạt panel của tab đầu tiên
            tabs[i].buttonText.color = (i == currentTabIndex) ? tabs[i].highlightColor : tabs[i].normalColor; // Đặt màu cho nút tab đầu tiên là highlight, các tab khác là normal
        }
    }

    private void SwitchTab(int tabIndex) // Hàm để chuyển đổi giữa các tab
    {
        if (tabIndex == currentTabIndex) return; // Nếu tab đã được chọn thì không làm gì

        GameObject oldPanel = tabs[currentTabIndex].panel; 
        GameObject newPanel = tabs[tabIndex].panel;

        oldPanel.GetComponent<CanvasGroup>().DOFade(0, fadeDuration).OnComplete(() => // Khi hiệu ứng fade out hoàn tất
        {
            oldPanel.SetActive(false); // Ẩn panel cũ
        });

        newPanel.SetActive(true); // Kích hoạt panel mới
        newPanel.GetComponent<CanvasGroup>().alpha = 0; // Đặt alpha về 0 để bắt đầu fade in
        newPanel.GetComponent<CanvasGroup>().DOFade(1, fadeDuration); // Bắt đầu hiệu ứng fade in

        // Cập nhật màu cho nút cũ và mới theo màu riêng
        tabs[currentTabIndex].buttonText.color = tabs[currentTabIndex].normalColor; // Đặt lại màu nút cũ về bình thường
        tabs[tabIndex].buttonText.color = tabs[tabIndex].highlightColor; // Đặt màu nút mới thành màu highlight

        currentTabIndex = tabIndex; // Cập nhật chỉ số tab hiện tại
    }
}
