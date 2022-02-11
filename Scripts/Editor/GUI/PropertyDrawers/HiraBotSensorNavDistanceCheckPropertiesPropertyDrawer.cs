using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots.Editor
{
    [CustomPropertyDrawer(typeof(HiraBotSensor.NavDistanceCheckProperties))]
    internal class HiraBotSensorNavDistanceCheckPropertiesPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var enabled = property.FindPropertyRelative(nameof(HiraBotSensor.NavDistanceCheckProperties.m_Enabled));
            return 21f + (enabled.boolValue ? (21f + 21f + 21f + 21f + 21f + 21f + 21f) : 0f);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var enabled = property.FindPropertyRelative(nameof(HiraBotSensor.NavDistanceCheckProperties.m_Enabled));

            var mainRect = position;
            mainRect.height = 19f;

            EditorGUI.PropertyField(mainRect, enabled, label);

            if (enabled.boolValue)
            {
                var invert = property.FindPropertyRelative(nameof(HiraBotSensor.NavDistanceCheckProperties.m_Invert));
                var range = property.FindPropertyRelative(nameof(HiraBotSensor.NavDistanceCheckProperties.m_Range));
                var agent = property.FindPropertyRelative(nameof(HiraBotSensor.NavDistanceCheckProperties.m_AgentType));
                var area = property.FindPropertyRelative(nameof(HiraBotSensor.NavDistanceCheckProperties.m_AreaMask));
                var tolerance = property.FindPropertyRelative(nameof(HiraBotSensor.NavDistanceCheckProperties.m_NavmeshDistanceTolerance));
                var sensorNotOnNavMesh = property.FindPropertyRelative(nameof(HiraBotSensor.NavDistanceCheckProperties.m_SensorNotOnNavMeshAction));
                var stimulusNotOnNavMesh = property.FindPropertyRelative(nameof(HiraBotSensor.NavDistanceCheckProperties.m_StimulusNotOnNavMeshAction));

                var secondaryRect = position;
                secondaryRect.height = 19f;
                secondaryRect.width -= 15f;
                secondaryRect.x += 15f;

                secondaryRect.y += 21f;
                EditorGUI.PropertyField(secondaryRect, invert);

                secondaryRect.y += 21f;
                EditorGUI.PropertyField(secondaryRect, range);

                secondaryRect.y += 21f;
                EditorGUI.PropertyField(secondaryRect, agent);

                secondaryRect.y += 21f;
                EditorGUI.PropertyField(secondaryRect, area);

                secondaryRect.y += 21f;
                EditorGUI.PropertyField(secondaryRect, tolerance);

                secondaryRect.y += 21f;
                EditorGUI.PropertyField(secondaryRect, sensorNotOnNavMesh);

                secondaryRect.y += 21f;
                EditorGUI.PropertyField(secondaryRect, stimulusNotOnNavMesh);
            }
        }
    }
}