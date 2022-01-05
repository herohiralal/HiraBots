using System.Collections.Generic;
using UnityEngine;

namespace HiraBots
{
    internal partial struct LGOAPRealtimeBotComponent
    {
        internal BlackboardComponent m_Blackboard;
        internal LGOAPPlannerComponent m_Planner;
        internal ExecutorComponent m_Executor;

        private short?[] m_ExecutionSet;
        private short?[] m_PreAllocatedExecutionSet;

        internal Queue<HiraBotsTaskProvider> m_TaskProviders;
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

                domainData.GetTaskProviders(i, containerIndex.Value, out var taskProviders);

                // clear the current queue because it's not valid anymore
                m_TaskProviders.Clear();

                // skip the first one because it'll need to be dequeued anyway
                for (var j = 1; j < taskProviders.count; j++)
                {
                    m_TaskProviders.Enqueue(taskProviders[j]);
                }

                // there's always gonna be at least one task provider (error executable)
                ExecuteTaskProvider(taskProviders[0]);
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
                        ServiceRunner.instance.Remove(service);
                    }

                    m_ActiveServicesByLayer[i].Clear();
                }

                if (newContainerIndex.HasValue)
                {
                    domainData.GetServiceProviders(i, newContainerIndex.Value, out var serviceProviders);

                    foreach (var serviceProvider in serviceProviders)
                    {
                        var service = serviceProvider.GetService(m_Blackboard, m_EffectiveArchetype);
                        m_ActiveServicesByLayer[i].Add(service);
                        ServiceRunner.instance.Add(service,
                            serviceProvider.tickInterval,
                            m_ExecutableTickIntervalMultiplier);
                    }
                }
            }
        }

        // execute a given task provider
        private void ExecuteTaskProvider(HiraBotsTaskProvider taskProvider)
        {
            TaskRunner.instance.Remove(m_Executor);
            TaskRunner.instance.Add(m_Executor,
                taskProvider.GetTask(m_Blackboard, m_EffectiveArchetype),
                taskProvider.tickInterval,
                m_ExecutableTickIntervalMultiplier);
        }
    }
}