#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
using System.Collections.Generic;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// The context to validate an LGOAP container.
    /// </summary>
    internal struct LGOAPContainerValidatorContext
    {
        /// <summary>
        /// The info regarding a bad function.
        /// </summary>
        internal struct BadFunctionInfo
        {
            /// <summary>
            /// The name of the container.
            /// </summary>
            internal string containerName { get; set; }

            /// <summary>
            /// The type of the function.
            /// </summary>
            internal string functionType { get; set; }

            /// <summary>
            /// The index of the function.
            /// </summary>
            internal int functionIndex { get; set; }

            /// <summary>
            /// Whether the function is null.
            /// </summary>
            internal bool functionIsNull { get; set; }

            /// <summary>
            /// Whether the function is a score calculator when it shouldn't be,
            /// </summary>
            internal bool functionIsScoreCalculatorWhenItShouldNotBe { get; set; }

            /// <summary>
            /// Whether the function is not a score calculator when it should be.
            /// </summary>
            internal bool functionIsNotScoreCalculatorWhenItShouldBe { get; set; }

            /// <summary>
            /// The list of bad keys on the function.
            /// </summary>
            internal BlackboardFunctionValidatorContext.BadKeyInfo[] badKeys { get; set; }
        }

        /// <summary>
        /// Whether the validation succeeded.
        /// </summary>
        internal bool succeeded { get; set; }

        /// <summary>
        /// List of bad functions.
        /// </summary>
        internal List<BadFunctionInfo> badFunctions { get; set; }

        /// <summary>
        /// Pre-allocated list for badly selected keys.
        /// </summary>
        internal List<BlackboardFunctionValidatorContext.BadKeyInfo> badlySelectedKeys { get; set; }

        /// <summary>
        /// The pool of allowed keys.
        /// </summary>
        internal ReadOnlyHashSetAccessor<BlackboardKey> allowedKeyPool { get; set; }
    }

    internal sealed partial class LGOAPGoal
    {
        /// <summary>
        /// Validate this LGOAP container.
        /// </summary>
        internal void Validate(ref LGOAPContainerValidatorContext context)
        {
            var functionValidator = new BlackboardFunctionValidatorContext
            {
                badlySelectedKeys = context.badlySelectedKeys,
                allowedKeyPool = context.allowedKeyPool
            };

            for (var i = 0; i < m_Insistence.m_Insistence.Length; i++)
            {
                var scoreCalculator = m_Insistence.m_Insistence[i];
                functionValidator.succeeded = true;
                functionValidator.badlySelectedKeys.Clear();

                var badFunctionInfo = new LGOAPContainerValidatorContext.BadFunctionInfo
                {
                    containerName = name,
                    functionType = "Insistence",
                    functionIndex = i,
                };

                if (scoreCalculator == null)
                {
                    badFunctionInfo.functionIsNull = true;

                    context.succeeded = false;
                    context.badFunctions.Add(badFunctionInfo);
                    continue;
                }

                var success = true;

                if (!scoreCalculator.isScoreCalculator)
                {
                    success = false;
                    badFunctionInfo.functionIsNotScoreCalculatorWhenItShouldBe = true;
                }

                scoreCalculator.Validate(ref functionValidator);

                if (!functionValidator.succeeded)
                {
                    success = false;
                    badFunctionInfo.badKeys = functionValidator.badlySelectedKeys.ToArray();
                }

                if (!success)
                {
                    context.succeeded = false;
                    context.badFunctions.Add(badFunctionInfo);
                }
            }

            for (var i = 0; i < m_Target.m_Target.Length; i++)
            {
                var decorator = m_Target.m_Target[i];
                functionValidator.succeeded = true;
                functionValidator.badlySelectedKeys.Clear();

                var badFunctionInfo = new LGOAPContainerValidatorContext.BadFunctionInfo
                {
                    containerName = name,
                    functionType = "Target",
                    functionIndex = i,
                };

                if (decorator == null)
                {
                    badFunctionInfo.functionIsNull = true;

                    context.succeeded = false;
                    context.badFunctions.Add(badFunctionInfo);
                    continue;
                }

                var success = true;

                if (decorator.isScoreCalculator)
                {
                    success = false;
                    badFunctionInfo.functionIsScoreCalculatorWhenItShouldNotBe = true;
                }

                decorator.Validate(ref functionValidator);

                if (!functionValidator.succeeded)
                {
                    success = false;
                    badFunctionInfo.badKeys = functionValidator.badlySelectedKeys.ToArray();
                }

                if (!success)
                {
                    context.succeeded = false;
                    context.badFunctions.Add(badFunctionInfo);
                }
            }
        }
    }

    internal sealed partial class LGOAPTask
    {
        /// <summary>
        /// Validate this LGOAP container.
        /// </summary>
        internal void Validate(ref LGOAPContainerValidatorContext context)
        {
            var functionValidator = new BlackboardFunctionValidatorContext
            {
                badlySelectedKeys = context.badlySelectedKeys,
                allowedKeyPool = context.allowedKeyPool
            };

            for (var i = 0; i < m_Action.m_Precondition.Length; i++)
            {
                var decorator = m_Action.m_Precondition[i];
                functionValidator.succeeded = true;
                functionValidator.badlySelectedKeys.Clear();

                var badFunctionInfo = new LGOAPContainerValidatorContext.BadFunctionInfo
                {
                    containerName = name,
                    functionType = "Precondition",
                    functionIndex = i,
                };

                if (decorator == null)
                {
                    badFunctionInfo.functionIsNull = true;

                    context.succeeded = false;
                    context.badFunctions.Add(badFunctionInfo);
                    continue;
                }

                var success = true;

                if (decorator.isScoreCalculator)
                {
                    success = false;
                    badFunctionInfo.functionIsScoreCalculatorWhenItShouldNotBe = true;
                }

                decorator.Validate(ref functionValidator);

                if (!functionValidator.succeeded)
                {
                    success = false;
                    badFunctionInfo.badKeys = functionValidator.badlySelectedKeys.ToArray();
                }

                if (!success)
                {
                    context.succeeded = false;
                    context.badFunctions.Add(badFunctionInfo);
                }
            }

            for (var i = 0; i < m_Action.m_Cost.Length; i++)
            {
                var scoreCalculator = m_Action.m_Cost[i];
                functionValidator.succeeded = true;
                functionValidator.badlySelectedKeys.Clear();

                var badFunctionInfo = new LGOAPContainerValidatorContext.BadFunctionInfo
                {
                    containerName = name,
                    functionType = "Cost",
                    functionIndex = i,
                };

                if (scoreCalculator == null)
                {
                    badFunctionInfo.functionIsNull = true;

                    context.succeeded = false;
                    context.badFunctions.Add(badFunctionInfo);
                    continue;
                }

                var success = true;

                if (!scoreCalculator.isScoreCalculator)
                {
                    success = false;
                    badFunctionInfo.functionIsNotScoreCalculatorWhenItShouldBe = true;
                }

                scoreCalculator.Validate(ref functionValidator);

                if (!functionValidator.succeeded)
                {
                    success = false;
                    badFunctionInfo.badKeys = functionValidator.badlySelectedKeys.ToArray();
                }

                if (!success)
                {
                    context.succeeded = false;
                    context.badFunctions.Add(badFunctionInfo);
                }
            }

            for (var i = 0; i < m_Action.m_Effect.Length; i++)
            {
                var effector = m_Action.m_Effect[i];
                functionValidator.succeeded = true;
                functionValidator.badlySelectedKeys.Clear();

                var badFunctionInfo = new LGOAPContainerValidatorContext.BadFunctionInfo
                {
                    containerName = name,
                    functionType = "Effect",
                    functionIndex = i,
                };

                if (effector == null)
                {
                    badFunctionInfo.functionIsNull = true;

                    context.succeeded = false;
                    context.badFunctions.Add(badFunctionInfo);
                    continue;
                }

                var success = true;

                effector.Validate(ref functionValidator);

                if (!functionValidator.succeeded)
                {
                    success = false;
                    badFunctionInfo.badKeys = functionValidator.badlySelectedKeys.ToArray();
                }

                if (!success)
                {
                    context.succeeded = false;
                    context.badFunctions.Add(badFunctionInfo);
                }
            }

            for (var i = 0; i < m_Target.m_Target.Length; i++)
            {
                var decorator = m_Target.m_Target[i];
                functionValidator.succeeded = true;
                functionValidator.badlySelectedKeys.Clear();

                var badFunctionInfo = new LGOAPContainerValidatorContext.BadFunctionInfo
                {
                    containerName = name,
                    functionType = "Target",
                    functionIndex = i,
                };

                if (decorator == null)
                {
                    badFunctionInfo.functionIsNull = true;

                    context.succeeded = false;
                    context.badFunctions.Add(badFunctionInfo);
                    continue;
                }

                var success = true;

                if (decorator.isScoreCalculator)
                {
                    success = false;
                    badFunctionInfo.functionIsScoreCalculatorWhenItShouldNotBe = true;
                }

                decorator.Validate(ref functionValidator);

                if (!functionValidator.succeeded)
                {
                    success = false;
                    badFunctionInfo.badKeys = functionValidator.badlySelectedKeys.ToArray();
                }

                if (!success)
                {
                    context.succeeded = false;
                    context.badFunctions.Add(badFunctionInfo);
                }
            }
        }
    }
}
#endif