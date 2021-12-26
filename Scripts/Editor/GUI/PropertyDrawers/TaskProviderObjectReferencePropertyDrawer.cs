using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    [CustomPropertyDrawer(typeof(HiraBotsTaskProvider), true)]
    internal class TaskProviderObjectReferencePropertyDrawer : PropertyDrawer
    {
        private static HashSet<string> propertiesToSkip { get; } = new HashSet<string> { "m_Description" };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var value = property.objectReferenceValue;

            if (value == null)
            {
                return 21f;
            }

            var expanded = InlinedObjectReferencesHelper.IsExpanded(value, out var cso);

            return !expanded
                ? 21f + 21f
                : 0f
                  + 21f // header
                  + GUIHelpers.GetTotalHeightForPropertyDrawers((SerializedObject) cso, true, propertiesToSkip)
                  + 0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // try find object
            var value = property.objectReferenceValue as HiraBotsTaskProvider;

            if (value == null)
            {
                position.height -= 2f;
                EditorGUI.HelpBox(position, "Empty task provider detected.", MessageType.Error);
                return;
            }

            var currentRect = position;
            currentRect.x += 20f;
            currentRect.width -= 20f;
            currentRect.height = 21f;
            
            if (InlinedObjectReferencesHelper.DrawHeader(currentRect, value, ExecutablesGUIHelpers.taskProviderColorFaded,
                    "Task", out var cso))
            {
                if (cso.hasError)
                {
                    currentRect.y += 22f;
                    EditorGUI.HelpBox(currentRect, cso.error, MessageType.Error);
                    return;
                }
            
                currentRect.y += 22f;
                currentRect.height -= 22f;
            
                GUIHelpers.DrawDefaultPropertyDrawers(currentRect, (SerializedObject) cso, true, propertiesToSkip);
            }
            else
            {
                currentRect.y += 21f;

                currentRect.height -= 4f;
                EditorGUI.DrawRect(currentRect, ExecutablesGUIHelpers.taskProviderColorFaded);
                currentRect.height += 4f;

                var description = value.description ?? "";
                EditorGUI.LabelField(currentRect, description, EditorStyles.miniBoldLabel);
            }
        }
    }
}