using HiraBots;

namespace UnityEngine
{
    public unsafe delegate void EffectorDelegate(LowLevelBlackboard blackboard, byte* memory);

    /// <summary>
    /// A effector that can be executed on a <see cref="LowLevelBlackboard"/>.
    /// =====================================================================================================
    /// Any changes to this class MUST be synchronized with <see cref="LowLevelEffectorBlackboardFunction"/>.
    /// =====================================================================================================
    /// </summary>
    public abstract class EffectorBlackboardFunction : BlackboardFunction<EffectorDelegate>
    {
        protected override int memorySize => base.memorySize + ByteStreamHelpers.NoCombinedSizes(); // header includes nothing

        protected internal override unsafe byte* AppendMemory(byte* stream)
        {
            stream = base.AppendMemory(stream);
            return stream;
        }
    }
}