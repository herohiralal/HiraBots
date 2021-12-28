using UnityEngine;

namespace HiraBots
{
    internal readonly struct LGOAPGoalCompiledData
    {
        internal LGOAPGoalCompiledData(LGOAPGoal goal)
        {
            name = goal.name;
            insistence = ((DecoratorBlackboardFunction[]) goal.insistence.m_Insistence.Clone()).ReadOnly();
            target = ((DecoratorBlackboardFunction[]) goal.target.m_Target.Clone()).ReadOnly();
        }

        internal string name { get; }
        internal ReadOnlyArrayAccessor<DecoratorBlackboardFunction> insistence { get; }
        internal ReadOnlyArrayAccessor<DecoratorBlackboardFunction> target { get; }
    }

    internal struct LGOAPTaskCompiledData
    {
        internal LGOAPTaskCompiledData(LGOAPTask task)
        {
            name = task.name;
            isAbstract = task.isAbstract;
            precondition = ((DecoratorBlackboardFunction[]) task.action.m_Precondition.Clone()).ReadOnly();
            effect = ((EffectorBlackboardFunction[]) task.action.m_Effect.Clone()).ReadOnly();
            target = ((DecoratorBlackboardFunction[]) task.target.m_Target.Clone()).ReadOnly();
            taskProvider = task.taskProviders.Length > 0 ? task.taskProviders[0] : ErrorTaskProvider.noneTaskProvider;
            serviceProviders = ((HiraBotsServiceProvider[]) task.serviceProviders.Clone()).ReadOnly();
        }

        internal string name { get; }
        internal bool isAbstract { get; }
        internal ReadOnlyArrayAccessor<DecoratorBlackboardFunction> precondition { get; }
        internal ReadOnlyArrayAccessor<EffectorBlackboardFunction> effect { get; }
        internal ReadOnlyArrayAccessor<DecoratorBlackboardFunction> target { get; }
        internal HiraBotsTaskProvider taskProvider { get; }
        internal ReadOnlyArrayAccessor<HiraBotsServiceProvider> serviceProviders { get; }
    }
}