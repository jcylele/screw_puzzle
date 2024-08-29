using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace ProceduralMeshes.ColliderInfo
{
    public abstract class BaseColliderInfo
    {
        // simple quad
        protected const int SimpleQuadVertexCount = 4;
        protected const int SimpleQuadTriangleCount = 2;
        // complex quad with edge radius
        protected const int ComplexQuadVertexCount = 12;
        protected const int ComplexQuadTriangleCount = 10;
        
        public abstract int VertexCount { get; }
        public abstract int TriangleCount { get; }
        public abstract Bounds Bounds { get; }
        public abstract int3 GetTriangle(int index);
        public abstract float2 GetVertex(int index);

        private readonly Matrix4x4 localMatrix;

        protected static readonly int3[] SimpleIndices =
        {
            int3(0, 2, 1),
            int3(1, 2, 3)
        };

        protected static readonly int3[] ComplexIndices =
        {
            int3(0, 2, 1),
            int3(1, 2, 3),
            int3(5, 0, 4),
            int3(4, 0, 1),
            int3(6, 7, 0),
            int3(0, 7, 2),
            int3(2, 8, 3),
            int3(3, 8, 9),
            int3(1, 3, 11),
            int3(11, 3, 10)
        };

        protected BaseColliderInfo(Collider2D collider2D)
        {
            var trans = collider2D.transform;
            this.localMatrix = Matrix4x4.TRS(
                trans.localPosition,
                trans.localRotation,
                trans.localScale
            );
        }

        protected float2 TransformPoint(float2 point)
        {
            return new float2(
                localMatrix.m00 * point.x + localMatrix.m01 * point.y + localMatrix.m03,
                localMatrix.m10 * point.x + localMatrix.m11 * point.y + localMatrix.m13
            );
        }

        protected Bounds CalcQuadBounds(float2 min, float2 max)
        {
            var min2 = this.TransformPoint(min);
            var max2 = this.TransformPoint(max);
            var mid2 = (min2 + max2) / 2f;
            var size2 = new float2(Mathf.Abs(max2.x - min2.x), Mathf.Abs(max2.y - min2.y));
            return new Bounds(
                new Vector3(mid2.x, mid2.y, 0f),
                new Vector3(size2.x, size2.y, 0f)
            );
        }
    }
}