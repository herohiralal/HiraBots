using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace HiraBots
{
    internal partial class LGOAPPlannerComponent
    {
        private enum Status
        {
            WillSchedulePlanner,
            Planning,
            Normal
        }

        private Coroutine m_PlannerCoroutine;
        private Status m_Status;
        private int? m_LayerToSchedulePlannerAt;
        private JobHandle? m_JobHandleToWaitOn;

        internal void StartPlannerAtLayer(int index, bool discardIfAlreadyPlanning)
        {
            var layerCount = m_Domain.layerCount;

            if (index < 0 || index >= layerCount)
            {
                throw new System.ArgumentOutOfRangeException(nameof(index), index, $"Range: [0, {layerCount}).");
            }

            switch (m_Status)
            {
                case Status.WillSchedulePlanner:
                    if (index < m_LayerToSchedulePlannerAt)
                    {
                        m_LayerToSchedulePlannerAt = index;
                    }
                    break;
                case Status.Planning when discardIfAlreadyPlanning:
                    m_LayerToSchedulePlannerAt = index;
                    break;
                case Status.Normal:
                    m_LayerToSchedulePlannerAt = index;
                    m_PlannerCoroutine = HiraBotsModule.StartCoroutine(SchedulePlannerCoroutine());
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }

        private IEnumerator SchedulePlannerCoroutine()
        {
            if (!m_LayerToSchedulePlannerAt.HasValue)
            {
                yield break;
            }

            // mark the status as about to start planning
            m_Status = Status.WillSchedulePlanner;

            yield return null;

            // get the index to schedule the planner job at and mark it as consumed
            var index = m_LayerToSchedulePlannerAt.Value;
            m_LayerToSchedulePlannerAt = null;

            // mark the status as planning
            m_Status = Status.Planning;

            var lastJobHandle = SchedulePlannerJob(index);

            {
                // this job handle will be forcefully completed when this component is destroyed
                m_JobHandleToWaitOn = lastJobHandle;
                m_Domain.AddDependentJob(m_Id, lastJobHandle);

                while (!lastJobHandle.IsCompleted)
                {
                    yield return null;
                }

                m_Domain.RemoveDependentJob(m_Id);
                m_JobHandleToWaitOn = null;
            }

            // if a re-plan has been requested, ignore the results and just schedule a new planner job
            if (m_LayerToSchedulePlannerAt.HasValue)
            {
                m_PlannerCoroutine = HiraBotsModule.StartCoroutine(SchedulePlannerCoroutine());
            }
            else // otherwise use the generated results
            {
                UsePlannerResults(index);

                // mark the status as normal
                m_Status = Status.Normal;

                m_PlannerCoroutine = null;
            }
        }
        

        // this function is not inlined to ensure its synchronicity
        private JobHandle SchedulePlannerJob(int index)
        {
            var layerCount = m_Domain.layerCount;

            // copy currently executing plan to the results set used for planning
            m_PlansForExecution.CopyTo(m_PlansForPlanning);

            JobHandle lastJobHandle = default;

            var domain = m_Domain.data;
            var blackboard = m_Blackboard.Copy(Allocator.TempJob);

            // goal calculator job
            if (index == 0)
            {
                var goalCalculatorJob = new LGOAPGoalCalculatorJob(domain, blackboard, m_Domain.fallbackPlans[0], m_PlansForPlanning[0]);
                lastJobHandle = goalCalculatorJob.Schedule();
                index++;
            }

            // main planner jobs
            for (var i = index; i < layerCount; i++)
            {
                var plannerJob = new LGOAPMainPlannerJob(
                    domain,
                    blackboard,
                    m_Domain.fallbackPlans[i],
                    100f,
                    i,
                    m_PlansForPlanning[i - 1],
                    m_PlansForPlanning[i]);

                lastJobHandle = plannerJob.Schedule(lastJobHandle);
            }

            // schedule a blackboard disposal job for after all the planner jobs have run
            return blackboard.Dispose(lastJobHandle);
        }

        // this function is not inlined to ensure its synchronicity
        private void UsePlannerResults(int index)
        {
            var layerCount = m_Domain.layerCount;

            for (var i = index; i < layerCount; i++)
            {
                // todo: actually use the plan please, that's sort of the whole point
                switch (m_PlansForPlanning[i].resultType)
                {
                    case LGOAPPlan.Type.NotRequired:
                        m_PlansForPlanning[i].CopyTo(m_PlansForExecution[i]);
                        Debug.Log($"No plan required at layer {i} on component {m_Id}. This can happen if one of the " +
                                  " previous layers contain a fake target.");
                        break;
                    case LGOAPPlan.Type.Unchanged:
                        // ignore the result and keep executing the current copy
                        Debug.Log($"Reused previous plan at layer {i} on component {m_Id}. This can happen if there " +
                                  "was an unexpected change in the blackboard but the original plan was still valid.");
                        break;
                    case LGOAPPlan.Type.NewPlan:
                        m_PlansForPlanning[i].CopyTo(m_PlansForExecution[i]);
                        Debug.Log($"New plan discovered at layer {i} on component {m_Id}.");
                        break;
                    default:
                        throw new System.ArgumentOutOfRangeException();
                }
            }
        }
    }
}