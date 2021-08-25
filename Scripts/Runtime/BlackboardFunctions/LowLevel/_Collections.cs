using System.Runtime.CompilerServices;
using LowLevelDecoratorCollection =
    HiraBots.DefaultLowLevelObjectCollection<
        HiraBots.LowLevelDecoratorBlackboardFunction,
        HiraBots.LowLevelDecoratorBlackboardFunction.PointerConverter>;
using LowLevelScoreCalculatorCollection =
    HiraBots.DefaultLowLevelObjectCollection<
        HiraBots.LowLevelScoreCalculatorBlackboardFunction,
        HiraBots.LowLevelScoreCalculatorBlackboardFunction.PointerConverter>;
using LowLevelEffectorCollection =
    HiraBots.DefaultLowLevelObjectCollection<
        HiraBots.LowLevelEffectorBlackboardFunction,
        HiraBots.LowLevelEffectorBlackboardFunction.PointerConverter>;

namespace HiraBots
{
    internal readonly unsafe struct LowLevelBlackboardCollection
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelBlackboardCollection(byte* address, ushort count, ushort size)
        {
            m_Address = address;
            m_Count = count;
            m_Size = size;
        }

        private readonly byte* m_Address;
        private readonly ushort m_Count;
        private readonly ushort m_Size;

        internal ushort count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Count;
        }

        /// <summary>
        /// Access a blackboard from the collection.
        /// </summary>
        internal LowLevelBlackboard this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var output = new LowLevelBlackboard(m_Address + (m_Size * index), m_Size);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                return index >= count
                    ? throw new System.IndexOutOfRangeException($"Index out of range[0-{count}) - " +
                                                                $"(index) {index}.")
                    : output;
#else
                return output;
#endif
            }
        }
    }

    internal readonly unsafe struct LowLevelDecoratorBlackboardFunctionCollection
    {
        private readonly LowLevelDecoratorCollection m_Collection;
        internal LowLevelDecoratorCollection collection
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Collection;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelDecoratorBlackboardFunctionCollection(LowLevelDecoratorCollection actual)
        {
            m_Collection = actual;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelDecoratorBlackboardFunctionCollection(byte* stream) : this(new LowLevelDecoratorCollection(stream))
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Execute(LowLevelBlackboard blackboard)
        {
            var enumerator = m_Collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.current.Execute(blackboard))
                {
                    continue;
                }

                return false;
            }

            return true;
        }
    }

    internal readonly unsafe struct LowLevelScoreCalculatorBlackboardFunctionCollection
    {
        private readonly LowLevelScoreCalculatorCollection m_Collection;
        internal LowLevelScoreCalculatorCollection collection
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Collection;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelScoreCalculatorBlackboardFunctionCollection(LowLevelScoreCalculatorCollection actual)
        {
            m_Collection = actual;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelScoreCalculatorBlackboardFunctionCollection(byte* stream) : this(new LowLevelScoreCalculatorCollection(stream))
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal float Execute(LowLevelBlackboard blackboard)
        {
            var score = 0f;

            var enumerator = m_Collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                score += enumerator.current.Execute(blackboard);
            }

            return score;
        }
    }

    internal readonly unsafe struct LowLevelEffectorBlackboardFunctionCollection
    {
        private readonly LowLevelEffectorCollection m_Collection;
        internal LowLevelEffectorCollection collection
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Collection;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelEffectorBlackboardFunctionCollection(LowLevelEffectorCollection actual)
        {
            m_Collection = actual;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelEffectorBlackboardFunctionCollection(byte* stream) : this(new LowLevelEffectorCollection(stream))
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Execute(LowLevelBlackboard blackboard)
        {
            var enumerator = m_Collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.current.Execute(blackboard);
            }
        }
    }
}