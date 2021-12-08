using UnityEngine;

namespace HiraBots
{
    internal unsafe delegate bool DecoratorDelegate(in LowLevelBlackboard blackboard, byte* memory);

    /// <summary>
    /// A decorator that can be executed on a <see cref="LowLevelBlackboard"/>.
    /// ======================================================================================================
    /// Any changes to this class MUST be synchronized with <see cref="LowLevelDecoratorBlackboardFunction"/>.
    /// ======================================================================================================
    /// </summary>
    internal abstract unsafe partial class DecoratorBlackboardFunction : BlackboardFunction<DecoratorDelegate>
    {
        [System.Serializable]
        internal struct Header
        {
            /// <summary>
            /// Whether this particular decorator is used as a score calculator.
            /// </summary>
            [SerializeField] internal bool m_IsScoreCalculator;

            [Tooltip("The score to add to the total if the condition-check returns true (after inversion).")]
            [SerializeField] internal float m_Score;

            [Tooltip("Whether to invert the result of this function.")]
            [SerializeField] internal bool m_Invert;
        }

        [SerializeField, HideInInspector] private Header m_Header = default;

        internal override void PrepareForCompilation()
        {
            base.PrepareForCompilation();
            m_MemorySize += m_Header.m_IsScoreCalculator
                ? ByteStreamHelpers.CombinedSizes<float, bool>() // score & inversion header
                : ByteStreamHelpers.CombinedSizes<bool>(); // inversion header
        }

        public override void Compile(ref byte* stream)
        {
            base.Compile(ref stream);

            if (m_Header.m_IsScoreCalculator)
            {
                // no offset
                ByteStreamHelpers.Write<float>(ref stream, m_Header.m_Score);
            }

            // no offset or offset sizeof(float)
            ByteStreamHelpers.Write<bool>(ref stream, m_Header.m_Invert);

            // offset sizeof(bool) or offset sizeof(float) + sizeof(bool)
        }
    }
}