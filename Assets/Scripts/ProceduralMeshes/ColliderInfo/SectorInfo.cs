using Unity.Mathematics;
using UnityEngine;

namespace ProceduralMeshes.ColliderInfo
{
    public class SectorInfo
    {
        // simulate a whole circle with 32 segments  
        private const int CircleVertexCount = 32;

        private readonly float2 center;
        private readonly float radius;
        private readonly float endAngle;
        private readonly int segmentCount;
        private readonly float stepAngle;

        public int VertexCount => segmentCount + 2;
        public int TriangleCount => segmentCount;

        public SectorInfo(float2 center, float radius, float startAngle, float endAngle)
        {
            var deltaAngle = endAngle - startAngle;
            if (deltaAngle < 0f || deltaAngle > 360f)
            {
                Debug.LogError($"Invalid angle range {startAngle} - {endAngle} for SectorInfo");
                return;
            }

            this.center = center;
            this.radius = radius;
            this.endAngle = endAngle;

            this.segmentCount = (int)math.ceil(CircleVertexCount * deltaAngle / 360f);
            this.stepAngle = (deltaAngle / this.segmentCount) * Mathf.Deg2Rad;
            this.endAngle *= Mathf.Deg2Rad;
        }

        public int3 GetTriangle(int index)
        {
            return new int3(0, index + 1, index + 2);
        }

        public float2 GetVertex(int index)
        {
            if (index == 0)
            {
                return center;
            }

            var angel = endAngle - stepAngle * (index - 1);
            // Debug.Log($"index: {index}, angel: {angel}");
            return new float2(
                center.x + radius * math.cos(angel),
                center.y + radius * math.sin(angel)
            );
        }
    }
}