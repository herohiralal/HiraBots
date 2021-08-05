namespace HiraBots
{
    internal unsafe delegate void EffectorDelegate(LowLevelBlackboard blackboard, byte* memory);

    /// <summary>
    /// A effector that can be executed on a <see cref="LowLevelBlackboard"/>.
    /// =====================================================================================================
    /// Any changes to this class MUST be synchronized with <see cref="LowLevelEffectorBlackboardFunction"/>.
    /// =====================================================================================================
    /// </summary>
    internal abstract class EffectorBlackboardFunction : BlackboardFunction<EffectorDelegate>
    {
        protected override int memorySize => base.memorySize + ByteStreamHelpers.NoCombinedSizes(); // header includes nothing

        internal override unsafe byte* AppendMemory(byte* stream)
        {
            stream = base.AppendMemory(stream);
            return stream;
        }
    }

    /// <summary>
    /// Helper functions to aid with collections of effectors.
    /// ===============================================================================================================
    /// Any changes to this class MUST be synchronized with <see cref="LowLevelEffectorBlackboardFunctionCollection"/>.
    /// ===============================================================================================================
    /// </summary>
    internal static unsafe class EffectorBlackboardFunctionExtensions
    {
        /// <summary>
        /// Get aligned memory size for a collection of effectors.
        /// </summary>
        internal static int GetAlignedMemorySize(this EffectorBlackboardFunction[] functions)
        {
            return BlackboardFunctionExtensions.GetAlignedMemorySize(functions);
        }

        /// <summary>
        /// Append the memory to the stream.
        /// </summary>
        internal static byte* AppendMemory(this EffectorBlackboardFunction[] functions, byte* stream)
        {
            return BlackboardFunctionExtensions.AppendMemory(functions, stream);
        }
    }
}