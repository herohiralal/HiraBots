using System;
using System.Collections;
using Unity.Jobs;

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

            JobHandle? lastJobHandle = null;

            if (index == -1)
            {
                m_ResultsSetForUse.CopyTo(m_ResultsSetForPlanning);
                var goalCalculatorJob = new LGOAPGoalCalculatorJob(m_Domain,, m_ResultsSetForPlanning.goalResult);
                index++;
            }
        }
    }
}