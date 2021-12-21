#if HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
namespace HiraBots
{
    internal partial class LGOAPGoal
    {
        /// <summary>
        /// Build an LGOAP goal.
        /// </summary>
        internal void BuildLGOAPGoal(DecoratorBlackboardFunction[] inInsistence, DecoratorBlackboardFunction[] inTarget)
        {
            m_Insistence.m_Insistence = inInsistence;
            m_Target.m_Target = inTarget;
        }
    }

    internal partial class LGOAPTask
    {
        /// <summary>
        /// Build an LGOAP task.
        /// </summary>
        internal void BuildLGOAPTask(DecoratorBlackboardFunction[] inPrecondition, DecoratorBlackboardFunction[] inCost,
            EffectorBlackboardFunction[] inEffect)
        {
            m_IsAbstract = false;
            m_Action.m_Precondition = inPrecondition;
            m_Action.m_Cost = inCost;
            m_Action.m_Effect = inEffect;
        }

        /// <summary>
        /// Build an LGOAP abstract task.
        /// </summary>
        internal void BuildLGOAPAbstractTask(DecoratorBlackboardFunction[] inPrecondition, DecoratorBlackboardFunction[] inCost,
            EffectorBlackboardFunction[] inEffect, DecoratorBlackboardFunction[] inTarget)
        {
            m_IsAbstract = true;
            m_Action.m_Precondition = inPrecondition;
            m_Action.m_Cost = inCost;
            m_Action.m_Effect = inEffect;
            m_Target.m_Target = inTarget;
        }
    }
}
#endif