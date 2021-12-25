namespace HiraBots
{
    internal partial class LGOAPPlannerComponent
    {
        private static ulong s_Id = 0;

        private readonly ulong m_Id;
        private LGOAPDomainCompiledData m_Domain;
        private BlackboardComponent m_Blackboard;
        private PlannerResultsSet m_ResultsSetForExecution;
        private PlannerResultsSet m_ResultsSetForPlanning;

        /// <summary>
        /// Reset the static id assigner.
        /// </summary>
        internal static void ResetStaticIDAssigner()
        {
            s_Id = 0;
        }

        /// <summary>
        /// Attempt to create an LGOAPComponent from a blackboard and the compiled data of a domain.
        /// </summary>
        /// <returns>Whether the process was successful.</returns>
        internal static bool TryCreate(BlackboardComponent blackboard, LGOAPDomainCompiledData domain, out LGOAPPlannerComponent component)
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

            component = new LGOAPPlannerComponent(blackboard, domain);
            return true;
        }

        private LGOAPPlannerComponent(BlackboardComponent blackboard, LGOAPDomainCompiledData domain)
        {
            m_Id = ++s_Id;

            m_Blackboard = blackboard;
            m_Domain = domain;

            m_ResultsSetForExecution = new PlannerResultsSet(m_Domain.planSizesByLayer);
            m_ResultsSetForPlanning = new PlannerResultsSet(m_Domain.planSizesByLayer);

            m_Status = Status.Normal;
            m_LayerToSchedulePlannerAt = null;
            m_JobHandleToWaitOn = null;
            m_PlannerCoroutine = null;
        }

        internal void Dispose()
        {
            if (m_PlannerCoroutine != null)
            {
                HiraBotsModule.StopCoroutine(m_PlannerCoroutine);
                m_PlannerCoroutine = null;
            }

            if (m_JobHandleToWaitOn.HasValue)
            {
                m_JobHandleToWaitOn.Value.Complete();
                m_Domain.RemoveDependentJob(m_Id);
                m_JobHandleToWaitOn = null;
            }

            m_LayerToSchedulePlannerAt = null;
            m_Status = Status.Normal;

            m_ResultsSetForPlanning.Dispose();
            m_ResultsSetForExecution.Dispose();

            m_Domain = null;
            m_Blackboard = null;
        }
    }
}