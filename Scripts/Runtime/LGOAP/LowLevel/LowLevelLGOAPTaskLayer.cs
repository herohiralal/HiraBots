using System.Runtime.CompilerServices;

namespace HiraBots
{
    internal readonly unsafe struct LowLevelLGOAPTaskLayer : ILowLevelObject
    {
        private readonly byte* m_Address;
        private readonly byte* m_ActionCollection;
        private readonly byte* m_TargetCollection;

        public byte* address
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Address;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelLGOAPTaskLayer(byte* address)
        {
            m_Address = address;
            m_ActionCollection = ByteStreamHelpers.JumpOver<int>(m_Address).AndGetAPointerOf<byte>(); // offset size
            m_TargetCollection = m_ActionCollection + new LowLevelLGOAPActionCollection(m_ActionCollection).collection.size; // offset size & action collection
        }

        internal readonly struct Converter : IPointerToLowLevelObjectConverter<LowLevelLGOAPTaskLayer>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public LowLevelLGOAPTaskLayer Convert(byte* address)
            {
                return new LowLevelLGOAPTaskLayer(address);
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
        /// The collection of all actions.
        /// </summary>
        internal LowLevelLGOAPActionCollection insistenceCollection
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new LowLevelLGOAPActionCollection(m_ActionCollection);
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