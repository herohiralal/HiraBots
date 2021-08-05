﻿using HiraBots;

namespace UnityEngine
{
    /// <summary>
    /// A score calculator that can be executed on a <see cref="LowLevelBlackboard"/>.
    /// ============================================================================================================
    /// Any changes to this class MUST be synchronized with <see cref="LowLevelScoreCalculatorBlackboardFunction"/>.
    /// ============================================================================================================
    /// </summary>
    public abstract class ScoreCalculatorBlackboardFunction : BlackboardFunction<DecoratorDelegate>
    {
        [Tooltip("The score to add to the total if the condition-check returns true (after inversion).")]
        [SerializeField] private float m_Score = 0f;

        [Tooltip("Whether to invert the result of this function.")]
        [SerializeField] private bool m_Invert = false;

        protected override int memorySize => base.memorySize + ByteStreamHelpers.CombinedSizes<float, bool>(); // header includes inversion & score

        protected internal override unsafe byte* AppendMemory(byte* stream)
        {
            stream = base.AppendMemory(stream);

            // no offset
            ByteStreamHelpers.Write<float>(ref stream, m_Score);

            // offset sizeof(float)
            ByteStreamHelpers.Write<bool>(ref stream, m_Invert);

            // offset sizeof(float) + sizeof(bool)
            return stream;
        }
    }
}