#if HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
namespace HiraBots
{
    internal abstract partial class DecoratorBlackboardFunction
    {
        /// <summary>
        /// Build a decorator.
        /// </summary>
        internal void BuildDecoratorBlackboardFunction(bool invert)
        {
            m_Header = new Header {m_IsScoreCalculator = false, m_Invert = invert};
        }

        /// <summary>
        /// Build a score calculator.
        /// </summary>
        internal void BuildScoreCalculatorBlackboardFunction(float score, bool invert)
        {
            m_Header = new Header {m_IsScoreCalculator = true, m_Score = score, m_Invert = invert};
        }
    }

    internal abstract partial class EffectorBlackboardFunction
    {
        /// <summary>
        /// Build an effector.
        /// </summary>
        internal void BuildEffectorBlackboardFunction()
        {
            // nothing lol
        }
    }
}
#endif