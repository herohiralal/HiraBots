using System.Runtime.CompilerServices;

namespace HiraBots
{
    internal readonly unsafe struct LowLevelLGOAPGoalLayer : ILowLevelObject
    {
        private readonly byte* m_Address;
        private readonly byte* m_InsistenceCollection;
        private readonly byte* m_TargetCollection;

        public byte* address
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Address;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelLGOAPGoalLayer(byte* address)
        {
            m_Address = address;
            m_InsistenceCollection = ByteStreamHelpers.JumpOver<int>(m_Address).AndGetAPointerOf<byte>(); // offset size
            m_TargetCollection = m_InsistenceCollection + new LowLevelLGOAPTargetCollection(m_InsistenceCollection).collection.size; // offset size & insistence collection
        }

        internal readonly struct Converter : IPointerToLowLevelObjectConverter<LowLevelLGOAPGoalLayer>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public LowLevelLGOAPGoalLayer Convert(byte* address)
            {
                return new LowLevelLGOAPGoalLayer(address);
            }
        }

        // no offset
        /// <summary>
        /// The total size occupied by this low level object.
        /// </summary>
        public int size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.JumpOverNothing(m_Address).AndAccess<int>();
        }

        /// <summary>
        /// The collection of all insistences.
        /// </summary>
        internal LowLevelLGOAPInsistenceCollection insistenceCollection
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new LowLevelLGOAPInsistenceCollection(m_InsistenceCollection);
        }

        /// <summary>
        /// The collection of all targets.
        /// </summary>
        internal LowLevelLGOAPTargetCollection targetCollection
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new LowLevelLGOAPTargetCollection(m_TargetCollection);
        }
    }
}