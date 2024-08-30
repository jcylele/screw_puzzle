using System.IO;
using Item;
using Layer;
using Stage;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(StageEditBehaviour))]
    public class StageEditor : UnityEditor.Editor
    {
        private string newStageFileName;
        private string newLayerName;

        private StageInfo CreateNewStageInfoFile()
        {
            if (string.IsNullOrWhiteSpace(newStageFileName))
            {
                return null;
            }

            newStageFileName = newStageFileName.Trim();

            var stageInfo = CreateInstance<StageInfo>();

            var assetPath = $"Assets/Resources/{Consts.StageInfoRootPath}/{newStageFileName}.asset";
            // check for existing file
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (!string.IsNullOrEmpty(guid))
            {
                Debug.LogError($"{newStageFileName} already exists, please choose another name.");
                return null;
            }

            AssetDatabase.CreateAsset(stageInfo, assetPath);
            AssetDatabase.SaveAssets();

            return stageInfo;
        }

        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();

            GUILayout.Label("Operations", MyEditorStyles.TitleStyle);

            // Get the Stage script
            var stage = (StageEditBehaviour)target;

            if (stage.stageInfo == null)
            {
                newStageFileName = EditorGUILayout.TextField(newStageFileName, MyEditorStyles.TextFieldStyle,
                    GUILayout.Height(30));
                if (GUILayout.Button("Create Stage Info", MyEditorStyles.ButtonStyle, GUILayout.Height(35)))
                {
                    stage.stageInfo = CreateNewStageInfoFile();
                }

                return;
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            newLayerName = EditorGUILayout.TextField(newLayerName,
                MyEditorStyles.TextFieldStyle, MyEditorStyles.BigButtonLayoutOption);

            if (GUILayout.Button("Add Layer",
                    MyEditorStyles.ButtonStyle, MyEditorStyles.BigButtonLayoutOption))
            {
                var newLayerInfo = new LayerInfo
                {
                    layerIndex = stage.stageInfo.layerInfos.Count + 1,
                    layerName = newLayerName
                };
                stage.stageInfo.layerInfos.Add(newLayerInfo);

                var layer = stage.AddLayer(newLayerInfo);
                layer.Rename();
                Selection.activeObject = layer.gameObject;
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Reorder Layer",
                    MyEditorStyles.ButtonStyle, MyEditorStyles.BigButtonLayoutOption))
            {
                for (int i = 0, j = 0; i < stage.transform.childCount; i++)
                {
                    var child = stage.transform.GetChild(i);
                    var layer = child.GetComponent<LayerEditBehaviour>();
                    if (layer == null)
                    {
                        continue;
                    }

                    j++;
                    layer.LayerInfo.layerIndex = j;
                    layer.Rename();
                    layer.RefreshLayerPosition();
                    layer.RefreshLayerColor();
                }

                stage.SerializeStage();
            }

            if (GUILayout.Button("Serialize Stage", MyEditorStyles.ButtonStyle, MyEditorStyles.BigButtonLayoutOption))
            {
                stage.SerializeStage();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Expand Stage", MyEditorStyles.ButtonStyle, MyEditorStyles.BigButtonLayoutOption))
            {
                stage.ExpandStage();
            }

            if (GUILayout.Button("Collapse Stage", MyEditorStyles.ButtonStyle, MyEditorStyles.BigButtonLayoutOption))
            {
                stage.ClearStage();
            }

            GUILayout.EndHorizontal();
        }
    }
}