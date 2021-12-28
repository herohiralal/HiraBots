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
                  + (!serializedObject.canBeAbstract
                     || serializedObject.taskTaskProviders.arraySize > 0
                     || serializedObject.taskTarget.arraySize == 0
                      ? 5f /* padding */ + serializedObject.taskTaskProvidersROL.GetHeight()
                                         + (serializedObject.canBeAbstract ? (5f /* padding */ + 21f) : 0f) // info regarding switching abstraction
                      : 0f)
                  + (serializedObject.canBeAbstract
                     && (serializedObject.taskTaskProviders.arraySize == 0
                         || serializedObject.taskTarget.arraySize > 0)
                      ? 5f /* padding */ + serializedObject.taskTargetROL.GetHeight()
                                         + 5f /* padding */ + 21f // info regarding switching abstraction
                      : 0f)
                  + 5f // padding
                  + serializedObject.taskServiceProvidersROL.GetHeight()
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
                    LGOAPDomainGUIHelpers.GetContainerColorFaded(value), value.canBeAbstract && value.isAbstract ? "Abstract Task" : "Task",
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
                TaskProviderROLDrawer.Bind(serializedObject);
                ServiceProviderROLDrawer.Bind(serializedObject);

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

                if (serializedObject.canBeAbstract
                    && (serializedObject.taskTaskProviders.arraySize == 0
                        || serializedObject.taskTarget.arraySize > 0))
                {
                    // padding
                    currentRect.y += 5f;

                    // info
                    currentRect.y += currentRect.height;
                    currentRect.height = 21f;
                    EditorGUI.HelpBox(currentRect, "To make this task non-abstract, remove the target.", MessageType.Info);

                    // padding
                    currentRect.y += 5f;

                    // target
                    currentRect.y += currentRect.height;
                    currentRect.height = serializedObject.taskTargetROL.GetHeight();
                    serializedObject.taskTargetROL.DoList(currentRect);
                }

                if (!serializedObject.canBeAbstract
                    || serializedObject.taskTaskProviders.arraySize > 0
                    || serializedObject.taskTarget.arraySize == 0)
                {
                    if (serializedObject.canBeAbstract)
                    {
                        // padding
                        currentRect.y += 5f;

                        // info
                        currentRect.y += currentRect.height;
                        currentRect.height = 21f;
                        EditorGUI.HelpBox(currentRect, "To make this task abstract, remove the task provider.", MessageType.Info);
                    }

                    // padding
                    currentRect.y += 5f;

                    // task
                    currentRect.y += currentRect.height;
                    currentRect.height = serializedObject.taskTaskProvidersROL.GetHeight();
                    serializedObject.taskTaskProvidersROL.DoList(currentRect);
                }

                // padding
                currentRect.y += 5f;

                // service providers
                currentRect.y += currentRect.height;
                currentRect.height = serializedObject.taskServiceProvidersROL.GetHeight();
                serializedObject.taskServiceProvidersROL.DoList(currentRect);

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}