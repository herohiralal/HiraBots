#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
            /// The list of bad keys on the function.
            /// </summary>
            internal HiraBotsBlackboardFunction.ValidatorContext.BadKeyInfo[] badKeys { get; set; }
        }

        /// <summary>
        /// The info regarding a bad executable.
        /// </summary>
        internal struct BadExecutableInfo
        {
            /// <summary>
            /// The name of the container.
            /// </summary>
            internal string containerName { get; set; }

            /// <summary>
            /// The type of the executable.
            /// </summary>
            internal string executableType { get; set; }

            /// <summary>
            /// The index of the executable.
            /// </summary>
            internal int executableIndex { get; set; }

            /// <summary>
            /// Whether the executable is null.
            /// </summary>
            internal bool executableIsNull { get; set; }

            /// <summary>
            /// The list of errors in the executable.
            /// </summary>
            internal string[] errors { get; set; }
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
        /// List of bad executables.
        /// </summary>
        internal List<BadExecutableInfo> badExecutables { get; set; }

        /// <summary>
        /// Pre-allocated list for badly selected keys.
        /// </summary>
        internal List<HiraBotsBlackboardFunction.ValidatorContext.BadKeyInfo> badlySelectedKeys { get; set; }

        /// <summary>
        /// Pre-allocated list for errors in executables.
        /// </summary>
        internal List<string> executablesErrors { get; set; }

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
            var functionValidator = new HiraBotsBlackboardFunction.ValidatorContext
            {
                badlySelectedKeys = context.badlySelectedKeys,
                allowedKeyPool = new UnityEngine.AI.BlackboardTemplate.KeySet(context.allowedKeyPool)
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
            var functionValidator = new HiraBotsBlackboardFunction.ValidatorContext
            {
                badlySelectedKeys = context.badlySelectedKeys,
                allowedKeyPool = new UnityEngine.AI.BlackboardTemplate.KeySet(context.allowedKeyPool)
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

            for (var i = 0; i < m_TaskProviders.Length; i++)
            {
                var taskProvider = m_TaskProviders[i];
                context.executablesErrors.Clear();

                var badExecutableInfo = new LGOAPContainerValidatorContext.BadExecutableInfo
                {
                    containerName = name,
                    executableType = "TaskProvider",
                    executableIndex = i
                };

                if (taskProvider == null)
                {
                    badExecutableInfo.executableIsNull = true;

                    context.succeeded = false;
                    context.badExecutables.Add(badExecutableInfo);
                    continue;
                }

                var success = true;

                taskProvider.Validate(context.executablesErrors, context.allowedKeyPool);

                if (context.executablesErrors.Count > 0)
                {
                    success = false;
                    badExecutableInfo.errors = context.executablesErrors.ToArray();
                }

                if (!success)
                {
                    context.succeeded = false;
                    context.badExecutables.Add(badExecutableInfo);
                }
            }

            for (var i = 0; i < m_ServiceProviders.Length; i++)
            {
                var serviceProvider = m_ServiceProviders[i];
                context.executablesErrors.Clear();

                var badExecutableInfo = new LGOAPContainerValidatorContext.BadExecutableInfo
                {
                    containerName = name,
                    executableType = "ServiceProvider",
                    executableIndex = i
                };

                if (serviceProvider == null)
                {
                    badExecutableInfo.executableIsNull = true;

                    context.succeeded = false;
                    context.badExecutables.Add(badExecutableInfo);
                    continue;
                }

                var success = true;

                serviceProvider.Validate(context.executablesErrors, context.allowedKeyPool);

                if (context.executablesErrors.Count > 0)
                {
                    success = false;
                    badExecutableInfo.errors = context.executablesErrors.ToArray();
                }

                if (!success)
                {
                    context.succeeded = false;
                    context.badExecutables.Add(badExecutableInfo);
                }
            }
        }
    }
}
#endif