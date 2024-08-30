using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace ColliderMesh
{
    public class BoxMeshInfo : BaseMeshInfo
    {
        // a full circle, last vertex is the same as the first vertex
        public override int VertexCount { get; }

        private readonly List<SectorMeshInfo> sectorMeshInfos;
        private readonly float2 min;
        private readonly float2 max;

        public BoxMeshInfo(BoxCollider2D boxCollider2D) : base(boxCollider2D)
        {
            var offset = boxCollider2D.offset;
            var size = boxCollider2D.size;
            min = offset - 0.5f * size;
            max = offset + 0.5f * size;
            var radius = boxCollider2D.edgeRadius;
            if (radius > 0)
            {
                sectorMeshInfos = new List<SectorMeshInfo>(4)
                {
                    // clockwise
                    new SectorMeshInfo(new float2(max.x, min.y), radius, 270f, 360f),
                    new SectorMeshInfo(new float2(min.x, min.y), radius, 180f, 270f),
                    new SectorMeshInfo(new float2(min.x, max.y), radius, 90f, 180f),
                    new SectorMeshInfo(new float2(max.x, max.y), radius, 0f, 90f),
                };
                VertexCount = 0;
                foreach (var sectorMeshInfo in sectorMeshInfos)
                {
                    VertexCount += sectorMeshInfo.VertexCount;
                }
            }
            else
            {
                VertexCount = 4;
            }
        }

        protected override float2 InnerGetVertex(int index)
        {
            if (sectorMeshInfos != null)
            {
                foreach (var sectorMeshInfo in sectorMeshInfos)
                {
                    if (index < sectorMeshInfo.VertexCount)
                    {
                        return sectorMeshInfo.GetVertex(index);
                    }

                    index -= sectorMeshInfo.VertexCount;
                }

                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }

            return index switch
            {
                0 => new float2(min.x, min.y),
                1 => new float2(min.x, max.y),
                2 => new float2(max.x, max.y),
                3 => new float2(max.x, min.y),
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };
        }
    }
}