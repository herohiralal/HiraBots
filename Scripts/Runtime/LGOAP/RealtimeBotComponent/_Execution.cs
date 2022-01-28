using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HiraBots
{
    internal partial struct LGOAPRealtimeBotComponent
    {
        internal BlackboardComponent m_Blackboard;
        internal LGOAPPlannerComponent m_Planner;
        internal ExecutorComponent m_Executor;

        private short?[] m_ExecutionSet;
        private short?[] m_PreAllocatedExecutionSet;

        // internal Queue<HiraBotsTaskProvider> m_TaskProviders;
        internal int m_CurrentTaskLayerIndex;
        internal int m_CurrentTaskContainerIndex;
        internal int m_CurrentTaskTaskProviderIndex;
        internal List<IHiraBotsService>[] m_ActiveServicesByLayer;

        internal bool runPlannerSynchronously
        {
            set
            {
                if (m_Planner != null)
                {
                    m_Planner.planSynchronously = value;
                }
            }
        }

        private void GrabPlannerResults()
        {
            m_Planner.CollectExecutionSet(m_PreAllocatedExecutionSet);

            var domainData = m_Domain.compiledData;
            var layerCount = domainData.layerCount;

            for (var i = layerCount - 1; i > 0; i--)
            {
                var containerIndex = m_PreAllocatedExecutionSet[i];

                if (!containerIndex.HasValue)
                {
                    continue;
                }

                m_CurrentTaskLayerIndex = i;
                m_CurrentTaskContainerIndex = containerIndex.Value;
                // there's always gonna be at least one task provider (error executable)
                m_CurrentTaskTaskProviderIndex = 0;

                if (!ExecuteCurrentTaskProvider())
                {
                    Debug.LogError($"Could not execute the container {domainData.GetContainerName(i, containerIndex.Value)} " +
                                   $"at layer {i} in domain {m_Domain.name}.");
                }
                break;
            }

            for (var i = layerCount - 1; i > 0; i--) // ignore goal layer
            {
                var existingValue = m_ExecutionSet[i];
                var newContainerIndex = m_ExecutionSet[i] = m_PreAllocatedExecutionSet[i];

                if (existingValue == newContainerIndex)
                {
                    continue;
                }

                if (existingValue.HasValue)
                {
                    foreach (var service in m_ActiveServicesByLayer[i])
                    {
                        ServiceRunner.Remove(service);
                    }

                    m_ActiveServicesByLayer[i].Clear();
                }

                if (newContainerIndex.HasValue)
                {
                    domainData.GetServiceProviders(i, newContainerIndex.Value, out var serviceProviders);

                    foreach (var serviceProvider in serviceProviders)
                    {
                        var service = serviceProvider.WrappedGetService(m_Blackboard, m_EffectiveArchetype)
                                      ?? ErrorExecutable.Get($"Service provider {serviceProvider.name} in container " +
                                                             $"{domainData.GetContainerName(i, newContainerIndex.Value)} at " +
                                                             $"layer index {i} in domain {m_Domain.name}.");
                        m_ActiveServicesByLayer[i].Add(service);
                        ServiceRunner.Add(service,
                            serviceProvider.tickInterval,
                            m_ExecutableTickIntervalMultiplier);
                    }
                }
            }
        }

        // execute the current task provider
        private bool ExecuteCurrentTaskProvider()
        {
            var domainData = m_Domain.compiledData;

            domainData.GetTaskProviders(m_CurrentTaskLayerIndex,
                m_CurrentTaskContainerIndex, out var providers);

            if (m_CurrentTaskTaskProviderIndex >= providers.count)
            {
                return false;
            }

            var taskProvider = providers[m_CurrentTaskTaskProviderIndex];

            var task = taskProvider.WrappedGetTask(m_Blackboard, m_EffectiveArchetype)
                       ?? ErrorExecutable.Get($"Task provider {taskProvider.name} in container " +
                                              $"{domainData.GetContainerName(m_CurrentTaskLayerIndex, m_CurrentTaskContainerIndex)}" +
                                              $" at layer index {m_CurrentTaskLayerIndex} in domain {m_Domain.name}.");

            TaskRunner.Remove(m_Executor);
            TaskRunner.Add(m_Executor,
                task,
                taskProvider.tickInterval,
                m_ExecutableTickIntervalMultiplier);

            return true;
        }
    }
}