#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
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

                intermediateLayersProperty = GetProperty<LGOAPTaskLayer>(nameof(m_IntermediateLayers),
                    true, true);

                bottomLayer = GetProperty<LGOAPTask>($"{nameof(m_BottomLayer)}.{nameof(LGOAPTaskLayer.m_Tasks)}",
                    true, true);

                if (intermediateLayersProperty != null)
                {
                    var intermediateLayerCount = intermediateLayersProperty.arraySize;
                    m_IntermediateLayerProperties = new SerializedProperty[intermediateLayerCount];
                    for (var i = 0; i < intermediateLayerCount; i++)
                    {
                        m_IntermediateLayerProperties[i] = GetIntermediateLayerProperty(i);
                    }
                }
            }

            internal SerializedProperty backends { get; }
            internal SerializedProperty blackboard { get; }
            internal SerializedProperty topLayer { get; }
            private SerializedProperty intermediateLayersProperty { get; }
            internal ReadOnlyArrayAccessor<SerializedProperty> intermediateLayers => m_IntermediateLayerProperties.ReadOnly();
            internal SerializedProperty bottomLayer { get; }

            internal int intermediateLayersCount
            {
                get => intermediateLayersProperty.arraySize;
                set
                {
                    var originalCount = intermediateLayersProperty.arraySize;

                    if (originalCount != value)
                    {
                        intermediateLayersProperty.arraySize = value;
                        System.Array.Resize(ref m_IntermediateLayerProperties, value);

                        var difference = value - originalCount;

                        for (var i = 0; i < difference; i++) // if original count was bigger, difference will be negative and the loop will be ignored
                        {
                            var p = GetIntermediateLayerProperty(originalCount + i);

                            if (p != null)
                            {
                                p.arraySize = 0;
                            }

                            m_IntermediateLayerProperties[originalCount + i] = p;
                        }
                    }
                }
            }

            private SerializedProperty GetIntermediateLayerProperty(int x)
            {
                return GetProperty<LGOAPTask>($"{nameof(m_IntermediateLayers)}.Array.data[{x}].{nameof(LGOAPTaskLayer.m_Tasks)}",
                    true, true);
            }

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