using System;
using System.Runtime.CompilerServices;

namespace HiraBots
{
    /// <summary>
    /// Low-level representation of a blackboard function.
    /// </summary>
    internal readonly unsafe struct LowLevelBlackboardFunction : ILowLevelObject
    {
        private readonly byte* m_Address;
        public byte* address
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Address;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal LowLevelBlackboardFunction(byte* address)
        {
            m_Address = address;
        }

        internal readonly struct PointerConverter : IPointerToLowLevelObjectConverter<LowLevelBlackboardFunction>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public LowLevelBlackboardFunction Convert(byte* address)
            {
                return new LowLevelBlackboardFunction(address);
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
        /// The address of the actual function.
        /// </summary>
        internal IntPtr functionPtr
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.JumpOver<int>(m_Address).AndAccess<IntPtr>();
        }

        // offset size & function
        /// <summary>
        /// The memory cached by the function.
        /// </summary>
        internal byte* memory
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.JumpOver<int, IntPtr>(m_Address).AndGetAPointerOf<byte>();
        }
    }
}