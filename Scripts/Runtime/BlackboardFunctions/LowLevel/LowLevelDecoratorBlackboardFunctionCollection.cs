using System.Runtime.CompilerServices;
using UnityEngine;

namespace HiraBots
{
    /// <summary>
    /// Low-level representation of a decorator blackboard function collection.
    /// </summary>
    internal readonly struct LowLevelDecoratorBlackboardFunctionCollection
    {
        private readonly LowLevelBlackboardFunctionCollection m_FunctionCollection;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private LowLevelDecoratorBlackboardFunctionCollection(LowLevelBlackboardFunctionCollection function)
        {
            m_FunctionCollection = function;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator LowLevelDecoratorBlackboardFunctionCollection(LowLevelBlackboardFunctionCollection function)
        {
            return new LowLevelDecoratorBlackboardFunctionCollection(function);
        }

        /// <summary>
        /// Execute the decorator collection on a blackboard.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Execute(LowLevelBlackboard blackboard)
        {
            var enumerator = m_FunctionCollection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (((LowLevelDecoratorBlackboardFunction) enumerator.current).Execute(blackboard))
                {
                    continue;
                }

                return false;
            }

            return true;
        }
    }
}