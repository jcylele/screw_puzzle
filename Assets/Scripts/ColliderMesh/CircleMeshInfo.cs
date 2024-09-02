using UnityEngine;

namespace ColliderMesh
{
    public class CircleMeshInfo : BaseMeshInfo
    {
        // a full circle, last vertex is the same as the first vertex
        public override int VertexCount => sectorMeshInfo.VertexCount - 1;

        private readonly SectorMeshInfo sectorMeshInfo;

        public CircleMeshInfo(CircleCollider2D circleCollider2D) : base(circleCollider2D)
        {
            var center = circleCollider2D.offset;
            var radius = circleCollider2D.radius;

            sectorMeshInfo = new SectorMeshInfo(center, radius, 0f, 360f);
        }

        protected override Vector2 InnerGetVertex(int index)
        {
            return sectorMeshInfo.GetVertex(index);
        }
    }
}