using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    public class ItemEditBehaviour : BaseItemBehaviour
    {
        protected override string BoxColliderPrefabPath => Consts.BoxColliderEdit;
        protected override string CapsuleColliderPrefabPath => Consts.CapsuleColliderEdit;
        protected override string CircleColliderPrefabPath => Consts.CircleColliderEdit;

        protected override void AddJoint(Vector3 jointPosition, Transform colliderTrans)
        {
            var jointPrefab = LoadComponent<JointEditBehaviour>(Consts.JointEdit);
            var joint = Instantiate(jointPrefab, colliderTrans);
            joint.transform.position = jointPosition;
        }

        private void Awake()
        {
            // Debug.LogError("ItemEditBehaviour should not run in play mode");
            this.gameObject.SetActive(false);
        }

        private T AddCollider<T>(string path) where T : Collider2D
        {
            var prefab = LoadComponent<T>(path);
            return Instantiate(prefab, transform);
        }

        public CapsuleCollider2D AddCapsuleCollider()
        {
            return AddCollider<CapsuleCollider2D>(CapsuleColliderPrefabPath);
        }

        public BoxCollider2D AddBoxCollider()
        {
            return AddCollider<BoxCollider2D>(BoxColliderPrefabPath);
        }

        public CircleCollider2D AddCircleCollider()
        {
            return AddCollider<CircleCollider2D>(CircleColliderPrefabPath);
        }

        public void ClearItem()
        {
            // Destroy all the colliders
            foreach (var col in GetComponentsInChildren<Collider2D>(true))
            {
                DestroyImmediate(col.gameObject);
            }
        }

#if UNITY_EDITOR

        public void SerializeItem()
        {
            if (itemInfo == null)
            {
                Debug.LogError("ItemInfo is null");
                return;
            }

            if (transform.childCount == 0)
            {
                Debug.LogError("no collider found in the item, nothing to serialize");
                return;
            }

            this.ResetTransform();

            // Set collider info of the itemInfo
            itemInfo.capsuleColliders = GetCapsuleColliders();
            itemInfo.boxColliders = GetBoxColliders();
            itemInfo.circleColliders = GetCircleColliders();

            UnityEditor.EditorUtility.SetDirty(itemInfo);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }

        private List<Vector3> GetJointPoints(Component component)
        {
            var jointPoints = new List<Vector3>();
            var joints = component.GetComponentsInChildren<JointEditBehaviour>(false);
            foreach (var joint in joints)
            {
                jointPoints.Add(joint.transform.position);
            }

            return jointPoints;
        }

        private List<CapsuleColliderInfo> GetCapsuleColliders()
        {
            var caps = GetComponentsInChildren<CapsuleCollider2D>(false);
            var capsuleColliderInfos = new List<CapsuleColliderInfo>(caps.Length);
            foreach (var cap in caps)
            {
                var capsuleColliderInfo = new CapsuleColliderInfo
                {
                    transInfo = GetTransInfo(true, cap.transform),
                    jointPoints = GetJointPoints(cap),
                    offset = cap.offset,
                    size = cap.size,
                    direction = cap.direction
                };
                capsuleColliderInfos.Add(capsuleColliderInfo);
            }

            return capsuleColliderInfos;
        }

        private List<BoxColliderInfo> GetBoxColliders()
        {
            var boxes = GetComponentsInChildren<BoxCollider2D>(false);
            var boxColliderInfos = new List<BoxColliderInfo>(boxes.Length);
            foreach (var box in boxes)
            {
                var boxColliderInfo = new BoxColliderInfo
                {
                    transInfo = GetTransInfo(true, box.transform),
                    jointPoints = GetJointPoints(box),
                    offset = box.offset,
                    size = box.size,
                    edgeRadius = box.edgeRadius
                };
                boxColliderInfos.Add(boxColliderInfo);
            }

            return boxColliderInfos;
        }

        private List<CircleColliderInfo> GetCircleColliders()
        {
            var circles = GetComponentsInChildren<CircleCollider2D>(false);
            var circleColliderInfos = new List<CircleColliderInfo>(circles.Length);

            foreach (var circle in circles)
            {
                var circleColliderInfo = new CircleColliderInfo
                {
                    transInfo = GetTransInfo(true, circle.transform),
                    jointPoints = GetJointPoints(circle),
                    offset = circle.offset,
                    radius = circle.radius,
                };
                circleColliderInfos.Add(circleColliderInfo);
            }

            return circleColliderInfos;
        }

#endif
    }
}