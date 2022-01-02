#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace HiraBots
{
    internal sealed partial class HiraLGOAPBot
    {
        internal class Serialized : CustomSerializedObject<HiraLGOAPBot>
        {
            internal Serialized(HiraLGOAPBot obj) : base(obj)
            {
                archetypeProperty = GetProperty<Component>(nameof(m_ArchetypeOverride), false, true);

                domainProperty = GetProperty<LGOAPDomain>(nameof(m_Domain), false, true);

                timeDilationProperty = GetProperty(nameof(m_TimeDilation), SerializedPropertyType.Float, false, true);
            }

            internal SerializedProperty archetypeProperty { get; }
            internal SerializedProperty domainProperty { get; }
            internal SerializedProperty timeDilationProperty { get; }

            internal LGOAPDomain domain => target.m_Domain;
            internal BlackboardComponent blackboard => target.m_Blackboard;
            internal LGOAPPlannerComponent planner => target.m_Planner;
            internal ExecutorComponent executor => target.m_Executor;
            internal int currentTaskProvidersQueueLength => target.m_TaskProviders.Count;
        }
    }
}
#endif