using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HiraBots.Editor
{
    /// <summary>
    /// Property drawer for a HiraBot object field.
    /// Optionally, can disable editing when in Play Mode.
    /// </summary>
    [CustomPropertyDrawer(typeof(DisallowPlayModeEdit))]
    [CustomPropertyDrawer(typeof(UnityEngine.AI.BlackboardTemplate))]
    [CustomPropertyDrawer(typeof(UnityEngine.AI.BlackboardTemplate.KeySelector))]
    [CustomPropertyDrawer(typeof(UnityEngine.AI.BlackboardKey))]
    internal class ExportedReferenceAPIPropertyDrawer : PropertyDrawer
    {
        private const string k_InternalPropertyName = "m_Value";
        private const string k_MissingInternalPropertyMessage = "Missing m_Value internal property.";

        private bool disallowEdit => attribute is DisallowPlayModeEdit && EditorApplication.isPlayingOrWillChangePlaymode;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var internalProperty = property.FindPropertyRelative(k_InternalPropertyName);
            if (internalProperty == null)
            {
                return new IMGUIContainer(() => EditorGUILayout.HelpBox(k_MissingInternalPropertyMessage, MessageType.Error));
            }

            var propertyField = new PropertyField(internalProperty, property.displayName) {tooltip = property.tooltip};
            propertyField.Bind(property.serializedObject);

            propertyField.SetEnabled(!disallowEdit);

            return propertyField;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new GUIEnabledChanger(GUI.enabled && !disallowEdit))
            {
                var internalProperty = property.FindPropertyRelative(k_InternalPropertyName);
                if (internalProperty == null)
                {
                    EditorGUI.HelpBox(position, k_MissingInternalPropertyMessage, MessageType.Error);
                    return;
                }

                EditorGUI.PropertyField(position, internalProperty, label, true);
            }
        }
    }
}