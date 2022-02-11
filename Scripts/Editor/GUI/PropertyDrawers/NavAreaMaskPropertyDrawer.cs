using UnityEditor;
using UnityEditor.AI;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots.Editor
{
    [CustomPropertyDrawer(typeof(NavAreaMask))]
    internal class NavAreaMaskPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 21f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative("m_Value");
            if (valueProperty == null)
            {
                EditorGUI.HelpBox(position, "Could not find value property.", MessageType.Error);
                return;
            }

            position.height = 19f;
            position.width -= 21f;

            valueProperty.intValue = EditorGUI.MaskField(position, label, valueProperty.intValue,
                GameObjectUtility.GetNavMeshAreaNames());

            position.x += position.width + 2f;
            position.width = 19f;

            var settingsIcon = EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_SettingsIcon" : "SettingsIcon");
            var buttonContent = new GUIContent(settingsIcon);

            if (GUI.Button(position, buttonContent))
            {
                NavMeshEditorHelpers.OpenAreaSettings();
            }
        }
    }
}