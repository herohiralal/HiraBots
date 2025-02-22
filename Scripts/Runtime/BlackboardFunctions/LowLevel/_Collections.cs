using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
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
    /// <summary>
    /// A collection of blackboards, arranged in an array.
    /// </summary>
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

        // get a pointer at a given index
        private byte* GetBlackboardPtrAt(int index)
        {
            return m_Address + (m_Size * index);
        }

        /// <summary>
        /// Access a blackboard from the collection.
        /// </summary>
        internal LowLevelBlackboard this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var output = new LowLevelBlackboard(GetBlackboardPtrAt(index), m_Size);

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

        /// <summary>
        /// Copy a blackboard from one index to another.
        /// </summary>
        internal void Copy(int indexDestination, int indexSource)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (indexDestination < 0 || indexDestination >= m_Size
                                     || indexSource < 0 || indexSource >= m_Size)
            {
                throw new System.IndexOutOfRangeException($"Index out of range [0-{count}) - " +
                                                          $"(indices) {indexSource} -> {indexDestination}.");
            }
#endif

            UnsafeUtility.MemCpy(GetBlackboardPtrAt(indexDestination), GetBlackboardPtrAt(indexSource), m_Size);
        }

        /// <summary>
        /// Copy a blackboard from an array to a given index.
        /// </summary>
        internal void Copy(int indexDestination, NativeArray<byte> source)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (indexDestination < 0 || indexDestination >= m_Size)
            {
                throw new System.IndexOutOfRangeException($"Index out of range [0-{count}) - " +
                                                          $"(indices) {indexDestination}.");
            }

            if (source.Length != m_Size)
            {
                throw new System.InvalidOperationException($"Size mismatch between source array and blackboard size. - " +
                                                           $"(blackboard size) {m_Size} & (array length) {source.Length}");
            }
#endif

            UnsafeUtility.MemCpy(GetBlackboardPtrAt(indexDestination), source.GetUnsafeReadOnlyPtr(), m_Size);
        }
    }

    /// <summary>
    /// A collection of low level decorators.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{info}")]
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

        private string info
        {
            get
            {
                var output = "unknown";
                CompilationRegistry.Find(m_Collection.address, ref output);
                return output;
            }
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int GetHeuristic(LowLevelBlackboard blackboard)
        {
            var heuristic = 0;
            var enumerator = m_Collection.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (!enumerator.current.Execute(blackboard))
                {
                    heuristic++;
                }
            }

            return heuristic;
        }
    }

    /// <summary>
    /// A collection of low level score-calculators.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{info}")]
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

        private string info
        {
            get
            {
                var output = "unknown";
                CompilationRegistry.Find(m_Collection.address, ref output);
                return output;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal float Execute(LowLevelBlackboard blackboard)
        {
            var score = 0f;

            var enumerator = m_Collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                score += enumerator.current.Execute(blackboard, score);
            }

            return score;
        }
    }

    /// <summary>
    /// A collection of low-level effectors.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{info}")]
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

        private string info
        {
            get
            {
                var output = "unknown";
                CompilationRegistry.Find(m_Collection.address, ref output);
                return output;
            }
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