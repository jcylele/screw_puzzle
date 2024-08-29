using System;
using Unity.Mathematics;
using UnityEngine;

namespace ProceduralMeshes.ColliderInfo
{
    public class CapsuleColliderInfo : BaseColliderInfo
    {
        public override int VertexCount => SimpleQuadVertexCount + sectorInfoList.VertexCount;
        public override int TriangleCount => SimpleQuadTriangleCount + sectorInfoList.TriangleCount;
        public override Bounds Bounds { get; }

        private readonly float2 min;
        private readonly float2 max;

        private readonly SectorInfoList sectorInfoList;

        public CapsuleColliderInfo(CapsuleCollider2D capsuleCollider2D) : base(capsuleCollider2D)
        {
            sectorInfoList = new SectorInfoList();

            var offset = capsuleCollider2D.offset;
            var size = capsuleCollider2D.size;
            // outer quad
            this.min = offset - 0.5f * size;
            this.max = offset + 0.5f * size;
            // a little larger than the actual bounds, but fine
            Bounds = this.CalcQuadBounds(min, max);

            switch (capsuleCollider2D.direction)
            {
                case CapsuleDirection2D.Vertical:
                {
                    var radius = size.x / 2f;
                    // adjust bounds to inner quad
                    min.y += radius;
                    max.y -= radius;

                    var topCenter = new float2(offset.x, max.y);
                    sectorInfoList.AddSector(topCenter, radius, 0f, 180f);

                    var bottomCenter = new float2(offset.x, min.y);
                    sectorInfoList.AddSector(bottomCenter, radius, 180f, 360f);

                    break;
                }
                case CapsuleDirection2D.Horizontal:
                {
                    var radius = size.y / 2f;
                    // adjust bounds to inner quad
                    min.x += radius;
                    max.x -= radius;

                    var leftCenter = new float2(min.x, offset.y);
                    sectorInfoList.AddSector(leftCenter, radius, 90f, 270f);

                    var rightCenter = new float2(max.x, offset.y);
                    sectorInfoList.AddSector(rightCenter, radius, 270f, 450f);

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override int3 GetTriangle(int index)
        {
            if (index < SimpleQuadTriangleCount) return SimpleIndices[index];
            return sectorInfoList.GetTriangle(index - SimpleQuadTriangleCount) + SimpleQuadVertexCount;
        }

        public override float2 GetVertex(int index)
        {
            return this.TransformPoint(InnerVertex(index));
        }

        private float2 InnerVertex(int index)
        {
            return index switch
            {
                0 => new float2(min.x, min.y),
                1 => new float2(max.x, min.y),
                2 => new float2(min.x, max.y),
                3 => new float2(max.x, max.y),
                _ => sectorInfoList.GetVertex(index - SimpleQuadVertexCount)
            };
        }
    }
}