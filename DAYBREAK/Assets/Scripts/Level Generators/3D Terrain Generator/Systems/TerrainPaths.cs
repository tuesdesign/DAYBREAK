using System;
using UnityEngine;

public class TerrainPaths
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

    public static IntersectionData LineIntersection(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
    {
        // calculate the intersection point
        float alpha = ((v4.x - v3.x) * (v3.y - v1.y) - (v4.y - v3.y) * (v3.x - v1.x)) / ((v4.x - v3.x) * (v2.y - v1.y) - (v4.y - v3.y) * (v2.x - v1.x));
        float beta = ((v2.x - v1.x) * (v3.y - v1.y) - (v2.y - v1.y) * (v3.x - v1.x)) / ((v4.x - v3.x) * (v2.y - v1.y) - (v4.y - v3.y) * (v2.x - v1.x));

        // If both alpha and beta are between 0 and 1, the lines intersect
        bool intersects = (alpha >= 0 && alpha <= 1 && beta >= 0 && beta <= 1);

        // return the intersection point
        if (intersects) return new IntersectionData(true, Vector3.Lerp(v1, v2, alpha), new Vector2(v4.y - v3.y, -(v4.x - v3.x)).normalized, new Path(), 0f);
        else return new IntersectionData(false, Vector3.zero, Vector3.zero, new Path(), 0f);
    }

    public static IntersectionData PathToLineIntersection(Path path, Vector2 v1, Vector2 v2, float detail)
    {
        // get the start point of the first line section of the path
        Vector2 v3 = path.Start;

        // check if the path intersects the vector
        for (float t = detail; t <= 1; t += detail)
        {
            t = Mathf.Clamp(t, 0f, 1f);

            // get the next point on the path
            Vector2 v4 = path.Spline(t);

            // return true if the path intersects the line
            IntersectionData intersectionData = LineIntersection(v1, v2, v3, v4);
            if (intersectionData.intersects) return intersectionData;

            // set the start point of the next line to the end point of the current line
            v3 = v4;
        }

        // return false if the paths don't intersect
        return new IntersectionData(false, Vector3.zero, Vector3.zero, new Path(), 0f);
    }

    public static IntersectionData PathToPathIntersection(Path p1, Path p2, float detail)
    {
        // get the start point of the first line section of the path
        Vector2 v1 = p1.Start;

        // check if the path intersects the vector
        for (float t = detail; t <= 1; t += detail)
        {
            t = Mathf.Clamp(t, 0f, 1f);

            // get the next point on the path
            Vector2 v2 = p1.Spline(t);

            // return true if the path intersects the line
            IntersectionData intersectionData = PathToLineIntersection(p2, v1, v2, detail);
            if (intersectionData.intersects) 
            {
                intersectionData.path = p2;
                intersectionData.t = t;
                return intersectionData;
            }

            // set the start point of the next line to the end point of the current line
            v1 = v2;
        }

        // return false if the paths don't intersect
        return new IntersectionData(false, Vector3.zero, Vector3.zero, new Path(), 0f);
    }

    public static IntersectionData PathToStructureIntersection(Path path, Vector3 structurePos, float strucureRadius, float detail)
    {
        // get the start point of the first line section of the path
        Vector2 v1 = path.Start;

        if (Vector3.Distance(v1, structurePos) - strucureRadius < 0)
        {
            Debug.LogWarning("Path intersects structure at start point!");
            return new IntersectionData(true, v1, Vector3.zero, new Path(), 0f);
        }

        // check if the path intersects the vector
        for (float t = detail; t <= 1; t += detail)
        {
            t = Mathf.Clamp(t, 0f, 1f);

            // get the next point on the path
            Vector2 v2 = path.Spline(t);

            // return true if the path intersects the line
            if (Vector3.Distance(v2, structurePos) - strucureRadius < 0) 
                return new IntersectionData(true, v1, (v2 - v1).normalized, new Path(), t);

            // set the start point of the next line to the end point of the current line
            v1 = v2;
        }
        // return false if the paths don't intersect
        return new IntersectionData(false, Vector3.zero, Vector3.zero, new Path(), 0f);
    }

    public struct IntersectionData
    {
        public bool intersects;
        public Vector3 intersection;
        public Vector3 normal;
        public Path path;
        public float t;

        public IntersectionData(bool intersects, Vector3 intersection, Vector3 normal, Path path, float t)
        {
            this.intersects = intersects;
            this.intersection = intersection;
            this.normal = normal;
            this.path = path;
            this.t = t;
        }
    }

    [Serializable]
    public struct Path
    {
        // The first and last points are control points, the rest are the actual path points
        // Start and End are the second and second last points respectively. They denote where the path begins and ends.
        // points the first and last points are control points that rest at the center of suructures or beyond the end of a path to control shape.

        public Vector3[] points;
        public TG_PathDataObject originData;
        public TG_PathDataObject destinationData;

        public Vector3 StartControl 
        {
            get => points[0];
            set => points[0] = value;
        }

        public Vector3 Start
        {
            get => points[1];
            set => points[1] = value;
        }

        public Vector3 End
        {
            get => points[points.Length - 2];
            set => points[points.Length - 2] = value;
        }

        public Vector3 EndControl
        {
            get => points[points.Length - 1];
            set => points[points.Length - 1] = value;
        }

        public Vector3 this[int index]
        {
            get => points[index];
            set => points[index] = value;
        }

        public Path(Vector3[] points)
        {
            this.points = points;
            if (points.Length < 4) Debug.LogWarning("Path must have at least 4 points to be a valid spline.");

            this.originData = null;
            this.destinationData = null;
        }

        public Path(Vector3[] points, TG_PathDataObject originData, TG_PathDataObject destinationData)
        {
            this.points = points;
            if (points.Length < 4) Debug.LogWarning("Path must have at least 4 points to be a valid spline.");

            this.originData = originData;
            this.destinationData = destinationData;
        }

        public void InsertPoint(Vector3 point, int index)
        {
            Vector3[] newPoints = new Vector3[points.Length + 1];
            for (int i = 0; i < newPoints.Length; i++)
            {
                if (i < index) newPoints[i] = points[i];
                else if (i == index) newPoints[i] = point;
                else newPoints[i] = points[i - 1];
            }

            if (newPoints.Length < 4) Debug.LogWarning("Path must have at least 4 points to be a valid spline.");

            points = newPoints;
        }

        public void RemovePoint(int index)
        {
            Vector3[] newPoints = new Vector3[points.Length - 1];
            for (int i = 0; i < newPoints.Length; i++)
            {
                if (i < index) newPoints[i] = points[i];
                else newPoints[i] = points[i + 1];
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
