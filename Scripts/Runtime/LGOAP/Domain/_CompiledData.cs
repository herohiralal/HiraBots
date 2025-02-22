using Unity.Collections;
using UnityEngine;

namespace HiraBots
{
    internal partial class LGOAPDomainCompiledData
    {
        private BlackboardTemplateCompiledData m_BlackboardCompiledData;
        private ReadOnlyArrayAccessor<byte> m_MaxPlanSizesByLayer;
        private LGOAPPlan.Set.ReadOnly m_FallbackPlans;
        private ReadOnlyArrayAccessor<LGOAPGoalCompiledData> m_Goals;
        private ReadOnlyArrayAccessor<ReadOnlyArrayAccessor<LGOAPTaskCompiledData>> m_TaskLayers;
        private NativeArray<byte> m_Domain = default;

        /// <summary>
        /// The compiled template.
        /// </summary>
        internal BlackboardTemplateCompiledData blackboardTemplate => m_BlackboardCompiledData;

        /// <summary>
        /// The size of a plan by the layer.
        /// </summary>
        internal ReadOnlyArrayAccessor<byte> maxPlanSizesByLayer => m_MaxPlanSizesByLayer;

        /// <summary>
        /// The fallback plans by each layer.
        /// </summary>
        internal LGOAPPlan.Set.ReadOnly fallbackPlans => m_FallbackPlans;

        /// <summary>
        /// The number of layers (excluding the goal layer).
        /// </summary>
        internal int layerCount => m_MaxPlanSizesByLayer.count;

        /// <summary>
        /// The compiled data for the domain.
        /// </summary>
        internal NativeArray<byte> data => m_Domain;

        internal LGOAPDomainCompiledData(BlackboardTemplateCompiledData blackboardCompiledData, NativeArray<byte> domain,
            ReadOnlyArrayAccessor<byte> maxPlanSizesByLayer, ReadOnlyArrayAccessor<short[]> fallbackPlans,
            ReadOnlyArrayAccessor<LGOAPGoalCompiledData> goals, ReadOnlyArrayAccessor<ReadOnlyArrayAccessor<LGOAPTaskCompiledData>> taskLayers)
        {
            m_BlackboardCompiledData = blackboardCompiledData;
            m_Domain = domain;
            m_FallbackPlans = new LGOAPPlan.Set.ReadOnly(fallbackPlans);
            m_MaxPlanSizesByLayer = maxPlanSizesByLayer;

            m_Goals = goals;
            m_TaskLayers = taskLayers;
        }

        internal void Dispose()
        {
            m_TaskLayers = default;
            m_Goals = default;

            m_MaxPlanSizesByLayer = default;

            m_FallbackPlans.Dispose();

            if (m_Domain.IsCreated)
            {
                m_Domain.Dispose();
            }

            m_BlackboardCompiledData = null;
        }
    }
}