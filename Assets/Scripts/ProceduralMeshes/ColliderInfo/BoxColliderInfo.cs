using Unity.Mathematics;
using UnityEngine;
using int3 = Unity.Mathematics.int3;

namespace ProceduralMeshes.ColliderInfo
{
    public class BoxColliderInfo : BaseColliderInfo
    {
        public override int VertexCount { get; }
        public override int TriangleCount { get; }
        public override Bounds Bounds { get; }

        private readonly float2 min;
        private readonly float2 max;
        private readonly float radius;

        // point to the indices array to use
        private readonly int3[] indices;

        private readonly SectorInfoList sectorInfoList;

        public BoxColliderInfo(BoxCollider2D boxCollider2D) : base(boxCollider2D)
        {
            sectorInfoList = new SectorInfoList();

            var offset = boxCollider2D.offset;
            var size = boxCollider2D.size;
            min = offset - 0.5f * size;
            max = offset + 0.5f * size;
            radius = boxCollider2D.edgeRadius;
            if (radius > 0)
            {
                indices = ComplexIndices;

                sectorInfoList.AddSector(new float2(max.x, max.y), radius, 0f, 90f);
                sectorInfoList.AddSector(new float2(min.x, max.y), radius, 90f, 180f);
                sectorInfoList.AddSector(new float2(min.x, min.y), radius, 180f, 270f);
                sectorInfoList.AddSector(new float2(max.x, min.y), radius, 270f, 360f);

                VertexCount = ComplexQuadVertexCount + sectorInfoList.VertexCount;
                TriangleCount = ComplexQuadTriangleCount + sectorInfoList.TriangleCount;
            }
            else
            {
                indices = SimpleIndices;

                VertexCount = SimpleQuadVertexCount;
                TriangleCount = SimpleQuadTriangleCount;
            }

            // Bounds
            var bounds = this.CalcQuadBounds(min, max);
            bounds.size += new Vector3(2 * radius, 2 * radius, 0f);
            Bounds = bounds;
        }

        public override int3 GetTriangle(int index)
        {
            // quad triangles
            if (index < ComplexQuadTriangleCount) return indices[index];
            // sector triangles
            return sectorInfoList.GetTriangle(index - ComplexQuadTriangleCount) + ComplexQuadVertexCount;
        }

        public override float2 GetVertex(int index)
        {
            return this.TransformPoint(InnerGetVertex(index));
        }

        private float2 InnerGetVertex(int index)
        {
            return index switch
            {
                0 => new float2(min.x, min.y),
                1 => new float2(max.x, min.y),
                2 => new float2(min.x, max.y),
                3 => new float2(max.x, max.y),
                4 => new float2(max.x, min.y - radius),
                5 => new float2(min.x, min.y - radius),
                6 => new float2(min.x - radius, min.y),
                7 => new float2(min.x - radius, max.y),
                8 => new float2(min.x, max.y + radius),
                9 => new float2(max.x, max.y + radius),
                10 => new float2(max.x + radius, max.y),
                11 => new float2(max.x + radius, min.y),
                _ => sectorInfoList.GetVertex(index - ComplexQuadVertexCount)
            };
        }
    }
}