using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_VFX_Spawner : MonoBehaviour
{
    [System.Serializable]
    public class VFXSpawnData
    {
        public GameObject vfxPrefab;
        public Vector2 offset; // Offset local position
        public float delay;
    }

    public List<VFXSpawnData> vfxList = new List<VFXSpawnData>();
    public Vector2 startPosition = Vector2.zero;
    public Vector2 direction = Vector2.right; // hướng spawn: right/down
    public float spacing = 50f; // khoảng cách giữa các VFX
    public bool autoPlayOnStart = false;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        if (autoPlayOnStart)
            StartCoroutine(SpawnVFXSequence());
    }

    public void PlayVFX()
    {
        StartCoroutine(SpawnVFXSequence());
    }

    IEnumerator SpawnVFXSequence()
    {
        for (int i = 0; i < vfxList.Count; i++)
        {
            var data = vfxList[i];
            Vector2 pos = startPosition + direction.normalized * spacing * i + data.offset;

            GameObject vfx = Instantiate(data.vfxPrefab, transform);
            vfx.transform.localPosition = pos;

            yield return new WaitForSeconds(data.delay);
        }
    }
}
