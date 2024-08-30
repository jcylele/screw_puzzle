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

            GUILayout.Label("Operations", MyEditorStyles.TitleStyle);

            // Get the BaseItemBehaviour script
            var itemEditBehaviour = (ItemEditBehaviour)target;

            if (itemEditBehaviour.itemInfo == null)
            {
                newInfoFileName = EditorGUILayout.TextField(newInfoFileName,
                    MyEditorStyles.TextFieldStyle, GUILayout.Height(30));
                if (GUILayout.Button("Create Item Info",
                        MyEditorStyles.ButtonStyle, GUILayout.Height(35)))
                {
                    itemEditBehaviour.itemInfo = CreateNewItemInfoFile();
                }

                return;
            }
            
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Generate mesh", MyEditorStyles.ButtonStyle, MyEditorStyles.BigButtonLayoutOption))
            {
                itemEditBehaviour.GenerateMesh();
            }
            
            GUILayout.EndHorizontal();

            // item operation buttons
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Serialize Item", MyEditorStyles.ButtonStyle, MyEditorStyles.BigButtonLayoutOption))
            {
                itemEditBehaviour.SerializeItem();
            }

            if (GUILayout.Button("Expand Item", MyEditorStyles.ButtonStyle, MyEditorStyles.BigButtonLayoutOption))
            {
                itemEditBehaviour.ExpandItem();
            }

            if (GUILayout.Button("Collapse Item", MyEditorStyles.ButtonStyle, MyEditorStyles.BigButtonLayoutOption))
            {
                itemEditBehaviour.ClearItem();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Collider buttons
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Capsule", MyEditorStyles.ButtonStyle, MyEditorStyles.BigButtonLayoutOption))
            {
                var capsule = itemEditBehaviour.AddCapsuleCollider();
                Selection.activeObject = capsule.gameObject;
            }

            if (GUILayout.Button("Add Box", MyEditorStyles.ButtonStyle, MyEditorStyles.BigButtonLayoutOption))
            {
                var collider = itemEditBehaviour.AddBoxCollider();
                Selection.activeObject = collider.gameObject;
            }

            if (GUILayout.Button("Add Circle", MyEditorStyles.ButtonStyle, MyEditorStyles.BigButtonLayoutOption))
            {
                var collider = itemEditBehaviour.AddCircleCollider();
                Selection.activeObject = collider.gameObject;
            }

            GUILayout.EndHorizontal();
        }
    }
}