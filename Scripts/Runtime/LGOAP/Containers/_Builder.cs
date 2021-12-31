#if HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
using UnityEngine;

namespace HiraBots
{
    internal partial class LGOAPGoal
    {
        /// <summary>
        /// Build an LGOAP goal.
        /// </summary>
        internal void BuildLGOAPGoal(HiraBotsScoreCalculatorBlackboardFunction[] inInsistence, HiraBotsDecoratorBlackboardFunction[] inTarget)
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
        internal void BuildLGOAPTask(HiraBotsDecoratorBlackboardFunction[] inPrecondition, HiraBotsScoreCalculatorBlackboardFunction[] inCost,
            HiraBotsEffectorBlackboardFunction[] inEffect)
        {
            m_Action.m_Precondition = inPrecondition;
            m_Action.m_Cost = inCost;
            m_Action.m_Effect = inEffect;
        }

        /// <summary>
        /// Build an LGOAP abstract task.
        /// </summary>
        internal void BuildLGOAPAbstractTask(HiraBotsDecoratorBlackboardFunction[] inPrecondition, HiraBotsScoreCalculatorBlackboardFunction[] inCost,
            HiraBotsEffectorBlackboardFunction[] inEffect, HiraBotsDecoratorBlackboardFunction[] inTarget)
        {
            m_Action.m_Precondition = inPrecondition;
            m_Action.m_Cost = inCost;
            m_Action.m_Effect = inEffect;
            m_Target.m_Target = inTarget;
        }
    }
}
#endif