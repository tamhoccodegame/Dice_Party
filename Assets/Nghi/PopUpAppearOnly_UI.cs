using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpAppearOnly_UI : MonoBehaviour
{
    public enum AnimationType
    {
        None,
        FadeIn,
        ScaleAndFade,
        MoveAndFade,
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
        FromFarBehind,   // Z = -999 -> 0 (từ sau lưng camera bay ra)
        FromFarFront     // Z = +999 -> 0 (từ trước mặt camera bay lại gần)
    }

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

        [Header("Press Settings")]
        public float ScaleUp = 1.2f; // Scale to bao nhiêu
        public float ScaleDown = 0.9f; // Nảy về thấp hơn
        public float UpDuration = 0.3f;
        public float DownDuration = 0.2f;
        public float SettleDuration = 0.3f;

        [Header("DropBounce Settings")] // 🎯 DropBounce mới
        public float DropHeight = 300f;      // Rơi từ cao bao nhiêu
        public int BounceCount = 3;          // Số lần nảy
        public float BounceDamping = 0.5f;   // Mỗi lần nảy giảm bao nhiêu % độ cao

        [Header("LaunchForward Settings")]
        public bool useLaunchForward = false;         // Bật tắt LaunchForward
        public LaunchDirection launchDirection = LaunchDirection.FromFarFront;
        public float LaunchDistanceZ = 999f;          // Khoảng cách Z
    }

    [Header("Cấu hình UI xuất hiện")]
    public List<UIElement> appearSequence = new List<UIElement>();
    public bool playOnStart = true;

    private bool hasPlayed = false; // đánh dấu đã chạy chưa

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
        yield return null; // 🔥 Delay 1 frame để LayoutGroup chạy xong!
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        if (hasPlayed) yield break; // nếu đã chạy rồi thì thôi không chạy nữa
        hasPlayed = true;


        foreach (var item in appearSequence)
        {
            if (item.target == null) continue;

            PrepareItem(item);
            yield return new WaitForSeconds(item.delay);
            PlayAnimation(item);
            yield return new WaitForSeconds(item.duration);
        }
    }

    bool IsMoveAnimation(AnimationType anim)
    {
        return anim == AnimationType.MoveAndFade;
    }

    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();

    void PrepareItem(UIElement item)
    {
        var t = item.target.transform;

        var cg = item.target.GetComponent<CanvasGroup>();
        if (cg == null) cg = item.target.AddComponent<CanvasGroup>();
        cg.alpha = item.fromAlpha;

        Vector3 offset = GetOffset(item);

        // Di chuyển nếu có hiệu ứng di chuyển
        if (IsMoveAnimation(item.animation) && item.moveDirection != MoveDirection.None)
        {
            t.localPosition += offset;
        }

        // Scale về 0 cho các animation cần scale
        if (item.animation.ToString().Contains("Scale") ||

            item.animation == AnimationType.ZoomIn ||
            //item.animation == AnimationType.BounceIn ||
            item.animation == AnimationType.Press)
        {
            t.localScale = Vector3.zero;
        }

        if (item.animation == AnimationType.DropBounce)
        {
            // DropBounce luôn rớt từ trên cao
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
                    startZ = -Mathf.Abs(item.LaunchDistanceZ); // Phóng từ trước
                    break;

                case LaunchDirection.FromFarBehind:
                    startZ = Mathf.Abs(item.LaunchDistanceZ); // Phóng từ sau
                    break;
            }

            t.localPosition = new Vector3(
                t.localPosition.x,
                t.localPosition.y,
                startZ
            );

            cg.alpha = 1f; // luôn hiện
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
                    float singleDuration = item.duration / (item.BounceCount * 2); // chia đều thời gian cho mỗi nửa nhịp

                    for (int i = 0; i < item.BounceCount; i++)
                    {
                        // Rơi xuống mặt đất
                        dropSeq.Append(t.DOLocalMoveY(originalPos.y, singleDuration).SetEase(Ease.InQuad));

                        // Nếu là cú nảy cuối thì không nảy lên nữa
                        if (i == item.BounceCount - 1) break;

                        // Nảy lên cao giảm dần (mỗi lần giảm BounceDamping%)
                        currentHeight *= item.BounceDamping;

                        // Nảy lên
                        dropSeq.Append(t.DOLocalMoveY(originalPos.y + currentHeight, singleDuration).SetEase(Ease.OutQuad));
                    }

                    // Đảm bảo dừng đúng vị trí gốc
                    dropSeq.Append(t.DOLocalMoveY(originalPos.y, singleDuration).SetEase(Ease.InQuad));

                    // Alpha tăng song song (cho đẹp)
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
                    t.localScale = Vector3.one; // Đảm bảo scale gốc ngay từ đầu
                    cg.alpha = 1f; // Không fade, hiện ngay

                    Sequence pressSeq = DOTween.Sequence();

                    // 1. Scale nhỏ xuống mạnh liền tay
                    pressSeq.Append(t.DOScale(0.85f, item.DownDuration).SetEase(Ease.InBack)); // InBack để ép mạnh hơn

                    // 2. Đợi 0.2 giây cho cảm giác bị ép
                    pressSeq.AppendInterval(0.2f);

                    // 3. Bounce Scale về scale cũ với lực mạnh dứt khoát
                    pressSeq.Append(t.DOScale(Vector3.one, item.UpDuration).SetEase(Ease.OutBounce)); // Bounce cho nó nảy đã con mắt
                }
                break;


            case AnimationType.LaunchForward:
                {
                    if (!item.useLaunchForward) break;

                    Sequence launchSeq = DOTween.Sequence();
                    Vector3 targetPos = originalPositions[item.target]; // Vị trí gốc Z = 0

                    launchSeq.AppendInterval(item.delay); // dùng delay tổng

                    launchSeq.Append(
                        t.DOLocalMoveZ(targetPos.z, item.duration) // dùng duration tổng
                            .SetEase(Ease.OutCubic) // smooth
                    );

                    cg.alpha = 1f; // đảm bảo luôn hiện
                }
                break;
        }
    }


    Vector3 GetOffset(UIElement item)
    {
        if (item.moveDirection == MoveDirection.None)
            return Vector3.zero;

        switch (item.moveDirection)
        {
            case MoveDirection.FromLeft: return new Vector3(-item.moveDistance, 0, 0);
            case MoveDirection.FromRight: return new Vector3(item.moveDistance, 0, 0);
            case MoveDirection.FromTop: return new Vector3(0, item.moveDistance, 0);
            case MoveDirection.FromBottom: return new Vector3(0, -item.moveDistance, 0);
            default: return Vector3.zero;
        }
    }


    void HideAllItemsInstant()
    {
        foreach (var item in appearSequence)
        {
            if (item.target != null)
            {
                var cg = item.target.GetComponent<CanvasGroup>();
                if (cg == null) cg = item.target.AddComponent<CanvasGroup>();
                cg.alpha = 0f;
                item.target.SetActive(true); // Phải bật mới set alpha được
            }
        }
    }

    public void StartAppearSequence()
    {
        StartCoroutine(PlaySequence());
    }
}
