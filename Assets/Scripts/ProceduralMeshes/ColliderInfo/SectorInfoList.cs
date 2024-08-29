using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace ProceduralMeshes.ColliderInfo
{
    public class SectorInfoList
    {
        private readonly List<SectorInfo> sectorInfos = new List<SectorInfo>();

        public int VertexCount { get; private set; }
        public int TriangleCount { get; private set; }

        public SectorInfoList()
        {
            VertexCount = 0;
            TriangleCount = 0;
        }

        public void AddSector(float2 center, float radius, float startAngle, float endAngle)
        {
            var sector = new SectorInfo(center, radius, startAngle, endAngle);
            sectorInfos.Add(sector);
            VertexCount += sector.VertexCount;
            TriangleCount += sector.TriangleCount;
        }

        public int3 GetTriangle(int index)
        {
            var offset = 0;
            foreach (var sectorInfo in sectorInfos)
            {
                if (index < sectorInfo.TriangleCount)
                {
                    return sectorInfo.GetTriangle(index) + offset;
                }

                index -= sectorInfo.TriangleCount;
                offset += sectorInfo.VertexCount;
            }

            throw new Exception($"triangle index {index} out of range");
        }

        public float2 GetVertex(int index)
        {
            foreach (var sectorInfo in sectorInfos)
            {
                if (index < sectorInfo.VertexCount)
                {
                    return sectorInfo.GetVertex(index);
                }

                index -= sectorInfo.VertexCount;
            }

            throw new Exception($"vertex index {index} out of range");
        }
    }
}