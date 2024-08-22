using Item;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class JointGizmoDrawer
    {
        [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.InSelectionHierarchy)]
        static void DrawJointGizmo(JointEditBehaviour joint, GizmoType gizmoType)
        {
            if (joint == null) return;

            var jointWorldPos = joint.transform.position;

            Handles.color = (gizmoType & GizmoType.InSelectionHierarchy) != 0 ? Color.cyan : Color.blue;
            Handles.DrawWireDisc(jointWorldPos, Vector3.forward, Consts.JointRadius);
        }
    }
}