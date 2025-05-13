
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnimatedGradientButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RawImage targetImage;
    [SerializeField] private float animateSpeed = 0.3f;     // Tốc độ khi hover
    [SerializeField] private float idleOffset = 0.0f;       // Offset khi idle
    [SerializeField] private float returnDurationPerUnit = 0.5f; // Thời gian quay về mỗi đơn vị offset

    private Material matInstance;
    private float offset = 0f;
    private bool isHovering = false;

    private Tween returnTween;

    private void Awake()
    {
        if (targetImage == null) targetImage = GetComponent<RawImage>();
        matInstance = Instantiate(targetImage.material);
        targetImage.material = matInstance;
    }

    private void Update()
    {
        if (isHovering)
        {
            offset += Time.deltaTime * animateSpeed;
            matInstance.SetFloat("_Offset", offset);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;

        // Hủy tween đang quay lại (nếu có)
        returnTween?.Kill();
        returnTween = null;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        float distance = Mathf.Abs(offset - idleOffset);
        float duration = distance * returnDurationPerUnit;

        // Quay về từ từ
        returnTween = DOTween.To(() => offset, x => {
            offset = x;
            matInstance.SetFloat("_Offset", offset);
        }, idleOffset, duration).SetEase(Ease.OutCubic);
    }
    //[SerializeField] private RawImage targetImage;
    //[SerializeField] private float animateSpeed = 0.3f;
    //[SerializeField] private float idleOffset = 0.0f;

    //private Material matInstance;
    //private float offset = 0f;
    //private bool isHovering = false;

    //private void Awake()
    //{
    //    if (targetImage == null) targetImage = GetComponent<RawImage>();
    //    matInstance = Instantiate(targetImage.material);
    //    targetImage.material = matInstance;
    //}

    //private void Update()
    //{
    //    if (isHovering)
    //    {
    //        offset += Time.deltaTime * animateSpeed;
    //    }
    //    else
    //    {
    //        offset = Mathf.Lerp(offset, idleOffset, Time.deltaTime * 4f);
    //    }

    //    matInstance.SetFloat("_Offset", offset);
    //}

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    isHovering = true;
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    isHovering = false;
    //}
}
