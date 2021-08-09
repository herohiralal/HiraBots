#if HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
using UnityEngine;

namespace HiraBots
{
    internal abstract partial class BlackboardFunction<TFunction>
        where TFunction : System.Delegate
    {
        /// <summary>
        /// Build a BlackboardFunction.
        /// </summary>
        protected static T Build<T>(string name, HideFlags hideFlags = HideFlags.None)
            where T : BlackboardFunction<TFunction>
        {
            var output = CreateInstance<T>();
            output.name = name;
            output.hideFlags = hideFlags;
            return output;
        }
    }

    internal abstract partial class DecoratorBlackboardFunction
    {
        /// <summary>
        /// Build a decorator.
        /// </summary>
        protected static T Build<T>(string name, bool invert, HideFlags hideFlags = HideFlags.None)
            where T : DecoratorBlackboardFunction
        {
            var output = BlackboardFunction<DecoratorDelegate>.Build<T>(name, hideFlags);
            output.m_Header = new Header {m_IsScoreCalculator = false, m_Invert = invert};
            return output;
        }

        /// <summary>
        /// Build a score calculator.
        /// </summary>
        protected static T Build<T>(string name, float score, bool invert, HideFlags hideFlags = HideFlags.None)
            where T : DecoratorBlackboardFunction
        {
            var output = BlackboardFunction<DecoratorDelegate>.Build<T>(name, hideFlags);
            output.m_Header = new Header {m_IsScoreCalculator = true, m_Score = score, m_Invert = invert};
            return output;
        }
    }

    internal abstract partial class EffectorBlackboardFunction
    {
        /// <summary>
        /// Build an effector.
        /// </summary>
        protected new static T Build<T>(string name, HideFlags hideFlags = HideFlags.None)
            where T : EffectorBlackboardFunction
        {
            var output = BlackboardFunction<EffectorDelegate>.Build<T>(name, hideFlags);
            return output;
        }
    }
}
#endif