using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer)), ExecuteAlways]
public class LineRendererUtil : MonoBehaviour
{
    LineRenderer line;
    public LineRenderer Line { get => line; }

    public Transform targetSurface; // 所有點吸附在此表面上(僅Plane)
    Collider[] surfaceCols;
    public float yOffset;   // 離表面高度

    public bool complexSurface = false;

    private void OnEnable()
    {
        if (line == null)
            line = GetComponent<LineRenderer>();

        if (targetSurface != null && targetSurface.GetComponentsInChildren<Collider>() != null)
            surfaceCols = targetSurface.GetComponentsInChildren<Collider>();
    }

    public void SetCorners(params Vector3[] corners)
    {
        if (targetSurface != null)
        {
            corners = ToSurface(corners);
        }

        line.positionCount = corners.Length;
        line.SetPositions(corners);
    }

    public Vector3[] ToSurface(Vector3[] positions)
    {
        for (int i = 0; i < positions.Length; ++i)
        {
            var temp = positions[i];
            temp = ToSurface(temp);
            positions[i] = temp;
        }
        return positions;
    }

    public Vector3 ToSurface(Vector3 position)
    {
        var minDst = float.MaxValue;
        var result = position;
        for (int i = 0; i < surfaceCols.Length; ++i)
        {
            var col = surfaceCols[i];
            var p = col.ClosestPoint(position);
            if (Vector3.Distance(p, position) < minDst)
            {
                minDst = Vector3.Distance(p, position);
                result = p;
            }
        }

        if (!complexSurface)
            result.y = yOffset + targetSurface.position.y;
        else
            result.y += yOffset;
        return result;
    }

    public Vector3[] WorldSpaceOffset(Vector3[] positions)
    {
        for (int i = 0; i < positions.Length; ++i)
        {
            positions[i] = positions[i] + targetSurface.position;
        }
        return positions;
    }

    private void OnDrawGizmosSelected()
    {
        if (targetSurface == null || surfaceCols == null)
            return;

        Gizmos.color = Color.green;
        foreach (var col in surfaceCols)
        {
            var bounds = col.bounds;
            var points = new Vector3[4];
            points[0] = bounds.center + new Vector3(+bounds.extents.x, 0, +bounds.extents.z) + Vector3.up * yOffset;
            points[1] = bounds.center + new Vector3(-bounds.extents.x, 0, +bounds.extents.z) + Vector3.up * yOffset;
            points[2] = bounds.center + new Vector3(-bounds.extents.x, 0, -bounds.extents.z) + Vector3.up * yOffset;
            points[3] = bounds.center + new Vector3(+bounds.extents.x, 0, -bounds.extents.z) + Vector3.up * yOffset;
            for (int i = 0; i < 4; ++i)
                Gizmos.DrawLine(points[i], points[i == 3 ? 0 : (i + 1)]);
        }
    }

    // FOR TEST
    // [ContextMenu("測試")]
    // void TEST()
    // {
    //     if (surfaceCols == null || targetSurface == null)
    //         return;

    //     var bounds = surfaceCols.bounds;

    //     int pCount = 3;
    //     var points = new Vector3[pCount];
    //     for (int i = 0; i < pCount; ++i)
    //     {
    //         var x = Random.Range(bounds.extents.x, -bounds.extents.x);
    //         var z = Random.Range(bounds.extents.z, -bounds.extents.z);
    //         var y = Random.Range(-5f, +5f);

    //         points[i] = new Vector3(x, y, z) + bounds.center;
    //         // print(points[i]);
    //     }

    //     SetCorners(points);
    // }
}
