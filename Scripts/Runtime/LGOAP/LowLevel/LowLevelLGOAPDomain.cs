using System.Runtime.CompilerServices;

namespace HiraBots
{
    [System.Diagnostics.DebuggerDisplay("{info}")]
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

        private string info
        {
            get
            {
                var output = "unknown";
                CompilationRegistry.Find(m_Address, ref output, 1);
                return output;
            }
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
            if (index < 1 || index >= layerCount)
            {
                throw new System.IndexOutOfRangeException($"Index out of range [1-{layerCount}) - (index) {index}.");
            }
#endif

            var address = ByteStreamHelpers.JumpOver<byte>(m_Address).AndGetAPointerOf<byte>();
            var insistenceColl = new LowLevelLGOAPInsistenceCollection(address);
            address += insistenceColl.collection.size;

            for (var i = 1; i < index; i++) // start from 1 because layer 0 is goal layer
            {
                var targetColl = new LowLevelLGOAPTargetCollection(address);
                address += targetColl.collection.size;
                var actionColl = new LowLevelLGOAPActionCollection(address);
                address += actionColl.collection.size;
            }

            var targetCollection = new LowLevelLGOAPTargetCollection(address);
            address += targetCollection.collection.size;

            var actionCollection = new LowLevelLGOAPActionCollection(address);

            return (targetCollection, actionCollection);
        }
    }
}