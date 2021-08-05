using System.Runtime.CompilerServices;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Low-level representation of a score calculator blackboard function collection.
    /// </summary>
    internal readonly struct LowLevelScoreCalculatorBlackboardFunctionCollection
    {
        private readonly LowLevelBlackboardFunctionCollection m_FunctionCollection;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private LowLevelScoreCalculatorBlackboardFunctionCollection(LowLevelBlackboardFunctionCollection function)
        {
            m_FunctionCollection = function;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator LowLevelScoreCalculatorBlackboardFunctionCollection(LowLevelBlackboardFunctionCollection function)
        {
            return new LowLevelScoreCalculatorBlackboardFunctionCollection(function);
        }

        /// <summary>
        /// Execute the score calculator collection on a blackboard.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal float Execute(LowLevelBlackboard blackboard)
        {
            var score = 0f;
            
            var enumerator = m_FunctionCollection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                score += ((LowLevelScoreCalculatorBlackboardFunction) enumerator.current).Execute(blackboard);
            }

            return score;
        }
    }
}