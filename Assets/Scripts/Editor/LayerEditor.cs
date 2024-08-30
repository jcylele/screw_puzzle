using System.IO;
using Item;
using Layer;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(LayerEditBehaviour))]
    public class LayerEditor : UnityEditor.Editor
    {
        private string[] itemNames;
        private int selectedItemIndex;

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

            // Get the Stage script
            var layer = (LayerEditBehaviour)target;

            if (layer.LayerInfo == null)
            {
                return;
            }

            GUILayout.Label("Operations", MyEditorStyles.TitleStyle);

            GUILayout.BeginHorizontal();

            selectedItemIndex = EditorGUILayout.Popup(selectedItemIndex, itemNames,
                MyEditorStyles.PopupStyle, GUILayout.Height(35));
            if (GUILayout.Button("Add Item",
                    MyEditorStyles.ButtonStyle, GUILayout.Height(35), GUILayout.Width(150)))
            {
                var itemEditContainer = Resources.Load<ItemEditContainer>(Consts.ItemEditContainer);
                var container = Instantiate(itemEditContainer, layer.transform);
                container.gameObject.name = itemNames[selectedItemIndex];
                container.itemName = itemNames[selectedItemIndex];
                container.LoadItem(layer);
                Selection.activeGameObject = container.gameObject;
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Serialize Layer", MyEditorStyles.ButtonStyle, MyEditorStyles.BigButtonLayoutOption))
            {
                layer.SerializeLayer(true);
            }

            if (GUILayout.Button("Remove Layer", MyEditorStyles.ButtonStyle, MyEditorStyles.BigButtonLayoutOption))
            {
                layer.RemoveLayer();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Expand Layer", MyEditorStyles.ButtonStyle, MyEditorStyles.BigButtonLayoutOption))
            {
                layer.ExpandLayer();
            }

            if (GUILayout.Button("Collapse Layer", MyEditorStyles.ButtonStyle, MyEditorStyles.BigButtonLayoutOption))
            {
                layer.ClearLayer();
            }

            GUILayout.EndHorizontal();
        }
    }
}