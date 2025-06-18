using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UI_IdolManager : MonoBehaviour
{
    [System.Serializable]
    public class IdolUI
    {
        public string idolName;
        public GameObject target;
        public float appearDuration = 0.5f;
        public float waitBeforeMove = 2f;
        public float moveDuration = 0.5f;
        public Ease moveEase = Ease.InOutQuad;
        public List<Transform> moveTargets;
        public AudioClip appearSFX;
        public ParticleSystem appearVFX;
    }

    [Header("Idol Configurations")]
    public List<IdolUI> idolUIList = new List<IdolUI>();

    [Header("UI Manager Reference")]
    public PopUpAppearOnly_UI uiAppearScript;

    [Header("Debug Mode")]
    [SerializeField] private bool debugMode = false;
    private int debugIndex = 0;

    private void OnEnable()
    {
        if (!debugMode)
        {
            HideAllIdols();
            StartCoroutine(PlayIdolSequence());
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (debugMode && Input.GetKeyDown(KeyCode.Space))
        {
            StopAllCoroutines();
            HideAllIdols();
            StartCoroutine(PlaySingleIdol(idolUIList[debugIndex]));
            debugIndex = (debugIndex + 1) % idolUIList.Count;
        }
#endif
    }

    private void HideAllIdols()
    {
        foreach (var idol in idolUIList)
        {
            if (idol.target != null)
                idol.target.SetActive(false);
        }
    }

    private IEnumerator PlayIdolSequence()
    {
        foreach (var idol in idolUIList)
        {
            if (idol.target == null || idol.moveTargets == null || idol.moveTargets.Count == 0)
                continue;

            yield return StartCoroutine(PlaySingleIdol(idol));
        }

        if (uiAppearScript != null)
        {
            uiAppearScript.gameObject.SetActive(true);
            uiAppearScript.StartAppearSequence();
        }
    }

    private IEnumerator PlaySingleIdol(IdolUI idol)
    {
        // Step 1: Position idol at moveTargets[0] and hide initially
        idol.target.transform.localPosition = idol.moveTargets[0].localPosition;
        idol.target.transform.localScale = Vector3.zero;
        idol.target.SetActive(true);

        // Step 2: Appear animation
        yield return StartCoroutine(AppearIdol(idol));

        // Step 3: Wait before movement
        yield return new WaitForSeconds(idol.waitBeforeMove);

        // Step 4: Move through path
        yield return StartCoroutine(MoveIdolThroughPoints(idol));
    }

    private IEnumerator AppearIdol(IdolUI idol)
    {
        Debug.Log($"[UI_IdolManager] {idol.idolName} is appearing...");

        CanvasGroup cg = idol.target.GetComponent<CanvasGroup>();
        if (cg == null) cg = idol.target.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        if (idol.appearSFX != null)
        {
            AudioSource.PlayClipAtPoint(idol.appearSFX, Camera.main.transform.position);
        }

        if (idol.appearVFX != null)
        {
            idol.appearVFX.Play();
        }

        Sequence appearSequence = DOTween.Sequence();
        appearSequence.Join(cg.DOFade(1f, idol.appearDuration).SetEase(Ease.OutQuad));
        appearSequence.Join(idol.target.transform.DOScale(Vector3.one, idol.appearDuration).SetEase(Ease.OutBack));

        yield return appearSequence.WaitForCompletion();
    }

    private IEnumerator MoveIdolThroughPoints(IdolUI idol)
    {
        Debug.Log($"[UI_IdolManager] {idol.idolName} starts moving...");

        for (int i = 1; i < idol.moveTargets.Count; i++)
        {
            var point = idol.moveTargets[i];
            if (point == null) continue;

            Tween moveTween = idol.target.transform.DOLocalMove(point.localPosition, idol.moveDuration)
                .SetEase(idol.moveEase);

            yield return moveTween.WaitForCompletion();
        }

        Debug.Log($"[UI_IdolManager] {idol.idolName} finished moving.");
    }
}
