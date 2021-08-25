using System.Runtime.CompilerServices;

namespace HiraBots
{
    /// <summary>
    /// Low-level representation of an LGOAP Action.
    /// </summary>
    internal readonly unsafe struct LowLevelLGOAPAction : ILowLevelObject
    {
        private readonly byte* m_Address;

        public byte* address
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Address;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelLGOAPAction(byte* address)
        {
            m_Address = address;
        }

        internal readonly struct Converter : IPointerToLowLevelObjectConverter<LowLevelLGOAPAction>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public LowLevelLGOAPAction Convert(byte* address)
            {
                return new LowLevelLGOAPAction(address);
            }
        }

        // no offset
        /// <summary>
        /// The total size occupied by this blackboard function.
        /// </summary>
        public int size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.JumpOverNothing(m_Address).AndAccess<int>();
        }

        // offset size
        /// <summary>
        /// Break an action into its respective functions.
        /// </summary>
        internal void Break(
            out LowLevelDecoratorBlackboardFunctionCollection precondition,
            out LowLevelScoreCalculatorBlackboardFunctionCollection cost,
            out LowLevelEffectorBlackboardFunctionCollection effect)
        {
            var it = ByteStreamHelpers.JumpOver<int>(m_Address).AndGetAPointerOf<byte>();
            precondition = new LowLevelDecoratorBlackboardFunctionCollection(it);

            it += precondition.collection.size;
            cost = new LowLevelScoreCalculatorBlackboardFunctionCollection(it);

            it += cost.collection.size;
            effect = new LowLevelEffectorBlackboardFunctionCollection(it);
        }
    }
}