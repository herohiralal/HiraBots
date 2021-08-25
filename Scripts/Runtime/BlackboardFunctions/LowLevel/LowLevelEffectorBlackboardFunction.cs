using System.Runtime.CompilerServices;
using Unity.Burst;

namespace HiraBots
{
    /// <summary>
    /// Low-level representation of a effector blackboard function.
    /// </summary>
    internal readonly unsafe struct LowLevelEffectorBlackboardFunction : ILowLevelObject
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
        internal LowLevelEffectorBlackboardFunction(LowLevelBlackboardFunction function)
        {
            m_Function = function;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelEffectorBlackboardFunction(byte* stream) : this(new LowLevelBlackboardFunction(stream))
        {
        }

        internal readonly struct PointerConverter : IPointerToLowLevelObjectConverter<LowLevelEffectorBlackboardFunction>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public LowLevelEffectorBlackboardFunction Convert(byte* address)
            {
                return new LowLevelEffectorBlackboardFunction(address);
            }
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