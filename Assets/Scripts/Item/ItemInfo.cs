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

    [Serializable]
    public class PolygonColliderInfo : BaseColliderInfo
    {
        public Vector2 offset;
        public Vector2[] path;
    }

    [Serializable]
    public class BezierColliderInfo : BaseColliderInfo
    {
        public Vector3[] points;
        public int steps;
    }

    [CreateAssetMenu(fileName = "ItemInfo", menuName = "Screw/ItemInfo", order = 1)]
    public class ItemInfo : ScriptableObject
    {
        public List<CapsuleColliderInfo> capsuleColliders;
        public List<BoxColliderInfo> boxColliders;
        public List<CircleColliderInfo> circleColliders;

        public List<PolygonColliderInfo> polygonColliders;
        public List<BezierColliderInfo> bezierColliders;

        public int GetTotalJointCount()
        {
            var totalJointPointCount = 0;
            foreach (var colliderInfo in capsuleColliders)
            {
                totalJointPointCount += colliderInfo.jointPoints.Count;
            }

            foreach (var colliderInfo in boxColliders)
            {
                totalJointPointCount += colliderInfo.jointPoints.Count;
            }

            foreach (var colliderInfo in circleColliders)
            {
                totalJointPointCount += colliderInfo.jointPoints.Count;
            }

            foreach (var colliderInfo in polygonColliders)
            {
                totalJointPointCount += colliderInfo.jointPoints.Count;
            }

            foreach (var colliderInfo in bezierColliders)
            {
                totalJointPointCount += colliderInfo.jointPoints.Count;
            }

            return totalJointPointCount;
        }
    }
}