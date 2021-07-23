using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    [CustomPropertyDrawer(typeof(BlackboardKey))]
    internal class BlackboardKeyObjectReferencePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var value = property.objectReferenceValue;

            if (value == null)
                return 21f;

            var isExpandedProperty = new SerializedObject(value).FindProperty("isExpanded");
                
            return isExpandedProperty == null || !isExpandedProperty.boolValue
                ? 21f
                : 0f
                  + 21f // header
                  + 21f // name edit
                  + 21f // traits - instance synced
                  + 21f // traits - essential to decision making
                  + 21f // default value
                  + 0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var value = property.objectReferenceValue;

            if (value == null)
            {
                position.height -= 2f;
                EditorGUI.HelpBox(position, "Empty key detected.", MessageType.Error);
                return;
            }

            var so = new SerializedObject(value);

            var currentRect = position;
            currentRect.x += 20f;
            currentRect.width -= 20f;
            currentRect.height = 21f;
            if (DrawHeader(currentRect, so))
            {
                currentRect.height = 19f;

                currentRect.y += 21f;
                GUIHelpers.DrawNameField(currentRect, value);

                currentRect.y += 21f;
                var instanceSyncedProperty = so.FindProperty("instanceSynced");
                if (instanceSyncedProperty == null)
                {
                    EditorGUI.HelpBox(currentRect, "Missing instance sync property.", MessageType.Error);
                }
                else
                {
                    EditorGUI.PropertyField(currentRect, instanceSyncedProperty);
                }

                currentRect.y += 21f;
                var essentialToDecisionMakingProperty = so.FindProperty("essentialToDecisionMaking");
                if (essentialToDecisionMakingProperty == null)
                {
                    EditorGUI.HelpBox(currentRect, "Missing essential to decision making property.", MessageType.Error);
                }
                else
                {
                    EditorGUI.PropertyField(currentRect, essentialToDecisionMakingProperty);
                }

                currentRect.y += 21f;
                var defaultValueProperty = so.FindProperty("defaultValue");
                if (defaultValueProperty == null)
                {
                    EditorGUI.HelpBox(currentRect, "Missing default value property.", MessageType.Error);
                }
                else
                {
                    EditorGUI.PropertyField(currentRect, defaultValueProperty);
                }
            }

            so.ApplyModifiedProperties();
        }

        private static bool DrawHeader(Rect position, SerializedObject so)
        {
            var isExpandedProperty = so.FindProperty("isExpanded");

            if (isExpandedProperty != null)
            {
                using (new GUIEnabledChanger(true))
                {
                    // begin group
                    var expanded = EditorGUI.BeginFoldoutHeaderGroup(position, isExpandedProperty.boolValue, GUIContent.none);

                    // background
                    position.height -= 2f;
                    EditorGUI.DrawRect(position, BlackboardGUIHelpers.GetBlackboardKeyColor(so.targetObject) * 0.35f);

                    // name
                    position.height += 2f;
                    position.x += 10f;
                    position = EditorGUI.PrefixLabel(position, GUIHelpers.ToGUIContent(so.targetObject.name), EditorStyles.boldLabel);

                    // type
                    EditorGUI.LabelField(position, GUIHelpers.ToGUIContent(BlackboardGUIHelpers.FORMATTED_NAMES[so.targetObject.GetType()]));

                    // end group
                    EditorGUI.EndFoldoutHeaderGroup();

                    // retrieve data
                    isExpandedProperty.boolValue = expanded;
                    return expanded;
                }
            }
            else
            {
                EditorGUI.HelpBox(position, $"Missing is expanded property.", MessageType.Error);
                return false;
            }
        }
    }
}