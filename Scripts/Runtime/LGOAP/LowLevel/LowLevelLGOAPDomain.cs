using System.Runtime.CompilerServices;

namespace HiraBots
{
    internal readonly unsafe struct LowLevelLGOAPDomain
    {
        private readonly byte* m_Address;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelLGOAPDomain(byte* address)
        {
            m_Address = address;
        }

        public static implicit operator LowLevelLGOAPDomain(Unity.Collections.NativeArray<byte> domain)
        {
            return new LowLevelLGOAPDomain((byte*) Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(domain));
        }

        // no offset
        /// <summary>
        /// The total number of task layers in this domain.
        /// </summary>
        private byte layerCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.JumpOverNothing(m_Address).AndAccess<byte>();
        }

        // offset layer count
        private byte* insistencesAddress
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.JumpOver<byte>(m_Address).AndGetAPointerOf<byte>();
        }

        /// <summary>
        /// The insistences of all goals.
        /// </summary>
        internal LowLevelLGOAPInsistenceCollection insistenceCollection
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new LowLevelLGOAPInsistenceCollection(insistencesAddress);
        }

        /// <summary>
        /// Get the task layer at a given index.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal (LowLevelLGOAPTargetCollection targets, LowLevelLGOAPActionCollection actions) GetTaskLayerAt(int index)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (index < 0 || index >= layerCount)
            {
                throw new System.IndexOutOfRangeException($"Index out of range [0-{layerCount}) - (index) {index}.");
            }
#endif

            var address = ByteStreamHelpers.JumpOver<byte>(m_Address).AndGetAPointerOf<byte>();
            address += new LowLevelLGOAPInsistenceCollection(address).collection.size;

            for (var i = 0; i < index; i++)
            {
                address += new LowLevelLGOAPTargetCollection(address).collection.size;
                address += new LowLevelLGOAPActionCollection(address).collection.size;
            }

            var targetCollection = new LowLevelLGOAPTargetCollection(address);
            address += targetCollection.collection.size;

            var actionCollection = new LowLevelLGOAPActionCollection(address);

            return (targetCollection, actionCollection);
        }
    }
}