using UnityEngine;

namespace HiraBots
{
    internal readonly struct LGOAPGoalCompiledData
    {
        internal LGOAPGoalCompiledData(LGOAPGoal goal)
        {
            name = goal.name;
            target = ((HiraBotsDecoratorBlackboardFunction[]) goal.target.m_Target.Clone()).ReadOnly();
        }

        internal string name { get; }
        internal ReadOnlyArrayAccessor<HiraBotsDecoratorBlackboardFunction> target { get; }
    }

    internal struct LGOAPTaskCompiledData
    {
        internal LGOAPTaskCompiledData(LGOAPTask task)
        {
            name = task.name;
            isAbstract = task.isAbstract;
            precondition = ((HiraBotsDecoratorBlackboardFunction[]) task.action.m_Precondition.Clone()).ReadOnly();
            effect = ((HiraBotsEffectorBlackboardFunction[]) task.action.m_Effect.Clone()).ReadOnly();
            target = ((HiraBotsDecoratorBlackboardFunction[]) task.target.m_Target.Clone()).ReadOnly();
            taskProvider = task.taskProviders.Length > 0 ? task.taskProviders[0] : ErrorTaskProvider.noneTaskProvider;
            serviceProviders = ((HiraBotsServiceProvider[]) task.serviceProviders.Clone()).ReadOnly();
        }

        internal string name { get; }
        internal bool isAbstract { get; }
        internal ReadOnlyArrayAccessor<HiraBotsDecoratorBlackboardFunction> precondition { get; }
        internal ReadOnlyArrayAccessor<HiraBotsEffectorBlackboardFunction> effect { get; }
        internal ReadOnlyArrayAccessor<HiraBotsDecoratorBlackboardFunction> target { get; }
        internal HiraBotsTaskProvider taskProvider { get; }
        internal ReadOnlyArrayAccessor<HiraBotsServiceProvider> serviceProviders { get; }
    }
}