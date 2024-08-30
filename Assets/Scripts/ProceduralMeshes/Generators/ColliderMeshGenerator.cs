using ProceduralMeshes.ColliderInfo;
using ProceduralMeshes.Streams;
using static Unity.Mathematics.math;
using UnityEngine;

namespace ProceduralMeshes.Generators
{
    public class ColliderMeshGenerator
    {
        public int VertexCount => colliderInfo.VertexCount;
        public int IndexCount => colliderInfo.TriangleCount * 3;
        public Bounds Bounds => colliderInfo.Bounds;
        private BaseColliderInfo colliderInfo;

        public static BaseColliderInfo CreateColliderInfo(Collider2D collider2D)
        {
            return collider2D switch
            {
                BoxCollider2D boxCollider2D => new BoxColliderInfo(boxCollider2D),
                CircleCollider2D circleCollider2D => new CircleColliderInfo(circleCollider2D),
                CapsuleCollider2D capsuleCollider2D => new CapsuleColliderInfo(capsuleCollider2D),
                _ => throw new System.NotImplementedException()
            };
        }

        public void SetCollider(Collider2D collider2D)
        {
            colliderInfo = CreateColliderInfo(collider2D);
        }

        public void SetColliderInfo(BaseColliderInfo colliderInfo)
        {
            this.colliderInfo = colliderInfo;
        }

        public void GenerateTo(MultiStream streams)
        {
            var vertex = new Vertex();
            vertex.normal.z = -1f;
            vertex.tangent.yw = float2(1f, -1f);
            vertex.texCoord0 = float2(0f, 0f);

            for (var j = 0; j < colliderInfo.VertexCount; j++)
            {
                vertex.position.xy = colliderInfo.GetVertex(j);
                streams.SetVertex(j, vertex);
            }

            for (var j = 0; j < colliderInfo.TriangleCount; j++)
            {
                streams.SetTriangle(j, colliderInfo.GetTriangle(j));
            }
        }
    }
}