using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots.Editor
{
    [CustomPropertyDrawer(typeof(StimulusType))]
    internal class StimulusTypePropertyDrawer : PropertyDrawer
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

            var value = 1 << Mathf.Clamp(valueProperty.intValue, 0, 31);
            GUIHelpers.DynamicEnumPopup(position, label, (IntPtr) (&value), PerceptionSystemGUIHelpers.stimulusMaskType, false);
            valueProperty.intValue = ((StimulusType) value).ToTypeIndex();
        }
    }
}