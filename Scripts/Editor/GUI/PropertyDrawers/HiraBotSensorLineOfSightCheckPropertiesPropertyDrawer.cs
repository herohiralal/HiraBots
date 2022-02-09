using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots.Editor
{
    [CustomPropertyDrawer(typeof(HiraBotSensor.LineOfSightCheckProperties))]
    internal class HiraBotSensorLineOfSightCheckPropertiesPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var enabled = property.FindPropertyRelative(nameof(HiraBotSensor.LineOfSightCheckProperties.m_Enabled));
            return 21f + (enabled.boolValue ? 21f : 0f);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var enabled = property.FindPropertyRelative(nameof(HiraBotSensor.LineOfSightCheckProperties.m_Enabled));

            var mainRect = position;
            mainRect.height = 19f;

            EditorGUI.PropertyField(mainRect, enabled, label);

            if (enabled.boolValue)
            {
                var blockingObjects = property.FindPropertyRelative(nameof(HiraBotSensor.LineOfSightCheckProperties.m_BlockingObjects));

                var secondaryRect = position;
                secondaryRect.height = 19f;
                secondaryRect.y += 21f;

                EditorGUI.PropertyField(secondaryRect, blockingObjects);
            }
        }
    }
}