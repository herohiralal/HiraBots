using System.Runtime.CompilerServices;
using LowLevelDecorator2DCollection =
    HiraBots.DefaultLowLevelObjectCollection<
        HiraBots.DefaultLowLevelObjectCollection<
            HiraBots.LowLevelDecoratorBlackboardFunction,
            HiraBots.LowLevelDecoratorBlackboardFunction.PointerConverter>,
        HiraBots.DefaultLowLevelObjectCollection<
            HiraBots.LowLevelDecoratorBlackboardFunction,
            HiraBots.LowLevelDecoratorBlackboardFunction.PointerConverter>.PointerConverter>;
using LowLevelScoreCalculator2DCollection =
    HiraBots.DefaultLowLevelObjectCollection<
        HiraBots.DefaultLowLevelObjectCollection<
            HiraBots.LowLevelScoreCalculatorBlackboardFunction,
            HiraBots.LowLevelScoreCalculatorBlackboardFunction.PointerConverter>,
        HiraBots.DefaultLowLevelObjectCollection<
            HiraBots.LowLevelScoreCalculatorBlackboardFunction,
            HiraBots.LowLevelScoreCalculatorBlackboardFunction.PointerConverter>.PointerConverter>;
using LowLevelEffector2DCollection =
    HiraBots.DefaultLowLevelObjectCollection<
        HiraBots.DefaultLowLevelObjectCollection<
            HiraBots.LowLevelEffectorBlackboardFunction,
            HiraBots.LowLevelEffectorBlackboardFunction.PointerConverter>,
        HiraBots.DefaultLowLevelObjectCollection<
            HiraBots.LowLevelEffectorBlackboardFunction,
            HiraBots.LowLevelEffectorBlackboardFunction.PointerConverter>.PointerConverter>;
using LowLevelLGOAPActionCollectionActual =
    HiraBots.DefaultLowLevelObjectCollection<
        HiraBots.LowLevelLGOAPAction,
        HiraBots.LowLevelLGOAPAction.Converter>;
using LowLevelLGOAPTargetCollectionActual =
    HiraBots.DefaultLowLevelObjectCollection<
        HiraBots.LowLevelLGOAPTarget,
        HiraBots.LowLevelLGOAPTarget.Converter>;

namespace HiraBots
{
    /// <summary>
    /// A collection of low-level LGOAP insistence.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{info}")]
    internal readonly unsafe struct LowLevelLGOAPInsistenceCollection
    {
        private readonly LowLevelScoreCalculator2DCollection m_Collection;

        internal LowLevelScoreCalculator2DCollection collection
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Collection;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelLGOAPInsistenceCollection(LowLevelScoreCalculator2DCollection actual)
        {
            m_Collection = actual;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelLGOAPInsistenceCollection(byte* stream) : this(new LowLevelScoreCalculator2DCollection(stream))
        {
        }

        private string info
        {
            get
            {
                var output = "unknown";
                CompilationRegistry.Find(m_Collection.address, ref output, 3);
                return output;
            }
        }

        /// <summary>
        /// Get the index of goal which has the highest insistence.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int GetMaxInsistenceIndex(LowLevelBlackboard blackboard)
        {
            var index = -1;
            var maxScore = float.MinValue;

            var enumerator = m_Collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var scCol = new LowLevelScoreCalculatorBlackboardFunctionCollection(enumerator.current);
                var currentScore = scCol.Execute(blackboard);

                (index, maxScore) = currentScore > maxScore ? (enumerator.currentIndex, currentScore) : (index, maxScore);
            }

            return index;
        }
    }

    /// <summary>
    /// Low-level representation of a collection of LGOAP targets.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{info}")]
    internal readonly unsafe struct LowLevelLGOAPTargetCollection
    {
        private readonly LowLevelLGOAPTargetCollectionActual m_Collection;

        internal LowLevelLGOAPTargetCollectionActual collection
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Collection;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelLGOAPTargetCollection(LowLevelLGOAPTargetCollectionActual actual)
        {
            m_Collection = actual;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelLGOAPTargetCollection(byte* stream) : this(new LowLevelLGOAPTargetCollectionActual(stream))
        {
        }

        private string info
        {
            get
            {
                var output = "unknown";
                CompilationRegistry.Find(m_Collection.address, ref output, 3);
                return output;
            }
        }

        /// <summary>
        /// Access a specific low-level LGOAP target by index.
        /// </summary>
        internal LowLevelLGOAPTarget this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Collection[index];
        }
    }

    /// <summary>
    /// Low-level representation of a collection of LGOAP actions.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{info}")]
    internal readonly unsafe struct LowLevelLGOAPActionCollection
    {
        private readonly LowLevelLGOAPActionCollectionActual m_Collection;

        internal LowLevelLGOAPActionCollectionActual collection
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Collection;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelLGOAPActionCollection(LowLevelLGOAPActionCollectionActual collection)
        {
            m_Collection = collection;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelLGOAPActionCollection(byte* stream) : this(new LowLevelLGOAPActionCollectionActual(stream))
        {
        }

        private string info
        {
            get
            {
                var output = "unknown";
                CompilationRegistry.Find(m_Collection.address, ref output, 3);
                return output;
            }
        }

        /// <summary>
        /// Get an enumerator to enumerate over the actions.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelLGOAPActionCollectionActual.Enumerator GetEnumerator()
        {
            return m_Collection.GetEnumerator();
        }
    }
}