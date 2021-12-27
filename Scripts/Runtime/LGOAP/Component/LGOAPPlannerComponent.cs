namespace HiraBots
{
    internal partial class LGOAPPlannerComponent
    {
        private static ulong s_Id = 0;

        private readonly ulong m_Id;
        private LGOAPDomainCompiledData m_Domain;
        private BlackboardComponent m_Blackboard;
        private LGOAPPlan.Set m_PlanForExecution;
        private LGOAPPlan.Set m_PlanForPlanning;

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

            m_PlanForExecution = new LGOAPPlan.Set(m_Domain.maxPlanSizesByLayer);
            m_PlanForPlanning = new LGOAPPlan.Set(m_Domain.maxPlanSizesByLayer);

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

            m_PlanForPlanning.Dispose();
            m_PlanForExecution.Dispose();

            m_Domain = null;
            m_Blackboard = null;
        }
    }
}