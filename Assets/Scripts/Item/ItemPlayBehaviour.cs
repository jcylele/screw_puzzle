using UnityEngine;

namespace Item
{
    public class ItemPlayBehaviour : BaseItemBehaviour
    {
        protected override string BoxColliderPrefabPath => Consts.BoxColliderPlay;
        protected override string CapsuleColliderPrefabPath => Consts.CapsuleColliderPlay;
        protected override string CircleColliderPrefabPath => Consts.CircleColliderPlay;

        private void Start()
        {
            ExpandItem();
        }

        protected override void AddJoint(Vector3 jointPosition, Transform colliderTrans)
        {
            var joint2D = gameObject.AddComponent<HingeJoint2D>();
            if (joint2D == null) return;

            joint2D.anchor = jointPosition;
        }

        public HingeJoint2D GetJointByHit(Vector2 hitWorldPos)
        {
            // TODO maybe should be cached, but deletion will be more complex
            var joints = GetComponents<HingeJoint2D>();
            foreach (var joint in joints)
            {
                var distance = Vector2.Distance(joint.connectedAnchor, hitWorldPos);
                if (distance <= Consts.JointRadius)
                {
                    return joint;
                }
            }

            return null;
        }

        public void OnFallToGround()
        {
            Destroy(gameObject);
        }

        public void OnJointClick(HingeJoint2D joint)
        {
            Destroy(joint);
            Game.Instance.OnScrewDrop(ScrewColor.None);
        }
    }
}