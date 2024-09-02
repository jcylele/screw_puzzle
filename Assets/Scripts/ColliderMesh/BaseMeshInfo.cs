using Unity.Mathematics;
using UnityEngine;

namespace ColliderMesh
{
    public abstract class BaseMeshInfo
    {
        public abstract int VertexCount { get; }

        protected abstract Vector2 InnerGetVertex(int index);

        private readonly Matrix4x4 localMatrix;

        protected BaseMeshInfo(Collider2D collider2D)
        {
            var trans = collider2D.transform;
            this.localMatrix = Matrix4x4.TRS(
                trans.localPosition,
                trans.localRotation,
                trans.localScale
            );
        }

        public Vector2 GetVertex(int index)
        {
            return this.TransformPoint(this.InnerGetVertex(index));
        }

        private float2 TransformPoint(float2 point)
        {
            return new float2(
                localMatrix.m00 * point.x + localMatrix.m01 * point.y + localMatrix.m03,
                localMatrix.m10 * point.x + localMatrix.m11 * point.y + localMatrix.m13
            );
        }
    }
}