using System.Runtime.CompilerServices;
using Unity.Burst;

namespace HiraBots
{
    /// <summary>
    /// Low-level representation of a score calculator blackboard function.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{info}")]
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

        private string info => m_Function.info;

        /// <summary>
        /// Execute the score calculator on a blackboard.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal float Execute(LowLevelBlackboard blackboard, float currentScore)
        {
            var fnPtr = new FunctionPointer<UnityEngine.HiraBotsScoreCalculatorBlackboardFunction.Delegate>(m_Function.functionPtr);
            return fnPtr.Invoke(blackboard, m_Function.memory, currentScore);
        }
    }
}