using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpAppearUI : MonoBehaviour
{
    public enum AnimationType
    {
        None,
        FadeIn,
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
        public GameObject target;  // UI cần tween (Kéo UI cần chạy hiệu ứng vào)
        public float delay = 0.2f; // Độ trễ trước khi tween item này
        public float duration = 0.4f; // Thời lượng tween
        public AnimationType animation = AnimationType.MoveAndFade; //Chọn hiệu ứng xuất hiện
        public MoveDirection moveDirection = MoveDirection.FromBottom; //Chọn hướng di chuyển nếu có
        public float moveDistance = 100f; // khoảng offset khi move
        public float fromAlpha = 0f; // alpha khởi điểm
    }

    [Header("Cấu hình UI xuất hiện")]
    public List<UIElement> appearSequence = new List<UIElement>(); // Danh sách các UI Element sẽ xuất hiện, đây là một List các UIElement

    void Awake() 
    {
        // Đảm bảo UI items có CanvasGroup
        foreach (var item in appearSequence) // Duyệt qua từng item UIElement trong List appearSequence
        {
            if (item.target != null) //Nếu item.target không null (tức là có UI GameObject được gán vào)
            {
                var cg = item.target.GetComponent<CanvasGroup>(); // Kiểm tra xem item.target có CanvasGroup không
                if (cg == null) cg = item.target.AddComponent<CanvasGroup>(); // Nếu không có, thêm CanvasGroup mới
            }
        }
    }

    // Ẩn tất cả items ngay lập tức (alpha = 0)
    public void HideAllItemsInstant() 
    {
        foreach (var item in appearSequence) // Duyệt qua từng item UIElement trong List appearSequence
        {
            if (item.target != null) // Nếu item.target không null (tức là có UI GameObject được gán vào)
            {
                var cg = item.target.GetComponent<CanvasGroup>();
                if (cg == null) cg = item.target.AddComponent<CanvasGroup>();
                cg.alpha = 0f; // Đặt alpha về 0 để ẩn item cũ đang hiển thị ban đầu trước khi bắt đầu tween 
                item.target.SetActive(true); // Đảm bảo item vẫn được kích hoạt để có thể hiện tween lại sau này
            }
        }
    }

    public IEnumerator PlaySequence()
    {
        foreach (var item in appearSequence) // Duyệt qua từng item UIElement trong List appearSequence
        {
            if (item.target == null) continue; // Nếu item.target là null, bỏ qua item này

            PrepareItem(item); // Chuẩn bị item trước khi bắt đầu tween
            yield return new WaitForSeconds(item.delay); // Chờ đợi thời gian delay đã chỉ định trong mỗi Inspector UIElement
            PlayAnimation(item);
            yield return new WaitForSeconds(item.duration);
        }
    }

    bool IsMoveAnimation(AnimationType anim) // Hàm này kiểm tra xem animation có phải là một dạng di chuyển hay không
    {
        //Giải thích: Sẽ trả về true nếu animation là một trong các loại di chuyển đã định nghĩa
        return anim == AnimationType.MoveAndFade || 
               anim == AnimationType.FadeSlide ||
               anim == AnimationType.EaseBackIn ||
               anim == AnimationType.FromBackZoom ||
               anim == AnimationType.DropBounce; 
    }

    void PrepareItem(UIElement item) // Hàm này chuẩn bị item trước khi bắt đầu tween
    {
        var t = item.target.transform; // Lấy Transform của item.target
        var cg = item.target.GetComponent<CanvasGroup>(); // Lấy CanvasGroup của item.target
        cg.alpha = item.fromAlpha; // Đặt alpha khởi điểm từ alpha đã chỉ định trong mỗi Inspector UIElement 

        Vector3 offset = GetOffset(item); // Lấy offset dựa trên hướng di chuyển đã chỉ định trong mỗi Inspector UIElement
        if (IsMoveAnimation(item.animation) && item.moveDirection != MoveDirection.None) // Nếu animation là dạng di chuyển và có hướng di chuyển đã chỉ định
            t.localPosition += offset; // Cộng thêm offset vào vị trí localPosition từ Transform hiện tại của item.target

        if (item.animation.ToString().Contains("Scale") || // Nếu animation là dạng Scale (bao gồm ScaleAndFade, Pop, ZoomIn, BounceIn, SmoothScaleFade, CenterReveal)
            item.animation == AnimationType.Pop ||
            item.animation == AnimationType.ZoomIn ||
            item.animation == AnimationType.BounceIn ||
            item.animation == AnimationType.CenterReveal ||
            item.animation == AnimationType.SmoothScaleFade)
        {
            t.localScale = Vector3.zero; // Đặt localScale về Vector3.zero để bắt đầu từ kích thước 0
        }

        item.target.SetActive(true); // Đảm bảo item.target được kích hoạt để có thể hiện tween lại sau này
    }

    void PlayAnimation(UIElement item) // Hàm này thực hiện tween cho item.target dựa trên animation đã chỉ định trong mỗi Inspector UIElement
    {
        var t = item.target.transform; // Lấy Transform của item.target
        var cg = item.target.GetComponent<CanvasGroup>(); // Lấy CanvasGroup của item.target

        switch (item.animation) // Dựa trên loại animation đã chỉ định trong mỗi Inspector UIElement, thực hiện tween tương ứng
        {
            case AnimationType.FadeIn: // Hiệu ứng FadeIn sẽ làm cho alpha từ 0 đến 1 trong thời gian đã chỉ định
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad); // DOFade (1f) tức là hiện UI ra với alpha = 1 trong thời gian item.duration đã chỉ định 
                //SetEase (Ease.OutQuad) để hiệu ứng mượt mà hơn
                break;
            case AnimationType.ScaleAndFade:// Hiệu ứng ScaleAndFade sẽ làm cho UI scale từ 0 đến 1 và alpha từ 0 đến 1
                LeanTween.scale(t.gameObject, Vector3.one, item.duration).setEaseOutBack(); // Sử dụng LeanTween để scale Transform của UI GameObject từ Vector3.zero đến Vector3.one trong thời gian item.duration đã chỉ định
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad); // DOFade (1f) tức là hiện UI ra với alpha = 1 trong thời gian item.duration đã chỉ định
                break;
            case AnimationType.MoveAndFade: // Hiệu ứng MoveAndFade sẽ làm cho UI di chuyển từ vị trí hiện tại đến vị trí mới và alpha từ 0 đến 1
                t.DOLocalMove(t.localPosition - GetOffset(item), item.duration).SetEase(Ease.OutCubic); // DOLocalMove sẽ di chuyển Transform của UI GameObject từ vị trí hiện tại đến vị trí mới đã tính toán dựa trên offset trong thời gian item.duration đã chỉ định
                // Phải lấy vị trí hiện tại trừ đi Offset đã tính được từ hàm GetOffset(item) để tính ra được khoảng cách cần di chuyển 
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;
            case AnimationType.Pop: // Hiệu ứng Pop sẽ làm cho UI scale từ 0 đến 1.1 và sau đó về 1, đồng thời alpha từ 0 đến 1
                t.localScale = Vector3.zero; // Đặt localScale về Vector3.zero để bắt đầu từ kích thước 0
                LeanTween.scale(t.gameObject, Vector3.one * 1.1f, item.duration * 0.6f).setEaseOutBack() // Sử dụng LeanTween để scale Transform của UI GameObject từ Vector3.zero đến Vector3.one * 1.1 trong thời gian item.duration * 0.6f đã chỉ định
                    .setOnComplete(() => { LeanTween.scale(t.gameObject, Vector3.one, item.duration * 0.4f).setEaseInOutCubic(); }); // Sau khi hoàn thành Scale * 1.1 (Tức kích thước Scale = 110%), scale lại về Vector3.one trong thời gian item.duration * 0.4f đã chỉ định
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;
            case AnimationType.ZoomIn:
                t.localScale = Vector3.zero; // Đặt localScale về Vector3.zero để bắt đầu từ kích thước 0
                LeanTween.scale(t.gameObject, Vector3.one, item.duration).setEaseOutExpo(); // Sử dụng LeanTween để scale Transform của UI GameObject từ Vector3.zero đến Vector3.one trong thời gian item.duration đã chỉ định
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;
            case AnimationType.BounceIn:
                t.localScale = Vector3.zero; // Đặt localScale về Vector3.zero để bắt đầu từ kích thước 0
                LeanTween.scale(t.gameObject, Vector3.one, item.duration).setEaseOutBounce(); // Sử dụng LeanTween để scale Transform của UI GameObject từ Vector3.zero đến Vector3.one trong thời gian item.duration đã chỉ định
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;
            case AnimationType.Blink:
                cg.DOFade(1f, item.duration * 0.2f).SetEase(Ease.InOutSine).SetLoops(4, LoopType.Yoyo).OnComplete(() => cg.alpha = 1f); // Hiệu ứng Blink sẽ làm cho alpha từ 0 đến 1 trong thời gian item.duration * 0.2f, sau đó lặp lại 4 lần với hiệu ứng Yoyo (tức là từ 1 về 0 và ngược lại) và cuối cùng đặt alpha về 1f
                break;
            case AnimationType.Swing:
                t.localRotation = Quaternion.Euler(0, 0, 30); // Đặt localRotation z ban đầu = 30 để tạo hiệu ứng Swing
                t.DOLocalRotate(Vector3.zero, item.duration, RotateMode.Fast).SetEase(Ease.OutElastic); // DOLocalRotate sẽ xoay Transform của UI GameObject từ vị trí hiện tại về Vector3.zero (tức là không xoay hướng z = 30 nữa) trong thời gian item.duration đã chỉ định
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;
            case AnimationType.DropBounce: // Hiệu ứng DropBounce sẽ làm cho UI di chuyển từ vị trí hiện tại xuống dưới một khoảng cách nhất định và sau đó nảy lên
                t.localPosition += new Vector3(0, item.moveDistance, 0); // Đặt vị trí localPosition ban đầu của item.target lên trên theo Vector y một khoảng cách moveDistance đã chỉ định trong mỗi Inspector UIElement
                t.DOLocalMoveY(t.localPosition.y - item.moveDistance, item.duration).SetEase(Ease.OutBounce); // DOLocalMoveY sẽ di chuyển Transform của UI GameObject từ vị trí y trên cao hiện tại xuống dưới một khoảng cách moveDistance trong thời gian item.duration đã chỉ định, tức là di chuyển từ vị trí trên cao về vị trí cũ ban đầu. 
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;
            case AnimationType.FromBackZoom:
                t.localPosition += GetOffset(item) * 2;
                t.DOLocalMove(t.localPosition - GetOffset(item) * 2, item.duration).SetEase(Ease.InOutBack);
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;
            case AnimationType.FadeSlide:
                t.DOLocalMove(t.localPosition - GetOffset(item), item.duration).SetEase(Ease.InOutSine);
                cg.DOFade(1f, item.duration).SetEase(Ease.InOutSine);
                break;
            case AnimationType.EaseBackIn:
                t.DOLocalMove(t.localPosition - GetOffset(item), item.duration).SetEase(Ease.InOutBack);
                cg.DOFade(1f, item.duration).SetEase(Ease.InOutCubic);
                break;
            case AnimationType.SmoothScaleFade:
                t.localScale = Vector3.zero;
                LeanTween.scale(t.gameObject, Vector3.one, item.duration).setEaseOutQuad();
                cg.DOFade(1f, item.duration).SetEase(Ease.InOutSine);
                break;
            case AnimationType.CenterReveal:
                t.localScale = new Vector3(0f, 1f, 1f); // Đặt localScale ban đầu theo chiều x = 0 để tạo hiệu ứng reveal từ giữa (Ban đầu nén UI từ 2 bên về phía chính giữa cho dẹp lép)
                LeanTween.scaleX(t.gameObject, 1f, item.duration).setEaseOutCubic(); // Sử dụng LeanTween.scaleX để scale Transform của UI GameObject từ Vector3.zero đến Vector3.one theo chiều x ngang ra sang 2 bên trong thời gian item.duration đã chỉ định
                cg.DOFade(1f, item.duration * 0.8f).SetEase(Ease.OutSine);
                break;
            default:
                cg.DOFade(1f, item.duration).SetEase(Ease.OutQuad);
                break;
        }
    }

    Vector3 GetOffset(UIElement item) // Hàm này trả về offset dựa trên hướng di chuyển đã chỉ định trong mỗi Inspector UIElement
    {
        switch (item.moveDirection) // Dựa trên hướng di chuyển đã chỉ định trong mỗi Inspector UIElement
        {
            case MoveDirection.FromLeft: return new Vector3(-item.moveDistance, 0, 0); //Trong trường hợp di chuyển từ trái, trả về offset bên trái từ Vector3.x = -moveDistance đã set ở mỗi Inspector UIElement
            case MoveDirection.FromRight: return new Vector3(item.moveDistance, 0, 0); //Trong trường hợp di chuyển từ phải, trả về offset bên phải từ Vector3.x = moveDistance đã set ở mỗi Inspector UIElement
            case MoveDirection.FromTop: return new Vector3(0, item.moveDistance, 0); //Trong trường hợp di chuyển từ trên xuống, trả về offset bên trên từ Vector3.y = moveDistance đã set ở mỗi Inspector UIElement
            case MoveDirection.FromBottom: return new Vector3(0, -item.moveDistance, 0); //Trong trường hợp di chuyển từ dưới lên, trả về offset bên dưới từ Vector3.y = -moveDistance đã set ở mỗi Inspector UIElement
            default: return Vector3.zero; // Nếu không có hướng di chuyển, trả về Vector3.zero (không có offset)
        }
    }
}
