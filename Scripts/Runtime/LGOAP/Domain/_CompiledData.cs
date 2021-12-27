﻿using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace HiraBots
{
    internal class LGOAPDomainCompiledData
    {
        private BlackboardTemplateCompiledData m_BlackboardCompiledData;
        private ReadOnlyArrayAccessor<byte> m_MaxPlanSizesByLayer;
        private LGOAPPlan.Set.ReadOnly m_FallbackPlans;
        private NativeArray<byte> m_Domain = default;

        private readonly Dictionary<ulong, JobHandle> m_DependentJobs = new Dictionary<ulong, JobHandle>();

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
            ReadOnlyArrayAccessor<byte> maxPlanSizesByLayer, ReadOnlyArrayAccessor<short[]> fallbackPlans)
        {
            m_BlackboardCompiledData = blackboardCompiledData;
            m_Domain = domain;
            m_FallbackPlans = new LGOAPPlan.Set.ReadOnly(fallbackPlans);
            m_MaxPlanSizesByLayer = maxPlanSizesByLayer;
        }

        internal void Dispose()
        {
            m_MaxPlanSizesByLayer = default;

            foreach (var dependentJob in m_DependentJobs.Values)
            {
                dependentJob.Complete(); // must complete all the dependent jobs before deallocating the domain.
            }

            m_DependentJobs.Clear();

            m_FallbackPlans.Dispose();

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
        internal void AddDependentJob(ulong id, JobHandle dependentJob)
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
        internal void RemoveDependentJob(ulong id)
        {
            m_DependentJobs.Remove(id);
        }
    }
}