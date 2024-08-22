namespace Item
{
    public class ColliderEditBehaviour : BaseBehaviour
    {
        public JointEditBehaviour AddJoint()
        {
            var jointPrefab = LoadComponent<JointEditBehaviour>(Consts.JointEdit);
            var joint = Instantiate(jointPrefab, transform);
            return joint;
        }

        public void ClearJoints()
        {
            foreach (var joint in GetComponentsInChildren<JointEditBehaviour>())
            {
                Destroy(joint.gameObject);
            }
        }
    }
}