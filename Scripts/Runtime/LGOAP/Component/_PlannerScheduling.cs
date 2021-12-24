using System;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace HiraBots
{
    internal partial class LGOAPComponent
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

        internal void StartPlannerAtLayer(int index)
        {
            var layerCount = m_Domain.planSizesByLayer.count;

            if (index < -1 || index >= layerCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Range: [-1, {layerCount}).");
            }

            switch (m_Status)
            {
                case Status.WillSchedulePlanner:
                    if (index < m_LayerToSchedulePlannerAt)
                    {
                        m_LayerToSchedulePlannerAt = index;
                    }
                    break;
                case Status.Planning:
                    m_LayerToSchedulePlannerAt = index;
                    break;
                case Status.Normal:
                    m_LayerToSchedulePlannerAt = index;
                    m_PlannerCoroutine = HiraBotsModule.StartCoroutine(SchedulePlannerCoroutine());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator SchedulePlannerCoroutine()
        {
            if (!m_LayerToSchedulePlannerAt.HasValue)
            {
                yield break;
            }

            // mark the status as planning
            m_Status = Status.Planning;

            // get the index to schedule the planner job at and mark it as consumed
            var index = m_LayerToSchedulePlannerAt.Value;
            m_LayerToSchedulePlannerAt = null;

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

            // mark the status as normal because the rest of the execution will take place on the main thread exclusively
            m_Status = Status.Normal;

            // if a re-plan has been requested, ignore the results and just schedule a new planner job
            if (m_LayerToSchedulePlannerAt.HasValue)
            {
                m_PlannerCoroutine = HiraBotsModule.StartCoroutine(SchedulePlannerCoroutine());
            }
            else // otherwise use the generated results
            {
                UsePlannerResults(index);
                m_PlannerCoroutine = null;
            }
        }
        

        // this function is not inlined to ensure its synchronicity
        private JobHandle SchedulePlannerJob(int index)
        {
            var layerCount = m_Domain.planSizesByLayer.count;

            // copy currently used plan to the results set used for planning
            m_ResultsSetForUse.CopyTo(m_ResultsSetForPlanning);

            JobHandle lastJobHandle = default;

            var domain = m_Domain.data;
            var blackboard = m_Blackboard.Copy(Allocator.TempJob);

            // goal calculator job
            if (index == -1)
            {
                var goalCalculatorJob = new LGOAPGoalCalculatorJob(domain, blackboard, m_ResultsSetForPlanning.goalResult);
                lastJobHandle = goalCalculatorJob.Schedule();
                index++;
            }

            // main planner jobs
            for (var i = index; i < layerCount; i++)
            {
                var plannerJob = new LGOAPMainPlannerJob(
                    domain,
                    blackboard,
                    100f,
                    i,
                    m_ResultsSetForPlanning[i - 1],
                    m_ResultsSetForPlanning[i]);

                lastJobHandle = plannerJob.Schedule(lastJobHandle);
            }

            // schedule a blackboard disposal job for after all the planner jobs have run
            return blackboard.Dispose(lastJobHandle);
        }

        // this function is not inlined to ensure its synchronicity
        private void UsePlannerResults(int index)
        {
            var layerCount = m_Domain.planSizesByLayer.count;

            // copy planner results to result set used for execution
            m_ResultsSetForPlanning.CopyTo(m_ResultsSetForUse);

            for (var i = index; i < layerCount; i++)
            {
                var result = m_ResultsSetForUse[i];

                switch (result.resultType)
                {
                    case PlannerResult.Type.Invalid:
                        Debug.Log($"Planner {m_Id} could not generate a plan at layer {i}.");
                        break;
                    case PlannerResult.Type.NotRequired:
                        Debug.Log($"Planner {m_Id} was not required to generate a plan at layer {i}.");
                        break;
                    case PlannerResult.Type.Unchanged:
                        Debug.Log($"Planner {m_Id} reused a previously generated plan at layer {i}.");
                        break;
                    case PlannerResult.Type.NewPlan:
                        
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}