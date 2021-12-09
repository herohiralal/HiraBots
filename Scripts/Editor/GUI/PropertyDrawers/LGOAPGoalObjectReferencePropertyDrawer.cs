using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    [CustomPropertyDrawer(typeof(LGOAPGoal))]
    internal class LGOAPGoalObjectReferencePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var value = property.objectReferenceValue;

            if (value == null)
            {
                return 21f;
            }

            var expanded = InlinedObjectReferencesHelper.IsExpanded(value, out var cso);

            return !expanded || !(cso is LGOAPGoal.Serialized serializedObject)
                ? 21f
                : 0f
                  + 21f // header
                  + 21f // name
                  + 5f // padding
                  + serializedObject.goalInsistenceROL.GetHeight()
                  + 5f // padding
                  + serializedObject.goalTargetROL.GetHeight()
                  + 0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // try find object
            var value = property.objectReferenceValue as LGOAPGoal;

            if (value == null)
            {
                position.height -= 2f;
                EditorGUI.HelpBox(position, "Empty goal detected.", MessageType.Error);
                return;
            }

            var currentRect = position;
            currentRect.x += 20f;
            currentRect.width -= 20f;
            currentRect.height = 21f;

            // draw header, and check if it's expanded
            if (InlinedObjectReferencesHelper.DrawHeader(currentRect, value,
                LGOAPDomainGUIHelpers.GetComponentColorFaded(value), "Goal",
                out var cso) && cso is LGOAPGoal.Serialized serializedObject)
            {
                if (serializedObject.hasError)
                {
                    currentRect.y += 22f;
                    EditorGUI.HelpBox(currentRect, serializedObject.error, MessageType.Error);
                    return;
                }

                serializedObject.Update();

                GoalInsistenceROLDrawer.Bind(serializedObject);
                GoalTargetROLDrawer.Bind(serializedObject);

                currentRect.y += 1f;

                // name field
                currentRect.y += 21f;
                currentRect.height = 19f;
                EditorGUI.PropertyField(currentRect, serializedObject.name);

                // padding
                currentRect.y += 5f;

                // insistence
                currentRect.y += 21f;
                currentRect.height = serializedObject.goalInsistenceROL.GetHeight();
                serializedObject.goalInsistenceROL.DoList(currentRect);

                // padding
                currentRect.y += 5f;

                // target
                currentRect.y += currentRect.height;
                currentRect.height = serializedObject.goalTargetROL.GetHeight();
                serializedObject.goalTargetROL.DoList(currentRect);

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}