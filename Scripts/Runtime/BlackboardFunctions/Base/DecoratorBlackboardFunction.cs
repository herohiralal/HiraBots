using UnityEngine;

namespace HiraBots
{
    internal unsafe delegate bool DecoratorDelegate(LowLevelBlackboard blackboard, byte* memory);

    /// <summary>
    /// A decorator that can be executed on a <see cref="LowLevelBlackboard"/>.
    /// ======================================================================================================
    /// Any changes to this class MUST be synchronized with <see cref="LowLevelDecoratorBlackboardFunction"/>.
    /// ======================================================================================================
    /// </summary>
    internal abstract class DecoratorBlackboardFunction : BlackboardFunction<DecoratorDelegate>
    {
        [Tooltip("Whether to invert the result of this function.")]
        [SerializeField] private bool m_Invert = false;

        protected override int memorySize => base.memorySize + ByteStreamHelpers.CombinedSizes<bool>(); // header includes inversion

        internal override unsafe byte* AppendMemory(byte* stream)
        {
            stream = base.AppendMemory(stream);

            // no offset
            ByteStreamHelpers.Write<bool>(ref stream, m_Invert);

            // offset sizeof(bool)
            return stream;
        }
    }

    /// <summary>
    /// Helper functions to aid with collections of decorators.
    /// ================================================================================================================
    /// Any changes to this class MUST be synchronized with <see cref="LowLevelDecoratorBlackboardFunctionCollection"/>.
    /// ================================================================================================================
    /// </summary>
    internal static unsafe class DecoratorBlackboardFunctionExtensions
    {
        /// <summary>
        /// Get aligned memory size for a collection of decorators.
        /// </summary>
        internal static int GetAlignedMemorySize(this DecoratorBlackboardFunction[] functions)
        {
            return BlackboardFunctionExtensions.GetAlignedMemorySize(functions);
        }

        /// <summary>
        /// Append the memory to the stream.
        /// </summary>
        internal static byte* AppendMemory(this DecoratorBlackboardFunction[] functions, byte* stream)
        {
            return BlackboardFunctionExtensions.AppendMemory(functions, stream);
        }
    }
}