using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    [CustomPropertyDrawer(typeof(LGOAPTask))]
    internal class LGOAPTaskObjectReferencePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var value = property.objectReferenceValue;

            if (value == null)
            {
                return 21f;
            }

            var expanded = InlinedObjectReferencesHelper.IsExpanded(value, out var cso);

            return !expanded || !(cso is LGOAPTask.Serialized serializedObject)
                ? 21f
                : 0f
                  + 21f // header
                  + 21f // name
                  + 5f // padding
                  + serializedObject.taskPreconditionROL.GetHeight()
                  + 5f // padding
                  + serializedObject.taskCostROL.GetHeight()
                  + 5f // padding
                  + serializedObject.taskEffectROL.GetHeight()
                  + (serializedObject.isAbstract
                      ? 5f /* padding */ + serializedObject.taskTargetROL.GetHeight()
                      : 0f)
                  + 0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // try find object
            var value = property.objectReferenceValue as LGOAPTask;

            if (value == null)
            {
                position.height -= 2f;
                EditorGUI.HelpBox(position, "Empty task detected.", MessageType.Error);
                return;
            }

            var currentRect = position;
            currentRect.x += 20f;
            currentRect.width -= 20f;
            currentRect.height = 21f;

            // draw header, and check if it's expanded
            if (InlinedObjectReferencesHelper.DrawHeader(currentRect, value,
                LGOAPDomainGUIHelpers.GetComponentColorFaded(value), null,
                out var cso) && cso is LGOAPTask.Serialized serializedObject)
            {
                if (serializedObject.hasError)
                {
                    currentRect.y += 22f;
                    EditorGUI.HelpBox(currentRect, serializedObject.error, MessageType.Error);
                    return;
                }

                serializedObject.Update();

                TaskPreconditionROLDrawer.Bind(serializedObject);
                TaskCostROLDrawer.Bind(serializedObject);
                TaskEffectROLDrawer.Bind(serializedObject);
                TaskTargetROLDrawer.Bind(serializedObject);

                currentRect.y += 1f;

                // name field
                currentRect.y += 21f;
                currentRect.height = 19f;
                EditorGUI.PropertyField(currentRect, serializedObject.name);

                // padding
                currentRect.y += 5f;

                // precondition
                currentRect.y += 21f;
                currentRect.height = serializedObject.taskPreconditionROL.GetHeight();
                serializedObject.taskPreconditionROL.DoList(currentRect);

                // padding
                currentRect.y += 5f;

                // cost
                currentRect.y += currentRect.height;
                currentRect.height = serializedObject.taskCostROL.GetHeight();
                serializedObject.taskCostROL.DoList(currentRect);

                // padding
                currentRect.y += 5f;

                // effect
                currentRect.y += currentRect.height;
                currentRect.height = serializedObject.taskEffectROL.GetHeight();
                serializedObject.taskEffectROL.DoList(currentRect);

                if (serializedObject.isAbstract)
                {
                    // padding
                    currentRect.y += 5f;

                    // target
                    currentRect.y += currentRect.height;
                    currentRect.height = serializedObject.taskTargetROL.GetHeight();
                    serializedObject.taskTargetROL.DoList(currentRect);
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}