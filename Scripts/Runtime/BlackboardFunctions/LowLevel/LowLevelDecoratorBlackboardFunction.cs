using System.Runtime.CompilerServices;
using Unity.Burst;

namespace HiraBots
{
    /// <summary>
    /// Low-level representation of a decorator blackboard function.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{info}")]
    internal readonly unsafe struct LowLevelDecoratorBlackboardFunction : ILowLevelObject
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
        internal LowLevelDecoratorBlackboardFunction(LowLevelBlackboardFunction function)
        {
            m_Function = function;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelDecoratorBlackboardFunction(byte* stream) : this(new LowLevelBlackboardFunction(stream))
        {
        }

        private string info => m_Function.info;

        internal readonly struct PointerConverter : IPointerToLowLevelObjectConverter<LowLevelDecoratorBlackboardFunction>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public LowLevelDecoratorBlackboardFunction Convert(byte* address)
            {
                return new LowLevelDecoratorBlackboardFunction(address);
            }
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