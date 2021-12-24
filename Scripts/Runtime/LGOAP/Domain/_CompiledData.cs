using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace HiraBots
{
    internal class LGOAPDomainCompiledData
    {
        private BlackboardTemplateCompiledData m_BlackboardCompiledData;
        private ReadOnlyArrayAccessor<byte> m_PlanSizesByLayer;
        private NativeArray<byte> m_Domain = default;

        private readonly Dictionary<int, JobHandle> m_DependentJobs = new Dictionary<int, JobHandle>();

        /// <summary>
        /// The size of a plan by the layer.
        /// </summary>
        internal ReadOnlyArrayAccessor<byte> planSizesByLayer => m_PlanSizesByLayer;

        /// <summary>
        /// The compiled data for the domain.
        /// </summary>
        internal NativeArray<byte> data => m_Domain;

        internal LGOAPDomainCompiledData(BlackboardTemplateCompiledData blackboardCompiledData, NativeArray<byte> domain,
            byte[] planSizesByLayer)
        {
            m_BlackboardCompiledData = blackboardCompiledData;
            m_Domain = domain;
            m_PlanSizesByLayer = planSizesByLayer.ReadOnly();
        }

        internal void Dispose()
        {
            m_PlanSizesByLayer = default;

            foreach (var dependentJob in m_DependentJobs.Values)
            {
                dependentJob.Complete(); // must complete all the dependent jobs before deallocating the domain.
            }

            m_DependentJobs.Clear();

            if (m_Domain.IsCreated)
            {
                m_Domain.Dispose();
            }

            m_BlackboardCompiledData = null;
        }

        /// <summary>
        /// Add a job dependent on the given compiled data.
        /// Dependent jobs will be completed before the domain gets deallocated.
        /// </summary>
        internal void AddDependentJob(int id, JobHandle dependentJob)
        {
            if (!m_DependentJobs.ContainsKey(id))
            {
                m_DependentJobs.Add(id, dependentJob);
            }
            else
            {
                Debug.Log($"Already contains a dependent job for id {id}.");
                m_DependentJobs[id] = dependentJob;
            }
        }

        /// <summary>
        /// Remove a dependent job.
        /// </summary>
        internal void RemoveDependentJob(int id)
        {
            m_DependentJobs.Remove(id);
        }
    }
}