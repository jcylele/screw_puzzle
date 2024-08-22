using Item;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ColliderEditBehaviour))]
    public class ColliderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();

            // Get the BaseItemBehaviour script
            var colliderEdit = (ColliderEditBehaviour)target;

            // item operation buttons
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Joint", GUILayout.Height(50)))
            {
                var joint = colliderEdit.AddJoint();
                Selection.activeObject = joint.gameObject;
            }

            if (GUILayout.Button("Clear Joints", GUILayout.Height(50)))
            {
                colliderEdit.ClearJoints();
            }

            GUILayout.EndHorizontal();
        }
    }
}