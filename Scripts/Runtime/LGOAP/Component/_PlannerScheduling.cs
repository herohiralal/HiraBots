using System;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace HiraBots
{
    internal partial class LGOAPComponent
    {
        private IEnumerator StartPlannerAtLayer(int index)
        {
            var layerCount = m_Domain.planSizesByLayer.count;

            if (index < -1 || index >= layerCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Range: [-1, {layerCount}).");
            }

            m_ResultsSetForUse.CopyTo(m_ResultsSetForPlanning);

            JobHandle lastJobHandle = default;

            var domain = m_Domain.data;
            var blackboard = m_Blackboard.Copy(Allocator.TempJob);

            if (index == -1)
            {
                var goalCalculatorJob = new LGOAPGoalCalculatorJob(domain, blackboard, m_ResultsSetForPlanning.goalResult);
                lastJobHandle = goalCalculatorJob.Schedule();
            }

            for (var i = Mathf.Max(0, index); i < layerCount; i++)
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

            lastJobHandle = blackboard.Dispose(lastJobHandle);

            while (!lastJobHandle.IsCompleted)
            {
                yield return null;
            }

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