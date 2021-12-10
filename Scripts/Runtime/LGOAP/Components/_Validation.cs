#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
using System.Collections.Generic;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// The context to validate an LGOAP component.
    /// </summary>
    internal struct LGOAPComponentValidatorContext
    {
        /// <summary>
        /// The identifier for this function.
        /// </summary>
        internal string identifier { get; set; }

        /// <summary>
        /// List of badly selected keys.
        /// </summary>
        internal List<string> badObjects { get; set; }

        /// <summary>
        /// The pool of allowed keys.
        /// </summary>
        internal ReadOnlyHashSetAccessor<BlackboardKey> allowedKeyPool { get; set; }
    }

    internal sealed partial class LGOAPGoal
    {
        /// <summary>
        /// Validate this LGOAP component.
        /// </summary>
        internal void Validate(ref LGOAPComponentValidatorContext context)
        {
            var functionValidator = new BlackboardFunctionValidatorContext
            {
                badObjects = context.badObjects,
                allowedKeyPool = context.allowedKeyPool
            };

            for (var i = 0; i < m_Insistence.m_Insistence.Length; i++)
            {
                var scoreCalculator = m_Insistence.m_Insistence[i];
                functionValidator.identifier = $"{context.identifier}({name})::Insistence[{i}]";

                if (scoreCalculator == null)
                {
                    context.badObjects.Add(functionValidator.identifier + " (missing function)");
                    continue;
                }

                scoreCalculator.Validate(ref functionValidator);
            }

            for (var i = 0; i < m_Target.m_Target.Length; i++)
            {
                var decorator = m_Target.m_Target[i];
                functionValidator.identifier = $"{context.identifier}({name})::Target[{i}]";

                if (decorator == null)
                {
                    context.badObjects.Add(functionValidator.identifier + " (missing function)");
                    continue;
                }

                decorator.Validate(ref functionValidator);
            }
        }
    }

    internal sealed partial class LGOAPTask
    {
        /// <summary>
        /// Validate this LGOAP component.
        /// </summary>
        internal void Validate(ref LGOAPComponentValidatorContext context)
        {
            var functionValidator = new BlackboardFunctionValidatorContext
            {
                badObjects = context.badObjects,
                allowedKeyPool = context.allowedKeyPool
            };

            for (var i = 0; i < m_Action.m_Precondition.Length; i++)
            {
                var decorator = m_Action.m_Precondition[i];
                functionValidator.identifier = $"{context.identifier}({name})::Precondition[{i}]";

                if (decorator == null)
                {
                    context.badObjects.Add(functionValidator.identifier + " (missing function)");
                    continue;
                }

                decorator.Validate(ref functionValidator);
            }

            for (var i = 0; i < m_Action.m_Cost.Length; i++)
            {
                var scoreCalculator = m_Action.m_Cost[i];
                functionValidator.identifier = $"{context.identifier}({name})::Cost[{i}]";

                if (scoreCalculator == null)
                {
                    context.badObjects.Add(functionValidator.identifier + " (missing function)");
                    continue;
                }

                scoreCalculator.Validate(ref functionValidator);
            }

            for (var i = 0; i < m_Action.m_Effect.Length; i++)
            {
                var effector = m_Action.m_Effect[i];
                functionValidator.identifier = $"{context.identifier}({name})::Effect[{i}]";

                if (effector == null)
                {
                    context.badObjects.Add(functionValidator.identifier + " (missing function)");
                    continue;
                }

                effector.Validate(ref functionValidator);
            }

            for (var i = 0; i < m_Target.m_Target.Length; i++)
            {
                var decorator = m_Target.m_Target[i];
                functionValidator.identifier = $"{context.identifier}({name})::Target[{i}]";

                if (decorator == null)
                {
                    context.badObjects.Add(functionValidator.identifier + " (missing function)");
                    continue;
                }

                decorator.Validate(ref functionValidator);
            }
        }
    }
}
#endif