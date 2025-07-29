using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpAppearGroup_UI : MonoBehaviour
{
    public enum AnimationType
    {
        None,
        FadeIn,
        ScaleAndFade,
        MoveAndFade,
        Move,
        ZoomIn,
        Blink,
        Swing,
        Press,
        DropBounce,
        LaunchForward
    }

    public enum MoveDirection
    {
        None,
        FromLeft,
        FromRight,
        FromTop,
        FromBottom
    }

    public enum LaunchDirection
    {
        FromFarBehind,   // Z = -999 -> 0
        FromFarFront     // Z = +999 -> 0
    }


    #region Class Element + Group
    [System.Serializable]
    public class UIElement
    {
        public GameObject target;
        public float delay = 0.2f;
        public float duration = 0.4f;
        public AnimationType animation = AnimationType.MoveAndFade;
        public MoveDirection moveDirection = MoveDirection.FromBottom;
        public float moveDistance = 100f;
        public float fromAlpha = 0f;

        //[Header("Press Settings")]
        public float ScaleUp = 1.2f;
        public float ScaleDown = 0.9f;
        public float UpDuration = 0.3f;
        public float DownDuration = 0.2f;
        public float SettleDuration = 0.3f;

        //[Header("DropBounce Settings")]
        public float DropHeight = 300f;
        public int BounceCount = 3;
        public float BounceDamping = 0.5f;

        //[Header("LaunchForward Settings")]
        public bool useLaunchForward = false;
        public LaunchDirection launchDirection = LaunchDirection.FromFarFront;
        public float LaunchDistanceZ = 999f;
    }

    [System.Serializable]
    public class UIGroup
    {
        public string groupName = "Group";
        public List<UIElement> elements = new List<UIElement>();
    }
    #endregion

    [Header("Danh sách các nhóm UI")]
    public List<UIGroup> appearGroups = new List<UIGroup>();
    public bool playOnStart = true;

    private bool hasPlayed = false;
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();

    void Start()
    {
        if (playOnStart)
        {
            HideAllItemsInstant();
            StartCoroutine(DelayedPlay());
        }
    }

    IEnumerator DelayedPlay()
    {
        yield return null;
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        if (hasPlayed) yield break;
        hasPlayed = true;

        // Duyệt từng group
        foreach (var group in appearGroups)
        {
            // Chuẩn bị tất cả phần tử trong group
            foreach (var item in group.elements)
            {
                if (item.target == null) continue;
                PrepareItem(item);
            }

            // Chạy tất cả phần tử trong group đồng thời
            float maxTime = 0f;
            foreach (var item in group.elements)
            {
                if (item.target == null) continue;

                // Delay cá nhân
                StartCoroutine(PlayElementWithDelay(item));

                // Tính thời gian dài nhất của group
                float totalTime = item.delay + item.duration;
                if (totalTime > maxTime) maxTime = totalTime;
            }

            // Đợi group này xong mới qua group kế
            yield return new WaitForSeconds(maxTime);
        }
    }

    IEnumerator PlayElementWithDelay(UIElement item)
    {
        yield return new WaitForSeconds(item.delay);
        PlayAnimation(item);
    }

    bool IsMoveAnimation(AnimationType anim)
    {
        return anim == AnimationType.MoveAndFade;
    }

    void PrepareItem(UIElement item)
    {
        var t = item.target.transform;

        var cg = item.target.GetComponent<CanvasGroup>();
        if (cg == null) cg = item.target.AddComponent<CanvasGroup>();
        cg.alpha = item.fromAlpha;

        Vector3 offset = GetOffset(item);

        if (item.animation == AnimationType.Move && item.moveDirection != MoveDirection.None)
        {
            // Đặt vị trí bắt đầu lệch theo MoveDirection
            t.localPosition += GetOffset(item);

            // Ẩn CanvasGroup
            //var cg = item.target.GetComponent<CanvasGroup>();
            //if (cg == null) cg = item.target.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
        }

        if (IsMoveAnimation(item.animation) && item.moveDirection != MoveDirection.None)
        {
            t.localPosition += offset;
        }

        if (item.animation.ToString().Contains("Scale") ||
            item.animation == AnimationType.ZoomIn ||
            item.animation == AnimationType.Press)
        {
            t.localScale = Vector3.zero;
        }

        if (item.animation == AnimationType.DropBounce)
        {
            t.localPosition += new Vector3(0, item.DropHeight, 0);
        }

        if (item.animation == AnimationType.LaunchForward && item.useLaunchForward)
        {
            if (!originalPositions.ContainsKey(item.target))
                originalPositions[item.target] = t.localPosition;

            float startZ = 0f;
            switch (item.launchDirection)
            {
                case LaunchDirection.FromFarFront:
                    startZ = -Mathf.Abs(item.LaunchDistanceZ);
                    break;
                case LaunchDirection.FromFarBehind:
                    startZ = Mathf.Abs(item.LaunchDistanceZ);
                    break;
            }

            t.localPosition = new Vector3(
                t.localPosition.x,
                t.localPosition.y,
                startZ
            );

            cg.alpha = 1f;
        }




        item.target.SetActive(true);
    }

    void PlayAnimation(UIElement item)
    {
        var t = item.target.transform;
        var cg = item.target.GetComponent<CanvasGroup>();

        switch (item.animation)
        {
            case AnimationType.FadeIn:
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;

            case AnimationType.ScaleAndFade:
                LeanTween.scale(t.gameObject, Vector3.one, item.duration).setEaseOutBack();
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;

            case AnimationType.MoveAndFade:
                t.DOLocalMove(t.localPosition - GetOffset(item), item.duration).SetEase(Ease.OutCubic);
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;

            case AnimationType.Move:
                {
                    //var cg = item.target.GetComponent<CanvasGroup>();

                    // Hiện alpha ngay lập tức
                    cg.DOFade(1f, 0.05f);

                    // Di chuyển về vị trí gốc
                    t.DOLocalMove(t.localPosition - GetOffset(item), item.duration)
                        .SetEase(Ease.OutCubic);
                }
                break;

            case AnimationType.ZoomIn:
                t.localScale = Vector3.zero;
                LeanTween.scale(t.gameObject, Vector3.one, item.duration).setEaseOutExpo();
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;

            case AnimationType.DropBounce:
                {
                    Sequence dropSeq = DOTween.Sequence();
                    Vector3 originalPos = t.localPosition - new Vector3(0, item.DropHeight, 0);
                    float currentHeight = item.DropHeight;
                    float singleDuration = item.duration / (item.BounceCount * 2);

                    for (int i = 0; i < item.BounceCount; i++)
                    {
                        dropSeq.Append(t.DOLocalMoveY(originalPos.y, singleDuration).SetEase(Ease.InQuad));

                        if (i == item.BounceCount - 1) break;

                        currentHeight *= item.BounceDamping;

                        dropSeq.Append(t.DOLocalMoveY(originalPos.y + currentHeight, singleDuration).SetEase(Ease.OutQuad));
                    }

                    dropSeq.Append(t.DOLocalMoveY(originalPos.y, singleDuration).SetEase(Ease.InQuad));

                    cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                }
                break;

            case AnimationType.Blink:
                cg.DOFade(1f, item.duration * 0.2f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(4, LoopType.Yoyo)
                    .OnComplete(() => cg.alpha = 1f);
                break;

            case AnimationType.Swing:
                t.localRotation = Quaternion.Euler(0, 0, 30);
                t.DOLocalRotate(Vector3.zero, item.duration, RotateMode.Fast).SetEase(Ease.OutElastic);
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;

            case AnimationType.Press:
                {
                    t.localScale = Vector3.one;
                    cg.alpha = 1f;

                    Sequence pressSeq = DOTween.Sequence();
                    pressSeq.Append(t.DOScale(0.85f, item.DownDuration).SetEase(Ease.InBack));
                    pressSeq.AppendInterval(0.2f);
                    pressSeq.Append(t.DOScale(Vector3.one, item.UpDuration).SetEase(Ease.OutBounce));
                }
                break;

            case AnimationType.LaunchForward:
                {
                    if (!item.useLaunchForward) break;

                    Sequence launchSeq = DOTween.Sequence();
                    Vector3 targetPos = originalPositions[item.target];

                    launchSeq.Append(
                        t.DOLocalMoveZ(targetPos.z, item.duration)
                            .SetEase(Ease.OutCubic)
                    );

                    cg.alpha = 1f;
                }
                break;
        }
    }

    Vector3 GetOffset(UIElement item)
    {
        // Offset luôn bắt đầu từ vị trí gốc và dịch đúng hướng
        float distance = Mathf.Abs(item.moveDistance); // luôn dương

        switch (item.moveDirection)
        {
            case MoveDirection.FromLeft:
                return new Vector3(-distance, 0f, 0f);

            case MoveDirection.FromRight:
                return new Vector3(distance, 0f, 0f);

            case MoveDirection.FromTop:
                return new Vector3(0f, distance, 0f);

            case MoveDirection.FromBottom:
                return new Vector3(0f, -distance, 0f);

            default:
                return Vector3.zero;
        }
    }


    void HideAllItemsInstant()
    {
        foreach (var group in appearGroups)
        {
            foreach (var item in group.elements)
            {
                if (item.target != null)
                {
                    var cg = item.target.GetComponent<CanvasGroup>();
                    if (cg == null) cg = item.target.AddComponent<CanvasGroup>();
                    cg.alpha = 0f;
                    item.target.SetActive(true);
                }
            }
        }
    }

    public void StartAppearSequence()
    {
        StartCoroutine(PlaySequence());
    }
}