using System;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    [Serializable]
    public class TransInfo
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    [Serializable]
    public abstract class BaseColliderInfo
    {
        public TransInfo transInfo;
        public List<Vector3> jointPoints;
    }

    [Serializable]
    public class CapsuleColliderInfo : BaseColliderInfo
    {
        public Vector2 offset;
        public Vector2 size;
        public CapsuleDirection2D direction;
    }

    [Serializable]
    public class BoxColliderInfo : BaseColliderInfo
    {
        public Vector2 offset;
        public Vector2 size;
        public float edgeRadius;
    }

    [Serializable]
    public class CircleColliderInfo : BaseColliderInfo
    {
        public Vector2 offset;
        public float radius;
    }

    [CreateAssetMenu(fileName = "ItemInfo", menuName = "Screw/ItemInfo", order = 1)]
    public class ItemInfo : ScriptableObject
    {
        public List<CapsuleColliderInfo> capsuleColliders;
        public List<BoxColliderInfo> boxColliders;
        public List<CircleColliderInfo> circleColliders;
    }
}