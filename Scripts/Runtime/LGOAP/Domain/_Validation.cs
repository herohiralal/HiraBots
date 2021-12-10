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
        /// A list of badly selected keys.
        /// </summary>
        List<string> badObjects { get; }
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

            if (!context.TryGetBlackboardKeySet(m_Blackboard, out var keySet))
            {
                context.MarkUnsuccessful();
                context.MarkUnvalidatedBlackboard(m_Blackboard);
                return;
            }

            BackendCheck(context);
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
            var badObjectCountOriginal = context.badObjects.Count;

            var componentValidator = new LGOAPComponentValidatorContext
            {
                badObjects = context.badObjects,
                allowedKeyPool = keys
            };

            for (var i = 0; i < m_TopLayer.m_Goals.Length; i++)
            {
                var goal = m_TopLayer.m_Goals[i];
                componentValidator.identifier = $"{name}::Layer[0]::Goal[{i}]";

                if (goal == null)
                {
                    context.badObjects.Add(componentValidator.identifier + " (missing component)");
                    continue;
                }

                goal.Validate(ref componentValidator);
            }

            for (var i = 0; i < m_IntermediateLayers.Length; i++)
            {
                var layer = m_IntermediateLayers[i];

                for (var j = 0; j < layer.m_Tasks.Length; j++)
                {
                    var abstractTask = layer.m_Tasks[j];
                    componentValidator.identifier = $"{name}::Layer[{i + 1}]::AbstractTask[{j}]";

                    if (abstractTask == null)
                    {
                        context.badObjects.Add(componentValidator.identifier + " (missing component)");
                        continue;
                    }

                    if (!abstractTask.isAbstract)
                    {
                        context.badObjects.Add(componentValidator.identifier + " (should be abstract but isn't)");
                    }

                    abstractTask.Validate(ref componentValidator);
                }
            }

            var bottomLayerIndex = m_IntermediateLayers.Length + 1;

            for (var i = 0; i < m_BottomLayer.m_Tasks.Length; i++)
            {
                var task = m_BottomLayer.m_Tasks[i];
                componentValidator.identifier = $"{name}::Layer[{bottomLayerIndex}]::Task[{i}]";

                if (task == null)
                {
                    context.badObjects.Add(componentValidator.identifier + " (missing component)");
                    continue;
                }

                if (task.isAbstract)
                {
                    context.badObjects.Add(componentValidator.identifier + " (shouldn't be abstract but is)");
                }

                task.Validate(ref componentValidator);
            }

            if (badObjectCountOriginal != context.badObjects.Count)
            {
                context.MarkUnsuccessful();
            }
        }
    }
}
#endif