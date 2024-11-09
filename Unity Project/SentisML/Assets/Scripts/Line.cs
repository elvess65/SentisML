using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public LineRenderer LineRenderer;

    List<Vector2> points;

    public void UpdateLine(Vector2 pos)
    {
        points ??= new();
        
        if (points.Count == 0)
        {
            SetPoint(pos);
            return;
        }

        if (Vector2.Distance(points[^1], pos) > 0.1f)
        {
            SetPoint(pos);
        }

    }

    void SetPoint(Vector2 point)
    {
        points.Add(point);

        LineRenderer.positionCount = points.Count;
        LineRenderer.SetPosition(points.Count - 1, point);
    }
}
