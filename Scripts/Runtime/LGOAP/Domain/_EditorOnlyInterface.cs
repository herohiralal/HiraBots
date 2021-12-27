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
            private SerializedProperty[] m_IntermediateLayerFallbackProperties;

            internal Serialized(LGOAPDomain obj) : base(obj)
            {
                backends = GetProperty(nameof(m_Backends),
                    SerializedPropertyType.Enum, false, true);

                blackboard = GetProperty<BlackboardTemplate>(nameof(m_Blackboard),
                    false, true);

                topLayer = GetProperty<LGOAPGoal>($"{nameof(m_TopLayer)}.{nameof(LGOAPGoalLayer.m_Goals)}",
                    true, true);

                topLayerMaxPlanSize = GetProperty(nameof(m_MaxTopLayerPlanSize), SerializedPropertyType.Integer,
                    false, true);

                topLayerFallback = GetProperty($"{nameof(m_TopLayer)}.{nameof(LGOAPGoalLayer.m_FallbackGoal)}", SerializedPropertyType.Integer,
                    true, true);

                intermediateLayersProperty = GetProperty<LGOAPTaskLayer>(nameof(m_IntermediateLayers),
                    true, true);

                intermediateLayerMaxPlanSizes = GetProperty(nameof(m_MaxIntermediateLayersPlanSizes), SerializedPropertyType.Integer,
                    true, true);

                bottomLayer = GetProperty<LGOAPTask>($"{nameof(m_BottomLayer)}.{nameof(LGOAPTaskLayer.m_Tasks)}",
                    true, true);

                bottomLayerMaxPlanSize = GetProperty(nameof(m_MaxBottomLayerPlanSize), SerializedPropertyType.Integer,
                    false, true);

                bottomLayerFallback = GetProperty($"{nameof(m_BottomLayer)}.{nameof(LGOAPTaskLayer.m_FallbackPlan)}", SerializedPropertyType.Integer,
                    true, true);

                if (intermediateLayersProperty != null)
                {
                    var intermediateLayerCount = intermediateLayersProperty.arraySize;

                    m_IntermediateLayerProperties = new SerializedProperty[intermediateLayerCount];
                    for (var i = 0; i < intermediateLayerCount; i++)
                    {
                        m_IntermediateLayerProperties[i] = GetIntermediateLayerProperty(i);
                    }

                    m_IntermediateLayerFallbackProperties = new SerializedProperty[intermediateLayerCount];
                    for (var i = 0; i < intermediateLayerCount; i++)
                    {
                        m_IntermediateLayerFallbackProperties[i] = GetIntermediateLayerFallbackProperty(i);
                    }
                }
            }

            internal SerializedProperty backends { get; }
            internal SerializedProperty blackboard { get; }
            internal SerializedProperty topLayer { get; }
            internal SerializedProperty topLayerFallback { get; }
            internal SerializedProperty topLayerMaxPlanSize { get; }
            private SerializedProperty intermediateLayersProperty { get; }
            internal ReadOnlyArrayAccessor<SerializedProperty> intermediateLayers => m_IntermediateLayerProperties.ReadOnly();
            internal SerializedProperty intermediateLayerMaxPlanSizes { get; }
            internal ReadOnlyArrayAccessor<SerializedProperty> intermediateLayersFallbacks => m_IntermediateLayerFallbackProperties.ReadOnly();
            internal SerializedProperty bottomLayer { get; }
            internal SerializedProperty bottomLayerMaxPlanSize { get; }
            internal SerializedProperty bottomLayerFallback { get; }

            internal int intermediateLayersCount
            {
                get => intermediateLayersProperty.arraySize;
                set
                {
                    var originalCount = intermediateLayersProperty.arraySize;

                    if (originalCount != value)
                    {
                        intermediateLayersProperty.arraySize = value;
                        intermediateLayerMaxPlanSizes.arraySize = value;
                        System.Array.Resize(ref m_IntermediateLayerProperties, value);
                        System.Array.Resize(ref m_IntermediateLayerFallbackProperties, value);

                        var difference = value - originalCount;

                        for (var i = 0; i < difference; i++) // if original count was bigger, difference will be negative and the loop will be ignored
                        {
                            var p = GetIntermediateLayerProperty(originalCount + i);

                            if (p != null)
                            {
                                p.arraySize = 0;
                            }

                            m_IntermediateLayerProperties[originalCount + i] = p;

                            var p2 = GetIntermediateLayerFallbackProperty(originalCount + i);

                            if (p2 != null)
                            {
                                p2.arraySize = 0;
                            }

                            m_IntermediateLayerFallbackProperties[originalCount + i] = p2;

                            intermediateLayerMaxPlanSizes.GetArrayElementAtIndex(originalCount + i).intValue = 5;
                        }
                    }
                }
            }

            private SerializedProperty GetIntermediateLayerProperty(int x)
            {
                return GetProperty<LGOAPTask>($"{nameof(m_IntermediateLayers)}.Array.data[{x}].{nameof(LGOAPTaskLayer.m_Tasks)}",
                    true, true);
            }

            private SerializedProperty GetIntermediateLayerFallbackProperty(int x)
            {
                return GetProperty($"{nameof(m_IntermediateLayers)}.Array.data[{x}].{nameof(LGOAPTaskLayer.m_FallbackPlan)}",
                    SerializedPropertyType.Integer, true, true);
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

                foreach (var taskProvider in task.taskProviders)
                {
                    OnBlackboardUpdate(taskProvider, keys);
                }

                foreach (var taskProvider in task.serviceProviders)
                {
                    OnBlackboardUpdate(taskProvider, keys);
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

            internal void OnBlackboardUpdate(HiraBotsTaskProvider taskProvider)
            {
                OnBlackboardUpdate(taskProvider, keySet);
            }

            private void OnBlackboardUpdate(HiraBotsTaskProvider taskProvider, ReadOnlyHashSetAccessor<BlackboardKey> keys)
            {
                taskProvider.OnTargetBlackboardTemplateChangedWrapped(target.m_Blackboard, keys);
            }

            internal void OnBlackboardUpdate(HiraBotsServiceProvider serviceProvider)
            {
                OnBlackboardUpdate(serviceProvider, keySet);
            }

            private void OnBlackboardUpdate(HiraBotsServiceProvider serviceProvider, ReadOnlyHashSetAccessor<BlackboardKey> keys)
            {
                serviceProvider.OnTargetBlackboardTemplateChangedWrapped(target.m_Blackboard, keys);
            }
        }
    }
}
#endif