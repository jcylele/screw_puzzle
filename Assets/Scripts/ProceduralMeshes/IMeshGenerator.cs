﻿using UnityEngine;

namespace ProceduralMeshes
{
    public interface IMeshGenerator
    {
        void Execute<S>(int i, S streams) where S : struct, IMeshStreams;
        int VertexCount { get; }
        int IndexCount { get; }
        int JobLength { get; }
        Bounds Bounds { get; }
        void Initialize(Collider2D collider2D);
    }
}