using Item;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(BezierCurveBehaviour))]
    public class BezierCurveEditor : UnityEditor.Editor
    {
        private BezierCurveBehaviour curve;
        private Transform handleTransform;
        private Quaternion handleRotation;

        private void OnSceneGUI()
        {
            curve = target as BezierCurveBehaviour;
            handleTransform = curve.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local
                ? handleTransform.rotation
                : Quaternion.identity;

            Vector3 p0 = ShowPoint(0);
            Vector3 p1 = ShowPoint(1);
            Vector3 p2 = ShowPoint(2);
            Vector3 p3 = ShowPoint(3);

            Handles.color = Color.cyan;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            Handles.DrawBezier(p0, p3, p1, p2, Color.yellow, null, 2f);
        }

        private Vector3 ShowPoint(int index)
        {
            Vector3 point = handleTransform.TransformPoint(curve.points[index]);
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curve, "Move Point");
                EditorUtility.SetDirty(curve);
                curve.points[index] = handleTransform.InverseTransformPoint(point);
            }

            return point;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Update Polygon Collider",
                    MyEditorStyles.ButtonStyle,
                    MyEditorStyles.BigButtonLayoutOption))
            {
                curve.UpdateCollider();
            }
        }
    }
}