#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HiraBots
{
    internal partial class LGOAPGoal
    {
        [field: SerializeField, HideInInspector] internal BlackboardTemplate blackboard { get; set; }

        internal class Serialized : CustomSerializedObject<LGOAPGoal>
        {
            internal Serialized(LGOAPGoal obj) : base(obj)
            {
                name = GetProperty("m_Name",
                    SerializedPropertyType.String, false, true);

                goalInsistence = GetProperty<DecoratorBlackboardFunction>($"{nameof(m_Insistence)}.{nameof(LGOAPInsistence.m_Insistence)}",
                    true, true);

                goalInsistenceROL = new ReorderableList((SerializedObject) this, goalInsistence,
                    true, true, true, true);

                goalTarget = GetProperty<DecoratorBlackboardFunction>($"{nameof(m_Target)}.{nameof(LGOAPTarget.m_Target)}",
                    true, true);

                goalTargetROL = new ReorderableList((SerializedObject) this, goalTarget,
                    true, true, true, true);
            }

            internal SerializedProperty name { get; }
            internal SerializedProperty goalInsistence { get; }
            internal bool goalInsistenceBound { get; set; } = false;
            internal ReorderableList goalInsistenceROL { get; }
            internal SerializedProperty goalTarget { get; }
            internal bool goalTargetBound { get; set; } = false;
            internal ReorderableList goalTargetROL { get; }

            private ReadOnlyHashSetAccessor<BlackboardKey> keySet
            {
                get
                {
                    var keys = new HashSet<BlackboardKey>();
                    if (target.blackboard != null)
                    {
                        target.blackboard.GetKeySet(keys);
                    }

                    return keys.ReadOnly();
                }
            }

            internal void OnBlackboardUpdate(BlackboardFunction function)
            {
                OnBlackboardUpdate(function, keySet);
            }

            private void OnBlackboardUpdate(BlackboardFunction function, ReadOnlyHashSetAccessor<BlackboardKey> keys)
            {
                function.OnTargetBlackboardTemplateChanged(target.blackboard, keys);
            }
        }
    }

    internal partial class LGOAPTask
    {
        [field: SerializeField, HideInInspector] internal BlackboardTemplate blackboard { get; set; }

        internal class Serialized : CustomSerializedObject<LGOAPTask>
        {
            private SerializedProperty m_IsAbstract;

            internal Serialized(LGOAPTask obj) : base(obj)
            {
                name = GetProperty("m_Name",
                    SerializedPropertyType.String, false, true);

                m_IsAbstract = GetProperty(nameof(m_IsAbstract),
                    SerializedPropertyType.Boolean, false, true);

                taskPrecondition = GetProperty<DecoratorBlackboardFunction>($"{nameof(m_Action)}.{nameof(LGOAPAction.m_Precondition)}",
                    true, true);

                taskPreconditionROL = new ReorderableList((SerializedObject) this, taskPrecondition,
                    true, true, true, true);

                taskCost = GetProperty<DecoratorBlackboardFunction>($"{nameof(m_Action)}.{nameof(LGOAPAction.m_Cost)}",
                    true, true);

                taskCostROL = new ReorderableList((SerializedObject) this, taskCost,
                    true, true, true, true);

                taskEffect = GetProperty<EffectorBlackboardFunction>($"{nameof(m_Action)}.{nameof(LGOAPAction.m_Effect)}",
                    true, true);

                taskEffectROL = new ReorderableList((SerializedObject) this, taskEffect,
                    true, true, true, true);

                taskTarget = GetProperty<DecoratorBlackboardFunction>($"{nameof(m_Target)}.{nameof(LGOAPTarget.m_Target)}",
                    true, true);

                taskTargetROL = new ReorderableList((SerializedObject) this, taskTarget,
                    true, true, true, true);
            }

            internal bool isAbstract => m_IsAbstract.boolValue;

            internal SerializedProperty name { get; }
            internal SerializedProperty taskPrecondition { get; }
            internal bool taskPreconditionBound { get; set; } = false;
            internal ReorderableList taskPreconditionROL { get; }
            internal SerializedProperty taskCost { get; }
            internal bool taskCostBound { get; set; } = false;
            internal ReorderableList taskCostROL { get; }
            internal SerializedProperty taskEffect { get; }
            internal bool taskEffectBound { get; set; } = false;
            internal ReorderableList taskEffectROL { get; }
            internal SerializedProperty taskTarget { get; }
            internal bool taskTargetBound { get; set; } = false;
            internal ReorderableList taskTargetROL { get; }

            private ReadOnlyHashSetAccessor<BlackboardKey> keySet
            {
                get
                {
                    var keys = new HashSet<BlackboardKey>();
                    if (target.blackboard != null)
                    {
                        target.blackboard.GetKeySet(keys);
                    }

                    return keys.ReadOnly();
                }
            }

            internal void OnBlackboardUpdate(BlackboardFunction function)
            {
                OnBlackboardUpdate(function, keySet);
            }

            private void OnBlackboardUpdate(BlackboardFunction function, ReadOnlyHashSetAccessor<BlackboardKey> keys)
            {
                function.OnTargetBlackboardTemplateChanged(target.blackboard, keys);
            }
        }
    }
}
#endif