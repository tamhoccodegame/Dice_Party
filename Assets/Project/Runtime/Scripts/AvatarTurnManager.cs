using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AvatarTurnManager : MonoBehaviour
{
    public static AvatarTurnManager instance;
    [Header("List Avatar UI (RectTransform)")]
    public List<RectTransform> avatars;

    [Header("Scale Settings")]
    public float normalScale = 1f;         // Scale bình thường
    public float activeScale = 1.3f;       // Scale to khi là người được chọn
    public float pressScale = 0.8f;        // Scale nhỏ khi nhấn

    [Header("Animation Settings")]
    public float pressDuration = 0.1f;     // Thời gian thu nhỏ
    public float popDuration = 0.2f;       // Thời gian bật to
    public Ease pressEase = Ease.InQuad;   // Hiệu ứng thu nhỏ
    public Ease popEase = Ease.OutBack;    // Hiệu ứng bật to

    private int currentIndex = -1;         // Lượt hiện tại

    public TextMeshProUGUI playerNameText;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        int index = 0;
        // Reset scale tất cả về bình thường
        foreach (var avatar in avatars)
        {
            Sprite avatarSprite = AvatarLoader.instance.GetAvatarSprite(index);
            avatar.Find("Avatar").GetComponent<Image>().sprite = avatarSprite;
            avatar.localScale = Vector3.one * normalScale;
            playerNameText.text = $"Player {index + 1}";
            avatar.gameObject.SetActive(true);
            index++;
            if (index > PlayerManager.instance.players.Count - 1) break;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int nextIndex = (currentIndex + 1) % avatars.Count;
            HighlightTurn(nextIndex);
        }
    }

    /// <summary>
    /// Gọi hàm này khi tới lượt playerIndex
    /// </summary>
    public void HighlightTurn(int playerIndex)
    {
        // Reset người cũ về scale bình thường
        if (currentIndex >= 0 && currentIndex < avatars.Count)
        {
            avatars[currentIndex].DOScale(normalScale, 0.2f).SetEase(Ease.OutQuad);
        }

        currentIndex = playerIndex;

        // Chạy animation Press → Pop → Active
        RectTransform target = avatars[playerIndex];

        // Kill tween cũ nếu còn chạy
        target.DOKill();

        // Thu nhỏ nhanh (Press)
        target.localScale = Vector3.one * normalScale;
        target.DOScale(pressScale, pressDuration).SetEase(pressEase).OnComplete(() =>
        {
            // Bật ra to (Pop)
            target.DOScale(activeScale, popDuration).SetEase(popEase);
        });
    }
}
