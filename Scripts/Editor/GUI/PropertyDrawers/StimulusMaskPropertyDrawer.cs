using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots.Editor
{
    [CustomPropertyDrawer(typeof(StimulusMask))]
    internal class StimulusMaskPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 21f;
        }

        public override unsafe void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative("m_Value");
            if (valueProperty == null)
            {
                EditorGUI.HelpBox(position, "Could not find value property.", MessageType.Error);
                return;
            }

            position.height = 19f;

            var value = valueProperty.intValue;
            GUIHelpers.DynamicEnumPopup(position, label, (IntPtr) (&value), PerceptionSystemGUIHelpers.stimulusMaskType, true);
            valueProperty.intValue = value;
        }
    }
}