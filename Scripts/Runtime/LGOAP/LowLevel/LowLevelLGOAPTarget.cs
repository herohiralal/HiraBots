using System.Runtime.CompilerServices;

namespace HiraBots
{
    /// <summary>
    /// Low-level representation of an LGOAP target.
    /// </summary>
    internal readonly unsafe struct LowLevelLGOAPTarget : ILowLevelObject
    {
        private readonly byte* m_Address;

        public byte* address
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Address;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelLGOAPTarget(byte* address)
        {
            m_Address = address;
        }

        internal readonly struct Converter : IPointerToLowLevelObjectConverter<LowLevelLGOAPTarget>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public LowLevelLGOAPTarget Convert(byte* address)
            {
                return new LowLevelLGOAPTarget(address);
            }
        }

        // no offset
        /// <summary>
        /// The total size occupied by this 
        /// </summary>
        public int size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.JumpOverNothing(m_Address).AndAccess<int>();
        }

        // offset size
        /// <summary>
        /// Check whether this target is fake, and doesn't require planning any actions.
        /// </summary>
        internal bool isFake
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.JumpOver<int>(m_Address).AndAccess<bool>();
        }

        /// <summary>
        /// Check if a blackboard has this target achieved.
        /// </summary>
        internal bool CheckIfAchieved(LowLevelBlackboard blackboard)
        {
            var ptr = ByteStreamHelpers.JumpOver<int, bool>(m_Address).AndGetAPointerOf<byte>();

            return new LowLevelDecoratorBlackboardFunctionCollection(ptr).Execute(blackboard);
        }
    }
}