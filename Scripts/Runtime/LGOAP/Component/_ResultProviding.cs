namespace HiraBots
{
    internal partial class LGOAPPlannerComponent
    {
        internal bool canProvidePlan { get; set; }

        internal void OnTaskExecutionComplete(bool success)
        {
            var layerCount = m_Domain.layerCount;

            for (var i = layerCount - 1; i > 0; i--) // goal layer intentionally ignored
            {
                var planAtCurrentLayer = m_PlansForExecution[i];

                if (planAtCurrentLayer.resultType == LGOAPPlan.Type.NotRequired)
                {
                    continue;
                }

                if (!success) // if the task was unsuccessful, figure out how to solve the current layer
                {
                    StartPlannerAtLayer(i, false);
                    return;
                }

                m_Domain.ApplyEffectOnBlackboard(i, planAtCurrentLayer[planAtCurrentLayer.currentIndex],
                    m_Blackboard, true);

                var currentIndex = ++planAtCurrentLayer.currentIndex; // update the current index

                // if the plan is not over
                if (currentIndex < planAtCurrentLayer.length)
                {
                    var currentContainerIndex = planAtCurrentLayer[planAtCurrentLayer[currentIndex]];

                    // if precondition isn't satisfied, this layer has failed, try to solve it
                    if (!m_Domain.CheckPreconditionOnBlackboard(i, currentContainerIndex, m_Blackboard))
                    {
                        StartPlannerAtLayer(i, false);
                        return;
                    }

                    // if new task is abstract, start the planner at the layer below
                    if (m_Domain.IsTaskAbstract(i, currentContainerIndex))
                    {
                        StartPlannerAtLayer(i + 1, false);
                        return;
                    }

                    // otherwise mark the results for collection
                    canProvidePlan = true;
                    return;
                }

                var planAtTheLayerAbove = m_PlansForExecution[i - 1];
                var currentContainerIndexAtLayerAbove = planAtTheLayerAbove[planAtTheLayerAbove.currentIndex];

                // if the target wasn't achieved, figure out a way to achieve it
                if (!m_Domain.CheckTargetOnBlackboard(i - 1, currentContainerIndexAtLayerAbove, m_Blackboard))
                {
                    StartPlannerAtLayer(i, false);
                    return;
                }

                // if it was achieved, continue to the next layer
            }

            // if the control flow made it past every layer, run the whole thing again lol
            StartPlannerAtLayer(0, false);
        }

        internal void CollectExecutionSet(short?[] executionSet)
        {
            var layerCount = m_Domain.layerCount;

            if (executionSet.Length != layerCount)
            {
                throw new System.InvalidOperationException();
            }

            for (var i = 0; i < layerCount; i++)
            {
                var currentPlan = m_PlansForExecution[i];

                // if the plan is not required, mark everything as such
                if (currentPlan.resultType == LGOAPPlan.Type.NotRequired)
                {
                    for (var j = i; j < layerCount; j++)
                    {
                        executionSet[j] = null;
                    }

                    return;
                }

                var currentIndex = currentPlan.currentIndex;

                // make sure there *is* a plan to execute
                UnityEngine.Assertions.Assert.IsFalse(currentIndex < 0 || currentIndex >= currentPlan.length);

                // supply the result
                executionSet[i] = currentPlan[currentIndex];
            }
        }
    }
}