using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveLineRenderer : MonoBehaviour
{
    public float interpoliteCount = 10;

    public Vector3 p1, p2, p3;
    List<Vector3> points = new List<Vector3>();

    LineRenderer lineRenderer;

    public Transform targetObject;

    public Gradient inactiveColor;
    public Gradient activeColor;

    private void OnEnable()
    {
        if (GetComponent<LineRenderer>())
            lineRenderer = GetComponent<LineRenderer>();
        else
            lineRenderer = gameObject.AddComponent<LineRenderer>();        
    }

    public void SetPoints(Vector3 start, Vector3 end, float curveAmount, float highestPoint)
    {
        p1 = start;
        p3 = end;
        p2 = ((p1 + p3) * highestPoint) + (Vector3.up * curveAmount);

        List<Vector3> points = new List<Vector3>();

        for (float i = 0; i <= 1; i += 1f / interpoliteCount)
        {
            Vector3 t1 = Vector3.Lerp(p1, p2, i);
            Vector3 t2 = Vector3.Lerp(p2, p3, i);

            Vector3 curve = Vector3.Lerp(t1, t2, i);

            points.Add(curve);
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());

        targetObject.position = end;
    }

    public void SetColor(Gradient color){
        lineRenderer.colorGradient = color;
        targetObject.gameObject.SetActive(color == inactiveColor?false:true);
    }
}
