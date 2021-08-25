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

namespace HiraBots
{
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

    internal readonly unsafe struct LGOAPTargetCollection
    {
        private readonly LowLevelDecorator2DCollection m_Collection;

        internal LowLevelDecorator2DCollection collection
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Collection;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LGOAPTargetCollection(LowLevelDecorator2DCollection actual)
        {
            m_Collection = actual;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LGOAPTargetCollection(byte* stream) : this(new LowLevelDecorator2DCollection(stream))
        {
        }

        internal LowLevelDecoratorBlackboardFunctionCollection this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new LowLevelDecoratorBlackboardFunctionCollection(m_Collection[index]);
        }
    }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelLGOAPActionCollectionActual.Enumerator GetEnumerator()
        {
            return m_Collection.GetEnumerator();
        }
    }
}