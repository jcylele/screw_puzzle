using Layer;
using UnityEngine;

namespace Item
{
    public abstract class BaseItemBehaviour : BaseBehaviour
    {
        public ItemInfo itemInfo;
        protected abstract string BoxColliderPrefabPath { get; }
        protected abstract string CapsuleColliderPrefabPath { get; }
        protected abstract string CircleColliderPrefabPath { get; }
        public LayerPlayBehaviour BelongLayerBehaviour { get; set; }
        public int LayerIndex => BelongLayerBehaviour.LayerInfo.layerIndex;

        public void ExpandItem()
        {
            if (itemInfo == null)
            {
                Debug.LogError("ItemInfo is null");
                return;
            }

            if (this.transform.childCount > 0)
            {
                Debug.LogError("Item has been expanded, please clear it first.");
                return;
            }

            var itemLayer = Game.Instance.GetLayerValue(LayerIndex, false);
            this.gameObject.layer = itemLayer;
            this.gameObject.name = itemInfo.name;

            // Instantiate colliders
            var capsulePrefab = LoadComponent<CapsuleCollider2D>(CapsuleColliderPrefabPath);
            foreach (var capsuleColliderInfo in itemInfo.capsuleColliders)
            {
                var capsule = Instantiate(capsulePrefab, transform);
                capsule.gameObject.layer = itemLayer;
                // set properties of capsule collider
                this.SetCapsuleColliderProperties(capsule, capsuleColliderInfo);
                // extra process
                this.ExtraProcessCollider(capsuleColliderInfo, capsule.transform);
            }

            var boxPrefab = LoadComponent<BoxCollider2D>(BoxColliderPrefabPath);
            foreach (var boxColliderInfo in itemInfo.boxColliders)
            {
                var box = Instantiate(boxPrefab, transform);
                box.gameObject.layer = itemLayer;
                // set properties of capsule collider
                this.SetBoxColliderProperties(box, boxColliderInfo);
                // extra process
                this.ExtraProcessCollider(boxColliderInfo, box.transform);
            }

            var circlePrefab = LoadComponent<CircleCollider2D>(CircleColliderPrefabPath);
            foreach (var circleColliderInfo in itemInfo.circleColliders)
            {
                var circle = Instantiate(circlePrefab, transform);
                circle.gameObject.layer = itemLayer;
                // set properties of capsule collider
                this.SetCircleColliderProperties(circle, circleColliderInfo);
                // extra process
                this.ExtraProcessCollider(circleColliderInfo, circle.transform);
            }
        }

        protected abstract void AddJoint(Vector3 jointPosition, Transform colliderTrans);

        private void ExtraProcessCollider(BaseColliderInfo colliderInfo, Transform colliderTrans)
        {
            this.SetTransInfo(colliderInfo.transInfo, true, colliderTrans);
            foreach (var jointPoint in colliderInfo.jointPoints)
            {
                AddJoint(jointPoint, colliderTrans);
            }
        }

        private void SetCapsuleColliderProperties(CapsuleCollider2D capsuleCollider,
            CapsuleColliderInfo capsuleColliderInfo)
        {
            capsuleCollider.offset = capsuleColliderInfo.offset;
            capsuleCollider.size = capsuleColliderInfo.size;
            capsuleCollider.direction = capsuleColliderInfo.direction;
        }

        private void SetBoxColliderProperties(BoxCollider2D boxCollider,
            BoxColliderInfo boxColliderInfo)
        {
            boxCollider.offset = boxColliderInfo.offset;
            boxCollider.size = boxColliderInfo.size;
            boxCollider.edgeRadius = boxColliderInfo.edgeRadius;
        }

        private void SetCircleColliderProperties(CircleCollider2D circleCollider, CircleColliderInfo circleColliderInfo)
        {
            circleCollider.offset = circleColliderInfo.offset;
            circleCollider.radius = circleColliderInfo.radius;
        }
    }
}