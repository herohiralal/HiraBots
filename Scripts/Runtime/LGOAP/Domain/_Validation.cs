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
        /// A list of bad containers.
        /// </summary>
        void AddBadContainer(ref LGOAPDomainValidatorContext.BadContainerInfo info);

        /// <summary>
        /// Pre-allocated list of bad functions.
        /// </summary>
        List<LGOAPContainerValidatorContext.BadFunctionInfo> badFunctions { get; }

        /// <summary>
        /// Pre-allocated list for badly selected keys.
        /// </summary>
        List<BlackboardFunctionValidatorContext.BadKeyInfo> badlySelectedKeys { get; }
    }

    internal struct LGOAPDomainValidatorContext
    {
        /// <summary>
        /// The info regarding a bad container.
        /// </summary>
        internal struct BadContainerInfo
        {
            /// <summary>
            /// The index of the layer.
            /// </summary>
            internal int layerIndex { get; set; }

            /// <summary>
            /// The type of the container.
            /// </summary>
            internal string containerType { get; set; }

            /// <summary>
            /// The index of the container.
            /// </summary>
            internal int containerIndex { get; set; }

            /// <summary>
            /// Whether the container is null.
            /// </summary>
            internal bool containerIsNull { get; set; }

            /// <summary>
            /// Whether the container is abstract when it should not be.
            /// </summary>
            internal bool containerIsAbstractWhenItShouldNotBe { get; set; }

            /// <summary>
            /// The list of bad functions on the container.
            /// </summary>
            internal LGOAPContainerValidatorContext.BadFunctionInfo[] badFunctions { get; set; }
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

            IndividualContainerCheck(context, keySet);
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
        private void IndividualContainerCheck(ILGOAPDomainValidatorContext context, ReadOnlyHashSetAccessor<BlackboardKey> keys)
        {
            var containerValidator = new LGOAPContainerValidatorContext
            {
                badFunctions = context.badFunctions,
                badlySelectedKeys = context.badlySelectedKeys,
                allowedKeyPool = keys
            };

            for (var i = 0; i < m_TopLayer.m_Goals.Length; i++)
            {
                var goal = m_TopLayer.m_Goals[i];
                containerValidator.succeeded = true;
                containerValidator.badFunctions.Clear();
                containerValidator.badlySelectedKeys.Clear();

                var badComponentInfo = new LGOAPDomainValidatorContext.BadContainerInfo
                {
                    layerIndex = 0,
                    containerType = "Goal",
                    containerIndex = i
                };

                if (goal == null)
                {
                    badComponentInfo.containerIsNull = true;

                    context.MarkUnsuccessful();
                    context.AddBadContainer(ref badComponentInfo);
                    continue;
                }

                var success = true;

                goal.Validate(ref containerValidator);

                if (!containerValidator.succeeded)
                {
                    success = false;
                    badComponentInfo.badFunctions = containerValidator.badFunctions.ToArray();
                }

                if (!success)
                {
                    context.MarkUnsuccessful();
                    context.AddBadContainer(ref badComponentInfo);
                }
            }

            for (var i = 0; i < m_IntermediateLayers.Length; i++)
            {
                var layer = m_IntermediateLayers[i];

                for (var j = 0; j < layer.m_Tasks.Length; j++)
                {
                    var abstractTask = layer.m_Tasks[j];
                    containerValidator.succeeded = true;
                    containerValidator.badFunctions.Clear();
                    containerValidator.badlySelectedKeys.Clear();

                    var badComponentInfo = new LGOAPDomainValidatorContext.BadContainerInfo
                    {
                        layerIndex = i + 1,
                        containerType = "AbstractTask",
                        containerIndex = j
                    };

                    if (abstractTask == null)
                    {
                        badComponentInfo.containerIsNull = true;

                        context.MarkUnsuccessful();
                        context.AddBadContainer(ref badComponentInfo);
                        continue;
                    }

                    var success = true;

                    abstractTask.Validate(ref containerValidator);

                    if (!containerValidator.succeeded)
                    {
                        success = false;
                        badComponentInfo.badFunctions = containerValidator.badFunctions.ToArray();
                    }

                    if (!success)
                    {
                        context.MarkUnsuccessful();
                        context.AddBadContainer(ref badComponentInfo);
                    }
                }
            }

            var bottomLayerIndex = m_IntermediateLayers.Length + 1;

            for (var i = 0; i < m_BottomLayer.m_Tasks.Length; i++)
            {
                var task = m_BottomLayer.m_Tasks[i];
                containerValidator.succeeded = true;
                containerValidator.badFunctions.Clear();
                containerValidator.badlySelectedKeys.Clear();

                var badContainerInfo = new LGOAPDomainValidatorContext.BadContainerInfo
                {
                    layerIndex = bottomLayerIndex,
                    containerType = "Task",
                    containerIndex = i
                };

                if (task == null)
                {
                    badContainerInfo.containerIsNull = true;

                    context.MarkUnsuccessful();
                    context.AddBadContainer(ref badContainerInfo);
                    continue;
                }

                var success = true;

                if (task.isAbstract)
                {
                    success = false;
                    badContainerInfo.containerIsAbstractWhenItShouldNotBe = true;
                }

                task.Validate(ref containerValidator);

                if (!containerValidator.succeeded)
                {
                    success = false;
                    badContainerInfo.badFunctions = containerValidator.badFunctions.ToArray();
                }

                if (!success)
                {
                    context.MarkUnsuccessful();
                    context.AddBadContainer(ref badContainerInfo);
                }
            }
        }
    }
}
#endif