using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    [CustomPropertyDrawer(typeof(DecoratorBlackboardFunction.Header))]
    internal class DecoratorBlackboardFunctionHeaderPropertyDrawer : PropertyDrawer
    {
        private const string k_IsScoreCalculatorPropertyName = "m_IsScoreCalculator";
        private const string k_ScorePropertyName = "m_Score";
        private const string k_InvertPropertyName = "m_Invert";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var isScoreCalculatorProperty = property.FindPropertyRelative(k_IsScoreCalculatorPropertyName);
            return isScoreCalculatorProperty == null || !isScoreCalculatorProperty.boolValue
                ? 21f
                : 42f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var isScoreCalculatorProperty = property.FindPropertyRelative(k_IsScoreCalculatorPropertyName);
            var scoreProperty = property.FindPropertyRelative(k_ScorePropertyName);
            var invertProperty = property.FindPropertyRelative(k_InvertPropertyName);

            if (isScoreCalculatorProperty == null || scoreProperty == null || invertProperty == null)
            {
                EditorGUI.HelpBox(position, $"Could not find one or more properties.", MessageType.Error);
                return;
            }

            position.height = 19f;

            if (isScoreCalculatorProperty.boolValue)
            {
                EditorGUI.PropertyField(position, scoreProperty);

                position.y += 21f;
            }

            EditorGUI.PropertyField(position, invertProperty);
        }
    }
}