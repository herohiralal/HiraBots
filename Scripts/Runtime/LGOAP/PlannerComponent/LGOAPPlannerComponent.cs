﻿namespace HiraBots
{
    internal partial class LGOAPPlannerComponent
    {
        private static ulong s_Id = 0;

        private readonly ulong m_Id;
        private LGOAPDomainCompiledData m_Domain;
        private BlackboardComponent m_Blackboard;
        private LGOAPPlan.Set m_PlansForExecution;
        private LGOAPPlan.Set m_PlansForPlanning;

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

            m_PlansForExecution = new LGOAPPlan.Set(m_Domain.maxPlanSizesByLayer);
            m_PlansForPlanning = new LGOAPPlan.Set(m_Domain.maxPlanSizesByLayer);

            domain.fallbackPlans.CopyTo(m_PlansForExecution);
            m_PlansForExecution.MarkDone();
            // mark the fallback goal as "achieved" when this object is constructed
            // so that when the planner runs for the first time, it doesn't
            // pick up the fallback planner and mark it as "unchanged"

            m_Status = Status.Normal;
            m_LayerToSchedulePlannerAt = null;
            m_JobHandleToWaitOn = null;
            m_PlannerCoroutine = null;
        }

        internal void Dispose()
        {
            if (m_PlannerCoroutine != null)
            {
                CoroutineRunner.Stop(m_PlannerCoroutine);
                m_PlannerCoroutine = null;
            }

            if (m_JobHandleToWaitOn.HasValue)
            {
                m_JobHandleToWaitOn.Value.Complete();
                m_JobHandleToWaitOn = null;
            }

            m_LayerToSchedulePlannerAt = null;
            m_Status = Status.Normal;

            m_PlansForPlanning.Dispose();
            m_PlansForExecution.Dispose();

            m_Domain = null;
            m_Blackboard = null;
        }
    }
}