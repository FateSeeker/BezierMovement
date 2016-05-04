using UnityEngine;
using System.Collections;

public static class MathUtils {

    public static Vector3 BezierFuncParam(Vector3 start, Vector3 TangentA, Vector3 TangentB, Vector3 end, float t)
    {
        //t = Mathf.Clamp01 (t);
        Vector3 result = (1 - t) * (1 - t) * (1 - t) * start + 3 * (1 - t) * (1 - t) * t * TangentA + 3 * (1 - t) * t * t * TangentB
            + t * t * t * end;
        return result;
    }
    public static bool PointInsidePolygon(Vector3[] PolygonPoints,Vector3 Point)
    {

        bool result = false;
        for (int i = 0, j = PolygonPoints.Length - 1; i < PolygonPoints.Length; j = i++)
        {

            if ((((PolygonPoints[i].y <= Point.y) && (Point.y < PolygonPoints[j].y)) ||
                ((PolygonPoints[j].y <= Point.y) && (Point.y < PolygonPoints[i].y))) &&
                (Point.x < (PolygonPoints[j].x - PolygonPoints[i].x) * (Point.y - PolygonPoints[i].y) / (PolygonPoints[j].y - PolygonPoints[i].y) + PolygonPoints[i].x))
            {
                result = !result;
            }
        }

        return result;
    }
}
