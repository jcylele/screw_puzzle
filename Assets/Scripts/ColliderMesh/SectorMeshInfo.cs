using Unity.Mathematics;
using UnityEngine;

namespace ColliderMesh
{
    public class SectorMeshInfo
    {
        // simulate a whole circle with 32 segments  
        private const int CircleVertexCount = 32;

        private readonly float2 center;
        private readonly float radius;
        private readonly float endAngle;
        private readonly int segmentCount;
        private readonly float stepAngle;

        public int VertexCount => segmentCount + 1;

        public SectorMeshInfo(float2 center, float radius, float startAngle, float endAngle)
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

        public float2 GetVertex(int index)
        {
            var angel = endAngle - stepAngle * index;
            // Debug.Log($"index: {index}, angel: {angel}");
            return new float2(
                center.x + radius * math.cos(angel),
                center.y + radius * math.sin(angel)
            );
        }
    }
}