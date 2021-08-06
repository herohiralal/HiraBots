using HiraBots;

namespace UnityEngine
{
    public unsafe delegate void EffectorDelegate(in LowLevelBlackboard blackboard, byte* memory);

    /// <summary>
    /// A effector that can be executed on a <see cref="LowLevelBlackboard"/>.
    /// =====================================================================================================
    /// Any changes to this class MUST be synchronized with <see cref="LowLevelEffectorBlackboardFunction"/>.
    /// =====================================================================================================
    /// </summary>
    public abstract unsafe partial class EffectorBlackboardFunction : BlackboardFunction<EffectorDelegate>
    {
        protected override int memorySize => base.memorySize + ByteStreamHelpers.NoCombinedSizes(); // header includes nothing

        protected internal override byte* Compile(byte* stream)
        {
            stream = base.Compile(stream);
            return stream;
        }
    }
}