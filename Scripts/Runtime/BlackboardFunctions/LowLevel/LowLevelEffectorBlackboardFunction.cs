using System.Runtime.CompilerServices;
using Unity.Burst;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Low-level representation of a effector blackboard function.
    /// </summary>
    internal readonly unsafe struct LowLevelEffectorBlackboardFunction
    {
        private readonly LowLevelBlackboardFunction m_Function;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private LowLevelEffectorBlackboardFunction(LowLevelBlackboardFunction function)
        {
            m_Function = function;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator LowLevelEffectorBlackboardFunction(LowLevelBlackboardFunction function)
        {
            return new LowLevelEffectorBlackboardFunction(function);
        }

        /// <summary>
        /// The function memory state.
        /// </summary>
        private byte* functionMemory
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.JumpOverNothing(m_Function.memory).AndGetAPointerOf<byte>();
        }

        /// <summary>
        /// Execute the effector on a blackboard.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Execute(LowLevelBlackboard blackboard)
        {
            var fnPtr = new FunctionPointer<EffectorDelegate>(m_Function.functionPtr);
            fnPtr.Invoke(blackboard, functionMemory);
        }
    }
}