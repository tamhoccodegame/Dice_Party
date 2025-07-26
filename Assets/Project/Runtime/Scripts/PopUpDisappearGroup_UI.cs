using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopUpDisappearGroup_UI : MonoBehaviour
{
    public enum TriggerMode
    {
        None,
        Space,
        Enter,
        ClickAnywhere,
        Manual // gọi hàm bằng code
    }

    [Header("Chọn cách trigger Disappear")]
    public TriggerMode triggerMode = TriggerMode.Space;

    [Header("Danh sách group giống Appear")]
    public List<PopUpAppearGroup_UI.UIGroup> disappearGroups = new List<PopUpAppearGroup_UI.UIGroup>();

    [Tooltip("Tham chiếu tới script Appear để lấy vị trí gốc")]
    public PopUpAppearGroup_UI appearScript;

    private bool hasDisappeared = false;
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();

    void Start()
    {
        if (appearScript != null)
        {
            // copy vị trí gốc từ Appear script
            foreach (var group in appearScript.appearGroups)
            {
                foreach (var item in group.elements)
                {
                    if (item.target != null)
                        originalPositions[item.target] = item.target.transform.localPosition;
                }
            }
        }
    }

    void Update()
    {
        if (hasDisappeared) return;

        switch (triggerMode)
        {
            case TriggerMode.Space:
                if (Input.GetKeyDown(KeyCode.Space)) StartDisappearSequence();
                break;

            case TriggerMode.Enter:
                if (Input.GetKeyDown(KeyCode.Return)) StartDisappearSequence();
                break;

            case TriggerMode.ClickAnywhere:
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                    StartDisappearSequence();
                break;
        }
    }

    public void StartDisappearSequence()
    {
        if (hasDisappeared) return;
        hasDisappeared = true;

        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        foreach (var group in disappearGroups)
        {
            foreach (var item in group.elements)
            {
                if (item.target == null) continue;
                PrepareDisappearItem(item);
            }

            float maxTime = 0f;
            foreach (var item in group.elements)
            {
                if (item.target == null) continue;
                StartCoroutine(PlayElementWithDelay(item));
                float totalTime = item.delay + item.duration;
                if (totalTime > maxTime) maxTime = totalTime;
            }

            yield return new WaitForSeconds(maxTime);
        }
    }

    IEnumerator PlayElementWithDelay(PopUpAppearGroup_UI.UIElement item)
    {
        yield return new WaitForSeconds(item.delay);
        PlayDisappearAnimation(item);
    }


    void PrepareDisappearItem(PopUpAppearGroup_UI.UIElement item)
    {
        // Không cần di chuyển ban đầu: vị trí hiện tại chính là vị trí xuất hiện
        // Chỉ cần đảm bảo alpha hiện = 1
        CanvasGroup cg = item.target.GetComponent<CanvasGroup>();
        if (cg == null) cg = item.target.AddComponent<CanvasGroup>();
        cg.alpha = 1f;
    }

    void PlayDisappearAnimation(PopUpAppearGroup_UI.UIElement item)
    {
        var t = item.target.transform;
        var cg = item.target.GetComponent<CanvasGroup>();

        switch (item.animation)
        {
            case PopUpAppearGroup_UI.AnimationType.FadeIn:
                cg.DOFade(0f, item.duration).SetEase(Ease.OutQuad);
                break;

            case PopUpAppearGroup_UI.AnimationType.ScaleAndFade:
                t.DOScale(Vector3.zero, item.duration).SetEase(Ease.InBack);
                cg.DOFade(0f, item.duration).SetEase(Ease.OutQuad);
                break;

            case PopUpAppearGroup_UI.AnimationType.MoveAndFade:
                {
                    Vector3 currentPos = t.localPosition;
                    t.DOLocalMove(currentPos + GetOffset(item), item.duration).SetEase(Ease.InCubic);
                    cg.DOFade(0f, item.duration).SetEase(Ease.OutQuad);
                }
                break;

            case PopUpAppearGroup_UI.AnimationType.Move:
                {
                    Vector3 currentPos = t.localPosition;
                    t.DOLocalMove(currentPos + GetOffset(item), item.duration)
                        .SetEase(Ease.InCubic);
                }
                break;


            case PopUpAppearGroup_UI.AnimationType.ZoomIn:
                t.DOScale(Vector3.zero, item.duration).SetEase(Ease.InExpo);
                cg.DOFade(0f, item.duration).SetEase(Ease.OutQuad);
                break;

            case PopUpAppearGroup_UI.AnimationType.DropBounce:
                t.DOLocalMove(originalPositions[item.target] + new Vector3(0, -item.DropHeight, 0), item.duration)
                    .SetEase(Ease.InQuad);
                cg.DOFade(0f, item.duration);
                break;

            case PopUpAppearGroup_UI.AnimationType.Blink:
                cg.DOFade(0f, item.duration * 0.2f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(4, LoopType.Yoyo)
                    .OnComplete(() => cg.alpha = 0f);
                break;

            case PopUpAppearGroup_UI.AnimationType.Swing:
                t.DOLocalRotate(new Vector3(0, 0, 30), item.duration / 2).SetEase(Ease.InOutElastic)
                    .OnComplete(() =>
                    {
                        t.DOLocalRotate(new Vector3(0, 0, -30), item.duration / 2).SetEase(Ease.InOutElastic);
                        cg.DOFade(0f, item.duration);
                    });
                break;

            case PopUpAppearGroup_UI.AnimationType.Press:
                t.DOScale(0f, item.duration).SetEase(Ease.InBack);
                cg.DOFade(0f, item.duration / 2);
                break;

            case PopUpAppearGroup_UI.AnimationType.LaunchForward:
                t.DOLocalMoveZ(t.localPosition.z + 999f, item.duration).SetEase(Ease.InCubic);
                cg.DOFade(0f, item.duration);
                break;
        }
    }

    Vector3 GetOffset(PopUpAppearGroup_UI.UIElement item)
    {
        float distance = Mathf.Abs(item.moveDistance);
        switch (item.moveDirection)
        {
            case PopUpAppearGroup_UI.MoveDirection.FromLeft:
                return new Vector3(-distance, 0f, 0f); // đi ra trái

            case PopUpAppearGroup_UI.MoveDirection.FromRight:
                return new Vector3(distance, 0f, 0f); // đi ra phải

            case PopUpAppearGroup_UI.MoveDirection.FromTop:
                return new Vector3(0f, distance, 0f); // đi ra trên

            case PopUpAppearGroup_UI.MoveDirection.FromBottom:
                return new Vector3(0f, -distance, 0f); // đi ra dưới

            default:
                return Vector3.zero;
        }
    }


}
