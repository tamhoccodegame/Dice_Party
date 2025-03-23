using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Import DOTween
using UnityEngine.EventSystems;
using TMPro;


public class SliderEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform sliderRect;  // Background của Slider
    [SerializeField] private RectTransform handleRect;  // Handle của Slider
    [SerializeField] private Image handleImage;         // Hình ảnh của Handle
    [SerializeField] private Slider slider;             // Slider Component

    private float normalSliderHeight = 40f;   // Chiều cao mặc định của Slider
    private float expandedSliderHeight = 80f; // Chiều cao mở rộng của Slider

    private float normalHandleScale = 1f;     // Kích thước mặc định của Handle
    private float expandedHandleScale = 1.5f; // Kích thước mở rộng của Handle

    private bool isDragging = false;

    [SerializeField] private TextMeshProUGUI valueText; // Text hiển thị giá trị

    private void Start()
    {
        // Ẩn Handle ban đầu
        handleImage.color = new Color(1, 1, 1, 0);
        handleRect.gameObject.SetActive(false);

        // Thêm EventTrigger bằng code
        AddEventTrigger(handleRect.gameObject, EventTriggerType.PointerDown, OnDragStart);
        AddEventTrigger(handleRect.gameObject, EventTriggerType.PointerUp, OnDragEnd);
        AddEventTrigger(handleRect.gameObject, EventTriggerType.Drag, OnDragging);

        // Lắng nghe sự kiện thay đổi giá trị của Slider
        slider.onValueChanged.AddListener(OnValueChanged);


        slider.onValueChanged.AddListener(UpdateValueText);
        UpdateValueText(slider.value); // Cập nhật giá trị ban đầu
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Hiện Handle khi hover
        handleRect.gameObject.SetActive(true);
        LeanTween.alpha(handleImage.rectTransform, 1f, 0.3f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isDragging)
        {
            // Ẩn Handle nếu không kéo nữa
            LeanTween.alpha(handleImage.rectTransform, 0f, 0.3f).setOnComplete(() =>
            {
                handleRect.gameObject.SetActive(false);
            });
        }
    }

    private void OnDragStart(BaseEventData eventData)
    {
        isDragging = true;

        // Mở rộng Background của Slider
        LeanTween.size(sliderRect, new Vector2(sliderRect.sizeDelta.x, expandedSliderHeight), 0.3f)
                 .setEaseOutQuad();

        // Scale Handle đồng bộ cả chiều ngang và dọc
        LeanTween.scale(handleRect, Vector3.one * expandedHandleScale, 0.2f).setEaseOutQuad();
    }

    private void OnDragging(BaseEventData eventData)
    {
        if (isDragging)
        {
            // Giữ hiệu ứng Scale khi kéo qua lại
            LeanTween.scale(handleRect, Vector3.one * expandedHandleScale, 0.1f).setEaseOutQuad();
        }
    }

    private void OnDragEnd(BaseEventData eventData)
    {
        isDragging = false;

        // Thu nhỏ Background của Slider
        LeanTween.size(sliderRect, new Vector2(sliderRect.sizeDelta.x, normalSliderHeight), 0.3f)
                 .setEaseInQuad();

        // Thu nhỏ Handle về kích thước ban đầu
        LeanTween.scale(handleRect, Vector3.one * normalHandleScale, 0.2f).setEaseInQuad();

        // Kiểm tra nếu chuột ra ngoài thì ẩn Handle
        if (!RectTransformUtility.RectangleContainsScreenPoint(sliderRect, Input.mousePosition))
        {
            LeanTween.alpha(handleImage.rectTransform, 0f, 0.3f).setOnComplete(() =>
            {
                handleRect.gameObject.SetActive(false);
            });
        }
    }

    private void OnValueChanged(float value)
    {
        if (isDragging)
        {
            // Giữ hiệu ứng Scale khi thay đổi giá trị
            LeanTween.scale(handleRect, Vector3.one * expandedHandleScale, 0.1f).setEaseOutQuad();
        }
    }

    // Hàm tạo EventTrigger bằng code
    private void AddEventTrigger(GameObject obj, EventTriggerType type, System.Action<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>() ?? obj.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener((data) => action(data));
        trigger.triggers.Add(entry);
    }

    private void UpdateValueText(float value)
    {
        valueText.text = value.ToString("0.0"); // Hiển thị 1 chữ số thập phân
    }
}
