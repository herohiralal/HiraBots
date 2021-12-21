﻿using UnityEngine;

namespace HiraBots
{
    internal partial class LGOAPComponent
    {
        private static ulong s_Id = 0;

        private readonly ulong m_Id;
        private LGOAPDomainCompiledData m_Domain;
        private BlackboardComponent m_Blackboard;
        private PlannerResultsSet m_ResultsSetForUse;
        private PlannerResultsSet m_ResultsSetForPlanning;

        /// <summary>
        /// Reset the static id assigner.
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        private static void Reset()
        {
            s_Id = 0;
        }

        /// <summary>
        /// Attempt to create an LGOAPComponent from a blackboard and the compiled data of a domain.
        /// </summary>
        /// <returns>Whether the process was successful.</returns>
        internal static bool TryCreate(BlackboardComponent blackboard, LGOAPDomainCompiledData domain, out LGOAPComponent component)
        {
            if (blackboard == null)
            {
                component = null;
                return false;
            }

            if (domain == null)
            {
                component = null;
                return false;
            }

            component = new LGOAPComponent(blackboard, domain);
            return true;
        }

        private LGOAPComponent(BlackboardComponent blackboard, LGOAPDomainCompiledData domain)
        {
            m_Id = ++s_Id;

            m_Blackboard = blackboard;
            m_Domain = domain;

            m_ResultsSetForUse = new PlannerResultsSet(m_Domain.planSizesByLayer);
            m_ResultsSetForPlanning = new PlannerResultsSet(m_Domain.planSizesByLayer);
        }

        internal void Dispose()
        {
            m_ResultsSetForPlanning.Dispose();
            m_ResultsSetForUse.Dispose();

            m_Domain = null;
            m_Blackboard = null;
        }
    }
}