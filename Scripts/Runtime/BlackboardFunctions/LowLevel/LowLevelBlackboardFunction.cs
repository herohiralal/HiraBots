using System;
using System.Runtime.CompilerServices;

namespace HiraBots
{
    /// <summary>
    /// Low-level representation of a blackboard function.
    /// </summary>
    internal readonly unsafe struct LowLevelBlackboardFunction
    {
        private readonly byte* m_Address;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private LowLevelBlackboardFunction(byte* address)
        {
            m_Address = address;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator LowLevelBlackboardFunction(byte* stream)
        {
            return new LowLevelBlackboardFunction(stream);
        }

        // no offset
        /// <summary>
        /// The total size occupied by this blackboard function.
        /// </summary>
        internal int size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.Read<int>(m_Address);
        }

        // offset size
        /// <summary>
        /// The address of the actual function.
        /// </summary>
        internal IntPtr functionPtr
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.Read<int, IntPtr>(m_Address);
        }

        // offset size & function
        /// <summary>
        /// The memory cached by the function.
        /// </summary>
        internal byte* memory
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ByteStreamHelpers.JumpOver<int, IntPtr>(m_Address).AsAPointerOf<byte>();
        }
    }
}