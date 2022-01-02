using System.Collections.Generic;
using UnityEngine;

namespace HiraBots
{
    internal sealed partial class HiraLGOAPBot : MonoBehaviour, IHiraBotArchetype
    {
        [Tooltip("The component to use as an archetype. If not provided, will use self.")]
        [SerializeField] private Component m_ArchetypeOverride;

        [Tooltip("The domain to use for this HiraBot.")]
        [SerializeField] private LGOAPDomain m_Domain;

        private void OnValidate()
        {
            if (!(m_ArchetypeOverride is IHiraBotArchetype))
            {
                m_ArchetypeOverride = null;
            }
        }

        private LGOAPDomain m_DomainCurrentlyInUse = null;

        private BlackboardComponent m_Blackboard = null;
        private LGOAPPlannerComponent m_Planner = null;
        private ExecutorComponent m_Executor = null;

        private short?[] m_ExecutionSet = null;
        private short?[] m_PreAllocatedExecutionSet = null;
        private Queue<HiraBotsTaskProvider> m_TaskProviders = null;
        private List<IHiraBotsService>[] m_ActiveServicesByLayer = null;

        private IHiraBotArchetype archetype => (IHiraBotArchetype) (m_ArchetypeOverride == null ? this : m_ArchetypeOverride);

        private void Awake()
        {
            StartUsingNewDomain();
        }

        private void OnDestroy()
        {
            StopUsingOldDomain();
        }

        private void Update()
        {
            if (!ReferenceEquals(m_Domain, m_DomainCurrentlyInUse))
            {
                StopUsingOldDomain();
                StartUsingNewDomain();
            }

            if (ReferenceEquals(m_DomainCurrentlyInUse, null))
            {
                return;
            }

            if (m_Blackboard.hasUnexpectedChanges)
            {
                m_Planner.StartPlannerAtLayer(0, true);
                m_Blackboard.ClearUnexpectedChanges();
            }

            // if the planner can provide a plan regardless of the executor being done
            if (m_Planner.canProvidePlan)
            {
                // grab the plan and provide it to the executor
                GrabPlannerResults();
                m_Planner.canProvidePlan = false;

                // silence the executor, there's a new plan which invalidates its finish status
                m_Executor.lastTaskEndSucceeded = null;
            }

            // if the executor is done executing
            if (!m_Executor.hasTask && m_Executor.lastTaskEndSucceeded.HasValue)
            {
                if (!m_Executor.lastTaskEndSucceeded.Value)
                {
                    m_Planner.OnTaskExecutionComplete(false);
                }
                else if (m_TaskProviders.Count > 0)
                {
                    ExecuteTaskProvider(m_TaskProviders.Dequeue());
                }
                else
                {
                    m_Planner.OnTaskExecutionComplete(true);
                }

                m_Executor.lastTaskEndSucceeded = null;
            }

            // if the planner is providing a plan because the executor asked for it
            if (m_Planner.canProvidePlan)
            {
                // grab the plan and provide it to the executor
                GrabPlannerResults();
                m_Planner.canProvidePlan = false;
            }
        }

        private void StartUsingNewDomain()
        {
            if (ReferenceEquals(m_Domain, null) || !m_Domain.isCompiled)
            {
                return;
            }

            if (!BlackboardComponent.TryCreate(m_Domain.compiledData.blackboardTemplate, out m_Blackboard))
            {
                return;
            }

            if (!LGOAPPlannerComponent.TryCreate(m_Blackboard, m_Domain.compiledData, out m_Planner))
            {
                m_Blackboard.Dispose();
                m_Blackboard = null;

                return;
            }

            if (!ExecutorComponent.TryCreate(out m_Executor))
            {
                m_Planner.Dispose();
                m_Planner = null;

                m_Blackboard.Dispose();
                m_Blackboard = null;

                return;
            }

            var layerCount = m_Domain.compiledData.layerCount;

            m_ExecutionSet = new short?[layerCount];
            m_PreAllocatedExecutionSet = new short?[layerCount];

            m_TaskProviders = new Queue<HiraBotsTaskProvider>();

            m_ActiveServicesByLayer = new List<IHiraBotsService>[layerCount];

            for (var i = 0; i < layerCount; i++)
            {
                m_ActiveServicesByLayer[i] = new List<IHiraBotsService>(layerCount);
            }

            m_DomainCurrentlyInUse = m_Domain;

            m_Planner.StartPlannerAtLayer(0, true);
        }

        private void StopUsingOldDomain()
        {
            if (ReferenceEquals(m_DomainCurrentlyInUse, null))
            {
                return;
            }

            foreach (var servicesInLayer in m_ActiveServicesByLayer)
            {
                servicesInLayer.Clear();
            }

            m_ActiveServicesByLayer = null;

            m_TaskProviders.Clear();
            m_TaskProviders = null;

            m_PreAllocatedExecutionSet = null;
            m_ExecutionSet = null;

            m_Executor.Dispose();
            m_Executor = null;

            m_Planner.Dispose();
            m_Planner = null;

            m_Blackboard.Dispose();
            m_Blackboard = null;

            m_DomainCurrentlyInUse = null;
        }

        private void GrabPlannerResults()
        {
            m_Planner.CollectExecutionSet(m_PreAllocatedExecutionSet);

            var domainData = m_DomainCurrentlyInUse.compiledData;
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
                        HiraBotsServiceRunner.Remove(service);
                    }

                    m_ActiveServicesByLayer[i].Clear();
                }

                if (newContainerIndex.HasValue)
                {
                    domainData.GetServiceProviders(i, newContainerIndex.Value, out var serviceProviders);

                    foreach (var serviceProvider in serviceProviders)
                    {
                        var service = serviceProvider.GetService(m_Blackboard, archetype);
                        m_ActiveServicesByLayer[i].Add(service);
                        HiraBotsServiceRunner.Add(service, serviceProvider.tickInterval);
                    }
                }
            }
        }

        // execute a given task provider
        private void ExecuteTaskProvider(HiraBotsTaskProvider taskProvider)
        {
            var task = taskProvider.GetTask(m_Blackboard, archetype);
            m_Executor.Execute(task, taskProvider.tickInterval);
        }
    }
}