using System.Runtime.CompilerServices;
using Unity.Burst;

namespace HiraBots
{
    /// <summary>
    /// Low-level representation of a score calculator blackboard function.
    /// </summary>
    internal readonly unsafe struct LowLevelScoreCalculatorBlackboardFunction
    {
        private readonly LowLevelBlackboardFunction m_Function;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private LowLevelScoreCalculatorBlackboardFunction(LowLevelBlackboardFunction function)
        {
            m_Function = function;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator LowLevelScoreCalculatorBlackboardFunction(LowLevelBlackboardFunction function)
        {
            return new LowLevelScoreCalculatorBlackboardFunction(function);
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