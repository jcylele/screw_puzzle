using System.Runtime.CompilerServices;
using ProceduralMeshes.ColliderInfo;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace ProceduralMeshes.Streams
{
    public struct MultiStream
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float3> stream0, stream1;

        [NativeDisableContainerSafetyRestriction]
        NativeArray<float4> stream2;

        [NativeDisableContainerSafetyRestriction]
        NativeArray<float2> stream3;

        [NativeDisableContainerSafetyRestriction]
        NativeArray<TriangleUInt16> triangles;

        private int triangleOffset, vertexOffset;

        public void Setup(Mesh.MeshData meshData, int subMeshCount, int vertexCount, int indexCount)
        {
            var descriptor = new NativeArray<VertexAttributeDescriptor>(
                4, Allocator.Temp, NativeArrayOptions.UninitializedMemory
            );
            descriptor[0] = new VertexAttributeDescriptor(dimension: 3);
            descriptor[1] = new VertexAttributeDescriptor(
                VertexAttribute.Normal, dimension: 3, stream: 1
            );
            descriptor[2] = new VertexAttributeDescriptor(
                VertexAttribute.Tangent, dimension: 4, stream: 2
            );
            descriptor[3] = new VertexAttributeDescriptor(
                VertexAttribute.TexCoord0, dimension: 2, stream: 3
            );
            meshData.SetVertexBufferParams(vertexCount, descriptor);
            descriptor.Dispose();

            meshData.SetIndexBufferParams(indexCount, IndexFormat.UInt16);

            meshData.subMeshCount = subMeshCount;

            stream0 = meshData.GetVertexData<float3>();
            stream1 = meshData.GetVertexData<float3>(1);
            stream2 = meshData.GetVertexData<float4>(2);
            stream3 = meshData.GetVertexData<float2>(3);
            triangles = meshData.GetIndexData<ushort>().Reinterpret<TriangleUInt16>(2);

            vertexOffset = 0;
            triangleOffset = 0;
        }

        public void SetupSubMesh(Mesh.MeshData meshData, int subMeshIndex,
            BaseColliderInfo info, int vertexStart, int triangleStart)
        {
            meshData.SetSubMesh(
                subMeshIndex, new SubMeshDescriptor(triangleStart * 3, info.TriangleCount * 3)
                {
                    bounds = info.Bounds,
                    vertexCount = info.VertexCount
                },
                MeshUpdateFlags.DontRecalculateBounds |
                MeshUpdateFlags.DontValidateIndices
            );
            this.vertexOffset = vertexStart;
            this.triangleOffset = triangleStart;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertex(int index, Vertex vertex)
        {
            index += vertexOffset;
            stream0[index] = vertex.position;
            stream1[index] = vertex.normal;
            stream2[index] = vertex.tangent;
            stream3[index] = vertex.texCoord0;
        }

        public void SetTriangle(int index, int3 triangle) =>
            triangles[index + triangleOffset] = triangle + vertexOffset;
    }
}