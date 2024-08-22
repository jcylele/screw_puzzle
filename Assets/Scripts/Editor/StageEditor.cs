using System.IO;
using Item;
using Stage;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(StageEditBehaviour))]
    public class StageEditor : UnityEditor.Editor
    {
        private string newStageFileName;
        private string[] itemNames;
        private int selectedItemIndex;

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

        private void OnEnable()
        {
            var itemInfoGuids =
                AssetDatabase.FindAssets("t:ItemInfo", new[] { $"Assets/Resources/{Consts.ItemInfoRootPath}" });
            itemNames = new string[itemInfoGuids.Length];
            for (var i = 0; i < itemInfoGuids.Length; i++)
            {
                var itemInfoPath = AssetDatabase.GUIDToAssetPath(itemInfoGuids[i]);
                var itemName = Path.GetFileNameWithoutExtension(itemInfoPath);
                itemNames[i] = itemName;
            }
        }

        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();
            
            GUILayout.Label("Operations", EditorStyles.boldLabel);

            // Get the Stage script
            var stage = (StageEditBehaviour)target;

            if (stage.stageInfo == null)
            {
                newStageFileName = EditorGUILayout.TextField("File Name: ", newStageFileName);
                if (GUILayout.Button("Create Stage Info", GUILayout.Height(25)))
                {
                    stage.stageInfo = CreateNewStageInfoFile();
                }

                return;
            }

            selectedItemIndex = EditorGUILayout.Popup("Item: ", selectedItemIndex, itemNames);
            if (GUILayout.Button("Add Item", GUILayout.Height(25)))
            {
                var itemEditContainer = stage.LoadComponent<ItemEditContainer>(Consts.ItemEditContainer);
                var container = Instantiate(itemEditContainer, stage.transform);
                container.gameObject.name = itemNames[selectedItemIndex];
                container.itemName = itemNames[selectedItemIndex];
                container.LoadItem();
                
                Selection.activeGameObject = container.gameObject;
            }
            
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Serialize Stage", GUILayout.Height(50)))
            {
                stage.SerializeStage();
            }
            
            if (GUILayout.Button("Expand Stage", GUILayout.Height(50)))
            {
                stage.ExpandStage();
            }

            if (GUILayout.Button("Clear Stage", GUILayout.Height(50)))
            {
                stage.ClearStage();
            }

            GUILayout.EndHorizontal();
        }
    }
}