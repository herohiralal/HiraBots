using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    /// <summary>
    /// Custom property drawer for a Blackboard Key.
    /// The main feature is that it draws the key object directly.
    /// todo: implement this in UIElements (get rid of repeated creation/disposal of SerializedObjects per key)
    /// </summary>
    [CustomPropertyDrawer(typeof(BlackboardKey))]
    internal class BlackboardKeyObjectReferencePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var value = property.objectReferenceValue;

            if (value == null)
            {
                return 21f;
            }

            var expanded = InlinedObjectReferencesHelper.IsExpanded(value);

            return !expanded
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
            // try find object
            var value = property.objectReferenceValue as BlackboardKey;

            if (value == null)
            {
                position.height -= 2f;
                EditorGUI.HelpBox(position, "Empty key detected.", MessageType.Error);
                return;
            }

            var currentRect = position;
            currentRect.x += 20f;
            currentRect.width -= 20f;
            currentRect.height = 21f;

            // draw header, and check if it's expanded
            if (InlinedObjectReferencesHelper.DrawHeader(currentRect, value,
                BlackboardGUIHelpers.GetBlackboardKeyColorFaded(value), BlackboardGUIHelpers.GetFormattedName(value),
                out var cso) && cso is BlackboardKey.Serialized serializedObject)
            {
                if (serializedObject.hasError)
                {
                    currentRect.y += 22f;
                    EditorGUI.HelpBox(currentRect, serializedObject.error, MessageType.Error);
                    return;
                }

                serializedObject.Update();

                currentRect.y += 1f;
                currentRect.height = 19f;

                // name field
                currentRect.y += 21f;
                EditorGUI.PropertyField(currentRect, serializedObject.name);

                // instance synced property field
                currentRect.y += 21f;
                EditorGUI.PropertyField(currentRect, serializedObject.instanceSynced);

                // essential to decision-making property field
                currentRect.y += 21f;
                EditorGUI.PropertyField(currentRect, serializedObject.essentialToDecisionMaking);

                // default value property field
                currentRect.y += 21f;
                if (serializedObject.defaultValue == null)
                {
                    EditorGUI.HelpBox(currentRect, "Unsupported default value property.", MessageType.Info);
                }
                else
                {
                    EditorGUI.PropertyField(currentRect, serializedObject.defaultValue);
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}