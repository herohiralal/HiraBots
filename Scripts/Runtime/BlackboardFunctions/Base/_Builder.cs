#if UNITY_EDITOR // ideally manual creation must only be done for testing purposes as it skips validation checks
namespace UnityEngine
{
    public abstract partial class BlackboardFunction<TFunction>
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

    public abstract partial class DecoratorBlackboardFunction
    {
        /// <summary>
        /// Build a decorator.
        /// </summary>
        protected static T Build<T>(string name, bool invert, HideFlags hideFlags = HideFlags.None)
            where T : DecoratorBlackboardFunction
        {
            var output = BlackboardFunction<DecoratorDelegate>.Build<T>(name, hideFlags);
            output.m_Invert = invert;
            return output;
        }
    }

    public abstract partial class EffectorBlackboardFunction
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

    public abstract partial class ScoreCalculatorBlackboardFunction
    {
        /// <summary>
        /// Build a score calculator.
        /// </summary>
        protected static T Build<T>(string name, float score, bool invert, HideFlags hideFlags = HideFlags.None)
            where T : ScoreCalculatorBlackboardFunction
        {
            var output = BlackboardFunction<DecoratorDelegate>.Build<T>(name, hideFlags);
            output.m_Score = score;
            output.m_Invert = invert;
            return output;
        }
    }
}
#endif