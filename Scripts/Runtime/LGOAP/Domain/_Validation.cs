#if UNITY_EDITOR || HIRA_BOTS_TESTS || ENABLE_HIRA_BOTS_RUNTIME_BUILDER
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// The context required to validate an LGOAP domain.
    /// </summary>
    internal interface ILGOAPDomainValidatorContext
    {
        /// <summary>
        /// Mark the validation as unsuccessful.
        /// </summary>
        void MarkUnsuccessful();

        /// <summary>
        /// Mark the domain as missing a blackboard.
        /// </summary>
        void MarkMissingBlackboard();

        /// <summary>
        /// Mark the domain as unable to validate because of an unvalidated blackboard.
        /// </summary>
        void MarkUnvalidatedBlackboard(BlackboardTemplate b);

        /// <summary>
        /// Check if a blackboard template is validated
        /// </summary>
        bool TryGetBlackboardKeySet(BlackboardTemplate b, out ReadOnlyHashSetAccessor<BlackboardKey> keys);

        /// <summary>
        /// Whether the backends are unsupported.
        /// </summary>
        BackendType missingBackends { get; set; }

        /// <summary>
        /// A list of bad components.
        /// </summary>
        void AddBadComponent(ref LGOAPDomainValidatorContext.BadComponentInfo info);

        /// <summary>
        /// Pre-allocated list of bad functions.
        /// </summary>
        List<LGOAPComponentValidatorContext.BadFunctionInfo> badFunctions { get; }

        /// <summary>
        /// Pre-allocated list for badly selected keys.
        /// </summary>
        List<BlackboardFunctionValidatorContext.BadKeyInfo> badlySelectedKeys { get; }
    }

    internal struct LGOAPDomainValidatorContext
    {
        /// <summary>
        /// The info regarding a bad component.
        /// </summary>
        internal struct BadComponentInfo
        {
            /// <summary>
            /// The index of the layer.
            /// </summary>
            internal int layerIndex { get; set; }

            /// <summary>
            /// The type of the component.
            /// </summary>
            internal string componentType { get; set; }

            /// <summary>
            /// The index of the component.
            /// </summary>
            internal int componentIndex { get; set; }

            /// <summary>
            /// Whether the component is null.
            /// </summary>
            internal bool componentIsNull { get; set; }

            /// <summary>
            /// Whether the component is abstract when it should not be.
            /// </summary>
            internal bool componentIsAbstractWhenItShouldNotBe { get; set; }

            /// <summary>
            /// Whether the component is not abstract when it should be.
            /// </summary>
            internal bool componentIsNotAbstractWhenItShouldBe { get; set; }

            /// <summary>
            /// The list of bad functions on the component.
            /// </summary>
            internal LGOAPComponentValidatorContext.BadFunctionInfo[] badFunctions { get; set; }
        }
    }

    internal partial class LGOAPDomain
    {
        internal void Validate(ILGOAPDomainValidatorContext context)
        {
            if (m_Blackboard == null)
            {
                context.MarkUnsuccessful();
                context.MarkMissingBlackboard();
                return;
            }

            BackendCheck(context);

            if (!context.TryGetBlackboardKeySet(m_Blackboard, out var keySet))
            {
                context.MarkUnsuccessful();
                context.MarkUnvalidatedBlackboard(m_Blackboard);
                return;
            }

            IndividualComponentCheck(context, keySet);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BackendCheck(ILGOAPDomainValidatorContext context)
        {
            var parentBackends = m_Blackboard.backends;
            var selfBackends = m_Backends;

            var missingBackends = 0;

            if (selfBackends.HasFlag(BackendType.RuntimeInterpreter) && !parentBackends.HasFlag(BackendType.RuntimeInterpreter))
            {
                missingBackends |= (int) BackendType.RuntimeInterpreter;
            }

            if (selfBackends.HasFlag(BackendType.CodeGenerator) && !parentBackends.HasFlag(BackendType.CodeGenerator))
            {
                missingBackends |= (int) BackendType.CodeGenerator;
            }

            if (missingBackends != 0)
            {
                context.MarkUnsuccessful();
                context.missingBackends = (BackendType) missingBackends;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IndividualComponentCheck(ILGOAPDomainValidatorContext context, ReadOnlyHashSetAccessor<BlackboardKey> keys)
        {
            var componentValidator = new LGOAPComponentValidatorContext
            {
                badFunctions = context.badFunctions,
                badlySelectedKeys = context.badlySelectedKeys,
                allowedKeyPool = keys
            };

            for (var i = 0; i < m_TopLayer.m_Goals.Length; i++)
            {
                var goal = m_TopLayer.m_Goals[i];
                componentValidator.succeeded = true;
                componentValidator.badFunctions.Clear();
                componentValidator.badlySelectedKeys.Clear();

                var badComponentInfo = new LGOAPDomainValidatorContext.BadComponentInfo
                {
                    layerIndex = 0,
                    componentType = "Goal",
                    componentIndex = i
                };

                if (goal == null)
                {
                    badComponentInfo.componentIsNull = true;

                    context.MarkUnsuccessful();
                    context.AddBadComponent(ref badComponentInfo);
                    continue;
                }

                var success = true;

                goal.Validate(ref componentValidator);

                if (!componentValidator.succeeded)
                {
                    success = false;
                    badComponentInfo.badFunctions = componentValidator.badFunctions.ToArray();
                }

                if (!success)
                {
                    context.MarkUnsuccessful();
                    context.AddBadComponent(ref badComponentInfo);
                }
            }

            for (var i = 0; i < m_IntermediateLayers.Length; i++)
            {
                var layer = m_IntermediateLayers[i];

                for (var j = 0; j < layer.m_Tasks.Length; j++)
                {
                    var abstractTask = layer.m_Tasks[j];
                    componentValidator.succeeded = true;
                    componentValidator.badFunctions.Clear();
                    componentValidator.badlySelectedKeys.Clear();

                    var badComponentInfo = new LGOAPDomainValidatorContext.BadComponentInfo
                    {
                        layerIndex = i + 1,
                        componentType = "AbstractTask",
                        componentIndex = j
                    };

                    if (abstractTask == null)
                    {
                        badComponentInfo.componentIsNull = true;

                        context.MarkUnsuccessful();
                        context.AddBadComponent(ref badComponentInfo);
                        continue;
                    }

                    var success = true;

                    if (!abstractTask.isAbstract)
                    {
                        success = false;
                        badComponentInfo.componentIsNotAbstractWhenItShouldBe = true;
                    }

                    abstractTask.Validate(ref componentValidator);

                    if (!componentValidator.succeeded)
                    {
                        success = false;
                        badComponentInfo.badFunctions = componentValidator.badFunctions.ToArray();
                    }

                    if (!success)
                    {
                        context.MarkUnsuccessful();
                        context.AddBadComponent(ref badComponentInfo);
                    }
                }
            }

            var bottomLayerIndex = m_IntermediateLayers.Length + 1;

            for (var i = 0; i < m_BottomLayer.m_Tasks.Length; i++)
            {
                var task = m_BottomLayer.m_Tasks[i];
                componentValidator.succeeded = true;
                componentValidator.badFunctions.Clear();
                componentValidator.badlySelectedKeys.Clear();

                var badComponentInfo = new LGOAPDomainValidatorContext.BadComponentInfo
                {
                    layerIndex = bottomLayerIndex,
                    componentType = "Task",
                    componentIndex = i
                };

                if (task == null)
                {
                    badComponentInfo.componentIsNull = true;

                    context.MarkUnsuccessful();
                    context.AddBadComponent(ref badComponentInfo);
                    continue;
                }

                var success = true;

                if (task.isAbstract)
                {
                    success = false;
                    badComponentInfo.componentIsAbstractWhenItShouldNotBe = true;
                }

                task.Validate(ref componentValidator);

                if (!componentValidator.succeeded)
                {
                    success = false;
                    badComponentInfo.badFunctions = componentValidator.badFunctions.ToArray();
                }

                if (!success)
                {
                    context.MarkUnsuccessful();
                    context.AddBadComponent(ref badComponentInfo);
                }
            }
        }
    }
}
#endif