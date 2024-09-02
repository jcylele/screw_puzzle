using UnityEngine;

namespace Item
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class BezierCurveBehaviour : BaseBehaviour
    {
        public int steps = 10;

        /// <summary>
        /// start, control, control, end
        /// <para>should be clockwise</para>
        /// </summary>
        public Vector3[] points = new Vector3[4]
        {
            new Vector3(0.5f, 0.5f, 0f),
            new Vector3(0.5f, -0.5f, 0f),
            new Vector3(-0.5f, -0.5f, 0f),
            new Vector3(-0.5f, 0.5f, 0f),
        };

        private Vector3 GetPoint(float t)
        {
            return Bezier.GetPoint(points[0], points[1], points[2], points[3], t);
        }

        public void UpdateCollider()
        {
            var polygonCollider2D = GetComponent<PolygonCollider2D>();
            polygonCollider2D.pathCount = 1;
            Vector2[] path = new Vector2[steps + 1];
            for (int i = 0; i <= steps; i++)
            {
                float t = i / (float)steps;
                path[i] = GetPoint(t);
            }

            polygonCollider2D.SetPath(0, path);
        }
    }

    public static class Bezier
    {
        /// <summary>
        /// B(t) = (1 - t)3 P0 + 3 (1 - t)2 t P1 + 3 (1 - t) t2 P2 + t3 P3
        /// </summary>
        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * oneMinusT * p0 +
                3f * oneMinusT * oneMinusT * t * p1 +
                3f * oneMinusT * t * t * p2 +
                t * t * t * p3;
        }

        /// <summary>
        /// B'(t) = 3 (1 - t)2 (P1 - P0) + 6 (1 - t) t (P2 - P1) + 3 t2 (P3 - P2).
        /// </summary>
        public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                3f * oneMinusT * oneMinusT * (p1 - p0) +
                6f * oneMinusT * t * (p2 - p1) +
                3f * t * t * (p3 - p2);
        }
    }
}