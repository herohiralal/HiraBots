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
        [Tooltip("Whether to invert the result of this function.")]
        [SerializeField] private bool m_Invert = false;

        protected override int memorySize => base.memorySize + ByteStreamHelpers.CombinedSizes<bool>(); // header includes inversion

        internal override byte* Compile(byte* stream)
        {
            stream = base.Compile(stream);

            // no offset
            ByteStreamHelpers.Write<bool>(ref stream, m_Invert);

            // offset sizeof(bool)
            return stream;
        }
    }
}