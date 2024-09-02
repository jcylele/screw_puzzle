using UnityEngine;

namespace ColliderMesh
{
    public class PolygonMeshInfo : BaseMeshInfo
    {
        private readonly Vector2[] path;
        private Vector2 center;

        public PolygonMeshInfo(PolygonCollider2D polygonCollider2D) : base(polygonCollider2D)
        {
            if (polygonCollider2D.pathCount == 0)
            {
                throw new System.ArgumentException("PolygonCollider2D has no path");
            }

            if (polygonCollider2D.pathCount > 1)
            {
                throw new System.ArgumentException("PolygonCollider2D should have only one path");
            }

            center = polygonCollider2D.offset;

            for (int i = 0; i < polygonCollider2D.pathCount; i++)
            {
                path = polygonCollider2D.GetPath(0);
            }
        }

        public override int VertexCount => path.Length;

        protected override Vector2 InnerGetVertex(int index)
        {
            return path[index] + center;
        }
    }
}