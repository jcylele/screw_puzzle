using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace ColliderMesh
{
    public class CapsuleMeshInfo : BaseMeshInfo
    {
        public override int VertexCount { get; }

        private readonly float2 min;
        private readonly float2 max;

        private readonly List<SectorMeshInfo> sectorMeshInfos;

        public CapsuleMeshInfo(CapsuleCollider2D capsuleCollider2D) : base(capsuleCollider2D)
        {
            var offset = capsuleCollider2D.offset;
            var size = capsuleCollider2D.size;
            // outer quad
            this.min = offset - 0.5f * size;
            this.max = offset + 0.5f * size;

            switch (capsuleCollider2D.direction)
            {
                case CapsuleDirection2D.Vertical:
                {
                    var radius = size.x / 2f;
                    // adjust bounds to inner quad
                    min.y += radius;
                    max.y -= radius;

                    VertexCount = 0;
                    var topCircle = new SectorMeshInfo(new float2(offset.x, max.y), radius, 0f, 180f);
                    VertexCount += topCircle.VertexCount;
                    var bottomCircle = new SectorMeshInfo(new float2(offset.x, min.y), radius, 180f, 360f);
                    VertexCount += bottomCircle.VertexCount;
                    sectorMeshInfos = new List<SectorMeshInfo>(2)
                    {
                        topCircle,
                        bottomCircle
                    };
                    break;
                }
                case CapsuleDirection2D.Horizontal:
                {
                    var radius = size.y / 2f;
                    // adjust bounds to inner quad
                    min.x += radius;
                    max.x -= radius;

                    VertexCount = 0;
                    var leftCircle = new SectorMeshInfo(new float2(min.x, offset.y), radius, 90f, 270f);
                    VertexCount += leftCircle.VertexCount;
                    var rightCircle = new SectorMeshInfo(new float2(max.x, offset.y), radius, 270f, 90f);
                    VertexCount += rightCircle.VertexCount;
                    sectorMeshInfos = new List<SectorMeshInfo>(2)
                    {
                        leftCircle,
                        rightCircle
                    };

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        protected override float2 InnerGetVertex(int index)
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
    }
}