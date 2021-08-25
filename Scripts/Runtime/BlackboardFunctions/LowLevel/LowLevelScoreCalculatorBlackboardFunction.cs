using System.Runtime.CompilerServices;
using Unity.Burst;

namespace HiraBots
{
    /// <summary>
    /// Low-level representation of a score calculator blackboard function.
    /// </summary>
    internal readonly unsafe struct LowLevelScoreCalculatorBlackboardFunction : ILowLevelObject
    {
        private readonly LowLevelBlackboardFunction m_Function;

        public byte* address
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Function.address;
        }

        public int size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Function.size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelScoreCalculatorBlackboardFunction(LowLevelBlackboardFunction function)
        {
            m_Function = function;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelScoreCalculatorBlackboardFunction(byte* stream) : this(new LowLevelBlackboardFunction(stream))
        {
        }

        internal readonly struct PointerConverter : IPointerToLowLevelObjectConverter<LowLevelScoreCalculatorBlackboardFunction>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public LowLevelScoreCalculatorBlackboardFunction Convert(byte* address)
            {
                return new LowLevelScoreCalculatorBlackboardFunction(address);
            }
        }

        // the score upon success
        private float score
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.JumpOverNothing(m_Function.memory).AndAccess<float>();
        }

        // whether the result must be inverted
        private bool invert
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.JumpOver<float>(m_Function.memory).AndAccess<bool>();
        }

        // the function memory state
        private byte* functionMemory
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.JumpOver<float, bool>(m_Function.memory).AndGetAPointerOf<byte>();
        }

        /// <summary>
        /// Execute the score calculator on a blackboard.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal float Execute(LowLevelBlackboard blackboard)
        {
            var fnPtr = new FunctionPointer<DecoratorDelegate>(m_Function.functionPtr);
            return invert != fnPtr.Invoke(blackboard, functionMemory) ? score : 0;
        }
    }
}