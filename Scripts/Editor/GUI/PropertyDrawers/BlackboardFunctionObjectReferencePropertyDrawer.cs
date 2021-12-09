using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    [CustomPropertyDrawer(typeof(BlackboardFunction), true)]
    internal class BlackboardFunctionObjectReferencePropertyDrawer : PropertyDrawer
    {
        private HashSet<string> propertiesToSkip = new HashSet<string> { "m_Subtitle" };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var value = property.objectReferenceValue;

            if (value == null)
            {
                return 21f;
            }

            var expanded = InlinedObjectReferencesHelper.IsExpanded(value, out var cso);

            return !expanded
                ? 21f
                : 0f
                  + 21f // header
                  + GUIHelpers.GetTotalHeightForPropertyDrawers((SerializedObject) cso, true, propertiesToSkip)
                  + 0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // try find object
            var value = property.objectReferenceValue as BlackboardFunction;

            if (value == null)
            {
                position.height -= 2f;
                EditorGUI.HelpBox(position, "Empty function detected.", MessageType.Error);
                return;
            }

            var currentRect = position;
            currentRect.x += 20f;
            currentRect.width -= 20f;
            currentRect.height = 21f;

            if (InlinedObjectReferencesHelper.DrawHeader(currentRect, value,
                BlackboardFunctionGUIHelpers.GetBlackboardFunctionColorFaded(value), value.subtitle,
                out var cso) && cso is BlackboardFunction.Serialized serializedObject)
            {
                if (serializedObject.hasError)
                {
                    currentRect.y += 22f;
                    EditorGUI.HelpBox(currentRect, serializedObject.error, MessageType.Error);
                    return;
                }

                currentRect.y += 22f;
                currentRect.height -= 22f;

                GUIHelpers.DrawDefaultPropertyDrawers(currentRect, (SerializedObject) cso, true, propertiesToSkip);
            }
        }
    }
}