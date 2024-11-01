using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;

public static class TerrainPaths
{
    public static Vector3 Spline(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float x = Float(p0.x, p1.x, p2.x, p3.x, t);
        float y = Float(p0.y, p1.y, p2.y, p3.y, t);
        float z = Float(p0.z, p1.z, p2.z, p3.z, t);

        return new Vector3(x, y, z);
    }

    public static Vector3 Spline(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float x = Float(p0.x, p1.x, p2.x, p3.x, t);
        float y = Float(p0.y, p1.y, p2.y, p3.y, t);

        return new Vector2(x, y);
    }

    public static float Float(float p0, float p1, float p2, float p3, float t)
    {
        return 0.5f * (2.0f * p1 + t * (-p0 + p2) + t * t * (2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3) + t * t * t * (-p0 + 3.0f * p1 - 3.0f * p2 + p3));
    }

    [Serializable]
    public struct Path
    {
        public Vector3[] points;
        
        public Vector3 Start => points[1];

        public Vector3 End => points[points.Length - 2];

        public Vector3 this[int index]
        {
            get => points[index];
            set => points[index] = value;
        }

        public Path(Vector3[] points)
        {
            this.points = points;
            if (points.Length < 4) Debug.LogWarning("Path must have at least 4 points to be a valid spline.");
        }

        public void InsertPoint(Vector3 point, int index)
        {
            Vector3[] newPoints = new Vector3[points.Length + 1];
            for (int i = 0; i < newPoints.Length; i++)
            {
                if (i < index)
                    newPoints[i] = points[i];
                else if (i == index)
                    newPoints[i] = point;
                else
                    newPoints[i] = points[i - 1];
            }

            if (newPoints.Length < 4) Debug.LogWarning("Path must have at least 4 points to be a valid spline.");

            points = newPoints;
        }

        public void RemovePoint(int index)
        {
            Vector3[] newPoints = new Vector3[points.Length - 1];
            for (int i = 0; i < newPoints.Length; i++)
            {
                if (i < index)
                    newPoints[i] = points[i];
                else
                    newPoints[i] = points[i + 1];
            }

            if (newPoints.Length < 4) Debug.LogWarning("Path must have at least 4 points to be a valid spline.");

            points = newPoints;
        }

        public Vector3 Spline(float t)
        {
            int splines = points.Length - 3;
            float splineT = t * splines;
            int splineIndex;

            if (t >= 1)
            {
                splineIndex = splines - 1;
                splineT = 1;
            }
            else
            {
                splineIndex = Mathf.FloorToInt(splineT);
                splineT = Mathf.Repeat(splineT, 1);
            }

            Vector3 p0 = points[splineIndex];
            Vector3 p1 = points[splineIndex + 1];
            Vector3 p2 = points[splineIndex + 2];
            Vector3 p3 = points[splineIndex + 3];

            return TerrainPaths.Spline(p0, p1, p2, p3, splineT);
        }
    }
}
