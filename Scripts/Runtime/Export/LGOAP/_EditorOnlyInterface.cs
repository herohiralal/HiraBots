#if UNITY_EDITOR
using HiraBots;
using UnityEditor;

namespace UnityEngine
{
    public sealed partial class HiraLGOAPRealtimeBot
    {
        private void OnValidate()
        {
            archetypeOverride = m_ArchetypeOverride;
            domain = m_Domain;
            m_Internal.executableTickIntervalMultiplier = m_Internal.m_ExecutableTickIntervalMultiplier;
        }

        internal class Serialized : CustomSerializedObject<HiraLGOAPRealtimeBot>
        {
            internal Serialized(HiraLGOAPRealtimeBot obj) : base(obj)
            {
                archetypeProperty = GetProperty<Component>(nameof(m_ArchetypeOverride), false, true);

                domainProperty = GetProperty<LGOAPDomain>(nameof(m_Domain), false, true);

                executableTickIntervalMultiplierProperty = GetProperty($"{nameof(m_Internal)}.{nameof(LGOAPRealtimeBotComponent.m_ExecutableTickIntervalMultiplier)}",
                    SerializedPropertyType.Float, false, true);

                runPlannerSynchronouslyProperty = GetProperty(nameof(m_RunPlannerSynchronously), SerializedPropertyType.Boolean,
                    false, true);
            }

            internal SerializedProperty archetypeProperty { get; }
            internal SerializedProperty domainProperty { get; }
            internal SerializedProperty executableTickIntervalMultiplierProperty { get; }
            internal SerializedProperty runPlannerSynchronouslyProperty { get; }

            internal HiraBots.LGOAPDomain domain => target.m_Domain;
            internal HiraBots.BlackboardComponent blackboard => target.m_Internal.m_Blackboard;
            internal LGOAPPlannerComponent planner => target.m_Internal.m_Planner;
            internal ExecutorComponent executor => target.m_Internal.m_Executor;
            internal int currentTaskProvidersQueueLength => target.m_Internal.m_TaskProviders.Count;
        }
    }
}
#endif