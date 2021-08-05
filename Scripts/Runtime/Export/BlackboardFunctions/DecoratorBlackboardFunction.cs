using HiraBots;

namespace UnityEngine
{
    public unsafe delegate bool DecoratorDelegate(LowLevelBlackboard blackboard, byte* memory);

    /// <summary>
    /// A decorator that can be executed on a <see cref="LowLevelBlackboard"/>.
    /// ======================================================================================================
    /// Any changes to this class MUST be synchronized with <see cref="LowLevelDecoratorBlackboardFunction"/>.
    /// ======================================================================================================
    /// </summary>
    public abstract unsafe partial class DecoratorBlackboardFunction : BlackboardFunction<DecoratorDelegate>
    {
        [Tooltip("Whether to invert the result of this function.")]
        [SerializeField] private bool m_Invert = false;

        protected override int memorySize => base.memorySize + ByteStreamHelpers.CombinedSizes<bool>(); // header includes inversion

        protected internal override byte* AppendMemory(byte* stream)
        {
            stream = base.AppendMemory(stream);

            // no offset
            ByteStreamHelpers.Write<bool>(ref stream, m_Invert);

            // offset sizeof(bool)
            return stream;
        }
    }
}