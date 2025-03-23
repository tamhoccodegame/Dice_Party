using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SwitchToggle : MonoBehaviour
{
    [SerializeField] private RectTransform handleRectTransform;
    private Toggle toggle;
    private Vector2 handlePostion;

    //COLOR
    [SerializeField] private Color backgroundActiveColor;
    [SerializeField] private Color handleActiveColor;

    private Color backgroundDefaultColor, handleDefaultColor;

    private Image backgroundImage, handleImage;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        //IMAGE
        //Muốn lấy BackgroundImage thì phải tham chiếu tới cha của HandleRectTransform
        //backgroundImage = handleRectTransform.GetComponentInParent<Image>();//Cái này không 
        //hoạt động
        backgroundImage = handleRectTransform.parent.GetComponent<Image>();
        handleImage = handleRectTransform.GetComponent<Image>();
        //Gán màu ban đầu của backgroundImage vào backgroundDefaultColor và tương tự với Handle
        //Tức mới mở lên thì giữ nguyên màu gốc của Image
        backgroundDefaultColor = backgroundImage.color;
        handleDefaultColor = handleImage.color;


        handlePostion = handleRectTransform.anchoredPosition;//(PosX, PosY, PosZ) trong RectTransform
        toggle.onValueChanged.AddListener(OnSwitch);
        //Nếu nhấn nút bật lên thì kích hoạt cho Handle di chuyển
        if (toggle.isOn)
        {
            OnSwitch(true);
        }
    }

    private void OnSwitch(bool on)
    {
        //if (on)
        //{
        //    handleRectTransform.anchoredPosition = handlePostion * -1;//Handle bật về phía đối diện 
        //    //Như 2 số dương và âm trong Vector 
        //}
        //else
        //{
        //    handleRectTransform.anchoredPosition = handlePostion; 
        //}
        //Bật On mà không có Animations và Fading
        //Có on không? Nếu on thì nhảy qua phía đối diện bên kia của Vector, không thì thôi
        //handleRectTransform.anchoredPosition = on?handlePostion*-1:handlePostion;
        //Bật On có Animations 
        handleRectTransform.DOAnchorPos(on ? handlePostion * -1 : handlePostion, 0.4f).SetEase(Ease.InOutBack);
        //Nếu On thì đổi màu. Lấy màu của Image ra, nếu on thì đổi màu mới, không thì giữ màu cũ
        //backgroundImage.color = on? backgroundActiveColor: backgroundDefaultColor;
        backgroundImage.DOColor(on ? backgroundActiveColor : backgroundDefaultColor, 0.6f);
        Debug.Log($"Background Image: {backgroundImage}");
        //handleImage.color = on? handleActiveColor: handleDefaultColor;
        handleImage.DOColor(on ? handleActiveColor : handleDefaultColor, 0.4f);

        Debug.Log($"OnSwitch: {on}, Background Color: {backgroundImage.color}, Handle Color: {handleImage.color}");
        Debug.Log($"Background Active Color: {backgroundActiveColor}, Alpha: {backgroundActiveColor.a}");
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
