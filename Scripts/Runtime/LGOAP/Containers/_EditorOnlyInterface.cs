#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

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

                goalInsistence = GetProperty<HiraBotsScoreCalculatorBlackboardFunction>($"{nameof(m_Insistence)}.{nameof(LGOAPInsistence.m_Insistence)}",
                    true, true);

                goalInsistenceROL = new ReorderableList((SerializedObject) this, goalInsistence,
                    true, true, true, true);

                goalTarget = GetProperty<HiraBotsDecoratorBlackboardFunction>($"{nameof(m_Target)}.{nameof(LGOAPTarget.m_Target)}",
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

            internal void OnBlackboardUpdate(HiraBotsBlackboardFunction function)
            {
                OnBlackboardUpdate(function, keySet);
            }

            private void OnBlackboardUpdate(HiraBotsBlackboardFunction function, ReadOnlyHashSetAccessor<BlackboardKey> keys)
            {
                if (function != null)
                {
                    function.OnTargetBlackboardTemplateChangedWrapped(target.blackboard, keys);
                }
            }
        }
    }

    internal partial class LGOAPTask
    {
        [field: SerializeField, HideInInspector] internal BlackboardTemplate blackboard { get; set; }
        [field: SerializeField, HideInInspector] internal bool canBeAbstract { get; set; }

        internal class Serialized : CustomSerializedObject<LGOAPTask>
        {
            internal Serialized(LGOAPTask obj) : base(obj)
            {
                name = GetProperty("m_Name",
                    SerializedPropertyType.String, false, true);

                loop = GetProperty(nameof(m_Loop), SerializedPropertyType.Boolean,
                    false, true);

                taskPrecondition = GetProperty<HiraBotsDecoratorBlackboardFunction>($"{nameof(m_Action)}.{nameof(LGOAPAction.m_Precondition)}",
                    true, true);

                taskPreconditionROL = new ReorderableList((SerializedObject) this, taskPrecondition,
                    true, true, true, true);

                taskCost = GetProperty<HiraBotsScoreCalculatorBlackboardFunction>($"{nameof(m_Action)}.{nameof(LGOAPAction.m_Cost)}",
                    true, true);

                taskCostROL = new ReorderableList((SerializedObject) this, taskCost,
                    true, true, true, true);

                taskEffect = GetProperty<HiraBotsEffectorBlackboardFunction>($"{nameof(m_Action)}.{nameof(LGOAPAction.m_Effect)}",
                    true, true);

                taskEffectROL = new ReorderableList((SerializedObject) this, taskEffect,
                    true, true, true, true);

                taskTarget = GetProperty<HiraBotsDecoratorBlackboardFunction>($"{nameof(m_Target)}.{nameof(LGOAPTarget.m_Target)}",
                    true, true);

                taskTargetROL = new ReorderableList((SerializedObject) this, taskTarget,
                    true, true, true, true);

                taskTaskProviders = GetProperty<HiraBotsTaskProvider>(nameof(m_TaskProviders),
                    true, true);

                taskTaskProvidersROL = new ReorderableList((SerializedObject) this, taskTaskProviders,
                    true, true, true, true);

                taskServiceProviders = GetProperty<HiraBotsServiceProvider>(nameof(m_ServiceProviders),
                    true, true);

                taskServiceProvidersROL = new ReorderableList((SerializedObject) this, taskServiceProviders,
                    true, true, true, true);
            }

            internal bool canBeAbstract => target.canBeAbstract;

            internal SerializedProperty name { get; }
            internal SerializedProperty loop { get; }
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
            internal SerializedProperty taskTaskProviders { get; }
            internal bool taskTaskProvidersBound { get; set; } = false;
            internal ReorderableList taskTaskProvidersROL { get; }
            internal SerializedProperty taskServiceProviders { get; }
            internal bool taskServiceProvidersBound { get; set; } = false;
            internal ReorderableList taskServiceProvidersROL { get; }

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

            internal void OnBlackboardUpdate(HiraBotsBlackboardFunction function)
            {
                OnBlackboardUpdate(function, keySet);
            }

            private void OnBlackboardUpdate(HiraBotsBlackboardFunction function, ReadOnlyHashSetAccessor<BlackboardKey> keys)
            {
                function.OnTargetBlackboardTemplateChangedWrapped(target.blackboard, keys);
            }

            internal void OnBlackboardUpdate(HiraBotsTaskProvider taskProvider)
            {
                OnBlackboardUpdate(taskProvider, keySet);
            }

            private void OnBlackboardUpdate(HiraBotsTaskProvider taskProvider, ReadOnlyHashSetAccessor<BlackboardKey> keys)
            {
                taskProvider.OnTargetBlackboardTemplateChangedWrapped(target.blackboard, keys);
            }

            internal void OnBlackboardUpdate(HiraBotsServiceProvider serviceProvider)
            {
                OnBlackboardUpdate(serviceProvider, keySet);
            }

            private void OnBlackboardUpdate(HiraBotsServiceProvider serviceProvider, ReadOnlyHashSetAccessor<BlackboardKey> keys)
            {
                serviceProvider.OnTargetBlackboardTemplateChangedWrapped(target.blackboard, keys);
            }
        }
    }
}
#endif