using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class MyEditorStyles
    {
        internal static readonly GUIStyle TitleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
            normal = new GUIStyleState
            {
                textColor = Color.white
            },
        };

        internal static readonly GUIStyle TextFieldStyle = new GUIStyle(GUI.skin.textField)
        {
            fontSize = 20,
        };

        internal static readonly GUIStyle PopupStyle = new GUIStyle(EditorStyles.popup)
        {
            fixedHeight = 35,
            fontSize = 20,
        };

        internal static readonly GUIStyle ButtonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter,
            normal = new GUIStyleState
            {
                textColor = Color.white
            }
        };

        internal static readonly GUILayoutOption BigButtonLayoutOption = GUILayout.Height(50);
    }
}