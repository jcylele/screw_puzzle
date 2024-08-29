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

            if (GUILayout.Button("Add Layer",
                    MyEditorStyles.ButtonStyle, MyEditorStyles.BigButtonLayoutOption))
            {
                var newLayerInfo = new LayerInfo
                {
                    layerIndex = stage.stageInfo.layerInfos.Count + 1
                };
                stage.stageInfo.layerInfos.Add(newLayerInfo);

                var layer = stage.AddLayer(newLayerInfo);
                newLayerInfo.uuid = layer.GetInstanceID();
                layer.Rename();
                Selection.activeObject = layer.gameObject;
            }

            if (GUILayout.Button("Reorder Layer",
                    MyEditorStyles.ButtonStyle, MyEditorStyles.BigButtonLayoutOption))
            {
                var layers = stage.GetComponentsInChildren<LayerEditBehaviour>(true);
                for (int i = 0; i < layers.Length; i++)
                {
                    layers[i].LayerInfo.layerIndex = i + 1;
                    layers[i].Rename();
                    layers[i].RefreshLayerPosition();
                }

                stage.SerializeStage();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Serialize Stage", MyEditorStyles.ButtonStyle, MyEditorStyles.BigButtonLayoutOption))
            {
                stage.SerializeStage();
            }

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