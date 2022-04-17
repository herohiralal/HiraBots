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
            Planning,
            Normal
        }

        private Coroutine m_PlannerCoroutine;
        private Status m_Status;
        private int? m_LayerToSchedulePlannerAt;
        private JobHandle? m_JobHandleToWaitOn;

        internal bool planSynchronously { get; set; }

        internal void StartPlannerAtLayer(int index, bool discardIfAlreadyPlanning)
        {
            var layerCount = m_Domain.layerCount;

            if (index < 0 || index >= layerCount)
            {
                throw new System.ArgumentOutOfRangeException(nameof(index), index, $"Range: [0, {layerCount}).");
            }

            switch (m_Status)
            {
                case Status.Planning when discardIfAlreadyPlanning:
                    m_LayerToSchedulePlannerAt = index;
                    break;
                case Status.Normal:
                    m_LayerToSchedulePlannerAt = index;
                    m_PlannerCoroutine = CoroutineRunner.Start(SchedulePlannerCoroutine());
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

            int index;
            if (planSynchronously)
            {
                // get the index to schedule the planner job at and mark it as consumed
                index = m_LayerToSchedulePlannerAt.Value;
                m_LayerToSchedulePlannerAt = null;

                // run on main thread
                RunPlannerJobSynchronously(index);
            }
            else
            {
                // get the index to schedule the planner job at and mark it as consumed
                index = m_LayerToSchedulePlannerAt.Value;
                m_LayerToSchedulePlannerAt = null;

                // mark the status as planning
                m_Status = Status.Planning;

                // RunPlannerJobSynchronously(index);
                var lastJobHandle = SchedulePlannerJob(index);

                {
                    // this job handle will be forcefully completed when this component is destroyed
                    m_JobHandleToWaitOn = lastJobHandle;

                    while (!lastJobHandle.IsCompleted)
                    {
                        yield return null;
                    }

                    lastJobHandle.Complete();

                    m_JobHandleToWaitOn = null;
                }
            }

            // if a re-plan has been requested, ignore the results and just schedule a new planner job
            if (m_LayerToSchedulePlannerAt.HasValue)
            {
                m_PlannerCoroutine = CoroutineRunner.Start(SchedulePlannerCoroutine());
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

        // run the planner job synchronously (for debug purposes)
        private void RunPlannerJobSynchronously(int index)
        {
            var layerCount = m_Domain.layerCount;

            // copy currently executing plan to the results set used for planning
            // we do this for synchronous runs also, to avoid writing the active plan
            // with possibly temporary values
            m_PlansForExecution.CopyTo(m_PlansForPlanning);

            var domain = m_Domain.data;
            var blackboard = m_Blackboard.Copy(Allocator.TempJob);

            // goal calculator job
            if (index == 0)
            {
                new LGOAPGoalCalculatorJob(domain, blackboard, m_Domain.fallbackPlans[0], m_PlansForPlanning[0])
                    .Run();
                ++index;
            }

            for (var i = index; i < layerCount; i++)
            {
                new LGOAPMainPlannerJob(
                    domain,
                    blackboard,
                    m_Domain.fallbackPlans[i],
                    100f,
                    i,
                    m_PlansForPlanning[i - 1],
                    m_PlansForPlanning[i])
                    .Run();
            }

            blackboard.Dispose();
        }

        // this function is not inlined to ensure its synchronicity
        private void UsePlannerResults(int index)
        {
            var layerCount = m_Domain.layerCount;

            for (var i = index; i < layerCount; i++)
            {
                var planOnCurrentLayerIsInvalid = false;

                var currentLayersPlanForPlanning = m_PlansForPlanning[i];
                var currentLayersPlanForExecution = m_PlansForExecution[i];

                switch (currentLayersPlanForPlanning.resultType)
                {
                    case LGOAPPlan.Type.NotRequired:
                        currentLayersPlanForPlanning.CopyTo(currentLayersPlanForExecution);
                        break;
                    case LGOAPPlan.Type.Unchanged:
                        // ignore the result and keep executing the current copy
                        break;
                    case LGOAPPlan.Type.NewPlan:
                        var firstContainerIndex = currentLayersPlanForPlanning[currentLayersPlanForPlanning.currentIndex];

                        if (m_Domain.CheckPreconditionOnBlackboard(i, firstContainerIndex, m_Blackboard))
                        {
                            currentLayersPlanForPlanning.CopyTo(currentLayersPlanForExecution);
                            canProvidePlan = true;
                        }
                        else
                        {
                            planOnCurrentLayerIsInvalid = true;
                        }

                        break;
                    default:
                        throw new System.ArgumentOutOfRangeException();
                }

                if (planOnCurrentLayerIsInvalid)
                {
                    for (var j = i; j < layerCount; j++)
                    {
                        m_Domain.fallbackPlans[j].CopyTo(m_PlansForExecution[j]);
                    }

                    canProvidePlan = true;
                    break;
                }
            }
        }
    }
}