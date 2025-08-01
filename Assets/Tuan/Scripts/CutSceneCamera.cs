using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutSceneCamera : MonoBehaviour
{
    public PlayableDirector timeline;
    public GameObject countdownCanvas;
    public Transform cameraTransform;
    private Vector3 originalPos;
    private Quaternion originalRot;

    void Start()
    {
        countdownCanvas.SetActive(false);
        originalPos = cameraTransform.position;
        originalRot = cameraTransform.rotation;

        timeline.stopped += OnTimelineFinished;
        timeline.Play();
    }
    void OnTimelineFinished(PlayableDirector director)
    {

        cameraTransform.position = originalPos;
        cameraTransform.rotation = originalRot;
        countdownCanvas.SetActive(true);
    }
}
