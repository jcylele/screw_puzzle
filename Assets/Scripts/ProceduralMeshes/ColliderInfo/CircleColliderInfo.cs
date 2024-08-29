using Unity.Mathematics;
using UnityEngine;

namespace ProceduralMeshes.ColliderInfo
{
    public class CircleColliderInfo : BaseColliderInfo
    {
        public override int VertexCount => sectorInfoList.VertexCount;
        public override int TriangleCount => sectorInfoList.TriangleCount;
        public override Bounds Bounds { get; }

        private SectorInfoList sectorInfoList;

        public CircleColliderInfo(CircleCollider2D circleCollider2D) : base(circleCollider2D)
        {
            var center = circleCollider2D.offset;
            var radius = circleCollider2D.radius;

            sectorInfoList = new SectorInfoList();
            sectorInfoList.AddSector(center, radius, 0f, 360f);

            // Bounds
            var center2 = this.TransformPoint(center);
            Bounds = new Bounds(
                new Vector3(center2.x, center2.y, 0f),
                new Vector3(2f * radius, 2f * radius, 0f)
            );
        }

        public override int3 GetTriangle(int index)
        {
            return sectorInfoList.GetTriangle(index);
        }

        public override float2 GetVertex(int index)
        {
            return this.TransformPoint(sectorInfoList.GetVertex(index));
        }
    }
}