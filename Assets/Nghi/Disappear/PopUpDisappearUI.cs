using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpDisappearUI : MonoBehaviour
{
    public enum AnimationType
    {
        None,
        FadeIn, // Thực chất là FadeOut (do đang dùng cho Disappear)
        ScaleAndFade,
        MoveAndFade,
        Pop,
        ZoomIn,
        BounceIn,
        Blink,
        Swing,
        DropBounce,
        FromBackZoom,
        FadeSlide,
        EaseBackIn,
        SmoothScaleFade,
        CenterReveal,
    }

    public enum MoveDirection
    {
        None,
        FromLeft,
        FromRight,
        FromTop,
        FromBottom
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
        public float fromAlpha = 0f; // có thể xóa nếu không dùng

        [Header("Delay đến khi hiện UI tiếp theo")] // Thêm trường này để delay trước khi UI tiếp theo được hiển thị
        public float delayBeforeNextShow = 0.2f; 
    }

    [Header("Cấu hình UI biến mất")]
    public List<UIElement> disappearSequence = new List<UIElement>();
    public bool playOnStart = true;

    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>(); // Lưu vị trí gốc của các GameObject
    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>(); // Lưu tỉ lệ gốc của các GameObject
    //Tại sao phải dùng Dictionary? Vì mỗi GameObject chỉ cần lưu 1 lần, tránh lặp lại nhiều lần nếu có cùng một GameObject trong danh sách.
    // Nếu có nhiều GameObject giống nhau, ta sẽ lưu vị trí và tỉ lệ gốc của chúng để reset sau này.
    //Tại sao phải dùng GameObject làm key? Vì mỗi GameObject là duy nhất trong scene, nên ta có thể dùng nó làm key để truy cập nhanh.
    //Tại sao phải dùng Vector3? Vì vị trí và tỉ lệ của GameObject là Vector3, nên ta cần lưu chúng dưới dạng Vector3 để dễ dàng thao tác.

    void Start()
    {
        if (playOnStart) 
            StartCoroutine(DelayedPlay());
    }

    IEnumerator DelayedPlay() // Thêm coroutine để delay trước khi bắt đầu play sequence
    {
        yield return null; // Đảm bảo mọi thứ đã được khởi tạo xong
        yield return PlaySequence();
    }

    public IEnumerator PlaySequence()
    {
        foreach (var item in disappearSequence)
        {
            if (item.target == null) continue; // Bỏ qua nếu không có target

            CacheOriginals(item); // Lưu vị trí và tỉ lệ gốc của GameObject
            // Tại sao phải cache originals? Để có thể reset vị trí và tỉ lệ của GameObject về trạng thái ban đầu sau khi animation kết thúc.

            yield return new WaitForSeconds(item.delay); // Thêm delay trước khi bắt đầu animation 

            var seq = DOTween.Sequence(); // Tạo một sequence mới để quản lý các animation
            //Tại sao phải dùng sequence? Để có thể kết hợp nhiều animation lại với nhau và điều khiển chúng một cách dễ dàng.
            //Tại sao phải dùng DOTween.Sequence()? Vì DOTween.Sequence() cho phép ta tạo một chuỗi các animation và điều khiển chúng theo thứ tự, dễ dàng hơn so với việc gọi từng animation riêng lẻ.
            //Tại sao phải tạo Sequence mới? Vì mỗi UIElement có thể có các animation khác nhau, nên ta cần tạo một sequence mới cho mỗi item để quản lý chúng riêng biệt.

            PlayHideAnimation(item, seq);

            seq.AppendCallback(() => // Callback để thực hiện hành động sau khi animation kết thúc
            {
                ResetToOriginal(item); // Reset trước khi tắt để tránh bị tắt rồi mới reset
                item.target.SetActive(false); // Tắt GameObject sau khi đã reset về vị trí và tỉ lệ gốc
            });

            yield return seq.WaitForCompletion(); // Chờ cho sequence hoàn thành trước khi tiếp tục với item tiếp theo
            //!!!!!!!!!!!!!!!
            // Thêm delay tùy chỉnh cho mỗi UI element
            yield return new WaitForSeconds(item.delayBeforeNextShow); // Đợi thêm thời gian trước khi hiển thị UI tiếp theo
        }
    }

    void CacheOriginals(UIElement item) // Lưu vị trí và tỉ lệ gốc của GameObject để reset sau này
    {
        var go = item.target; // Lấy GameObject từ item
        var t = go.transform; // Lấy Transform của GameObject

        if (!originalPositions.ContainsKey(go)) // Kiểm tra nếu chưa lưu vị trí gốc
            //Taị sao phải dùng ContainsKey? Để kiểm tra xem đã tồn tại key của GameObject trong dictionary originalPositions chưa, tránh lưu trùng lặp vị trí gốc nếu có nhiều UIElement cùng target.
            originalPositions[go] = t.localPosition;  // Lưu vị trí gốc của GameObject
        //Dùng cú pháp dictionary[key] = value để lưu, thêm và thay thế, cập nhật giá trị của key nếu đã tồn tại.
        //Tại sao phải dùng localPosition? Vì ta muốn lưu vị trí tương đối của GameObject so với cha của nó, tránh bị ảnh hưởng bởi vị trí toàn cục.

        if (!originalScales.ContainsKey(go)) // Kiểm tra nếu chưa lưu tỉ lệ gốc
            originalScales[go] = t.localScale;
    }

    void ResetToOriginal(UIElement item) // Hàm này sẽ reset vị trí và tỉ lệ của GameObject về trạng thái ban đầu
    {
        var go = item.target; // Lấy GameObject từ item
        var t = go.transform; // Lấy Transform của GameObject

        if (originalPositions.TryGetValue(go, out var pos)) // Kiểm tra nếu đã lưu vị trí gốc, nếu đã lưu mà tìm thấy thì gán vào pos
            t.localPosition = pos; // Gán vị trí gốc đã lưu vào Transform của GameObject để reset về vị trí ban đầu

        if (originalScales.TryGetValue(go, out var scale)) // Kiểm tra nếu đã lưu tỉ lệ gốc, nếu đã lưu mà tìm thấy thì gán vào scale
            t.localScale = scale; // Gán tỉ lệ gốc đã lưu vào Transform của GameObject để reset về tỉ lệ ban đầu

        var cg = go.GetComponent<CanvasGroup>();
        if (cg != null)
            cg.alpha = 1f; // Nếu có CanvasGroup, đặt alpha về 1 để đảm bảo UI element hiển thị lại bình thường

        DOTween.Kill(go); // Hủy tất cả các tween hiện tại trên GameObject để tránh xung đột
        LeanTween.cancel(go); // Hủy tất cả các tween hiện tại trên GameObject để tránh xung đột
    }

    void PlayHideAnimation(UIElement item, Sequence seq) // Hàm này sẽ thực hiện các animation ẩn UI element dựa trên loại animation đã chọn
    {
        var t = item.target.transform;
        var go = item.target;
        var cg = go.GetComponent<CanvasGroup>(); // Lấy CanvasGroup nếu có, nếu không thì tạo mới
        if (cg == null) cg = go.AddComponent<CanvasGroup>();

        DOTween.Kill(go); // Hủy tất cả các tween hiện tại trên GameObject để tránh xung đột
        LeanTween.cancel(go); // Hủy tất cả các tween hiện tại trên GameObject để tránh xung đột

        Vector3 offset = GetOffset(item); // Lấy offset dựa trên hướng di chuyển đã chỉ định trong mỗi Inspector UIElement
        Vector3 startPos = originalPositions[go]; // Lấy vị trí gốc đã lưu trước đó

        switch (item.animation)
        {
            case AnimationType.MoveAndFade:
                seq.Append(t.DOLocalMove(startPos + offset, item.duration).SetEase(Ease.InCubic)); //Từ vị trí gốc đã lưu trong Dictionary originalPositions, di chuyển đến vị trí mới dựa trên offset đã tính toán.
                //Tại sao phải dùng DOLocalMove? Vì ta muốn di chuyển GameObject từ vị trí gốc đến vị trí mới dựa trên offset đã tính toán, sử dụng tweening để tạo hiệu ứng mượt mà.
                seq.Join(cg.DOFade(0f, item.duration).SetEase(Ease.InQuad));
                break;

            case AnimationType.FadeIn: // thực chất là FadeOut
            case AnimationType.Blink:
                seq.Append(cg.DOFade(0f, item.duration).SetEase(Ease.OutSine)); 
                break;

            case AnimationType.ScaleAndFade:
            case AnimationType.Pop:
            case AnimationType.ZoomIn:
            case AnimationType.BounceIn:
            case AnimationType.SmoothScaleFade:
                LeanTween.scale(go, Vector3.zero, item.duration).setEaseInBack();
                seq.Append(cg.DOFade(0f, item.duration).SetEase(Ease.InOutSine));
                break;

            case AnimationType.Swing:
                seq.Append(t.DOLocalRotate(new Vector3(0, 0, 30), item.duration * 0.5f, RotateMode.Fast).SetEase(Ease.InBack));
                seq.Join(cg.DOFade(0f, item.duration).SetEase(Ease.InQuad));
                break;

            case AnimationType.DropBounce:
                seq.Append(t.DOLocalMoveY(startPos.y - item.moveDistance, item.duration).SetEase(Ease.InBack));
                seq.Join(cg.DOFade(0f, item.duration).SetEase(Ease.InQuad));
                break;

            case AnimationType.FromBackZoom:
                seq.Append(t.DOLocalMove(startPos + offset * 2, item.duration).SetEase(Ease.InBack));
                seq.Join(cg.DOFade(0f, item.duration).SetEase(Ease.InQuad));
                break;

            case AnimationType.FadeSlide:
                seq.Append(t.DOLocalMove(startPos + offset, item.duration).SetEase(Ease.InOutSine));
                seq.Join(cg.DOFade(0f, item.duration).SetEase(Ease.InOutSine));
                break;

            case AnimationType.EaseBackIn:
                seq.Append(t.DOLocalMove(startPos + offset, item.duration).SetEase(Ease.InBack));
                seq.Join(cg.DOFade(0f, item.duration).SetEase(Ease.InOutCubic));
                break;

            case AnimationType.CenterReveal:
                LeanTween.scaleX(go, 0f, item.duration).setEaseInBack();
                seq.Append(cg.DOFade(0f, item.duration).SetEase(Ease.OutSine));
                break;

            default:
                seq.Append(cg.DOFade(0f, item.duration).SetEase(Ease.OutQuad));
                break;
        }
    }

    Vector3 GetOffset(UIElement item) // Hàm này trả về offset dựa trên hướng di chuyển đã chỉ định trong mỗi Inspector UIElement
    {
        switch (item.moveDirection) // // Dựa trên hướng di chuyển đã chỉ định trong mỗi Inspector UIElement
        {
            case MoveDirection.FromLeft: return new Vector3(-item.moveDistance, 0, 0); //Trong trường hợp di chuyển từ trái, trả về offset bên trái từ Vector3.x = -moveDistance đã set ở mỗi Inspector UIElement
            case MoveDirection.FromRight: return new Vector3(item.moveDistance, 0, 0); // Trong trường hợp di chuyển từ phải, trả về offset bên phải từ Vector3.x = moveDistance đã set ở mỗi Inspector UIElement
            case MoveDirection.FromTop: return new Vector3(0, item.moveDistance, 0); // Trong trường hợp di chuyển từ trên xuống, trả về offset bên trên từ Vector3.y = moveDistance đã set ở mỗi Inspector UIElement
            case MoveDirection.FromBottom: return new Vector3(0, -item.moveDistance, 0);  // Trong trường hợp di chuyển từ dưới lên, trả về offset bên dưới từ Vector3.y = -moveDistance đã set ở mỗi Inspector UIElement
            default: return Vector3.zero; // Nếu không có hướng di chuyển, trả về Vector3.zero (không có offset)
        }
    }
}
