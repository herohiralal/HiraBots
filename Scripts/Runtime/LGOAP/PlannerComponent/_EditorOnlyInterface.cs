#if UNITY_EDITOR
namespace HiraBots
{
    internal partial class LGOAPPlannerComponent
    {
        internal readonly struct Serialized
        {
            private readonly LGOAPPlannerComponent m_Internal;

            internal Serialized(LGOAPPlannerComponent value)
            {
                m_Internal = value;
            }

            internal LGOAPPlan.Set executionSet => m_Internal.m_PlansForExecution;
        }
    }
}
#endif