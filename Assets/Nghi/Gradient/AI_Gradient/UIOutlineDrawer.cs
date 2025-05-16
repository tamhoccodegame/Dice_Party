using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class UIOutlineDrawer : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Line Settings")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.cyan;
    [Range(0.5f, 20f)] public float thickness = 2f;

    [Header("Enable Sides")]
    public bool showTop = true;
    public bool showBottom = true;
    public bool showLeft = true;
    public bool showRight = true;

    [Header("Edge Expansion Direction (true = Outward, false = Inward)")]
    public bool expandTopOutward = true;
    public bool expandBottomOutward = true;
    public bool expandLeftOutward = true;
    public bool expandRightOutward = true;

    [Header("Hover Source")]
    [SerializeField] GameObject hoverTarget;

    private RectTransform rectTransform;
    private bool isHovering = false;
    private bool hasOutlineDrawn = false;

    private readonly string[] sideNames = { "Top", "Bottom", "Left", "Right" };
    private readonly Vector2[] pivots = {
        new Vector2(0.5f, 1f),
        new Vector2(0.5f, 0f),
        new Vector2(0f, 0.5f),
        new Vector2(1f, 0.5f)
    };

    private Dictionary<string, Image> outlineImages = new Dictionary<string, Image>();

    void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        if (hoverTarget == null && transform.parent != null)
        {
            hoverTarget = transform.parent.gameObject;
        }

        AttachHoverEvents();
        StartCoroutine(DelayedUpdate());
    }

    IEnumerator DelayedUpdate()
    {
        yield return new WaitForEndOfFrame();
        UpdateOutline();
        ApplyColorInstant(normalColor);
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying)
            UpdateOutline();
    }
#endif

    void UpdateOutline()
    {
        hasOutlineDrawn = false;
        outlineImages.Clear();

        for (int i = 0; i < 4; i++)
        {
            string name = "[Outline]_" + sideNames[i];
            Transform child = transform.Find(name);
            bool shouldShow = GetSideEnabled(i);

            if (shouldShow)
            {
                if (child == null)
                {
                    GameObject lineObj = new GameObject(name, typeof(RectTransform), typeof(Image));
                    lineObj.transform.SetParent(transform);
                    lineObj.transform.SetAsFirstSibling();
                    child = lineObj.transform;
                }

                RectTransform lineRect = child.GetComponent<RectTransform>();
                Image img = child.GetComponent<Image>();

                bool outward = GetOutward(i);
                float offset = outward ? thickness / 2f : -thickness / 2f;

                lineRect.pivot = pivots[i];
                lineRect.anchorMin = pivots[i];
                lineRect.anchorMax = pivots[i];
                lineRect.localScale = Vector3.one;
                lineRect.anchoredPosition = Vector2.zero;

                float width = rectTransform.rect.width;
                float height = rectTransform.rect.height;

                if (i < 2) // Top or Bottom
                {
                    float extraWidth = 0f;
                    if (showLeft && expandLeftOutward) extraWidth += thickness;
                    if (showRight && expandRightOutward) extraWidth += thickness;

                    lineRect.sizeDelta = new Vector2(width + extraWidth, thickness);
                    lineRect.anchoredPosition = new Vector2(0, offset);
                }
                else // Left or Right
                {
                    float extraHeight = 0f;
                    if (showTop && expandTopOutward) extraHeight += thickness;
                    if (showBottom && expandBottomOutward) extraHeight += thickness;

                    lineRect.sizeDelta = new Vector2(thickness, height + extraHeight);
                    lineRect.anchoredPosition = new Vector2(offset, 0);
                }

                outlineImages[name] = img;
                hasOutlineDrawn = true;
            }
            else if (child != null)
            {
#if UNITY_EDITOR
                if (Application.isEditor)
                    DestroyImmediate(child.gameObject);
                else
#endif
                    Destroy(child.gameObject);
            }
        }
    }

    void AttachHoverEvents()
    {
        if (hoverTarget == null) return;

        EventTrigger trigger = hoverTarget.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = hoverTarget.AddComponent<EventTrigger>();
        }

        trigger.triggers ??= new List<EventTrigger.Entry>();

        AddEvent(trigger, EventTriggerType.PointerEnter, OnPointerEnter);
        AddEvent(trigger, EventTriggerType.PointerExit, OnPointerExit);
    }

    void AddEvent(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }

    public void OnPointerEnter(BaseEventData data)
    {
        isHovering = true;
        AnimateColorChange(hoverColor);
    }

    public void OnPointerExit(BaseEventData data)
    {
        isHovering = false;
        AnimateColorChange(normalColor);
    }

    void AnimateColorChange(Color targetColor)
    {
        foreach (var pair in outlineImages)
        {
            Image img = pair.Value;
            if (img == null) continue;

            LeanTween.value(gameObject, img.color, targetColor, 0.3f)
                .setOnUpdate((Color val) => {
                    if (img != null) img.color = val;
                });
        }
    }

    void ApplyColorInstant(Color color)
    {
        foreach (var pair in outlineImages)
        {
            if (pair.Value != null)
                pair.Value.color = color;
        }
    }

    bool GetSideEnabled(int i) => i switch
    {
        0 => showTop,
        1 => showBottom,
        2 => showLeft,
        3 => showRight,
        _ => false,
    };

    bool GetOutward(int i) => i switch
    {
        0 => expandTopOutward,
        1 => expandBottomOutward,
        2 => expandLeftOutward,
        3 => expandRightOutward,
        _ => true,
    };
}
