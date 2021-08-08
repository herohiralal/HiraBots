using System.Runtime.CompilerServices;

namespace HiraBots
{
    /// <summary>
    /// Low-level representation of an effector blackboard function collection.
    /// </summary>
    internal readonly struct LowLevelEffectorBlackboardFunctionCollection
    {
        private readonly LowLevelBlackboardFunctionCollection m_FunctionCollection;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private LowLevelEffectorBlackboardFunctionCollection(LowLevelBlackboardFunctionCollection function)
        {
            m_FunctionCollection = function;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator LowLevelEffectorBlackboardFunctionCollection(LowLevelBlackboardFunctionCollection function)
        {
            return new LowLevelEffectorBlackboardFunctionCollection(function);
        }

        /// <summary>
        /// Execute the effector collection on a blackboard.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Execute(LowLevelBlackboard blackboard)
        {
            var enumerator = m_FunctionCollection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ((LowLevelEffectorBlackboardFunction) enumerator.current).Execute(blackboard);
            }
        }
    }
}