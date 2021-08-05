using System.Runtime.CompilerServices;
using Unity.Burst;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Low-level representation of a decorator blackboard function.
    /// </summary>
    internal readonly unsafe struct LowLevelDecoratorBlackboardFunction
    {
        private readonly LowLevelBlackboardFunction m_Function;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private LowLevelDecoratorBlackboardFunction(LowLevelBlackboardFunction function)
        {
            m_Function = function;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator LowLevelDecoratorBlackboardFunction(LowLevelBlackboardFunction function)
        {
            return new LowLevelDecoratorBlackboardFunction(function);
        }

        // whether the result must be inverted
        private bool invert
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.JumpOverNothing(m_Function.memory).AndAccess<bool>();
        }

        // the function memory state
        private byte* functionMemory
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.JumpOver<bool>(m_Function.memory).AndGetAPointerOf<byte>();
        }

        /// <summary>
        /// Execute the decorator on a blackboard.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Execute(LowLevelBlackboard blackboard)
        {
            var fnPtr = new FunctionPointer<DecoratorDelegate>(m_Function.functionPtr);
            return invert != fnPtr.Invoke(blackboard, functionMemory);
        }
    }
}