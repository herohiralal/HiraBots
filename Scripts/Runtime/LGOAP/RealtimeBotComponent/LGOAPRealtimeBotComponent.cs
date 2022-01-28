using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    [System.Serializable]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto)]
    internal partial struct LGOAPRealtimeBotComponent
    {
        internal IHiraBotArchetype m_EffectiveArchetype;
        internal readonly LGOAPDomain m_Domain;

        private float m_ExecutableTickIntervalMultiplier;

        internal LGOAPRealtimeBotComponent(LGOAPDomain domain)
        {
            if (ReferenceEquals(domain, null))
            {
                m_Domain = null;
                m_EffectiveArchetype = null;
                m_ExecutableTickIntervalMultiplier = 1f;
                m_Blackboard = null;
                m_Planner = null;
                m_Executor = null;
                m_ExecutionSet = null;
                m_PreAllocatedExecutionSet = null;
                m_CurrentTaskLayerIndex = m_CurrentTaskContainerIndex = m_CurrentTaskTaskProviderIndex = 0;
                m_ActiveServicesByLayer = null;
                return;
            }

            if (!domain.isCompiled)
            {
                Debug.LogError($"Cannot use un-compiled domain: {domain}.");
                m_Domain = null;
                m_EffectiveArchetype = null;
                m_ExecutableTickIntervalMultiplier = 1f;
                m_Blackboard = null;
                m_Planner = null;
                m_Executor = null;
                m_ExecutionSet = null;
                m_PreAllocatedExecutionSet = null;
                m_CurrentTaskLayerIndex = m_CurrentTaskContainerIndex = m_CurrentTaskTaskProviderIndex = 0;
                m_ActiveServicesByLayer = null;
                return;
            }

            if (!BlackboardComponent.TryCreate(domain.compiledData.blackboardTemplate, out m_Blackboard))
            {
                m_Domain = null;
                m_EffectiveArchetype = null;
                m_ExecutableTickIntervalMultiplier = 1f;
                m_Blackboard = null;
                m_Planner = null;
                m_Executor = null;
                m_ExecutionSet = null;
                m_PreAllocatedExecutionSet = null;
                m_CurrentTaskLayerIndex = m_CurrentTaskContainerIndex = m_CurrentTaskTaskProviderIndex = 0;
                m_ActiveServicesByLayer = null;
                return;
            }

            if (!LGOAPPlannerComponent.TryCreate(m_Blackboard, domain.compiledData, out m_Planner))
            {
                m_Blackboard.Dispose();

                m_Domain = null;
                m_EffectiveArchetype = null;
                m_ExecutableTickIntervalMultiplier = 1f;
                m_Blackboard = null;
                m_Planner = null;
                m_Executor = null;
                m_ExecutionSet = null;
                m_PreAllocatedExecutionSet = null;
                m_CurrentTaskLayerIndex = m_CurrentTaskContainerIndex = m_CurrentTaskTaskProviderIndex = 0;
                m_ActiveServicesByLayer = null;
                return;
            }

            if (!ExecutorComponent.TryCreate(out m_Executor))
            {
                m_Planner.Dispose();
                m_Blackboard.Dispose();

                m_Domain = null;
                m_EffectiveArchetype = null;
                m_ExecutableTickIntervalMultiplier = 1f;
                m_Blackboard = null;
                m_Planner = null;
                m_Executor = null;
                m_ExecutionSet = null;
                m_PreAllocatedExecutionSet = null;
                m_CurrentTaskLayerIndex = m_CurrentTaskContainerIndex = m_CurrentTaskTaskProviderIndex = 0;
                m_ActiveServicesByLayer = null;
                return;
            }

            var layerCount = domain.compiledData.layerCount;

            m_ExecutionSet = new short?[layerCount];
            m_PreAllocatedExecutionSet = new short?[layerCount];

            m_CurrentTaskLayerIndex = m_CurrentTaskContainerIndex = m_CurrentTaskTaskProviderIndex = 0;

            m_ActiveServicesByLayer = new List<IHiraBotsService>[layerCount];

            for (var i = 0; i < layerCount; i++)
            {
                m_ActiveServicesByLayer[i] = new List<IHiraBotsService>(layerCount);
            }

            m_Domain = domain;
            m_EffectiveArchetype = null;
            m_ExecutableTickIntervalMultiplier = 1f;

            m_Planner.StartPlannerAtLayer(0, true);
        }

        internal void Dispose()
        {
            if (ReferenceEquals(m_Domain, null))
            {
                return;
            }

            foreach (var servicesInLayer in m_ActiveServicesByLayer)
            {
                foreach (var service in servicesInLayer)
                {
                    ServiceRunner.Remove(service);
                }

                servicesInLayer.Clear();
            }

            m_ActiveServicesByLayer = null;

            m_CurrentTaskLayerIndex = m_CurrentTaskContainerIndex = m_CurrentTaskTaskProviderIndex = -1;

            m_PreAllocatedExecutionSet = null;
            m_ExecutionSet = null;

            TaskRunner.Remove(m_Executor);
            m_Executor.Dispose();
            m_Executor = null;

            m_Planner.Dispose();
            m_Planner = null;

            m_Blackboard.Dispose();
            m_Blackboard = null;
        }
    }
}