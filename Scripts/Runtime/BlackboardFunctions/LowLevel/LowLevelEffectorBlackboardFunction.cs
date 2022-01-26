using System.Runtime.CompilerServices;
using Unity.Burst;

namespace HiraBots
{
    /// <summary>
    /// Low-level representation of a effector blackboard function.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{info}")]
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

        private string info => m_Function.info;

        internal readonly struct PointerConverter : IPointerToLowLevelObjectConverter<LowLevelEffectorBlackboardFunction>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public LowLevelEffectorBlackboardFunction Convert(byte* address)
            {
                return new LowLevelEffectorBlackboardFunction(address);
            }
        }

        /// <summary>
        /// Execute the effector on a blackboard.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Execute(LowLevelBlackboard blackboard)
        {
            var fnPtr = new FunctionPointer<UnityEngine.AI.HiraBotsEffectorBlackboardFunction.Delegate>(m_Function.functionPtr);
            fnPtr.Invoke(blackboard, m_Function.memory);
        }
    }
}