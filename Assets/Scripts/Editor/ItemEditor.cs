using Item;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ItemEditBehaviour))]
    public class ItemEditor : UnityEditor.Editor
    {
        private string newInfoFileName;


        private ItemInfo CreateNewItemInfoFile()
        {
            if (string.IsNullOrWhiteSpace(newInfoFileName))
            {
                return null;
            }

            newInfoFileName = newInfoFileName.Trim();

            var itemInfo = CreateInstance<ItemInfo>();

            var assetPath = $"Assets/Resources/{Consts.ItemInfoRootPath}/{newInfoFileName}.asset";
            // check for existing file
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (!string.IsNullOrEmpty(guid))
            {
                Debug.LogError($"{newInfoFileName} already exists, please choose another name.");
                return null;
            }

            AssetDatabase.CreateAsset(itemInfo, assetPath);
            AssetDatabase.SaveAssets();

            return itemInfo;
        }

        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();

            // Get the BaseItemBehaviour script
            var itemEditBehaviour = (ItemEditBehaviour)target;

            if (itemEditBehaviour.itemInfo == null)
            {
                newInfoFileName = EditorGUILayout.TextField("File Name: ", newInfoFileName);
                if (GUILayout.Button("Create Item Info", GUILayout.Height(25)))
                {
                    itemEditBehaviour.itemInfo = CreateNewItemInfoFile();
                }

                return;
            }

            // item operation buttons
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Serialize Item", GUILayout.Height(50)))
            {
                itemEditBehaviour.SerializeItem();
            }

            if (GUILayout.Button("Expand Item", GUILayout.Height(50)))
            {
                itemEditBehaviour.ExpandItem();
            }

            if (GUILayout.Button("Clear Item", GUILayout.Height(50)))
            {
                itemEditBehaviour.ClearItem();
            }

            GUILayout.EndHorizontal();

            // Collider buttons
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Capsule Collider", GUILayout.Height(50)))
            {
                var capsule = itemEditBehaviour.AddCapsuleCollider();
                Selection.activeObject = capsule.gameObject;
            }

            if (GUILayout.Button("Add Box Collider", GUILayout.Height(50)))
            {
                var collider = itemEditBehaviour.AddBoxCollider();
                Selection.activeObject = collider.gameObject;
            }

            if (GUILayout.Button("Add Circle Collider", GUILayout.Height(50)))
            {
                var collider = itemEditBehaviour.AddCircleCollider();
                Selection.activeObject = collider.gameObject;
            }

            GUILayout.EndHorizontal();
        }
    }
}