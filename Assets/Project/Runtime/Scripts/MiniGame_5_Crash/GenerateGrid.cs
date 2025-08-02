using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject tilePrefab;
    public float spacing = 1.5f;
    public AnimationCurve heightCurve;

    public int coreSize = 5;   // 5x5 core
    public int wingLength = 5; // wing dài ra

    [Header("Auto-Generated Spawn Data")]
    public Transform topEdge;
    public Transform bottomEdge;
    public Transform leftEdge;
    public Transform rightEdge;

    public List<Vector3> topLines = new List<Vector3>();
    public List<Vector3> bottomLines = new List<Vector3>();
    public List<Vector3> leftLines = new List<Vector3>();
    public List<Vector3> rightLines = new List<Vector3>();

    public float groundY;

    float actualSpacing; // spacing thực = tile + khoảng hở

    void Start()
    {
        // Tính spacing thực
        float tileSize = 1f;
        if (tilePrefab.TryGetComponent(out Renderer rend))
            tileSize = rend.bounds.size.x; // assume square tile
        actualSpacing = tileSize + spacing;

        Generate();
        CreateSpawnEdgesAndLines();
    }

    void Generate()
    {
        int halfCore = coreSize / 2;

        // Core
        for (int r = -halfCore; r <= halfCore; r++)
        {
            for (int c = -halfCore; c <= halfCore; c++)
                CreateTile(c, r);
        }

        // Wings
        for (int r = halfCore + 1; r <= halfCore + wingLength; r++)
            for (int c = -halfCore; c <= halfCore; c++)
                CreateTile(c, r);

        for (int r = -halfCore - wingLength; r <= -halfCore - 1; r++)
            for (int c = -halfCore; c <= halfCore; c++)
                CreateTile(c, r);

        for (int c = -halfCore - wingLength; c <= -halfCore - 1; c++)
            for (int r = -halfCore; r <= halfCore; r++)
                CreateTile(c, r);

        for (int c = halfCore + 1; c <= halfCore + wingLength; c++)
            for (int r = -halfCore; r <= halfCore; r++)
                CreateTile(c, r);
    }

    void CreateTile(int col, int row)
    {
        Vector3 localPos = new Vector3(col * actualSpacing, 0, row * actualSpacing);
        Vector3 worldPos = transform.position + localPos;

        float dist = Mathf.Abs(localPos.x) + Mathf.Abs(localPos.z);
        worldPos.y = transform.position.y + heightCurve.Evaluate(dist);

        Instantiate(tilePrefab, worldPos, Quaternion.identity, transform);
    }

    void CreateSpawnEdgesAndLines()
    {
        float halfCore = coreSize / 2f;
        float edgeOffset = (halfCore + wingLength) * actualSpacing;

        // 4 Edge point
        topEdge = CreateEdgePoint(new Vector3(0, groundY, edgeOffset), "TopEdge");
        bottomEdge = CreateEdgePoint(new Vector3(0, groundY, -edgeOffset), "BottomEdge");
        leftEdge = CreateEdgePoint(new Vector3(-edgeOffset, groundY, 0), "LeftEdge");
        rightEdge = CreateEdgePoint(new Vector3(edgeOffset, groundY, 0), "RightEdge");

        // Generate line positions
        topLines = GenerateLineOffsets(topEdge.position, true);
        bottomLines = GenerateLineOffsets(bottomEdge.position, true);
        leftLines = GenerateLineOffsets(leftEdge.position, false);
        rightLines = GenerateLineOffsets(rightEdge.position, false);
    }

    Transform CreateEdgePoint(Vector3 localOffset, string name)
    {
        GameObject edgeObj = new GameObject(name);
        edgeObj.transform.parent = transform;
        edgeObj.transform.position = transform.position + localOffset;
        return edgeObj.transform;
    }

    List<Vector3> GenerateLineOffsets(Vector3 edgePos, bool horizontal)
    {
        List<Vector3> lines = new List<Vector3>();
        float half = (coreSize - 1) / 2f;

        for (int i = 0; i < coreSize; i++)
        {
            float offsetValue = (i - half) * actualSpacing;

            Vector3 pos;
            if (horizontal)
                pos = edgePos + new Vector3(offsetValue, 0, 0);
            else
                pos = edgePos + new Vector3(0, 0, offsetValue);

            pos.y = groundY;
            lines.Add(pos);
        }

        return lines;
    }

    public float GetGridSpacing() => actualSpacing;
    public int GetLinesPerSide() => coreSize;
}
