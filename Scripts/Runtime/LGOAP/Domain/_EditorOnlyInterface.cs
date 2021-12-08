#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HiraBots
{
    internal partial class LGOAPDomain
    {
        internal class Serialized : CustomSerializedObject<LGOAPDomain>
        {
            private SerializedProperty[] m_IntermediateLayerProperties;

            internal Serialized(LGOAPDomain obj) : base(obj)
            {
                backends = GetProperty(nameof(m_Backends),
                    SerializedPropertyType.Enum, false, true);

                blackboard = GetProperty<BlackboardTemplate>(nameof(m_Blackboard),
                    false, true);

                topLayer = GetProperty<LGOAPGoal>($"{nameof(m_TopLayer)}.{nameof(LGOAPGoalLayer.m_Goals)}",
                    true, true);

                topLayerROL = new ReorderableList((SerializedObject) this, topLayer,
                    true, true, true, true);

                intermediateLayers = GetProperty<LGOAPTaskLayer>(nameof(m_IntermediateLayers),
                    true, true);

                bottomLayer = GetProperty<LGOAPTask>($"{nameof(m_BottomLayer)}.{nameof(LGOAPTaskLayer.m_Tasks)}",
                    true, true);
            }

            internal SerializedProperty backends { get; }
            internal SerializedProperty blackboard { get; }
            internal SerializedProperty topLayer { get; }
            internal ReorderableList topLayerROL { get; }
            internal SerializedProperty intermediateLayers { get; }
            internal SerializedProperty bottomLayer { get; }

            private ReadOnlyHashSetAccessor<BlackboardKey> keySet
            {
                get
                {
                    var keys = new HashSet<BlackboardKey>();
                    if (target.m_Blackboard != null)
                    {
                        target.m_Blackboard.GetKeySet(keys);
                    }

                    return keys.ReadOnly();
                }
            }

            internal void OnBlackboardUpdate()
            {
                var readOnlyKeySet = keySet;

                foreach (var topLayerGoal in target.m_TopLayer.m_Goals)
                {
                    OnBlackboardUpdate(topLayerGoal, readOnlyKeySet);
                }

                foreach (var layer in target.m_IntermediateLayers)
                {
                    foreach (var intermediateLayerTask in layer.m_Tasks)
                    {
                        OnBlackboardUpdate(intermediateLayerTask, readOnlyKeySet);
                    }
                }

                foreach (var bottomLayerTask in target.m_BottomLayer.m_Tasks)
                {
                    OnBlackboardUpdate(bottomLayerTask, readOnlyKeySet);
                }
            }

            internal void OnBlackboardUpdate(LGOAPGoal goal)
            {
                OnBlackboardUpdate(goal, keySet);
            }

            internal void OnBlackboardUpdate(LGOAPTask task)
            {
                OnBlackboardUpdate(task, keySet);
            }

            private void OnBlackboardUpdate(LGOAPGoal goal, ReadOnlyHashSetAccessor<BlackboardKey> keys)
            {
                goal.blackboard = target.m_Blackboard;

                foreach (var scoreCalculator in goal.insistence.m_Insistence)
                {
                    OnBlackboardUpdate(scoreCalculator, keys);
                }

                foreach (var decorator in goal.target.m_Target)
                {
                    OnBlackboardUpdate(decorator, keys);
                }
            }

            private void OnBlackboardUpdate(LGOAPTask task, ReadOnlyHashSetAccessor<BlackboardKey> keys)
            {
                task.blackboard = target.m_Blackboard;

                var action = task.action;

                foreach (var decorator in action.m_Precondition)
                {
                    OnBlackboardUpdate(decorator, keys);
                }

                foreach (var scoreCalculator in action.m_Cost)
                {
                    OnBlackboardUpdate(scoreCalculator, keys);
                }

                foreach (var effector in action.m_Effect)
                {
                    OnBlackboardUpdate(effector, keys);
                }

                foreach (var decorator in task.target.m_Target)
                {
                    OnBlackboardUpdate(decorator, keys);
                }
            }

            internal void OnBlackboardUpdate(BlackboardFunction function)
            {
                OnBlackboardUpdate(function, keySet);
            }

            private void OnBlackboardUpdate(BlackboardFunction function, ReadOnlyHashSetAccessor<BlackboardKey> keys)
            {
                function.OnTargetBlackboardTemplateChanged(target.m_Blackboard, keys);
            }
        }
    }
}
#endif