namespace HiraBots
{
    internal partial struct LGOAPRealtimeBotComponent
    {
        internal void Tick()
        {
            if (ReferenceEquals(m_Domain, null))
            {
                return;
            }

            if (m_Blackboard.hasUnexpectedChanges)
            {
                m_Planner.StartPlannerAtLayer(0, true);
                m_Blackboard.ClearUnexpectedChanges();
            }

            // if the planner can provide a plan regardless of the executor being done
            if (m_Planner.canProvidePlan)
            {
                // grab the plan and provide it to the executor
                GrabPlannerResults();
                m_Planner.canProvidePlan = false;

                // silence the executor, there's a new plan which invalidates its finish status
                m_Executor.lastTaskEndSucceeded = null;
            }

            // if the executor is done executing
            if (!m_Executor.hasTask && m_Executor.lastTaskEndSucceeded.HasValue)
            {
                // fail
                if (!m_Executor.lastTaskEndSucceeded.Value)
                {
                    m_Planner.OnTaskExecutionComplete(false);
                }
                // full success (all task providers done)
                else
                {
                    m_CurrentTaskTaskProviderIndex++;

                    if (!ExecuteCurrentTaskProvider())
                    {
                        m_Planner.OnTaskExecutionComplete(true);
                    }
                }

                m_Executor.lastTaskEndSucceeded = null;
            }

            // if the planner is providing a plan because the executor asked for it
            if (m_Planner.canProvidePlan)
            {
                // grab the plan and provide it to the executor
                GrabPlannerResults();
                m_Planner.canProvidePlan = false;
            }
        }

        internal float executableTickIntervalMultiplier
        {
            set
            {
                m_ExecutableTickIntervalMultiplier = value;

                if (ReferenceEquals(m_Domain, null))
                {
                    return;
                }

                TaskRunner.ChangeTickIntervalMultiplier(m_Executor, value);

                foreach (var layer in m_ActiveServicesByLayer)
                {
                    foreach (var service in layer)
                    {
                        ServiceRunner.ChangeTickIntervalMultiplier(service, value);
                    }
                }
            }
        }

        internal bool executableTickPaused
        {
            set
            {
                if (ReferenceEquals(m_Domain, null))
                {
                    return;
                }

                TaskRunner.ChangeTickPaused(m_Executor, value);

                foreach (var layer in m_ActiveServicesByLayer)
                {
                    foreach (var service in layer)
                    {
                        ServiceRunner.ChangeTickPaused(service, value);
                    }
                }
            }
        }
    }
}