﻿using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HiraBots.Editor
{
    internal abstract class InlinedObjectReferenceROLDrawer<TInlinedReference, TTargetSerializedObject>
        where TInlinedReference : Object
        where TTargetSerializedObject : Object
    {
        protected InlinedObjectReferenceROLDrawer(string header, ReadOnlyDictionaryAccessor<Type, string>? typesWithFormattedNames = null)
        {
            m_Header = header;
            m_TypesWithFormattedNames = typesWithFormattedNames;
        }

        protected readonly string m_Header;
        protected readonly ReadOnlyDictionaryAccessor<Type, string>? m_TypesWithFormattedNames;

        protected void Bind(ReorderableList rol, CustomSerializedObject<TTargetSerializedObject> serializedObject, SerializedProperty property)
        {
            rol.drawHeaderCallback = r =>
            {
                r.x += 20f;
                EditorGUI.LabelField(r, m_Header, EditorStyles.boldLabel);
            };

            if (m_TypesWithFormattedNames.HasValue)
            {
                rol.onAddDropdownCallback = (r, l) =>
                {
                    var menu = new GenericMenu();

                    foreach (var type in m_TypesWithFormattedNames.Value.keys)
                    {
                        menu.AddItem(GUIHelpers.ToGUIContent(m_TypesWithFormattedNames.Value[type]), false,
                            () =>
                            {
                                var newObject = AssetDatabaseUtility.AddInlinedObject(serializedObject.target, (SerializedObject) serializedObject,
                                    property, type);

                                ProcessCreatedObject(serializedObject, (TInlinedReference) newObject);
                            });
                    }

                    menu.ShowAsContext();
                };
            }
            else
            {
                rol.onAddCallback = r =>
                {
                    var newObject = AssetDatabaseUtility.AddInlinedObject(serializedObject.target, (SerializedObject) serializedObject,
                        property, typeof(TInlinedReference));

                    ProcessCreatedObject(serializedObject, (TInlinedReference) newObject);
                };
            }

            rol.onRemoveCallback = l =>
            {
                InlinedObjectReferencesHelper.Collapse(property.GetArrayElementAtIndex(l.index).objectReferenceValue);

                AssetDatabaseUtility.RemoveInlinedObject(serializedObject.target, (SerializedObject) serializedObject,
                    property, l.index);
            };

            rol.elementHeightCallback = i =>
                EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(i)) + 4;

            rol.drawElementCallback = (r, i, a, f) =>
                EditorGUI.PropertyField(r, property.GetArrayElementAtIndex(i), GUIContent.none, true);

            rol.drawElementBackgroundCallback = (r, i, a, f) =>
            {
                if (i < 0)
                {
                    return;
                }

                r.y -= 2;
                r.height -= 2;

                // default background
                r.x += 20f;
                r.width -= 20f;
                ReorderableList.defaultBehaviours.DrawElementBackground(r, i, a, true, true);

                // override portion with a colour
                r.x -= 20f;
                r.width = 20f;
                EditorGUI.DrawRect(r, GetThemeColor((TInlinedReference) property.GetArrayElementAtIndex(i).objectReferenceValue));
            };
        }

        internal static void Unbind(ReorderableList rol)
        {
            rol.drawHeaderCallback = null;
            rol.onAddDropdownCallback = null;
            rol.onAddCallback = null;
            rol.onRemoveCallback = null;
            rol.elementHeightCallback = null;
            rol.drawElementCallback = null;
            rol.drawElementBackgroundCallback = null;
        }

        protected virtual void ProcessCreatedObject(CustomSerializedObject<TTargetSerializedObject> serializedObject, TInlinedReference newObject)
        {
        }

        protected virtual Color GetThemeColor(TInlinedReference value)
        {
            return Color.black;
        }
    }

    internal class BlackboardKeyInlinedObjectReferenceROLDrawer : InlinedObjectReferenceROLDrawer<BlackboardKey, BlackboardTemplate>
    {
        private BlackboardKeyInlinedObjectReferenceROLDrawer() : base("Self Keys", BlackboardGUIHelpers.formattedNames)
        {
        }

        protected override Color GetThemeColor(BlackboardKey value)
        {
            return BlackboardGUIHelpers.GetBlackboardKeyColor(value);
        }

        private static readonly BlackboardKeyInlinedObjectReferenceROLDrawer s_Instance = new BlackboardKeyInlinedObjectReferenceROLDrawer();

        internal static ReorderableList Bind(BlackboardTemplate.Serialized serializedObject)
        {
            var rol = new ReorderableList((SerializedObject) serializedObject, serializedObject.keys,
                true, true, true, true);
            s_Instance.Bind(rol, serializedObject, serializedObject.keys);
            return rol;
        }
    }

    internal abstract class BlackboardFunctionInlinedObjectReferenceROLDrawer<TTargetSerializedObject>
        : InlinedObjectReferenceROLDrawer<BlackboardFunction, TTargetSerializedObject>
        where TTargetSerializedObject : Object
    {
        protected BlackboardFunctionInlinedObjectReferenceROLDrawer(string header, ReadOnlyDictionaryAccessor<Type, string> typesWithFormattedNames)
            : base(header, typesWithFormattedNames)
        {
        }

        protected override Color GetThemeColor(BlackboardFunction value)
        {
            return BlackboardFunctionGUIHelpers.GetBlackboardFunctionColor(value);
        }
    }

    internal class GoalInsistenceROLDrawer : BlackboardFunctionInlinedObjectReferenceROLDrawer<LGOAPGoal>
    {
        private GoalInsistenceROLDrawer() : base("Insistence", BlackboardFunctionGUIHelpers.formattedScoreCalculatorNames)
        {
        }

        protected override void ProcessCreatedObject(CustomSerializedObject<LGOAPGoal> serializedObject, BlackboardFunction newObject)
        {
            InlinedObjectReferencesHelper.Expand(newObject, out _);
            ((DecoratorBlackboardFunction) newObject).isScoreCalculator = true;

            if (serializedObject is LGOAPGoal.Serialized goal)
            {
                goal.OnBlackboardUpdate(newObject);
            }
        }

        private static readonly GoalInsistenceROLDrawer s_Instance = new GoalInsistenceROLDrawer();

        internal static void Bind(LGOAPGoal.Serialized goal)
        {
            if (!goal.goalInsistenceBound)
            {
                s_Instance.Bind(goal.goalInsistenceROL, goal, goal.goalInsistence);
                goal.goalInsistenceBound = true;
            }
        }
    }

    internal class GoalTargetROLDrawer : BlackboardFunctionInlinedObjectReferenceROLDrawer<LGOAPGoal>
    {
        private GoalTargetROLDrawer() : base("Target", BlackboardFunctionGUIHelpers.formattedDecoratorNames)
        {
        }

        protected override void ProcessCreatedObject(CustomSerializedObject<LGOAPGoal> serializedObject, BlackboardFunction newObject)
        {
            InlinedObjectReferencesHelper.Expand(newObject, out _);
            ((DecoratorBlackboardFunction) newObject).isScoreCalculator = false;

            if (serializedObject is LGOAPGoal.Serialized goal)
            {
                goal.OnBlackboardUpdate(newObject);
            }
        }

        private static readonly GoalTargetROLDrawer s_Instance = new GoalTargetROLDrawer();

        internal static void Bind(LGOAPGoal.Serialized goal)
        {
            if (!goal.goalTargetBound)
            {
                s_Instance.Bind(goal.goalTargetROL, goal, goal.goalTarget);
                goal.goalTargetBound = true;
            }
        }
    }

    internal class TaskPreconditionROLDrawer : BlackboardFunctionInlinedObjectReferenceROLDrawer<LGOAPTask>
    {
        private TaskPreconditionROLDrawer() : base("Precondition", BlackboardFunctionGUIHelpers.formattedDecoratorNames)
        {
        }

        protected override void ProcessCreatedObject(CustomSerializedObject<LGOAPTask> serializedObject, BlackboardFunction newObject)
        {
            InlinedObjectReferencesHelper.Expand(newObject, out _);
            ((DecoratorBlackboardFunction) newObject).isScoreCalculator = false;

            if (serializedObject is LGOAPTask.Serialized task)
            {
                task.OnBlackboardUpdate(newObject);
            }
        }

        private static readonly TaskPreconditionROLDrawer s_Instance = new TaskPreconditionROLDrawer();

        internal static void Bind(LGOAPTask.Serialized task)
        {
            if (!task.taskPreconditionBound)
            {
                s_Instance.Bind(task.taskPreconditionROL, task, task.taskPrecondition);
                task.taskPreconditionBound = true;
            }
        }
    }

    internal class TaskCostROLDrawer : BlackboardFunctionInlinedObjectReferenceROLDrawer<LGOAPTask>
    {
        private TaskCostROLDrawer() : base("Cost", BlackboardFunctionGUIHelpers.formattedScoreCalculatorNames)
        {
        }

        protected override void ProcessCreatedObject(CustomSerializedObject<LGOAPTask> serializedObject, BlackboardFunction newObject)
        {
            InlinedObjectReferencesHelper.Expand(newObject, out _);
            ((DecoratorBlackboardFunction) newObject).isScoreCalculator = true;

            if (serializedObject is LGOAPTask.Serialized task)
            {
                task.OnBlackboardUpdate(newObject);
            }
        }

        private static readonly TaskCostROLDrawer s_Instance = new TaskCostROLDrawer();

        internal static void Bind(LGOAPTask.Serialized task)
        {
            if (!task.taskCostBound)
            {
                s_Instance.Bind(task.taskCostROL, task, task.taskCost);
                task.taskCostBound = true;
            }
        }
    }

    internal class TaskEffectROLDrawer : BlackboardFunctionInlinedObjectReferenceROLDrawer<LGOAPTask>
    {
        private TaskEffectROLDrawer() : base("Effect", BlackboardFunctionGUIHelpers.formattedEffectorNames)
        {
        }

        protected override void ProcessCreatedObject(CustomSerializedObject<LGOAPTask> serializedObject, BlackboardFunction newObject)
        {
            InlinedObjectReferencesHelper.Expand(newObject, out _);

            if (serializedObject is LGOAPTask.Serialized task)
            {
                task.OnBlackboardUpdate(newObject);
            }
        }

        private static readonly TaskEffectROLDrawer s_Instance = new TaskEffectROLDrawer();

        internal static void Bind(LGOAPTask.Serialized task)
        {
            if (!task.taskEffectBound)
            {
                s_Instance.Bind(task.taskEffectROL, task, task.taskEffect);
                task.taskEffectBound = true;
            }
        }
    }

    internal class TaskTargetROLDrawer : BlackboardFunctionInlinedObjectReferenceROLDrawer<LGOAPTask>
    {
        private TaskTargetROLDrawer() : base("Target", BlackboardFunctionGUIHelpers.formattedDecoratorNames)
        {
        }

        protected override void ProcessCreatedObject(CustomSerializedObject<LGOAPTask> serializedObject, BlackboardFunction newObject)
        {
            InlinedObjectReferencesHelper.Expand(newObject, out _);
            ((DecoratorBlackboardFunction) newObject).isScoreCalculator = false;

            if (serializedObject is LGOAPTask.Serialized task)
            {
                task.OnBlackboardUpdate(newObject);
            }
        }

        private static readonly TaskTargetROLDrawer s_Instance = new TaskTargetROLDrawer();

        internal static void Bind(LGOAPTask.Serialized task)
        {
            if (!task.taskTargetBound)
            {
                s_Instance.Bind(task.taskTargetROL, task, task.taskTarget);
                task.taskTargetBound = true;
            }
        }
    }

    internal abstract class LGOAPComponentROLDrawer<T> : InlinedObjectReferenceROLDrawer<T, LGOAPDomain>
        where T : ScriptableObject
    {
        protected LGOAPComponentROLDrawer(string header) : base(header)
        {
        }

        protected override Color GetThemeColor(T value)
        {
            return LGOAPDomainGUIHelpers.GetComponentColor(value);
        }
    }

    internal class LGOAPGoalROLDrawer : InlinedObjectReferenceROLDrawer<LGOAPGoal, LGOAPDomain>
    {
        private LGOAPGoalROLDrawer() : base("Goals")
        {
        }

        protected override void ProcessCreatedObject(CustomSerializedObject<LGOAPDomain> serializedObject, LGOAPGoal newObject)
        {
            InlinedObjectReferencesHelper.Expand(newObject, out _);

            if (serializedObject is LGOAPDomain.Serialized d)
            {
                d.OnBlackboardUpdate(newObject);
            }
        }
    }

    internal class LGOAPAbstractTaskROLDrawer : InlinedObjectReferenceROLDrawer<LGOAPTask, LGOAPDomain>
    {
        private LGOAPAbstractTaskROLDrawer() : base("Abstract Tasks")
        {
        }

        protected override void ProcessCreatedObject(CustomSerializedObject<LGOAPDomain> serializedObject, LGOAPTask newObject)
        {
            InlinedObjectReferencesHelper.Expand(newObject, out _);
            newObject.isAbstract = true;

            if (serializedObject is LGOAPDomain.Serialized d)
            {
                d.OnBlackboardUpdate(newObject);
            }
        }
    }

    internal class LGOAPTaskROLDrawer : InlinedObjectReferenceROLDrawer<LGOAPTask, LGOAPDomain>
    {
        private LGOAPTaskROLDrawer() : base("Tasks")
        {
        }

        protected override void ProcessCreatedObject(CustomSerializedObject<LGOAPDomain> serializedObject, LGOAPTask newObject)
        {
            InlinedObjectReferencesHelper.Expand(newObject, out _);
            newObject.isAbstract = false;

            if (serializedObject is LGOAPDomain.Serialized d)
            {
                d.OnBlackboardUpdate(newObject);
            }
        }
    }
}