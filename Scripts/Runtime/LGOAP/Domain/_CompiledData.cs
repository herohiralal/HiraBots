using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace HiraBots
{
    internal class LGOAPDomainCompiledData
    {
        private BlackboardTemplateCompiledData m_BlackboardCompiledData;
        private readonly byte[] m_PlanSizesByLayer;
        private NativeArray<byte> m_Domain = default;
        private readonly Dictionary<int, (BlackboardComponent, PlannerResult[])> m_InitializedAgents = new Dictionary<int, (BlackboardComponent, PlannerResult[])>();

        /// <summary>
        /// Create per-agent data for an LGOAP agent.
        /// </summary>
        internal bool InitializeLGOAPAgent(Object target, out BlackboardComponent blackboard, out PlannerResult[] plannerResults)
        {
            var instanceID = target.GetInstanceID();

            if (m_InitializedAgents.ContainsKey(instanceID))
            {
                blackboard = null;
                plannerResults = null;
                return false;
            }

            BlackboardComponent.TryCreate(m_BlackboardCompiledData, out blackboard);

            var layerCount = m_PlanSizesByLayer.Length;

            plannerResults = new PlannerResult[layerCount + 1];

            plannerResults[0] = new PlannerResult(1, Allocator.Persistent); // for goal layer

            for (var i = 1; i < layerCount; i++)
            {
                plannerResults[i] = new PlannerResult(m_PlanSizesByLayer[i], Allocator.Persistent);
            }

            m_InitializedAgents.Add(instanceID, (blackboard, plannerResults));

            return true;
        }

        /// <summary>
        /// Dispose per-agent data for an LGOAP agent.
        /// </summary>
        internal void DisposeLGOAPAgent(Object target)
        {
            var instanceID = target.GetInstanceID();

            if (m_InitializedAgents.TryGetValue(instanceID, out var data))
            {
                data.Item1.Dispose();

                for (var i = 0; i < data.Item2.Length; i++)
                {
                    data.Item2[i].Dispose();
                }

                m_InitializedAgents.Remove(instanceID);
            }
        }

        internal LGOAPDomainCompiledData(BlackboardTemplateCompiledData blackboardCompiledData, NativeArray<byte> domain,
            byte[] planSizesByLayer)
        {
            m_BlackboardCompiledData = blackboardCompiledData;
            m_Domain = domain;
            m_PlanSizesByLayer = planSizesByLayer;
        }

        internal void Dispose()
        {
            foreach (var (blackboard, plannerResultSet) in m_InitializedAgents.Values)
            {
                blackboard.Dispose();

                for (var i = 0; i < plannerResultSet.Length; i++)
                {
                    plannerResultSet[i].Dispose();
                }
            }

            m_InitializedAgents.Clear();

            if (m_Domain.IsCreated)
            {
                m_Domain.Dispose();
            }

            m_BlackboardCompiledData = null;
        }
    }
}